import { useLocalSearchParams } from "expo-router";
import { useEffect, useState } from "react";
import { Alert, Linking, StyleSheet, Text, View } from "react-native";
import { Button } from "@/components/Button";
import { Screen } from "@/components/Screen";
import { StateMessage } from "@/components/StateMessage";
import { TextField } from "@/components/TextField";
import { api, buildAssetUrl } from "@/services/api";
import { useAuth } from "@/state/AuthContext";
import { colors } from "@/theme/colors";
import type { Comment, Project } from "@/types";

export default function ProjectDetailScreen() {
  const { projectId = "" } = useLocalSearchParams<{ projectId: string }>();
  const { token, user } = useAuth();
  const [project, setProject] = useState<Project | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [commentText, setCommentText] = useState("");
  const [reportReason, setReportReason] = useState("");
  const [reportDetails, setReportDetails] = useState("");
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [commentBusy, setCommentBusy] = useState(false);
  const [reportBusy, setReportBusy] = useState(false);
  const [likeBusy, setLikeBusy] = useState(false);
  const [liked, setLiked] = useState(false);

  useEffect(() => {
    async function loadPage() {
      setLoading(true);
      setError(null);
      setFeedback(null);
      setLiked(false);

      try {
        const [projectData, commentData] = await Promise.all([
          api.getProject(projectId),
          api.getComments(projectId)
        ]);

        setProject(projectData);
        setComments(commentData);

        if (token) {
          const myLikes = await api.getMyLikes(token);
          setLiked(myLikes.some((item) => item.id === projectData.id));
        }
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Proje yuklenemedi.");
      } finally {
        setLoading(false);
      }
    }

    if (projectId) {
      void loadPage();
    }
  }, [projectId, token]);

  async function handleCommentSubmit() {
    if (!token) {
      setError("Yorum eklemek icin giris yapmalisin.");
      return;
    }

    if (!commentText.trim()) {
      setError("Bos yorum gonderemezsin.");
      return;
    }

    setCommentBusy(true);
    setError(null);
    try {
      const createdComment = await api.createComment(projectId, commentText.trim(), token);
      setComments((current) => [createdComment, ...current]);
      setCommentText("");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Yorum eklenemedi.");
    } finally {
      setCommentBusy(false);
    }
  }

  function confirmDeleteComment(commentId: number) {
    Alert.alert("Yorumu sil", "Bu yorum silinsin mi?", [
      { text: "Vazgec", style: "cancel" },
      { text: "Sil", style: "destructive", onPress: () => void handleDeleteComment(commentId) }
    ]);
  }

  async function handleDeleteComment(commentId: number) {
    if (!token) {
      return;
    }

    setError(null);
    try {
      await api.deleteComment(commentId, token);
      setComments((current) => current.filter((comment) => comment.id !== commentId));
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Yorum silinemedi.");
    }
  }

  async function handleLikeToggle() {
    if (!token) {
      setError("Begeni icin giris yapmalisin.");
      return;
    }

    setLikeBusy(true);
    setError(null);
    try {
      if (liked) {
        await api.unlikeProject(projectId, token);
        setLiked(false);
      } else {
        await api.likeProject(projectId, token);
        setLiked(true);
      }
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Begeni guncellenemedi.");
    } finally {
      setLikeBusy(false);
    }
  }

  async function handleReportSubmit() {
    if (!token) {
      setError("Raporlamak icin giris yapmalisin.");
      return;
    }

    if (!reportReason.trim()) {
      setError("Rapor sebebi bos birakilamaz.");
      return;
    }

    setReportBusy(true);
    setError(null);
    setFeedback(null);
    try {
      const response = await api.reportProject(projectId, {
        reason: reportReason.trim(),
        details: reportDetails.trim()
      }, token);
      setFeedback(response.message);
      setReportReason("");
      setReportDetails("");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Rapor gonderilemedi.");
    } finally {
      setReportBusy(false);
    }
  }

  async function openAsset() {
    const assetUrl = buildAssetUrl(project?.filePath);
    if (assetUrl) {
      await Linking.openURL(assetUrl);
    }
  }

  if (loading) {
    return <Screen><StateMessage message="Proje yukleniyor..." /></Screen>;
  }

  if (!project) {
    return <Screen><StateMessage message={error ?? "Proje bulunamadi."} tone="error" /></Screen>;
  }

  const assetUrl = buildAssetUrl(project.filePath);

  return (
    <Screen>
      <View style={styles.hero}>
        <Text style={styles.badge}>{project.status}</Text>
        <Text style={styles.title}>{project.title}</Text>
        <Text style={styles.copy}>{project.description}</Text>
        <View style={styles.ownerBox}>
          <Text style={styles.owner}>{project.owner.firstName} {project.owner.lastName}</Text>
          <Text style={styles.copy}>{new Date(project.createdAt).toLocaleDateString("tr-TR")} tarihinde paylasildi</Text>
        </View>
        <View style={styles.actionRow}>
          <Button loading={likeBusy} onPress={handleLikeToggle} title={liked ? "Begeniyi Kaldir" : "Begeni Birak"} />
          {assetUrl ? <Button onPress={openAsset} title="Dosyayi Ac" variant="secondary" /> : null}
        </View>
      </View>

      <View style={styles.detailGrid}>
        <Info label="Sahip" value={`${project.owner.firstName} ${project.owner.lastName}`} />
        <Info label="Malzeme" value={project.technicalDetails.material ?? "Belirtilmedi"} />
        <Info label="Katman" value={project.technicalDetails.layers?.toString() ?? "-"} />
        <Info label="Adet" value={project.technicalDetails.quantity?.toString() ?? "-"} />
      </View>

      {feedback ? <StateMessage message={feedback} tone="success" /> : null}
      {error ? <StateMessage message={error} tone="error" /> : null}

      <View style={styles.panel}>
        <Text style={styles.sectionTitle}>Yorumlar</Text>
        <TextField multiline onChangeText={setCommentText} placeholder="Projeye geri bildirim birak..." value={commentText} />
        <Button loading={commentBusy} onPress={handleCommentSubmit} title="Yorum Ekle" />
        {comments.length > 0 ? comments.map((comment) => (
          <View key={comment.id} style={styles.comment}>
            <Text style={styles.commentAuthor}>{comment.author.firstName} {comment.author.lastName}</Text>
            <Text style={styles.copy}>{comment.content}</Text>
            {user?.id === comment.userId ? <Button onPress={() => confirmDeleteComment(comment.id)} title="Sil" variant="danger" /> : null}
          </View>
        )) : <StateMessage message="Bu projede henuz yorum yok." />}
      </View>

      <View style={styles.panel}>
        <Text style={styles.sectionTitle}>Raporla</Text>
        <TextField label="Sebep" onChangeText={setReportReason} value={reportReason} />
        <TextField label="Detay" multiline onChangeText={setReportDetails} value={reportDetails} />
        <Button loading={reportBusy} onPress={handleReportSubmit} title="Rapor Gonder" variant="secondary" />
      </View>
    </Screen>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.info}>
      <Text style={styles.infoLabel}>{label}</Text>
      <Text style={styles.infoValue}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  hero: {
    backgroundColor: colors.surface,
    borderColor: colors.border,
    borderWidth: 1,
    borderRadius: 8,
    padding: 16,
    gap: 12
  },
  badge: {
    alignSelf: "flex-start",
    color: colors.primaryDark,
    backgroundColor: "#ccfbf1",
    borderRadius: 999,
    paddingHorizontal: 10,
    paddingVertical: 5,
    fontSize: 12,
    fontWeight: "900",
    overflow: "hidden"
  },
  title: {
    color: colors.text,
    fontSize: 28,
    lineHeight: 34,
    fontWeight: "900"
  },
  copy: {
    color: colors.muted,
    fontSize: 14,
    lineHeight: 20
  },
  ownerBox: {
    borderRadius: 8,
    backgroundColor: colors.surfaceMuted,
    padding: 12,
    gap: 3
  },
  owner: {
    color: colors.text,
    fontWeight: "900"
  },
  actionRow: {
    gap: 10
  },
  detailGrid: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 10
  },
  info: {
    width: "47.5%",
    borderRadius: 8,
    backgroundColor: colors.surface,
    borderWidth: 1,
    borderColor: colors.border,
    padding: 12,
    gap: 4
  },
  infoLabel: {
    color: colors.muted,
    fontSize: 12,
    fontWeight: "800"
  },
  infoValue: {
    color: colors.text,
    fontWeight: "900"
  },
  panel: {
    backgroundColor: colors.surface,
    borderColor: colors.border,
    borderWidth: 1,
    borderRadius: 8,
    padding: 14,
    gap: 12
  },
  sectionTitle: {
    color: colors.text,
    fontSize: 20,
    fontWeight: "900"
  },
  comment: {
    borderTopWidth: 1,
    borderTopColor: colors.border,
    paddingTop: 12,
    gap: 8
  },
  commentAuthor: {
    color: colors.text,
    fontWeight: "900"
  }
});

