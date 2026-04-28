import { useFocusEffect, router } from "expo-router";
import type { ReactNode } from "react";
import { useCallback, useState } from "react";
import { StyleSheet, Text, View } from "react-native";
import { Button } from "@/components/Button";
import { ProjectCard } from "@/components/ProjectCard";
import { Screen } from "@/components/Screen";
import { StateMessage } from "@/components/StateMessage";
import { api } from "@/services/api";
import { useAuth } from "@/state/AuthContext";
import { colors } from "@/theme/colors";
import type { Project, Report } from "@/types";

export default function ProfileScreen() {
  const { user, token, logout, loading: authLoading } = useAuth();
  const [myProjects, setMyProjects] = useState<Project[]>([]);
  const [likedProjects, setLikedProjects] = useState<Project[]>([]);
  const [reports, setReports] = useState<Report[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useFocusEffect(useCallback(() => {
    if (!token || !user) {
      return;
    }

    const authToken = token;
    const authUser = user;
    let active = true;
    async function loadProfileData() {
      setLoading(true);
      setError(null);
      try {
        const [projects, likes, myReports] = await Promise.all([
          api.getUserProjects(authUser.id, authToken),
          api.getMyLikes(authToken),
          api.getMyReports(authToken)
        ]);

        if (active) {
          setMyProjects(projects);
          setLikedProjects(likes);
          setReports(myReports);
        }
      } catch (requestError) {
        if (active) {
          setError(requestError instanceof Error ? requestError.message : "Profil yuklenemedi.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    void loadProfileData();
    return () => {
      active = false;
    };
  }, [token, user]));

  async function handleLogout() {
    await logout();
    router.replace("/");
  }

  if (authLoading) {
    return <Screen><StateMessage message="Oturum yukleniyor..." /></Screen>;
  }

  if (!user || !token) {
    return (
      <Screen contentContainerStyle={styles.guest}>
        <Text style={styles.title}>Profilini gormek icin giris yap.</Text>
        <Button onPress={() => router.push("/login?redirectTo=/(tabs)/profile")} title="Giris Yap" />
        <Button onPress={() => router.push("/register")} title="Kayit Ol" variant="secondary" />
      </Screen>
    );
  }

  return (
    <Screen>
      <View style={styles.hero}>
        <Text style={styles.eyebrow}>Profil</Text>
        <Text style={styles.title}>{user.firstName} {user.lastName}</Text>
        <Text style={styles.copy}>{user.email}</Text>
        <View style={styles.stats}>
          <Stat label="proje" value={myProjects.length} />
          <Stat label="begeni" value={likedProjects.length} />
          <Stat label="rapor" value={reports.length} />
        </View>
        <Button onPress={handleLogout} title="Cikis Yap" variant="secondary" />
      </View>

      {loading ? <StateMessage message="Profil yukleniyor..." /> : null}
      {error ? <StateMessage message={error} tone="error" /> : null}

      <Section title="Projelerim">
        {myProjects.length > 0 ? myProjects.map((project) => <ProjectCard key={project.id} project={project} />) : <StateMessage message="Henuz kendi hesabina ait proje bulunmuyor." />}
      </Section>

      <Section title="Begenilenler">
        {likedProjects.length > 0 ? likedProjects.map((project) => <ProjectCard key={project.id} project={project} />) : <StateMessage message="Henuz begendigin proje bulunmuyor." />}
      </Section>

      <Section title="Raporlarim">
        {reports.length > 0 ? reports.map((report) => (
          <View key={report.id} style={styles.reportCard}>
            <Text style={styles.reportTitle}>{report.reason}</Text>
            <Text style={styles.copy}>{report.details || "Ek aciklama yok."}</Text>
            <Text style={styles.reportStatus}>{report.status}</Text>
          </View>
        )) : <StateMessage message="Henuz gonderdigin rapor bulunmuyor." />}
      </Section>
    </Screen>
  );
}

function Stat({ label, value }: { label: string; value: number }) {
  return (
    <View style={styles.stat}>
      <Text style={styles.statValue}>{value}</Text>
      <Text style={styles.statLabel}>{label}</Text>
    </View>
  );
}

function Section({ title, children }: { title: string; children: ReactNode }) {
  return (
    <View style={styles.section}>
      <Text style={styles.sectionTitle}>{title}</Text>
      {children}
    </View>
  );
}

const styles = StyleSheet.create({
  guest: {
    flexGrow: 1,
    justifyContent: "center"
  },
  hero: {
    backgroundColor: colors.surface,
    borderColor: colors.border,
    borderWidth: 1,
    borderRadius: 8,
    padding: 16,
    gap: 10
  },
  eyebrow: {
    color: colors.primary,
    fontSize: 12,
    fontWeight: "900",
    textTransform: "uppercase"
  },
  title: {
    color: colors.text,
    fontSize: 26,
    lineHeight: 32,
    fontWeight: "900"
  },
  copy: {
    color: colors.muted,
    fontSize: 14,
    lineHeight: 20
  },
  stats: {
    flexDirection: "row",
    gap: 8
  },
  stat: {
    flex: 1,
    borderRadius: 8,
    backgroundColor: colors.surfaceMuted,
    padding: 10
  },
  statValue: {
    color: colors.text,
    fontWeight: "900",
    fontSize: 18
  },
  statLabel: {
    color: colors.muted,
    fontSize: 12
  },
  section: {
    gap: 10
  },
  sectionTitle: {
    color: colors.text,
    fontSize: 20,
    fontWeight: "900"
  },
  reportCard: {
    backgroundColor: colors.surface,
    borderColor: colors.border,
    borderWidth: 1,
    borderRadius: 8,
    padding: 14,
    gap: 8
  },
  reportTitle: {
    color: colors.text,
    fontWeight: "900",
    fontSize: 16
  },
  reportStatus: {
    color: colors.primaryDark,
    fontWeight: "800"
  }
});
