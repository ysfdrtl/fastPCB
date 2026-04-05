# Yusuf Doruatli Mobil Frontend Gorevleri

## Onerilen Mobil Akis

1. Giris
2. Kayit
3. Kesfet
4. Proje detay
5. Proje yukleme
6. Profil

## Mobil Tarafinda Oncelikli Isler

- JWT token saklama
- Proje kartlarinin mobil uyumlu listesi
- Yorum ekleme aksiyonu
- Begeni butonu
- Rapor gonderme
- Dosya secici ile proje yukleme

## Kullanilacak API'ler

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `GET /api/projects`
- `GET /api/projects/{projectId}`
- `POST /api/projects/{projectId}/comments`
- `POST /api/projects/{projectId}/like`
- `POST /api/projects/{projectId}/report`
