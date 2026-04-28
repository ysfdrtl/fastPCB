import { useEffect, useState } from "react";
import { FlatList, Pressable, StyleSheet, Text, View } from "react-native";
import { ProjectCard } from "@/components/ProjectCard";
import { Screen } from "@/components/Screen";
import { StateMessage } from "@/components/StateMessage";
import { TextField } from "@/components/TextField";
import { Button } from "@/components/Button";
import { api } from "@/services/api";
import { colors } from "@/theme/colors";
import type { Project } from "@/types";

const statuses = ["", "Draft", "Published", "Featured", "Archived", "Removed"];

export default function DiscoverScreen() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const timeout = setTimeout(() => {
      void loadProjects(1, true);
    }, 350);

    return () => clearTimeout(timeout);
  }, [search, status]);

  async function loadProjects(nextPage = page, replace = false) {
    setLoading(true);
    setError(null);

    try {
      const params = new URLSearchParams({
        page: nextPage.toString(),
        pageSize: "10"
      });

      if (search.trim()) {
        params.set("search", search.trim());
      }

      if (status) {
        params.set("status", status);
      }

      const data = await api.getProjects(params);
      setProjects((current) => replace ? data.items : [...current, ...data.items]);
      setPage(data.pagination.page);
      setTotalPages(Math.max(data.pagination.totalPages, 1));
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Projeler yuklenemedi.");
    } finally {
      setLoading(false);
    }
  }

  function handleLoadMore() {
    if (!loading && page < totalPages) {
      void loadProjects(page + 1);
    }
  }

  return (
    <Screen scroll={false}>
      <FlatList
        contentContainerStyle={styles.content}
        data={projects}
        keyExtractor={(item) => item.id.toString()}
        ListHeaderComponent={(
          <View style={styles.header}>
            <Text style={styles.eyebrow}>Topluluga acik pcb galerisi</Text>
            <Text style={styles.title}>Dosyani yukle, kartini anlat, yeni tasarimlar kesfet.</Text>
            <Text style={styles.copy}>KiCad ve Gerber paylasimlarini yorumlar, begeniler ve teknik detaylarla takip et.</Text>
            <View style={styles.stats}>
              <View style={styles.statPill}>
                <Text style={styles.statValue}>{projects.length}</Text>
                <Text style={styles.statLabel}>gorunen proje</Text>
              </View>
              <View style={styles.statPill}>
                <Text style={styles.statValue}>{status || "Tum"}</Text>
                <Text style={styles.statLabel}>durum filtresi</Text>
              </View>
            </View>
            <TextField autoCapitalize="none" label="Ara" onChangeText={setSearch} placeholder="STM32, power board, fr4..." value={search} />
            <View style={styles.chips}>
              {statuses.map((item) => (
                <Pressable key={item || "all"} onPress={() => setStatus(item)} style={[styles.chip, status === item ? styles.activeChip : null]}>
                  <Text style={[styles.chipText, status === item ? styles.activeChipText : null]}>{item || "Tum"}</Text>
                </Pressable>
              ))}
            </View>
            {error ? <StateMessage message={error} tone="error" /> : null}
          </View>
        )}
        ListEmptyComponent={!loading && !error ? <StateMessage message="Bu filtrelerde proje bulunamadi." /> : null}
        ListFooterComponent={(
          <View style={styles.footer}>
            {loading ? <StateMessage message="Projeler yukleniyor..." /> : null}
            {!loading && page < totalPages ? <Button onPress={handleLoadMore} title="Daha fazla yukle" variant="secondary" /> : null}
          </View>
        )}
        renderItem={({ item }) => <ProjectCard project={item} />}
      />
    </Screen>
  );
}

const styles = StyleSheet.create({
  content: {
    padding: 16,
    gap: 14
  },
  header: {
    gap: 14
  },
  eyebrow: {
    color: colors.primary,
    fontSize: 12,
    fontWeight: "900",
    textTransform: "uppercase"
  },
  title: {
    color: colors.text,
    fontSize: 29,
    lineHeight: 34,
    fontWeight: "900"
  },
  copy: {
    color: colors.muted,
    fontSize: 15,
    lineHeight: 22
  },
  stats: {
    flexDirection: "row",
    gap: 10
  },
  statPill: {
    flex: 1,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colors.border,
    backgroundColor: colors.surface,
    padding: 12
  },
  statValue: {
    color: colors.text,
    fontSize: 18,
    fontWeight: "900"
  },
  statLabel: {
    color: colors.muted,
    fontSize: 12
  },
  chips: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 8
  },
  chip: {
    borderRadius: 999,
    borderWidth: 1,
    borderColor: colors.border,
    backgroundColor: colors.surface,
    paddingHorizontal: 12,
    paddingVertical: 8
  },
  activeChip: {
    backgroundColor: colors.primary,
    borderColor: colors.primary
  },
  chipText: {
    color: colors.muted,
    fontWeight: "800"
  },
  activeChipText: {
    color: "#fff"
  },
  footer: {
    gap: 12,
    paddingBottom: 24
  }
});

