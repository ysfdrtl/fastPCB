import { startTransition, useDeferredValue, useEffect, useState } from "react";
import { ProjectCard } from "../components/ProjectCard";
import { api } from "../lib/api";
import type { Project } from "../types";

export function DiscoverPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [searchInput, setSearchInput] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const deferredSearch = useDeferredValue(searchInput);

  useEffect(() => {
    async function loadProjects() {
      setLoading(true);
      setError(null);

      try {
        const params = new URLSearchParams({
          page: page.toString(),
          pageSize: "9"
        });

        if (deferredSearch.trim()) {
          params.set("search", deferredSearch.trim());
        }

        if (status) {
          params.set("status", status);
        }

        const data = await api.getProjects(params);
        startTransition(() => {
          setProjects(data.items);
          setTotalPages(Math.max(data.pagination.totalPages, 1));
        });
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Projeler yuklenemedi.");
      } finally {
        setLoading(false);
      }
    }

    void loadProjects();
  }, [deferredSearch, page, status]);

  return (
    <section className="page-layout">
      <div className="hero-panel">
        <div>
          <span className="eyebrow">Topluluga acik pcb galerisi</span>
          <h1>Dosyani yukle, kartini anlat, yeni tasarimlar kesfet.</h1>
          <p>
            FastPCB, KiCad ve Gerber paylasimlarini tek yerde toplayan hizli bir vitrin.
            Tasarim notlari, dosya ekleri ve yorumlarla projeler canlaniyor.
          </p>
          <div className="hero-stats">
            <div className="stat-pill">
              <strong>{projects.length}</strong>
              <span>gorunen proje</span>
            </div>
            <div className="stat-pill">
              <strong>{status || "Tum"}</strong>
              <span>durum filtresi</span>
            </div>
          </div>
        </div>
      </div>

      <div className="toolbar">
        <input
          className="text-input"
          onChange={(event) => {
            setPage(1);
            setSearchInput(event.target.value);
          }}
          placeholder="STM32, power board, fr4..."
          value={searchInput}
        />

        <select
          className="text-input"
          onChange={(event) => {
            setPage(1);
            setStatus(event.target.value);
          }}
          value={status}
        >
          <option value="">Tum durumlar</option>
          <option value="Draft">Draft</option>
          <option value="Published">Published</option>
          <option value="Featured">Featured</option>
          <option value="Archived">Archived</option>
          <option value="Removed">Removed</option>
        </select>
      </div>

      {loading ? <div className="state-box">Projeler yukleniyor...</div> : null}
      {error ? <div className="state-box error">{error}</div> : null}

      {!loading && !error ? (
        <>
          <div className="card-grid">
            {projects.length > 0 ? (
              projects.map((project) => <ProjectCard key={project.id} project={project} />)
            ) : (
              <div className="state-box">Bu filtrelerde proje bulunamadi.</div>
            )}
          </div>

          <div className="pager">
            <button className="ghost-button" disabled={page <= 1} onClick={() => setPage((current) => current - 1)} type="button">
              Onceki
            </button>
            <span>Sayfa {page} / {totalPages}</span>
            <button className="ghost-button" disabled={page >= totalPages} onClick={() => setPage((current) => current + 1)} type="button">
              Sonraki
            </button>
          </div>
        </>
      ) : null}
    </section>
  );
}
