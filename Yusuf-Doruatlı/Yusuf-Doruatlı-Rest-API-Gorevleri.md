# Yusuf Doruatli REST API Gorevleri

## Auth

- `POST /api/Auth/register`
- `POST /api/Auth/login`

## Projects

- `GET /api/projects`
- `POST /api/projects`
- `GET /api/projects/{projectId}`
- `POST /api/projects/{projectId}/details`
- `POST /api/projects/{projectId}/files`
- `DELETE /api/projects/{projectId}`

## Comments

- `GET /api/projects/{projectId}/comments`
- `POST /api/projects/{projectId}/comments`
- `DELETE /api/comments/{commentId}`

## Likes

- `POST /api/projects/{projectId}/like`
- `DELETE /api/projects/{projectId}/like`
- `GET /api/likes/me`

## Reports

- `POST /api/projects/{projectId}/report`
- `GET /api/reports/me`
