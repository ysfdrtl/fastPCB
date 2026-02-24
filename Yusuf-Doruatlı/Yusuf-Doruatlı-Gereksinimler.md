
1. **Giriş Yapma** (Yusuf Doruatlı)
   - **API Metodu:** `POST /auth/login`
   - **Açıklama:** Kullanıcıların sisteme giriş yaparak hizmetlere erişmesini sağlar. Email adresi ve şifre ile kimlik doğrulama yapılır. Başarılı giriş sonrası kullanıcıya erişim izni verilir ve kişisel verilerin güvenliği sağlanır.

2. **Üye Olma** (Yusuf Doruatlı)
   - **API Metodu:** `POST /auth/register`
   - **Açıklama:** Kullanıcıların yeni hesaplar oluşturarak sisteme kayıt olmasını sağlar. Kişisel bilgilerin toplanmasını ve hesap oluşturma işlemlerini içerir. Kullanıcılar email adresi ve şifre belirleyerek hesap oluşturur.
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

17. **Sipariş Silme** (Yusuf Doruatlı)
   - **API metodu:** `DELETE /order/delete/{orderID}`
   - **Açıklama:** sipariş uretim aşamasına geçmediyse sipariş silinir.