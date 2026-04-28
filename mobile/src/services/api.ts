import { API_BASE_URL, API_ORIGIN } from "@/config/env";
import type { Comment, PickedUploadFile, Project, ProjectListResponse, Report, UserSummary } from "@/types";

type RequestOptions = RequestInit & {
  token?: string | null;
};

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
  uploadProjectFile(projectId: number, file: PickedUploadFile, token: string) {
    const formData = new FormData();
    formData.append("file", {
      uri: file.uri,
      name: file.name,
      type: file.mimeType || "application/octet-stream"
    } as unknown as Blob);

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
  }
};

export function buildAssetUrl(path: string | null | undefined) {
  if (!path) {
    return null;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  return `${API_ORIGIN}${path.startsWith("/") ? path : `/${path}`}`;
}

