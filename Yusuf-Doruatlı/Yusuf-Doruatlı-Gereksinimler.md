# Yusuf Doruatli Gereksinimleri

1. **Giris Yapma**
   - **API:** `POST /api/Auth/login`
   - **Aciklama:** Kullanici email ve sifre ile giris yapar, JWT token alir.

2. **Uye Olma**
   - **API:** `POST /api/Auth/register`
   - **Aciklama:** Yeni kullanici hesabi olusturulur.

3. **Proje Olusturma**
   - **API:** `POST /api/projects`
   - **Aciklama:** Kullanici yeni PCB projesi baslatir.

4. **Teknik Detay Kaydetme**
   - **API:** `POST /api/projects/{projectId}/details`
   - **Aciklama:** Katman, malzeme, min mesafe ve adet bilgileri kaydedilir.

5. **Dosya Yukleme**
   - **API:** `POST /api/projects/{projectId}/files`
   - **Aciklama:** Gerber, KiCad, ZIP veya gorsel dosyasi projeye eklenir.

6. **Proje Listeleme**
   - **API:** `GET /api/projects`
   - **Aciklama:** Projeler filtreli ve sayfali sekilde listelenir.

7. **Yorum Sistemi**
   - **API:** `GET/POST /api/projects/{projectId}/comments`
   - **Aciklama:** Kullanici projelere yorum ekler ve yorumlari gorur.

8. **Begeni Sistemi**
   - **API:** `POST/DELETE /api/projects/{projectId}/like`
   - **Aciklama:** Kullanici proje begenecek veya begenisini kaldiracaktir.

9. **Raporlama**
   - **API:** `POST /api/projects/{projectId}/report`
   - **Aciklama:** Uygunsuz veya eksik icerik moderasyon icin raporlanir.
