export type UserSummary = {
  id: number;
  email?: string;
  firstName: string;
  lastName: string;
  token?: string;
  role?: string;
};

export type Project = {
  id: number;
  userId: number;
  title: string;
  description: string;
  filePath: string;
  technicalDetails: {
    layers: number | null;
    material: string | null;
    minDistance: number | null;
    quantity: number | null;
  };
  createdAt: string;
  updatedAt: string;
  status: string;
  owner: {
    id: number;
    firstName: string;
    lastName: string;
  };
};

export type ProjectListResponse = {
  items: Project[];
  pagination: {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };
};

export type Comment = {
  id: number;
  projectId: number;
  userId: number;
  content: string;
  createdAt: string;
  updatedAt: string;
  author: {
    id: number;
    firstName: string;
    lastName: string;
  };
};

export type Report = {
  id: number;
  projectId: number;
  userId: number;
  reason: string;
  details: string;
  status: string;
  response: string;
  createdAt: string;
  updatedAt: string;
  project?: {
    id: number;
    title: string;
  };
  reporter?: {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
  };
};

export type Pagination = {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type AdminDashboardStats = {
  totalUsers: number;
  adminUsers: number;
  totalProjects: number;
  draftProjects: number;
  publishedProjects: number;
  featuredProjects: number;
  archivedProjects: number;
  removedProjects: number;
  openReports: number;
  inProgressReports: number;
  resolvedReports: number;
  closedReports: number;
};

export type AdminUser = {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  address: string;
  role: string;
  createdAt: string;
  updatedAt: string;
};

export type PagedResponse<T> = {
  items: T[];
  pagination: Pagination;
};
