import { type FormEvent, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { api, buildAssetUrl } from "../lib/api";
import { useAuth } from "../state/AuthContext";
import type { Comment, Project } from "../types";

export function ProjectDetailPage() {
  const { projectId = "" } = useParams();
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

  async function handleCommentSubmit(event: FormEvent) {
    event.preventDefault();
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

  async function handleReportSubmit(event: FormEvent) {
    event.preventDefault();
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
      const response = await api.reportProject(projectId, { reason: reportReason.trim(), details: reportDetails.trim() }, token);
      setFeedback(response.message);
      setReportReason("");
      setReportDetails("");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Rapor gonderilemedi.");
    } finally {
      setReportBusy(false);
    }
  }

  if (error && !project) {
    return <div className="state-box error">{error}</div>;
  }

  if (loading || !project) {
    return <div className="state-box">Proje yukleniyor...</div>;
  }

  const assetUrl = buildAssetUrl(project.filePath);

  return (
    <section className="detail-layout">
      <article className="detail-main">
        <span className={`status-badge status-${project.status.toLowerCase()}`}>{project.status}</span>
        <h1>{project.title}</h1>
        <p className="lead-copy">{project.description}</p>

        <div className="panel-banner">
          <div>
            <strong>{project.owner.firstName} {project.owner.lastName}</strong>
            <span>{new Date(project.createdAt).toLocaleDateString("tr-TR")} tarihinde paylasildi</span>
          </div>
          <span className="muted-chip">{comments.length} yorum</span>
        </div>

        <div className="action-row">
          <button className="primary-button small" disabled={likeBusy} onClick={handleLikeToggle} type="button">
            {liked ? "Begeniyi Kaldir" : "Begeni Birak"}
          </button>
          {assetUrl ? (
            <a className="ghost-button" href={assetUrl} rel="noreferrer" target="_blank">
              Dosyayi Ac
            </a>
          ) : null}
        </div>

        <div className="detail-grid">
          <div className="info-box">
            <strong>Sahip</strong>
            <span>{project.owner.firstName} {project.owner.lastName}</span>
          </div>
          <div className="info-box">
            <strong>Malzeme</strong>
            <span>{project.technicalDetails.material ?? "Belirtilmedi"}</span>
          </div>
          <div className="info-box">
            <strong>Katman</strong>
            <span>{project.technicalDetails.layers ?? "-"}</span>
          </div>
          <div className="info-box">
            <strong>Adet</strong>
            <span>{project.technicalDetails.quantity ?? "-"}</span>
          </div>
        </div>
      </article>

      <aside className="detail-sidebar">
        <form className="side-panel" onSubmit={handleCommentSubmit}>
          <h2>Yorumlar</h2>
          <textarea className="text-area compact" onChange={(event) => setCommentText(event.target.value)} placeholder="Projeye geri bildirim birak..." value={commentText} />
          <button className="primary-button small" disabled={commentBusy} type="submit">
            {commentBusy ? "Gonderiliyor..." : "Yorum Ekle"}
          </button>
          <div className="comment-list">
            {comments.map((comment) => (
              <div key={comment.id} className="comment-item">
                <strong>{comment.author.firstName} {comment.author.lastName}</strong>
                <p>{comment.content}</p>
                {user?.id === comment.userId ? (
                  <button className="inline-link button-link" onClick={() => void handleDeleteComment(comment.id)} type="button">
                    Sil
                  </button>
                ) : null}
              </div>
            ))}
          </div>
        </form>

        <form className="side-panel" onSubmit={handleReportSubmit}>
          <h2>Raporla</h2>
          <input className="text-input" onChange={(event) => setReportReason(event.target.value)} placeholder="Sebep" value={reportReason} />
          <textarea className="text-area compact" onChange={(event) => setReportDetails(event.target.value)} placeholder="Moderasyona iletmek istedigin detaylar..." value={reportDetails} />
          <button className="ghost-button" disabled={reportBusy} type="submit">
            {reportBusy ? "Gonderiliyor..." : "Rapor Gonder"}
          </button>
          {feedback ? <div className="state-box success">{feedback}</div> : null}
          {error ? <div className="state-box error">{error}</div> : null}
        </form>
      </aside>
    </section>
  );
}
