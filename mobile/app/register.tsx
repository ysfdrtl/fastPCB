import { Link, router } from "expo-router";
import { useState } from "react";
import { StyleSheet, Text, View } from "react-native";
import { Button } from "@/components/Button";
import { Screen } from "@/components/Screen";
import { StateMessage } from "@/components/StateMessage";
import { TextField } from "@/components/TextField";
import { api } from "@/services/api";
import { useAuth } from "@/state/AuthContext";
import { colors } from "@/theme/colors";

export default function RegisterScreen() {
  const { setAuth } = useAuth();
  const [form, setForm] = useState({ firstName: "", lastName: "", email: "", password: "" });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleRegister() {
    setLoading(true);
    setError(null);

    try {
      const response = await api.register({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
        password: form.password
      });
      await setAuth({
        id: response.id,
        email: response.email,
        firstName: response.firstName,
        lastName: response.lastName,
        role: response.role ?? "User"
      }, response.token);
      router.replace("/(tabs)/profile");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Kayit olusturulamadi.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Screen contentContainerStyle={styles.content}>
      <View style={styles.panel}>
        <Text style={styles.eyebrow}>Topluluga katil</Text>
        <Text style={styles.title}>Yeni Hesap</Text>
        <TextField label="Ad" onChangeText={(value) => setForm((current) => ({ ...current, firstName: value }))} value={form.firstName} />
        <TextField label="Soyad" onChangeText={(value) => setForm((current) => ({ ...current, lastName: value }))} value={form.lastName} />
        <TextField autoCapitalize="none" keyboardType="email-address" label="Email" onChangeText={(value) => setForm((current) => ({ ...current, email: value }))} value={form.email} />
        <TextField label="Sifre" onChangeText={(value) => setForm((current) => ({ ...current, password: value }))} secureTextEntry value={form.password} />
        {error ? <StateMessage message={error} tone="error" /> : null}
        <Button loading={loading} onPress={handleRegister} title="Hesap olustur" />
        <Text style={styles.helper}>
          Zaten hesabin var mi? <Link href="/login" style={styles.link}>Giris yap</Link>
        </Text>
      </View>
    </Screen>
  );
}

const styles = StyleSheet.create({
  content: {
    flexGrow: 1,
    justifyContent: "center"
  },
  panel: {
    backgroundColor: colors.surface,
    borderColor: colors.border,
    borderWidth: 1,
    borderRadius: 8,
    padding: 18,
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
    fontSize: 28,
    fontWeight: "900"
  },
  helper: {
    color: colors.muted,
    textAlign: "center",
    fontSize: 14
  },
  link: {
    color: colors.primary,
    fontWeight: "800"
  }
});

