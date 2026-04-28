import { Link } from "expo-router";
import { Pressable, StyleSheet, Text, View } from "react-native";
import { colors } from "@/theme/colors";
import type { Project } from "@/types";

export function ProjectCard({ project }: { project: Project }) {
  return (
    <Link asChild href={`/projects/${project.id}`}>
      <Pressable style={({ pressed }) => [styles.card, pressed ? styles.pressed : null]}>
        <View style={styles.header}>
          <Text style={styles.badge}>{project.status}</Text>
          <Text style={styles.fileBadge}>{project.filePath ? "Dosya var" : "Taslak"}</Text>
        </View>
        <Text numberOfLines={2} style={styles.title}>{project.title}</Text>
        <Text numberOfLines={3} style={styles.description}>{project.description}</Text>
        <View style={styles.metaRow}>
          <Meta label="Malzeme" value={project.technicalDetails.material ?? "Belirtilmedi"} />
          <Meta label="Katman" value={project.technicalDetails.layers?.toString() ?? "-"} />
          <Meta label="Sahip" value={`${project.owner.firstName} ${project.owner.lastName}`} />
        </View>
      </Pressable>
    </Link>
  );
}

function Meta({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.metaItem}>
      <Text style={styles.metaLabel}>{label}</Text>
      <Text numberOfLines={1} style={styles.metaValue}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: colors.surface,
    borderColor: colors.border,
    borderWidth: 1,
    borderRadius: 8,
    padding: 16,
    gap: 10
  },
  pressed: {
    opacity: 0.82
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    gap: 8
  },
  badge: {
    color: colors.primaryDark,
    backgroundColor: "#ccfbf1",
    borderRadius: 999,
    paddingHorizontal: 10,
    paddingVertical: 5,
    fontSize: 12,
    fontWeight: "800",
    overflow: "hidden"
  },
  fileBadge: {
    color: colors.muted,
    backgroundColor: colors.surfaceMuted,
    borderRadius: 999,
    paddingHorizontal: 10,
    paddingVertical: 5,
    fontSize: 12,
    overflow: "hidden"
  },
  title: {
    color: colors.text,
    fontSize: 19,
    fontWeight: "800",
    lineHeight: 24
  },
  description: {
    color: colors.muted,
    fontSize: 14,
    lineHeight: 20
  },
  metaRow: {
    flexDirection: "row",
    gap: 8
  },
  metaItem: {
    flex: 1,
    gap: 3
  },
  metaLabel: {
    color: colors.muted,
    fontSize: 11,
    fontWeight: "700"
  },
  metaValue: {
    color: colors.text,
    fontSize: 13,
    fontWeight: "700"
  }
});

