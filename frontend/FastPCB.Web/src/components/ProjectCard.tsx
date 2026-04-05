import { Link } from "react-router-dom";
import type { Project } from "../types";

export function ProjectCard({ project }: { project: Project }) {
  return (
    <article className="project-card">
      <div className="project-card-header">
        <span className={`status-badge status-${project.status.toLowerCase()}`}>{project.status}</span>
        {project.filePath ? <span className="muted-chip">Dosya var</span> : <span className="muted-chip">Taslak</span>}
      </div>

      <h3>{project.title}</h3>
      <p>{project.description}</p>

      <dl className="project-meta">
        <div>
          <dt>Malzeme</dt>
          <dd>{project.technicalDetails.material ?? "Belirtilmedi"}</dd>
        </div>
        <div>
          <dt>Katman</dt>
          <dd>{project.technicalDetails.layers ?? "-"}</dd>
        </div>
        <div>
          <dt>Sahip</dt>
          <dd>{project.owner.firstName} {project.owner.lastName}</dd>
        </div>
      </dl>

      <Link className="inline-link" to={`/projects/${project.id}`}>
        Detayi ac
      </Link>
    </article>
  );
}
