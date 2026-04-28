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
  filePath: string | null;
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
  pagination: Pagination;
};

export type Pagination = {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
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
  details: string | null;
  status: string;
  response: string | null;
  createdAt: string;
  updatedAt: string;
  project?: {
    id: number;
    title: string;
  };
};

export type PickedUploadFile = {
  uri: string;
  name: string;
  mimeType?: string | null;
};

