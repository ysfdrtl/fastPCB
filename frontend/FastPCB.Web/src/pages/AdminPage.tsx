import { type FormEvent, useEffect, useMemo, useState } from "react";
import { api } from "../lib/api";
import { useAuth } from "../state/AuthContext";
import type { AdminDashboardStats, AdminUser, Pagination, Project, Report } from "../types";

type AdminTab = "dashboard" | "users" | "projects" | "reports";

const userRoles = ["User", "Admin"];
const projectStatuses = ["Draft", "Published", "Featured", "Archived", "Removed"];
const reportStatuses = ["Open", "InProgress", "Resolved", "Closed"];

const emptyPagination: Pagination = {
  page: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 0
};

export function AdminPage() {
  const { token, user } = useAuth();
  const [activeTab, setActiveTab] = useState<AdminTab>("dashboard");
  const [stats, setStats] = useState<AdminDashboardStats | null>(null);
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [reports, setReports] = useState<Report[]>([]);
  const [pagination, setPagination] = useState<Record<Exclude<AdminTab, "dashboard">, Pagination>>({
    users: emptyPagination,
    projects: emptyPagination,
    reports: emptyPagination
  });
  const [filters, setFilters] = useState({
    users: { search: "", role: "", page: 1 },
    projects: { search: "", status: "", page: 1 },
    reports: { status: "", page: 1 }
  });
  const [reportResponses, setReportResponses] = useState<Record<number, string>>({});
  const [loading, setLoading] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const tabLabel = useMemo(() => ({
    dashboard: "Ozet",
    users: "Kullanicilar",
    projects: "Projeler",
    reports: "Raporlar"
  }), []);

  useEffect(() => {
    if (!token) {
      return;
    }

    const authToken = token;
    async function loadDashboard() {
      setError(null);
      try {
        const dashboard = await api.getAdminDashboard(authToken);
        setStats(dashboard);
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Admin ozeti yuklenemedi.");
      }
    }

    void loadDashboard();
  }, [token]);

  useEffect(() => {
    if (!token || activeTab === "dashboard") {
      return;
    }

    const authToken = token;
    async function loadTab() {
      setLoading(true);
      setError(null);
      setFeedback(null);

      try {
        if (activeTab === "users") {
          const params = new URLSearchParams({
            page: filters.users.page.toString(),
            pageSize: "10"
          });
          if (filters.users.search.trim()) {
            params.set("search", filters.users.search.trim());
          }
          if (filters.users.role) {
            params.set("role", filters.users.role);
          }

          const result = await api.getAdminUsers(params, authToken);
          setUsers(result.items);
          setPagination((current) => ({ ...current, users: result.pagination }));
        }

        if (activeTab === "projects") {
          const params = new URLSearchParams({
            page: filters.projects.page.toString(),
            pageSize: "10"
          });
          if (filters.projects.search.trim()) {
            params.set("search", filters.projects.search.trim());
          }
          if (filters.projects.status) {
            params.set("status", filters.projects.status);
          }

          const result = await api.getAdminProjects(params, authToken);
          setProjects(result.items);
          setPagination((current) => ({ ...current, projects: result.pagination }));
        }

        if (activeTab === "reports") {
          const params = new URLSearchParams({
            page: filters.reports.page.toString(),
            pageSize: "10"
          });
          if (filters.reports.status) {
            params.set("status", filters.reports.status);
          }

          const result = await api.getAdminReports(params, authToken);
          setReports(result.items);
          setReportResponses(Object.fromEntries(result.items.map((report) => [report.id, report.response ?? ""])));
          setPagination((current) => ({ ...current, reports: result.pagination }));
        }
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Admin verisi yuklenemedi.");
      } finally {
        setLoading(false);
      }
    }

    void loadTab();
  }, [activeTab, filters.projects, filters.reports, filters.users, token]);

  async function refreshDashboard() {
    if (!token) {
      return;
    }

    const dashboard = await api.getAdminDashboard(token);
    setStats(dashboard);
  }

  async function handleUserRoleChange(targetUser: AdminUser, role: string) {
    if (!token) {
      return;
    }

    setError(null);
    setFeedback(null);
    try {
      const updatedUser = await api.updateAdminUserRole(targetUser.id, role, token);
      setUsers((current) => current.map((item) => item.id === updatedUser.id ? updatedUser : item));
      setFeedback(`${updatedUser.email} rolu guncellendi.`);
      await refreshDashboard();
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Kullanici rolu guncellenemedi.");
    }
  }

  async function handleDeleteUser(targetUser: AdminUser) {
    if (!token || targetUser.id === user?.id || !window.confirm(`${targetUser.email} silinsin mi?`)) {
      return;
    }

    setError(null);
    setFeedback(null);
    try {
      await api.deleteAdminUser(targetUser.id, token);
      setUsers((current) => current.filter((item) => item.id !== targetUser.id));
      setFeedback(`${targetUser.email} silindi.`);
      await refreshDashboard();
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Kullanici silinemedi.");
    }
  }

  async function handleProjectStatusChange(project: Project, status: string) {
    if (!token) {
      return;
    }

    setError(null);
    setFeedback(null);
    try {
      const updatedProject = await api.updateAdminProjectStatus(project.id, status, token);
      setProjects((current) => current.map((item) => item.id === updatedProject.id ? updatedProject : item));
      setFeedback(`${updatedProject.title} durumu guncellendi.`);
      await refreshDashboard();
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Proje durumu guncellenemedi.");
    }
  }

  async function handleDeleteProject(project: Project) {
    if (!token || !window.confirm(`${project.title} silinsin mi?`)) {
      return;
    }

    setError(null);
    setFeedback(null);
    try {
      await api.deleteAdminProject(project.id, token);
      setProjects((current) => current.filter((item) => item.id !== project.id));
      setFeedback(`${project.title} silindi.`);
      await refreshDashboard();
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Proje silinemedi.");
    }
  }

  async function handleReportSubmit(event: FormEvent, report: Report, status: string) {
    event.preventDefault();
    if (!token) {
      return;
    }

    setError(null);
    setFeedback(null);
    try {
      const updatedReport = await api.updateAdminReport(report.id, status, reportResponses[report.id] ?? "", token);
      setReports((current) => current.map((item) => item.id === updatedReport.id ? updatedReport : item));
      setFeedback(`Rapor #${updatedReport.id} guncellendi.`);
      await refreshDashboard();
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Rapor guncellenemedi.");
    }
  }

  return (
    <section className="page-layout">
      <div className="hero-panel compact">
        <div>
          <span className="eyebrow">Admin paneli</span>
          <h1>FastPCB yonetimi</h1>
          <p>Kullanici, proje ve rapor akislarini tek yerden denetle.</p>
        </div>
      </div>

      <div className="admin-tabs" role="tablist">
        {(Object.keys(tabLabel) as AdminTab[]).map((tab) => (
          <button
            className={activeTab === tab ? "primary-button small" : "ghost-button small"}
            key={tab}
            onClick={() => setActiveTab(tab)}
            type="button"
          >
            {tabLabel[tab]}
          </button>
        ))}
      </div>

      {feedback ? <div className="state-box success">{feedback}</div> : null}
      {error ? <div className="state-box error">{error}</div> : null}

      {activeTab === "dashboard" ? (
        <DashboardPanel stats={stats} />
      ) : null}

      {activeTab === "users" ? (
        <section className="stack-section">
          <div className="toolbar">
            <input
              className="text-input"
              onChange={(event) => setFilters((current) => ({ ...current, users: { ...current.users, search: event.target.value, page: 1 } }))}
              placeholder="Email veya isim ara"
              value={filters.users.search}
            />
            <select
              className="text-input"
              onChange={(event) => setFilters((current) => ({ ...current, users: { ...current.users, role: event.target.value, page: 1 } }))}
              value={filters.users.role}
            >
              <option value="">Tum roller</option>
              {userRoles.map((role) => <option key={role} value={role}>{role}</option>)}
            </select>
          </div>

          <div className="admin-list">
            {loading ? <div className="state-box">Kullanicilar yukleniyor...</div> : null}
            {!loading && users.map((item) => (
              <article className="admin-row" key={item.id}>
                <div>
                  <strong>{item.firstName} {item.lastName}</strong>
                  <span>{item.email}</span>
                </div>
                <select className="text-input compact-select" onChange={(event) => void handleUserRoleChange(item, event.target.value)} value={item.role}>
                  {userRoles.map((role) => <option key={role} value={role}>{role}</option>)}
                </select>
                <button className="ghost-button small" disabled={item.id === user?.id} onClick={() => void handleDeleteUser(item)} type="button">
                  Sil
                </button>
              </article>
            ))}
          </div>

          <Pager pagination={pagination.users} onPageChange={(page) => setFilters((current) => ({ ...current, users: { ...current.users, page } }))} />
        </section>
      ) : null}

      {activeTab === "projects" ? (
        <section className="stack-section">
          <div className="toolbar">
            <input
              className="text-input"
              onChange={(event) => setFilters((current) => ({ ...current, projects: { ...current.projects, search: event.target.value, page: 1 } }))}
              placeholder="Proje ara"
              value={filters.projects.search}
            />
            <select
              className="text-input"
              onChange={(event) => setFilters((current) => ({ ...current, projects: { ...current.projects, status: event.target.value, page: 1 } }))}
              value={filters.projects.status}
            >
              <option value="">Tum durumlar</option>
              {projectStatuses.map((status) => <option key={status} value={status}>{status}</option>)}
            </select>
          </div>

          <div className="admin-list">
            {loading ? <div className="state-box">Projeler yukleniyor...</div> : null}
            {!loading && projects.map((project) => (
              <article className="admin-row" key={project.id}>
                <div>
                  <strong>{project.title}</strong>
                  <span>{project.owner.firstName} {project.owner.lastName}</span>
                </div>
                <select className="text-input compact-select" onChange={(event) => void handleProjectStatusChange(project, event.target.value)} value={project.status}>
                  {projectStatuses.map((status) => <option key={status} value={status}>{status}</option>)}
                </select>
                <button className="ghost-button small" onClick={() => void handleDeleteProject(project)} type="button">
                  Sil
                </button>
              </article>
            ))}
          </div>

          <Pager pagination={pagination.projects} onPageChange={(page) => setFilters((current) => ({ ...current, projects: { ...current.projects, page } }))} />
        </section>
      ) : null}

      {activeTab === "reports" ? (
        <section className="stack-section">
          <div className="toolbar">
            <select
              className="text-input"
              onChange={(event) => setFilters((current) => ({ ...current, reports: { ...current.reports, status: event.target.value, page: 1 } }))}
              value={filters.reports.status}
            >
              <option value="">Tum durumlar</option>
              {reportStatuses.map((status) => <option key={status} value={status}>{status}</option>)}
            </select>
          </div>

          <div className="report-list">
            {loading ? <div className="state-box">Raporlar yukleniyor...</div> : null}
            {!loading && reports.map((report) => (
              <form className="report-card admin-report-card" key={report.id} onSubmit={(event) => void handleReportSubmit(event, report, (event.currentTarget.elements.namedItem("status") as HTMLSelectElement).value)}>
                <div className="panel-banner">
                  <div>
                    <strong>{report.reason}</strong>
                    <span>{report.project?.title ?? `Proje #${report.projectId}`}</span>
                  </div>
                  <span className="muted-chip">{report.reporter?.email ?? `Kullanici #${report.userId}`}</span>
                </div>
                <p>{report.details || "Ek aciklama yok."}</p>
                <div className="form-grid">
                  <select className="text-input" name="status" defaultValue={report.status}>
                    {reportStatuses.map((status) => <option key={status} value={status}>{status}</option>)}
                  </select>
                  <button className="primary-button small" type="submit">
                    Kaydet
                  </button>
                </div>
                <textarea
                  className="text-area compact"
                  onChange={(event) => setReportResponses((current) => ({ ...current, [report.id]: event.target.value }))}
                  placeholder="Admin cevabi"
                  value={reportResponses[report.id] ?? ""}
                />
              </form>
            ))}
          </div>

          <Pager pagination={pagination.reports} onPageChange={(page) => setFilters((current) => ({ ...current, reports: { ...current.reports, page } }))} />
        </section>
      ) : null}
    </section>
  );
}

function DashboardPanel({ stats }: { stats: AdminDashboardStats | null }) {
  if (!stats) {
    return <div className="state-box">Ozet yukleniyor...</div>;
  }

  const cards = [
    ["Kullanici", stats.totalUsers],
    ["Admin", stats.adminUsers],
    ["Proje", stats.totalProjects],
    ["Taslak", stats.draftProjects],
    ["Yayinda", stats.publishedProjects],
    ["One cikan", stats.featuredProjects],
    ["Arsiv", stats.archivedProjects],
    ["Kaldirilan", stats.removedProjects],
    ["Acik rapor", stats.openReports],
    ["Islemde", stats.inProgressReports],
    ["Cozulen", stats.resolvedReports],
    ["Kapali", stats.closedReports]
  ];

  return (
    <div className="admin-stat-grid">
      {cards.map(([label, value]) => (
        <div className="stat-pill admin-stat" key={label}>
          <strong>{value}</strong>
          <span>{label}</span>
        </div>
      ))}
    </div>
  );
}

function Pager({ pagination, onPageChange }: { pagination: Pagination; onPageChange: (page: number) => void }) {
  const totalPages = Math.max(pagination.totalPages, 1);
  const page = Math.min(pagination.page, totalPages);

  return (
    <div className="pager">
      <button className="ghost-button" disabled={page <= 1} onClick={() => onPageChange(page - 1)} type="button">
        Onceki
      </button>
      <span>Sayfa {page} / {totalPages}</span>
      <button className="ghost-button" disabled={page >= totalPages} onClick={() => onPageChange(page + 1)} type="button">
        Sonraki
      </button>
    </div>
  );
}
