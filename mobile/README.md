# FastPCB Mobile

Expo Router + TypeScript ile hazirlanan FastPCB mobil MVP uygulamasi. Paketler Expo Go `54.x` client ile uyumlu Expo SDK 54 hattina sabitlenmistir.

## Calistirma

```bash
cd fastPCB/mobile
npm install
npm run start
```

Alternatif komutlar:

```bash
npm run android
npm run ios
npm run web
npm run typecheck
```

Android cihazda surum uyumsuzlugu gorurseniz once Metro cache'i temizleyin:

```bash
npx expo start --clear --android
```

## Backend URL

Varsayilan backend adresi Railway production API'dir:

```bash
https://fastpcb-backend-production.up.railway.app/api
```

`EXPO_PUBLIC_API_BASE_URL` ile local gelistirme icin override edilebilir.

- Android emulator: `http://10.0.2.2:5000/api`
- Gercek cihaz: `http://<bilgisayar-LAN-IP>:5000/api`
- Expo web: `http://localhost:5000/api`

Backend varsayilan Swagger adresi: `http://localhost:5000/swagger`.

## Kapsam

Bu ilk mobil surum login, kayit, kesfet, proje detay, yorum, begeni, rapor, profil ve proje yukleme akislarini icerir. Admin paneli sonraki faza birakilmistir.
