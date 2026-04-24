# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Vite reads VITE_* variables during build time.
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

COPY package*.json ./
RUN npm ci
RUN chmod +x node_modules/.bin/*

# Fix permissions for executables in node_modules (Alpine Linux / npm ci issue)
RUN chmod +x node_modules/.bin/*

COPY . ./
RUN npm run build

# Runtime stage
FROM node:20-alpine AS runtime
WORKDIR /app

COPY --from=build /app/dist ./dist
COPY server.js ./server.js

# Railway injects PORT at runtime; 8080 is the local/container fallback.
EXPOSE 8080

ENTRYPOINT ["node", "server.js"]
