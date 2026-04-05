import type { PropsWithChildren } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../state/AuthContext";

// Token gerektiren sayfalara anonim erisimi engeller.
export function ProtectedRoute({ children }: PropsWithChildren) {
  const { token } = useAuth();
  const location = useLocation();

  if (!token) {
    return <Navigate replace state={{ redirectTo: location.pathname }} to="/login" />;
  }

  return <>{children}</>;
}
