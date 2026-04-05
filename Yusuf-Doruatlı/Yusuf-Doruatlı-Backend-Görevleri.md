# Yusuf Doruatli Backend Gorevleri

## Guncel Odak Alanlari

1. Auth servislerinin surdurulmesi
2. Project endpointlerinin korunmasi
3. Dosya yukleme akislarinin iyilestirilmesi
4. Comment, Like ve Report servislerinin bakimi
5. Migration ve veritabani senkronizasyonu

## Sorumlu Olunan Guncel Endpointler

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `GET /api/projects`
- `POST /api/projects`
- `POST /api/projects/{projectId}/details`
- `POST /api/projects/{projectId}/files`
- `GET /api/projects/{projectId}/comments`
- `POST /api/projects/{projectId}/comments`
- `POST /api/projects/{projectId}/like`
- `POST /api/projects/{projectId}/report`
