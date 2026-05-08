# Yusuf Doruatli Gereksinimleri

1. **Uye Olma**
   - **API Metodu:** `POST /api/Auth/register`
   - **Aciklama:** Kullanici email, sifre, ad ve soyad bilgileriyle yeni hesap olusturur. Basarili kayit sonrasinda sistem kullanici bilgisini ve giris icin kullanilacak JWT tokenini dondurur.

2. **Giris Yapma**
   - **API Metodu:** `POST /api/Auth/login`
   - **Aciklama:** Kayitli kullanici email ve sifresi ile sisteme giris yapar. Basarili giris sonrasinda korumali endpointlerde kullanilacak JWT tokeni alir.

3. **Proje Olusturma**
   - **API Metodu:** `POST /api/projects`
   - **Aciklama:** Giris yapan kullanici yeni bir PCB projesi olusturur. Bu adim proje basligi ve aciklamasinin kaydedildigi ilk proje olusturma adimidir.

4. **Proje Teknik Bilgisi Girme**
   - **API Metodu:** `POST /api/projects/{projectId}/details`
   - **Aciklama:** Proje sahibinin katman sayisi, malzeme, minimum iz araligi ve uretim adedi gibi teknik detaylari kaydetmesini saglar. Bu islem yalnizca ilgili projenin sahibi tarafindan yapilabilir.

5. **Proje Dosyasi Yukleme**
   - **API Metodu:** `POST /api/projects/{projectId}/files`
   - **Aciklama:** Proje sahibinin projeye Gerber, KiCad, ZIP veya gorsel gibi dosyalar yuklemesini saglar. Dosya yukleme sonrasi proje kaydindaki dosya yolu guncellenir ve yuklenen icerik statik olarak erisilebilir hale gelir.

6. **Proje Listeleme ve Filtreleme**
   - **API Metodu:** `GET /api/projects`
   - **Aciklama:** Sistemdeki projelerin arama, durum, dosya varligi, sayfa ve sayfa boyutu parametreleriyle listelenmesini saglar. Bu gereksinim kesfet sayfasinin ana veri kaynagidir.

7. **Proje Detayi Goruntuleme**
   - **API Metodu:** `GET /api/projects/{projectId}`
   - **Aciklama:** Tek bir projenin aciklamasi, sahibi, teknik detaylari, dosya yolu ve durum bilgisinin ayrintili olarak goruntulenmesini saglar.

8. **Kullaniciya Ait Projeleri Goruntuleme**
   - **API Metodu:** `GET /api/users/{userId}/projects`
   - **Aciklama:** Belirli bir kullaniciya ait projelerin ayri bir liste halinde goruntulenmesini saglar. Profil ve kullanici odakli listeleme ekranlari bu gereksinime dayanir.

9. **Yorumlari Yonetme**
   - **API Metotlari:** `GET /api/projects/{projectId}/comments`, `POST /api/projects/{projectId}/comments`, `DELETE /api/comments/{commentId}`
   - **Aciklama:** Kullanici bir proje altindaki yorumlari listeleyebilir, giris yaptiginda yeni yorum ekleyebilir ve kendi yorumunu silebilir. Boylece proje detay sayfasinda temel topluluk etkilesimi saglanir.

10. **Begenileri Yonetme**
   - **API Metotlari:** `POST /api/projects/{projectId}/like`, `DELETE /api/projects/{projectId}/like`, `GET /api/likes/me`
   - **Aciklama:** Kullanici projeyi begenebilir, onceki begenisini kaldirabilir ve kendi begendigi projeleri ayri bir listede goruntuleyebilir. Bu gereksinim hem etkilesim hem de kisisel takip akisini kapsar.

11. **Raporlari Yonetme**
   - **API Metotlari:** `POST /api/projects/{projectId}/report`, `GET /api/reports/me`
   - **Aciklama:** Kullanici uygunsuz veya hatali buldugu bir projeyi moderasyon icin raporlayabilir ve daha once olusturdugu raporlarin durumunu kendi profil alaninda takip edebilir.

12. **Admin Moderasyonu Yapma**
   - **API Metotlari:** `GET /api/admin/dashboard`, `GET /api/admin/users`, `PATCH /api/admin/users/{userId}/role`, `DELETE /api/admin/users/{userId}`, `GET /api/admin/projects`, `PATCH /api/admin/projects/{projectId}/status`, `DELETE /api/admin/projects/{projectId}`, `GET /api/admin/reports`, `PATCH /api/admin/reports/{reportId}`
   - **Aciklama:** Yalnizca admin rolundeki kullanicinin dashboard istatistiklerini goruntulemesini, kullanici rollerini yonetmesini, projeleri moderasyon amaciyla guncellemesini veya silmesini ve kullanici raporlarini inceleyip durumlandirmasini saglar. Repo yapisindaki `AdminController` ve web tarafindaki admin sayfasi bu gereksinimin dogrudan karsiligidir.

## Kontrol Edilmesi Gereken Gereksinim

- **Profil Guncelleme / Hesap Silme:** Ornek projede profil yonetimi odakli gereksinim ornekleri yer alsa da mevcut FastPCB yapisinda kullanicinin kendi profilini guncelledigi veya hesabini sildigi public bir controller bulunmuyor. Bu akisin ders kapsamina dahil olup olmadigi hocayla netlestirilmelidir.
