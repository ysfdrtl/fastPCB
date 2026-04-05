import { type FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../lib/api";
import { useAuth } from "../state/AuthContext";

const MATERIAL_OPTIONS = [
  "FR4",
  "Aluminum",
  "CEM-1",
  "CEM-3",
  "Flexible Polyimide",
  "Rogers"
] as const;

export function UploadPage() {
  const navigate = useNavigate();
  const { token } = useAuth();
  const [form, setForm] = useState({
    title: "",
    description: "",
    layers: 2,
    material: "FR4",
    minDistance: 0.2,
    quantity: 1
  });
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!token) {
      return;
    }

    if (!form.title.trim()) {
      setError("Proje basligi bos birakilamaz.");
      return;
    }

    if (!form.description.trim()) {
      setError("Proje aciklamasi bos birakilamaz.");
      return;
    }

    if (!form.material.trim()) {
      setError("Malzeme bilgisi bos birakilamaz.");
      return;
    }

    if (form.layers <= 0 || form.quantity <= 0 || form.minDistance <= 0) {
      setError("Teknik detay alanlari gecerli degerler icermeli.");
      return;
    }

    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const project = await api.createProject(
        {
          title: form.title.trim(),
          description: form.description.trim()
        },
        token
      );

      await api.saveProjectDetails(
        project.id,
        {
          layers: form.layers,
          material: form.material.trim(),
          minDistance: form.minDistance,
          quantity: form.quantity
        },
        token
      );

      if (file) {
        await api.uploadProjectFile(project.id, file, token);
      }

      setSuccess("Proje basariyla olusturuldu.");
      navigate(`/projects/${project.id}`);
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Proje olusturulamadi.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="page-layout">
      <div className="section-heading">
        <span className="eyebrow">Yeni proje</span>
        <h1>PCB paylasimini yayina hazirla</h1>
        <p className="section-copy">
          Baslik, kisa aciklama ve teknik bilgilerle projeni duzenle. Dosya eklemek zorunlu degil ama kesifte daha guclu gorunmesini saglar.
        </p>
      </div>

      <form className="editor-panel" onSubmit={handleSubmit}>
        <div className="panel-banner">
          <div>
            <strong>Yayin akisi</strong>
            <span>1. Kimlik 2. Teknik detay 3. Dosya ekleme</span>
          </div>
          <span className="muted-chip">{file ? "Dosya hazir" : "Dosya opsiyonel"}</span>
        </div>

        <p className="required-note">
          <span className="required-mark">*</span> ile isaretli alanlar zorunludur.
        </p>

        <label className="field-block">
          <span>
            Proje Basligi <span className="required-mark">*</span>
          </span>
          <input className="text-input" onChange={(event) => setForm((current) => ({ ...current, title: event.target.value }))} placeholder="Proje basligi" value={form.title} />
        </label>

        <label className="field-block">
          <span>
            Proje Aciklamasi <span className="required-mark">*</span>
          </span>
          <textarea className="text-area" onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))} placeholder="Projeyi ne icin yaptin, hangi kartlar kullanildi, neyi ogrenmek istiyorsun?" value={form.description} />
        </label>

        <div className="form-grid labeled-grid">
          <label className="field-block">
            <span>
              Katman Sayisi <span className="required-mark">*</span>
            </span>
            <input className="text-input" min={1} onChange={(event) => setForm((current) => ({ ...current, layers: Number(event.target.value) }))} type="number" value={form.layers} />
            <small>PCB kac katmandan olusuyor?</small>
          </label>

          <label className="field-block">
            <span>
              Malzeme <span className="required-mark">*</span>
            </span>
            <select className="text-input" onChange={(event) => setForm((current) => ({ ...current, material: event.target.value }))} value={form.material}>
              {MATERIAL_OPTIONS.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
            <small>Kartin ana malzemesi.</small>
          </label>

          <label className="field-block">
            <span>
              Minimum Iz Araligi <span className="required-mark">*</span>
            </span>
            <input className="text-input" min={0.01} onChange={(event) => setForm((current) => ({ ...current, minDistance: Number(event.target.value) }))} step={0.01} type="number" value={form.minDistance} />
            <small>Teknik detaylar icin mm cinsinden en kucuk mesafe.</small>
          </label>

          <label className="field-block">
            <span>
              Adet <span className="required-mark">*</span>
            </span>
            <input className="text-input" min={1} onChange={(event) => setForm((current) => ({ ...current, quantity: Number(event.target.value) }))} type="number" value={form.quantity} />
            <small>Bu tasarim icin hedeflenen uretim adedi.</small>
          </label>
        </div>

        <label className="upload-dropzone">
          <span>Gerber, KiCad, ZIP veya gorsel ekle</span>
          <input onChange={(event) => setFile(event.target.files?.[0] ?? null)} type="file" />
          <strong>{file ? file.name : "Dosya secilmedi"}</strong>
          <small>Desteklenenler: zip, rar, 7z, gbr, gerber, kicad_pcb, png, jpg, pdf</small>
        </label>

        {error ? <div className="state-box error">{error}</div> : null}
        {success ? <div className="state-box success">{success}</div> : null}

        <button className="primary-button" disabled={loading} type="submit">
          {loading ? "Yayin hazirlaniyor..." : "Projeyi yayinla"}
        </button>
      </form>
    </section>
  );
}
