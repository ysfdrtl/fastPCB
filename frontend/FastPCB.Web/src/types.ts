export type UserSummary = {
  id: number;
  email?: string;
  firstName: string;
  lastName: string;
  token?: string;
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
};
