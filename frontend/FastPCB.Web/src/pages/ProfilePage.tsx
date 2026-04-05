import { useEffect, useState } from "react";
import { ProjectCard } from "../components/ProjectCard";
import { api } from "../lib/api";
import { useAuth } from "../state/AuthContext";
import type { Project, Report } from "../types";

export function ProfilePage() {
  const { user, token } = useAuth();
  const [myProjects, setMyProjects] = useState<Project[]>([]);
  const [likedProjects, setLikedProjects] = useState<Project[]>([]);
  const [reports, setReports] = useState<Report[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadProfileData() {
      if (!user || !token) {
        return;
      }

      try {
        const [projects, likes, myReports] = await Promise.all([
          api.getUserProjects(user.id, token),
          api.getMyLikes(token),
          api.getMyReports(token)
        ]);

        setMyProjects(projects);
        setLikedProjects(likes);
        setReports(myReports);
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Profil yuklenemedi.");
      }
    }

    void loadProfileData();
  }, [token, user]);

  if (!user) {
    return null;
  }

  return (
    <section className="page-layout">
      <div className="hero-panel compact">
        <div>
          <span className="eyebrow">Profil</span>
          <h1>{user.firstName} {user.lastName}</h1>
          <p>{user.email}</p>
          <div className="hero-stats">
            <div className="stat-pill">
              <strong>{myProjects.length}</strong>
              <span>proje</span>
            </div>
            <div className="stat-pill">
              <strong>{likedProjects.length}</strong>
              <span>begeni</span>
            </div>
            <div className="stat-pill">
              <strong>{reports.length}</strong>
              <span>rapor</span>
            </div>
          </div>
        </div>
      </div>

      {error ? <div className="state-box error">{error}</div> : null}

      <section className="stack-section">
        <div className="section-heading">
          <h2>Projelerim</h2>
        </div>
        <div className="card-grid">
          {myProjects.length > 0 ? myProjects.map((project) => <ProjectCard key={project.id} project={project} />) : <div className="state-box">Henuz kendi hesabina ait proje bulunmuyor.</div>}
        </div>
      </section>

      <section className="stack-section">
        <div className="section-heading">
          <h2>Begenilenler</h2>
        </div>
        <div className="card-grid">
          {likedProjects.length > 0 ? likedProjects.map((project) => <ProjectCard key={project.id} project={project} />) : <div className="state-box">Henuz begendigin proje bulunmuyor.</div>}
        </div>
      </section>

      <section className="stack-section">
        <div className="section-heading">
          <h2>Raporlarim</h2>
        </div>
        <div className="report-list">
          {reports.length > 0 ? reports.map((report) => (
            <article className="report-card" key={report.id}>
              <strong>{report.reason}</strong>
              <p>{report.details || "Ek aciklama yok."}</p>
              <span>{report.status}</span>
            </article>
          )) : <div className="state-box">Henuz gonderdigin rapor bulunmuyor.</div>}
        </div>
      </section>
    </section>
  );
}
