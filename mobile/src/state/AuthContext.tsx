import * as SecureStore from "expo-secure-store";
import { createContext, type PropsWithChildren, useContext, useEffect, useMemo, useState } from "react";
import type { UserSummary } from "@/types";

type StoredAuth = {
  user: UserSummary;
  token: string;
};

type AuthState = {
  user: UserSummary | null;
  token: string | null;
  loading: boolean;
  setAuth: (user: UserSummary, token: string) => Promise<void>;
  logout: () => Promise<void>;
};

const storageKey = "fastpcb.auth";
const AuthContext = createContext<AuthState | undefined>(undefined);

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<UserSummary | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function restoreAuth() {
      try {
        const raw = await SecureStore.getItemAsync(storageKey);
        if (raw) {
          const stored = JSON.parse(raw) as StoredAuth;
          setUser(stored.user);
          setToken(stored.token);
        }
      } finally {
        setLoading(false);
      }
    }

    void restoreAuth();
  }, []);

  const value = useMemo<AuthState>(() => ({
    user,
    token,
    loading,
    async setAuth(nextUser, nextToken) {
      setUser(nextUser);
      setToken(nextToken);
      await SecureStore.setItemAsync(storageKey, JSON.stringify({ user: nextUser, token: nextToken }));
    },
    async logout() {
      setUser(null);
      setToken(null);
      await SecureStore.deleteItemAsync(storageKey);
    }
  }), [loading, token, user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return context;
}

