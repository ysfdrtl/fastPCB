# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Vite reads VITE_* variables during build time.
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

COPY frontend/FastPCB.Web/package*.json ./
RUN npm ci

COPY frontend/FastPCB.Web/ ./
RUN npm run build

# Runtime stage
FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/dist ./dist

# Railway injects PORT at runtime; 8080 is the local/container fallback.
EXPOSE 8080

CMD ["sh", "-c", "serve -s dist -l tcp://0.0.0.0:${PORT:-8080}"]
