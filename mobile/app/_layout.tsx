import { Stack } from "expo-router";
import { StatusBar } from "expo-status-bar";
import { AuthProvider } from "@/state/AuthContext";
import { colors } from "@/theme/colors";

export default function RootLayout() {
  return (
    <AuthProvider>
      <StatusBar style="dark" />
      <Stack
        screenOptions={{
          headerStyle: { backgroundColor: colors.background },
          headerShadowVisible: false,
          headerTintColor: colors.text,
          headerTitleStyle: { fontWeight: "800" }
        }}
      >
        <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
        <Stack.Screen name="login" options={{ title: "Giris Yap" }} />
        <Stack.Screen name="register" options={{ title: "Yeni Hesap" }} />
        <Stack.Screen name="projects/[projectId]" options={{ title: "Proje Detayi" }} />
      </Stack>
    </AuthProvider>
  );
}

