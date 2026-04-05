import { Navigate, Route, Routes } from "react-router-dom";
import { Shell } from "./components/Shell";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { DiscoverPage } from "./pages/DiscoverPage";
import { LoginPage } from "./pages/LoginPage";
import { ProfilePage } from "./pages/ProfilePage";
import { ProjectDetailPage } from "./pages/ProjectDetailPage";
import { RegisterPage } from "./pages/RegisterPage";
import { UploadPage } from "./pages/UploadPage";

// Uygulamanin tum sayfa rotalarini ve korumali alanlarini tanimlar.
export function App() {
  return (
    <Shell>
      <Routes>
        <Route path="/" element={<DiscoverPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/projects/:projectId" element={<ProjectDetailPage />} />
        <Route
          path="/upload"
          element={
            <ProtectedRoute>
              <UploadPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate replace to="/" />} />
      </Routes>
    </Shell>
  );
}
