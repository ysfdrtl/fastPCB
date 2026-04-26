import { type FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { api } from "../lib/api";
import { useAuth } from "../state/AuthContext";

export function RegisterPage() {
  const navigate = useNavigate();
  const { setAuth } = useAuth();
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: ""
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await api.register(form);
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
      navigate("/profile");
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Kayit olusturulamadi.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="form-page">
      <form className="glass-form" onSubmit={handleSubmit}>
        <span className="eyebrow">Topluluga katil</span>
        <h1>Yeni Hesap</h1>
        <div className="form-grid">
          <input className="text-input" onChange={(event) => setForm((current) => ({ ...current, firstName: event.target.value }))} placeholder="Ad" value={form.firstName} />
          <input className="text-input" onChange={(event) => setForm((current) => ({ ...current, lastName: event.target.value }))} placeholder="Soyad" value={form.lastName} />
        </div>
        <input className="text-input" onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))} placeholder="Email" type="email" value={form.email} />
        <input className="text-input" onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))} placeholder="Sifre" type="password" value={form.password} />
        {error ? <div className="state-box error">{error}</div> : null}
        <button className="primary-button" disabled={loading} type="submit">
          {loading ? "Kaydediliyor..." : "Hesap olustur"}
        </button>
        <p className="helper-text">
          Zaten hesabin var mi? <Link to="/login">Giris yap</Link>
        </p>
      </form>
    </section>
  );
}
