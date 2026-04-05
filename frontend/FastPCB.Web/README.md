# FastPCB.Web

Bu proje, FastPCB platformunun web arayuzudur. React, Vite ve TypeScript kullanilarak gelistirilmistir.

## Ana Parcalar

- `src/pages`: Sayfa seviyesindeki ekranlar
- `src/components`: Tekrar kullanilan arayuz bilesenleri
- `src/state`: Global auth durumu
- `src/lib`: API istemcisi ve tarayici yardimcilari
- `src/types.ts`: Frontend tarafinda kullanilan tip tanimlari

## Uygulama Akisi

1. Kullanici `Discover` sayfasinda projeleri gorur.
2. Giris yaptiginda JWT bilgisi `AuthContext` icinde saklanir.
3. Korumali sayfalarda token ile backend'e istek atilir.
4. Yukleme, yorum, begeni ve rapor gibi islemler API uzerinden gerceklesir.

Bu katman, backend'den gelen veriyi kullanicinin anlayacagi bir deneyime donusturur.
