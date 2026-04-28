import { Link, router, useLocalSearchParams } from "expo-router";
import { useState } from "react";
import { StyleSheet, Text, View } from "react-native";
import { Button } from "@/components/Button";
import { Screen } from "@/components/Screen";
import { StateMessage } from "@/components/StateMessage";
import { TextField } from "@/components/TextField";
import { api } from "@/services/api";
import { useAuth } from "@/state/AuthContext";
import { colors } from "@/theme/colors";

export default function LoginScreen() {
  const { setAuth } = useAuth();
  const params = useLocalSearchParams<{ redirectTo?: string }>();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleLogin() {
    setLoading(true);
    setError(null);

    try {
      const response = await api.login(email.trim(), password);
      await setAuth({
        id: response.id,
        email: response.email,
        firstName: response.firstName,
        lastName: response.lastName,
        role: response.role ?? "User"
      }, response.token);
      router.replace(params.redirectTo || "/(tabs)/profile");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Giris yapilamadi.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Screen contentContainerStyle={styles.content}>
      <View style={styles.panel}>
        <Text style={styles.eyebrow}>Hesabina don</Text>
        <Text style={styles.title}>Giris Yap</Text>
        <TextField autoCapitalize="none" keyboardType="email-address" label="Email" onChangeText={setEmail} value={email} />
        <TextField label="Sifre" onChangeText={setPassword} secureTextEntry value={password} />
        {error ? <StateMessage message={error} tone="error" /> : null}
        <Button loading={loading} onPress={handleLogin} title="Devam et" />
        <Text style={styles.helper}>
          Hesabin yok mu? <Link href="/register" style={styles.link}>Kayit ol</Link>
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

