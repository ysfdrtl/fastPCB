# Gereksinim Analizi

Tüm gereksinimlerinizi çıkardıktan sonra beraber tartışıyoruz ve son gereksinimlerin isimlerini hangi API metoduna karşılık geleceğini ve kısa açıklamalarını buraya numaralı bir şekilde yazıyorsunuz. Daha sonra aşağıya herkes kendi gereksinimiyle ilgili sayfayı oluşturmalı ve kendi sayfasında kendine ait gereksinimleri numaralı bir şekilde listeleyerek her bir gereksinimin açıklamalarını yazmalı. Toplamda grup üyesi sayısı kadar sayfa oluşturulmalı. Her grup üyesine eşit sayıda gereksinim atanmalı.

## Gereksinim Sayıları (En Az)

- **1 Kişi:** 10 gereksinim
- **2 Kişi:** 16 gereksinim
- **3 Kişi:** 21 gereksinim
- **4 Kişi:** 24 gereksinim
- **5 Kişi:** 30 gereksinim

## Gereksinimlerde Uyulması Gereken Kurallar

1. **İsimler anlamlı olmalı:** Gereksinim isimleri net ve anlaşılır olmalıdır.
2. **Açıklamalar net olmalı:** Her gereksinimin açıklaması açık ve anlaşılır şekilde yazılmalıdır.
3. **Açıklamalar teknik jargon ve kısaltmalar içermemeli:** Gereksinim açıklamaları herkesin anlayabileceği basit bir dille yazılmalıdır.
4. **Gereksinim isimleri çok uzun olmamalı ve bir eylem bildirmeli:** 
   - İsimler kısa ve öz olmalıdır
   - Bir eylem fiili içermelidir
   - Örnekler: "Kayıt Olma", "Giriş Yapma", "Profil Güncelleme", "Hesap Silme"

# Tüm Gereksinimler 

1. **Giriş Yapma** (Yusuf Doruatlı)
   - **API Metodu:** `POST /auth/login`
   - **Açıklama:** Kullanıcıların sisteme giriş yaparak hizmetlere erişmesini sağlar. Email adresi ve şifre ile kimlik doğrulama yapılır. Başarılı giriş sonrası kullanıcıya erişim izni verilir ve kişisel verilerin güvenliği sağlanır.

2. **Üye Olma** (Yusuf Doruatlı)
   - **API Metodu:** `POST /auth/register`
   - **Açıklama:** Kullanıcıların yeni hesaplar oluşturarak sisteme kayıt olmasını sağlar. Kişisel bilgilerin toplanmasını ve hesap oluşturma işlemlerini içerir. Kullanıcılar email adresi ve şifre belirleyerek hesap oluşturur.
3. **Şifre Sıfırlama** (Yusuf Doruatlı)
   - **API Metodu:** `POST /auth/paswordReset`
   - **Açıklama:** kullanıcı e postasına gelicek bir mail ile şifre sıfırlama işlemi yapabilir.
4. **Profil bilgileri Güncelleme** (Yusuf Doruatlı)
   - **API Metodu:** `PUT /auth/Update`
   - **Açıklama:** kullanıcı ad, soyad, adres vb. bilgileri güncelleyebilcek.

5. **Sipariş Oluştur** (Yusuf Doruatlı)
   - **API Metodu:** `POST /order/create`
   - **Açıklama:** kullanıcı yeni sipariş oluşturcak siapriş ile bilgiler kaydedilcek.
6. **Dosya Yükleme** (Yusuf Doruatlı)
   - **API metodu:** `POST /order/Upload`
   - **Açıklama:** kullanıcının yapmak istediği Pcb'nin gerber dosyasını yüklemesi.

7. **Teknik bilgi detayları** (Yusuf Doruatlı)
   - **API metodu:** `POST /order/summary`
   - **Açıklama:** siparişte istenen PCBnin detay bilgisinin verilmesi(katman sayısı,kanalllar arası min mesafe,malzeme çeşitleri vb).
8. **otomatik fiyat hesabı** (Yusuf Doruatlı)
   - **API metodu:** `GET /order/getPrice`
   - **Açıklama:** kullanıcnın yüklemiş olduğu bilgilerden fiyat hesaplayıp ekrana dödürülmesi.
9. **sipariş durumu takibi** (Yusuf Doruatlı)
   - **API metodu:** `GET /order/tracking`
   - **Açıklama:** siparişin ne durumda olduğu kullanıcıya döncek(inceleme,üretim,kargoda,teslim edildi).

10. **Online Ödeme** (Yusuf Doruatlı)
   - **API metodu:** `POST /order/payment`
   - **Açıklama:** kullanıcının siparişi tamamlayıp odeme yapması
11. **sipariş aktifleştirme** (Yusuf Doruatlı)
   - **API metodu:** `POST /order/paymentCheck`
   - **Açıklama:** siparişin son kontroller ile birlikte onaylanması.

12. **admin sipariş incelemesi** (Yusuf Doruatlı)
   - **API metodu:** `GET /order/orders`
   - **Açıklama:** admin panelinden tum siparişlerin görunmesi
13. **sipariş durumu guncelleme** (Yusuf Doruatlı)
   - **API metodu:** `PUT /order/UpdateTrace`
   - **Açıklama:** siparişin durumunun güncelleme.
14. **Kullanıcı yönetimi** (Yusuf Doruatlı)
   - **API metodu:** `PUT /auth/auths`
   - **Açıklama:** kullanıcı profillerini inceleyip bu profilleri güncellenmesi.

15. **Destek Talebi oluşturma** (Yusuf Doruatlı)
   - **API metodu:** `POST /ticket/create`
   - **Açıklama:** kullanıcının teknik destek yardımı alması için ticket oluşturması.
16. **destek talebi cevaplama** (Yusuf Doruatlı)
   - **API metodu:** `POST /ticket/response`
   - **Açıklama:** Admin panelinden ticket'a cevap verme işlemi.




# Gereksinim Dağılımları

1.  [Yusuf Doruatlı'nın Gereksinimleri](Yusuf-Doruatlı/Yusuf-Doruatlı-Backend-Görevleri.md)