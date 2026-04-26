import type { PropsWithChildren } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../state/AuthContext";

type ProtectedRouteProps = PropsWithChildren<{
  requiredRole?: string;
}>;

// Token gerektiren sayfalara anonim erisimi ve gerekirse rol disi erisimi engeller.
export function ProtectedRoute({ children, requiredRole }: ProtectedRouteProps) {
  const { token, user } = useAuth();
  const location = useLocation();

  if (!token) {
    return <Navigate replace state={{ redirectTo: location.pathname }} to="/login" />;
  }

  if (requiredRole && user?.role !== requiredRole) {
    return <Navigate replace to="/" />;
  }

  return <>{children}</>;
}
