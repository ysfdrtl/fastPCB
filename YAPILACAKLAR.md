# FastPCB Yapilacaklar

Bu dosya, projenin yeni kapsamına gore guncellenmistir.
Proje artik PCB uretim ve odeme sistemi degil; kullanicilarin PCB dosyalarini yukleyip paylastigi bir platform olarak devam edecektir.

## Mevcut Durum Ozeti

- Backend katmanli yapi hazir.
- Authentication akisi temel seviyede calisiyor.
- Veritabani, migration ve JWT altyapisi mevcut.
- Dokumantasyon yeni proje yonune gore guncellendi.
- API yuzeyi `projects` odakli hale getirildi.
- Domain isimleri `Project` etrafinda toplandi.
- Frontend iskeleti kuruldu.

## Yeni Oncelik Sirasi

### 1. API yuzeyinde siparis dilini proje diline cevir

Durum:
- Tamamlandi.

Yapilanlar:
- `api/projects` endpointleri eklendi
- Swagger tarafinda ana kaynak `projects` oldu
- `ProjectController` aktif hale getirildi

### 2. Domain modelini yeniden adlandir

Durum:
- Tamamlandi.

Yapilanlar:
- `Project` modeli eklendi
- `ProjectService` aktif hale getirildi
- `ProjectConfiguration` eklendi
- `FastPCBContext` artik `DbSet<Project>` kullaniyor
- Veritabani haritalamasi yeni `Projects` tablosuna gore hazirlandi

Not:
- Migration klasorundeki eski dosyalar tarihsel olarak eski isimler tasiyor olabilir
- Yeni migration olusturuldugunda veritabani bu yeni modele gore guncellenecek

### 3. Proje yukleme akisini tamamla

Durum:
- Baslandi.

Dosyalar:
- `backend/FastPCB.API/Controllers/ProjectController.cs`
- `backend/FastPCB.Services/` altinda dosya yukleme servisi
- `backend/FastPCB.API/Program.cs`

Yapilacaklar:
- PCB dosyasi, gorsel veya zip yukleme
- dosya yolu kaydetme
- temel dosya dogrulama

Bu tur yapilanlar:
- `POST /api/projects/{projectId}/files` endpointi eklendi
- dosyalar `uploads/projects/{projectId}` altina kaydediliyor
- uzanti ve maksimum boyut kontrolu eklendi
- yuklenen dosyanin yolu `Project.FilePath` alanina yaziliyor

### 4. Proje listeleme ve detay sayfasi API'lerini netlestir

Durum:
- Baslandi.

Dosyalar:
- `backend/FastPCB.API/Controllers/ProjectController.cs`
- `backend/FastPCB.Services/ProjectService.cs`

Yapilacaklar:
- proje listeleme
- proje detay getirme
- kullaniciya ait projeleri getirme
- arama ve filtreleme icin uygun query parametreleri ekleme

Bu tur yapilanlar:
- `GET /api/users/{userId}/projects` endpointi eklendi
- `GET /api/projects` icin `search`, `status`, `hasFile`, `userId` filtreleri eklendi
- proje cevaplarina temel sahip bilgisi eklendi
- `GET /api/projects` icin sayfalama eklendi
- teknik detaylar `description` yerine ayri alanlarda tutuluyor

### 5. Etkilesim katmanini ekle

Durum:
- Tamamlandi.

Dosyalar:
- yeni controller ve model dosyalari

Yapilacaklar:
- yorum sistemi
- begenme veya kaydetme
- raporlama veya moderasyon

Bu tur yapilanlar:
- `Comment` modeli eklendi
- `GET /api/projects/{projectId}/comments` endpointi eklendi
- `POST /api/projects/{projectId}/comments` endpointi eklendi
- yorum servis katmani ve EF konfigurasyonu baglandi
- `DELETE /api/comments/{commentId}` endpointi eklendi
- `ProjectLike` modeli eklendi
- `POST /api/projects/{projectId}/like` endpointi eklendi
- `DELETE /api/projects/{projectId}/like` endpointi eklendi
- `GET /api/likes/me` endpointi eklendi
- `POST /api/projects/{projectId}/report` endpointi eklendi
- `GET /api/reports/me` endpointi eklendi
- raporlama servis katmani ve proje iliskisi baglandi

### 6. Frontend projesini baslat

Durum:
- Baslandi.

Dosyalar:
- `frontend/FastPCB.Web/`

Yapilacaklar:
- giris ve kayit
- proje listeleme
- proje detay
- proje yukleme
- profil sayfasi

Bu tur yapilanlar:
- React + Vite + TypeScript frontend iskeleti kuruldu
- giris, kayit, kesfet, proje detay, yukleme ve profil sayfalari eklendi
- auth durumu local storage ile baglandi
- backend endpointlerine baglanan temel API katmani eklendi

## Hizli Kontrol Listesi

- [x] Dokumantasyon yeni konsepte gore guncellendi
- [x] Auth temeli var
- [x] `api/projects` endpointleri eklendi
- [x] Domain isimleri yeni konsepte cevrildi
- [x] Dosya yukleme temeli var
- [x] Yorum / etkilesim sistemi eklendi
- [x] Frontend baslatildi
