const { createReadStream, existsSync, statSync } = require("node:fs");
const { createServer } = require("node:http");
const { extname, join, normalize, resolve } = require("node:path");

const root = resolve(__dirname, "dist");
const configuredPort = Number.parseInt(process.env.PORT || "8080", 10);
const port = Number.isNaN(configuredPort) ? 8080 : configuredPort;
const host = "0.0.0.0";

const contentTypes = {
  ".css": "text/css; charset=utf-8",
  ".html": "text/html; charset=utf-8",
  ".js": "application/javascript; charset=utf-8",
  ".json": "application/json; charset=utf-8",
  ".png": "image/png",
  ".jpg": "image/jpeg",
  ".jpeg": "image/jpeg",
  ".svg": "image/svg+xml",
  ".webp": "image/webp",
  ".ico": "image/x-icon"
};

function getFilePath(requestUrl) {
  const url = new URL(requestUrl, `http://${host}:${port}`);
  const decodedPath = decodeURIComponent(url.pathname);
  const requestedPath = normalize(decodedPath).replace(/^(\.\.[/\\])+/, "");
  const absolutePath = join(root, requestedPath);

  if (!absolutePath.startsWith(root)) {
    return null;
  }

  if (existsSync(absolutePath) && statSync(absolutePath).isFile()) {
    return absolutePath;
  }

  return join(root, "index.html");
}

const server = createServer((request, response) => {
  const filePath = getFilePath(request.url ?? "/");

  if (!filePath || !existsSync(filePath)) {
    response.writeHead(404);
    response.end("Not found");
    return;
  }

  const extension = extname(filePath);
  response.writeHead(200, {
    "Content-Type": contentTypes[extension] ?? "application/octet-stream"
  });

  createReadStream(filePath).pipe(response);
});

server.on("error", (error) => {
  console.error("FastPCB frontend server failed to start", error);
  process.exit(1);
});

console.log("FastPCB frontend booting", {
  root,
  port,
  railwayPort: process.env.PORT ?? null
});

server.listen(port, host, () => {
  console.log(`FastPCB frontend listening on http://${host}:${port}`);
});
