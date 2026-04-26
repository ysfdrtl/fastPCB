import { type FormEvent, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { api } from "../lib/api";
import { useAuth } from "../state/AuthContext";

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { setAuth } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await api.login(email, password);
      setAuth(
        {
          id: response.id,
          email: response.email,
          firstName: response.firstName,
          lastName: response.lastName,
          role: response.role ?? "User"
        },
        response.token
      );
      navigate((location.state as { redirectTo?: string } | null)?.redirectTo ?? "/profile");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Giris yapilamadi.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="form-page">
      <form className="glass-form" onSubmit={handleSubmit}>
        <span className="eyebrow">Hesabina don</span>
        <h1>Giris Yap</h1>
        <input className="text-input" onChange={(event) => setEmail(event.target.value)} placeholder="Email" type="email" value={email} />
        <input className="text-input" onChange={(event) => setPassword(event.target.value)} placeholder="Sifre" type="password" value={password} />
        {error ? <div className="state-box error">{error}</div> : null}
        <button className="primary-button" disabled={loading} type="submit">
          {loading ? "Giris yapiliyor..." : "Devam et"}
        </button>
        <p className="helper-text">
          Hesabin yok mu? <Link to="/register">Kayit ol</Link>
        </p>
      </form>
    </section>
  );
}
