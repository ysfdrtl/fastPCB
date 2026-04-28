import * as DocumentPicker from "expo-document-picker";
import { router, useFocusEffect } from "expo-router";
import { useCallback, useState } from "react";
import { Pressable, StyleSheet, Text, View } from "react-native";
import { Button } from "@/components/Button";
import { Screen } from "@/components/Screen";
import { StateMessage } from "@/components/StateMessage";
import { TextField } from "@/components/TextField";
import { api } from "@/services/api";
import { useAuth } from "@/state/AuthContext";
import { colors } from "@/theme/colors";
import type { PickedUploadFile } from "@/types";

const materialOptions = ["FR4", "Aluminum", "CEM-1", "CEM-3", "Flexible Polyimide", "Rogers"];

export default function UploadScreen() {
  const { token, loading: authLoading } = useAuth();
  const [form, setForm] = useState({
    title: "",
    description: "",
    layers: "2",
    material: "FR4",
    minDistance: "0.2",
    quantity: "1"
  });
  const [file, setFile] = useState<PickedUploadFile | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useFocusEffect(useCallback(() => {
    if (!authLoading && !token) {
      router.replace("/login?redirectTo=/(tabs)/upload");
    }
  }, [authLoading, token]));

  async function pickFile() {
    const result = await DocumentPicker.getDocumentAsync({
      copyToCacheDirectory: true,
      multiple: false
    });

    if (!result.canceled) {
      const asset = result.assets[0];
      setFile({
        uri: asset.uri,
        name: asset.name,
        mimeType: asset.mimeType
      });
    }
  }

  async function handleSubmit() {
    if (!token) {
      return;
    }

    const layers = Number(form.layers);
    const minDistance = Number(form.minDistance);
    const quantity = Number(form.quantity);

    if (!form.title.trim()) {
      setError("Proje basligi bos birakilamaz.");
      return;
    }

    if (!form.description.trim()) {
      setError("Proje aciklamasi bos birakilamaz.");
      return;
    }

    if (!form.material.trim() || layers <= 0 || minDistance <= 0 || quantity <= 0) {
      setError("Teknik detay alanlari gecerli degerler icermeli.");
      return;
    }

    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const project = await api.createProject({
        title: form.title.trim(),
        description: form.description.trim()
      }, token);

      await api.saveProjectDetails(project.id, {
        layers,
        material: form.material.trim(),
        minDistance,
        quantity
      }, token);

      if (file) {
        await api.uploadProjectFile(project.id, file, token);
      }

      setSuccess("Proje basariyla olusturuldu.");
      router.push(`/projects/${project.id}`);
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Proje olusturulamadi.");
    } finally {
      setLoading(false);
    }
  }

  if (authLoading || !token) {
    return <Screen><StateMessage message="Giris kontrol ediliyor..." /></Screen>;
  }

  return (
    <Screen>
      <View style={styles.header}>
        <Text style={styles.eyebrow}>Yeni proje</Text>
        <Text style={styles.title}>PCB paylasimini yayina hazirla</Text>
        <Text style={styles.copy}>Baslik, aciklama, teknik detay ve opsiyonel dosyayla projen topluluga hazir olur.</Text>
      </View>

      <TextField label="Proje Basligi" onChangeText={(value) => setForm((current) => ({ ...current, title: value }))} value={form.title} />
      <TextField label="Proje Aciklamasi" multiline onChangeText={(value) => setForm((current) => ({ ...current, description: value }))} value={form.description} />
      <TextField keyboardType="numeric" label="Katman Sayisi" onChangeText={(value) => setForm((current) => ({ ...current, layers: value }))} value={form.layers} />

      <View style={styles.fieldGroup}>
        <Text style={styles.label}>Malzeme</Text>
        <View style={styles.chips}>
          {materialOptions.map((material) => (
            <Pressable key={material} onPress={() => setForm((current) => ({ ...current, material }))} style={[styles.chip, form.material === material ? styles.activeChip : null]}>
              <Text style={[styles.chipText, form.material === material ? styles.activeChipText : null]}>{material}</Text>
            </Pressable>
          ))}
        </View>
      </View>

      <TextField keyboardType="decimal-pad" label="Minimum Iz Araligi (mm)" onChangeText={(value) => setForm((current) => ({ ...current, minDistance: value }))} value={form.minDistance} />
      <TextField keyboardType="numeric" label="Adet" onChangeText={(value) => setForm((current) => ({ ...current, quantity: value }))} value={form.quantity} />

      <View style={styles.fileBox}>
        <Text style={styles.label}>Gerber, KiCad, ZIP veya gorsel</Text>
        <Text style={styles.copy}>{file ? file.name : "Dosya secilmedi. Dosya eklemek opsiyoneldir."}</Text>
        <Button onPress={pickFile} title="Dosya Sec" variant="secondary" />
      </View>

      {error ? <StateMessage message={error} tone="error" /> : null}
      {success ? <StateMessage message={success} tone="success" /> : null}
      <Button loading={loading} onPress={handleSubmit} title="Projeyi yayinla" />
    </Screen>
  );
}

const styles = StyleSheet.create({
  header: {
    gap: 8
  },
  eyebrow: {
    color: colors.primary,
    fontSize: 12,
    fontWeight: "900",
    textTransform: "uppercase"
  },
  title: {
    color: colors.text,
    fontSize: 27,
    lineHeight: 32,
    fontWeight: "900"
  },
  copy: {
    color: colors.muted,
    fontSize: 14,
    lineHeight: 20
  },
  fieldGroup: {
    gap: 8
  },
  label: {
    color: colors.text,
    fontSize: 13,
    fontWeight: "800"
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
  fileBox: {
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colors.border,
    backgroundColor: colors.surface,
    padding: 14,
    gap: 10
  }
});

