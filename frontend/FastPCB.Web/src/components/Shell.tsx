import type { PropsWithChildren } from "react";
import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../state/AuthContext";

export function Shell({ children }: PropsWithChildren) {
  const { user, logout } = useAuth();

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link className="brand" to="/">
          <span className="brand-mark">FP</span>
          <span>
            <strong>FastPCB</strong>
            <small>PCB share space</small>
          </span>
        </Link>

        <nav className="topnav">
          <NavLink to="/">Kesfet</NavLink>
          <NavLink to="/upload">Yukle</NavLink>
          {user ? <NavLink to="/profile">Profil</NavLink> : null}
          {user?.role === "Admin" ? <NavLink to="/admin">Admin</NavLink> : null}
        </nav>

        <div className="topbar-actions">
          {user ? (
            <>
              <span className="user-pill">{user.firstName} {user.lastName}</span>
              <button className="ghost-button" onClick={logout} type="button">
                Cikis
              </button>
            </>
          ) : (
            <>
              <Link className="ghost-button" to="/login">Giris</Link>
              <Link className="primary-button small" to="/register">Kayit Ol</Link>
            </>
          )}
        </div>
      </header>

      <main className="page-frame">{children}</main>
    </div>
  );
}
