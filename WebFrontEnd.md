# Web Frontend Gorev Dagilimi

**Web Frontend Adresi:** `http://localhost:5173`

Bu dokumanda, web uygulamasinin kullanici arayuzu ve deneyimi ile ilgili gorevler listelenmektedir.

---

## Grup Uyelerinin Web Frontend Gorevleri

1. [Yusuf Doruatli'nin Web Frontend Gorevleri](Yusuf-Doruatlı/yusuf-Doruatlı-Web-Frontend-Gorevleri.md)

---

## Genel Web Frontend Prensipleri

### 1. Responsive Tasarim
- Mobile-first yaklasim
- Telefon ve masaustu ekranlarina uyum
- Esnek kart ve grid yapisi

### 2. Tasarim Sistemi
- Tutarlı renk paleti
- Okunabilir tipografi
- Yeniden kullanilabilir bilesenler

### 3. Performans
- Gereksiz yeniden renderlari azaltma
- API isteklerinde uygun loading durumlari
- Hafif ve anlasilir sayfa yapilari

### 4. Erişilebilirlik
- Form alanlarinda acik etiket kullanimi
- Buton ve linklerde anlasilir metinler
- Bos durum ve hata mesajlarinin gorunur olmasi

### 5. Routing
- Kesfet, giris, kayit, upload, detay ve profil sayfalari
- Korumali rotalarda token kontrolu

### 6. API Entegrasyonu
- Tum istekler merkezi `api.ts` dosyasindan yapilir
- JWT token gerekli endpointlerde otomatik gonderilir
- Hata mesajlari kullaniciya anlamli sekilde yansitilir
