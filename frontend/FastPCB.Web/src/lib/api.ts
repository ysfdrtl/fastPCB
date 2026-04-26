import type { AdminDashboardStats, AdminUser, Comment, PagedResponse, Project, ProjectListResponse, Report, UserSummary } from "../types";

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ?? "http://localhost:5000/api";
const API_ORIGIN = API_BASE_URL.replace(/\/api$/, "");

type RequestOptions = RequestInit & {
  token?: string | null;
};

// Tum HTTP istekleri icin ortak fetch yardimcisidir ve hata cevabini anlamli mesaja cevirir.
async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const headers = new Headers(options.headers);

  if (!(options.body instanceof FormData) && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (options.token) {
    headers.set("Authorization", `Bearer ${options.token}`);
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  });

  if (response.status === 204) {
    return undefined as T;
  }

  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    const message = data?.message ?? data?.detail ?? data?.title ?? "Beklenmeyen bir hata olustu.";
    throw new Error(message);
  }

  return data as T;
}

// Frontend'in backend endpointlerine yaptigi tum cagri noktalarini tek yerde toplar.
export const api = {
  login(email: string, password: string) {
    return request<UserSummary & { token: string }>("/Auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password })
    });
  },
  register(payload: { email: string; password: string; firstName: string; lastName: string }) {
    return request<UserSummary & { token: string }>("/Auth/register", {
      method: "POST",
      body: JSON.stringify(payload)
    });
  },
  getProjects(query: URLSearchParams) {
    return request<ProjectListResponse>(`/projects?${query.toString()}`);
  },
  getProject(projectId: string) {
    return request<Project>(`/projects/${projectId}`);
  },
  getUserProjects(userId: number, token?: string | null) {
    return request<Project[]>(`/users/${userId}/projects`, { token });
  },
  createProject(payload: { title: string; description: string }, token: string) {
    return request<Project>("/projects", {
      method: "POST",
      token,
      body: JSON.stringify(payload)
    });
  },
  saveProjectDetails(projectId: number, payload: { layers: number; material: string; minDistance: number; quantity: number }, token: string) {
    return request<{ message: string; project: Project }>(`/projects/${projectId}/details`, {
      method: "POST",
      token,
      body: JSON.stringify(payload)
    });
  },
  uploadProjectFile(projectId: number, file: File, token: string) {
    const formData = new FormData();
    formData.append("file", file);

    return request<{ message: string; project: Project }>(`/projects/${projectId}/files`, {
      method: "POST",
      token,
      body: formData
    });
  },
  getComments(projectId: string) {
    return request<Comment[]>(`/projects/${projectId}/comments`);
  },
  createComment(projectId: string, content: string, token: string) {
    return request<Comment>(`/projects/${projectId}/comments`, {
      method: "POST",
      token,
      body: JSON.stringify({ content })
    });
  },
  deleteComment(commentId: number, token: string) {
    return request<void>(`/comments/${commentId}`, {
      method: "DELETE",
      token
    });
  },
  likeProject(projectId: string, token: string) {
    return request<{ message: string }>(`/projects/${projectId}/like`, {
      method: "POST",
      token
    });
  },
  unlikeProject(projectId: string, token: string) {
    return request<void>(`/projects/${projectId}/like`, {
      method: "DELETE",
      token
    });
  },
  getMyLikes(token: string) {
    return request<Project[]>("/likes/me", { token });
  },
  reportProject(projectId: string, payload: { reason: string; details: string }, token: string) {
    return request<{ message: string; report: Report }>(`/projects/${projectId}/report`, {
      method: "POST",
      token,
      body: JSON.stringify(payload)
    });
  },
  getMyReports(token: string) {
    return request<Report[]>("/reports/me", { token });
  },
  getAdminDashboard(token: string) {
    return request<AdminDashboardStats>("/admin/dashboard", { token });
  },
  getAdminUsers(query: URLSearchParams, token: string) {
    return request<PagedResponse<AdminUser>>(`/admin/users?${query.toString()}`, { token });
  },
  updateAdminUserRole(userId: number, role: string, token: string) {
    return request<AdminUser>(`/admin/users/${userId}/role`, {
      method: "PATCH",
      token,
      body: JSON.stringify({ role })
    });
  },
  deleteAdminUser(userId: number, token: string) {
    return request<void>(`/admin/users/${userId}`, {
      method: "DELETE",
      token
    });
  },
  getAdminProjects(query: URLSearchParams, token: string) {
    return request<PagedResponse<Project>>(`/admin/projects?${query.toString()}`, { token });
  },
  updateAdminProjectStatus(projectId: number, status: string, token: string) {
    return request<Project>(`/admin/projects/${projectId}/status`, {
      method: "PATCH",
      token,
      body: JSON.stringify({ status })
    });
  },
  deleteAdminProject(projectId: number, token: string) {
    return request<void>(`/admin/projects/${projectId}`, {
      method: "DELETE",
      token
    });
  },
  getAdminReports(query: URLSearchParams, token: string) {
    return request<PagedResponse<Report>>(`/admin/reports?${query.toString()}`, { token });
  },
  updateAdminReport(reportId: number, status: string, response: string, token: string) {
    return request<Report>(`/admin/reports/${reportId}`, {
      method: "PATCH",
      token,
      body: JSON.stringify({ status, response })
    });
  }
};

// Yuklenen dosya gibi statik varliklar icin tarayicinin acabilecegi tam URL uretir.
export function buildAssetUrl(path: string | null | undefined) {
  if (!path) {
    return null;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  return `${API_ORIGIN}${path.startsWith("/") ? path : `/${path}`}`;
}
