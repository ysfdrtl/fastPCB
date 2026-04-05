# Yusuf Doruatli Mobil Backend Gorevleri

Bu belge, mobil istemci gelistirildiginde REST API ile kurulacak baglanti akislarini ve sorumluluk alanlarini ozetler.

## Planlanan Mobil Backend Gorevleri

1. **Auth Entegrasyonu**
   - `POST /api/Auth/register`
   - `POST /api/Auth/login`
   - Mobil istemciden kayit ve giris akislarini baglama

2. **Proje Listeleme Entegrasyonu**
   - `GET /api/projects`
   - Arama, filtreleme ve sayfalama parametrelerini mobil istemciye baglama

3. **Proje Detay Entegrasyonu**
   - `GET /api/projects/{projectId}`
   - Proje teknik detaylarini ve sahibi bilgisini gosterecek veri akisini baglama

4. **Dosya Yukleme Entegrasyonu**
   - `POST /api/projects/{projectId}/files`
   - Mobil cihazdan proje dosyasi secme ve yukleme akislarini hazirlama

5. **Etkilesim Entegrasyonu**
   - `GET/POST /api/projects/{projectId}/comments`
   - `POST/DELETE /api/projects/{projectId}/like`
   - `POST /api/projects/{projectId}/report`
   - Mobil istemciden yorum, begeni ve rapor isteklerini yonetme

## Not

Mobil istemci henuz gelistirilmemis olsa da API yuzeyi bu entegrasyonlari destekleyecek sekilde hazirdir.
