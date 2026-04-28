const fallbackApiBaseUrl = "https://fastpcb-backend-production.up.railway.app/api";

export const API_BASE_URL = (
  process.env.EXPO_PUBLIC_API_BASE_URL?.trim() || fallbackApiBaseUrl
).replace(/\/$/, "");

export const API_ORIGIN = API_BASE_URL.replace(/\/api$/, "");
