# Yusuf Doruatli Web Frontend Gorevleri

## Guncel Stack

- React
- Vite
- TypeScript
- React Router

## Guncel Ekranlar

### 1. Giris Sayfasi

- `POST /api/Auth/login`
- Email ve sifre ile giris
- Basarili durumda token saklama

### 2. Kayit Sayfasi

- `POST /api/Auth/register`
- Ad, soyad, email ve sifre alanlari

### 3. Kesfet Sayfasi

- `GET /api/projects`
- Proje kartlari
- Arama ve durum filtreleme
- Sayfalama

### 4. Proje Yukleme Sayfasi

- `POST /api/projects`
- `POST /api/projects/{projectId}/details`
- `POST /api/projects/{projectId}/files`
- Zorunlu alanlar ve teknik detay formu

### 5. Proje Detay Sayfasi

- `GET /api/projects/{projectId}`
- `GET /api/projects/{projectId}/comments`
- `POST /api/projects/{projectId}/comments`
- `POST /api/projects/{projectId}/like`
- `POST /api/projects/{projectId}/report`

### 6. Profil Sayfasi

- `GET /api/users/{userId}/projects`
- `GET /api/likes/me`
- `GET /api/reports/me`
