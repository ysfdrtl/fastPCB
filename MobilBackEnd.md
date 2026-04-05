# Mobil Backend (REST API Baglantisi) Gorev Dagilimi

**REST API Adresi:** `http://localhost:5000/api`

Bu dokumanda, mobil uygulamanin REST API ile iletisimini saglayan entegrasyon gorevleri listelenmektedir. Mevcut asamada mobil istemci gelistirilmemistir; belge planlama ve is bolumu icin hazirdir.

---

## Grup Uyelerinin Mobil Backend Gorevleri

1. [Yusuf Doruatli'nin Mobil Backend Gorevleri](Yusuf-Doruatlı/Yusuf-Doruatlı-Mobil-Backend-Gorevleri.md)

---

## Genel Mobil Backend Prensipleri

### 1. HTTP Client Yapilandirmasi
- Base URL: `http://localhost:5000/api`
- `Content-Type: application/json`
- Yetkili endpointlerde `Authorization: Bearer {token}`

### 2. Authentication Yonetimi
- Token'i guvenli sekilde saklama
- Yetkisiz durumda kullaniciyi tekrar girise yonlendirme

### 3. Error Handling
- Network ve timeout hatalarina uygun mesaj gosterme
- Sunucudan donen hata metnini kullaniciya acik sekilde yansitma

### 4. Caching ve State
- Listeleme ekranlarinda makul cache kullanimi
- Yorum, begeni ve rapor aksiyonlarinda guncel veri yenileme
