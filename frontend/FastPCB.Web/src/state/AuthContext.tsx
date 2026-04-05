import { createContext, useContext, useEffect, useState } from "react";
import type { PropsWithChildren } from "react";
import type { UserSummary } from "../types";
import { readStorage, removeStorage, storageKeys, writeStorage } from "../lib/storage";

type AuthState = {
  user: UserSummary | null;
  token: string | null;
  setAuth: (user: UserSummary, token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthState | undefined>(undefined);

type StoredAuth = {
  user: UserSummary;
  token: string;
};

// Giris bilgisini tum uygulama genelinde paylasan auth provider katmanidir.
export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<UserSummary | null>(null);
  const [token, setToken] = useState<string | null>(null);

  // Sayfa ilk acildiginda kayitli kullanici bilgisini local storage'dan geri yukler.
  useEffect(() => {
    const stored = readStorage<StoredAuth>(storageKeys.auth);
    if (stored) {
      setUser(stored.user);
      setToken(stored.token);
    }
  }, []);

  // Basarili giris veya kayit sonrasinda kullanici bilgisini bellekte ve storage'da saklar.
  const setAuth = (nextUser: UserSummary, nextToken: string) => {
    setUser(nextUser);
    setToken(nextToken);
    writeStorage(storageKeys.auth, { user: nextUser, token: nextToken });
  };

  // Cikis yaparken auth durumunu temizler.
  const logout = () => {
    setUser(null);
    setToken(null);
    removeStorage(storageKeys.auth);
  };

  return (
    <AuthContext.Provider value={{ user, token, setAuth, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// Auth context'e guvenli sekilde erismek icin ortak hook sunar.
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return context;
}
