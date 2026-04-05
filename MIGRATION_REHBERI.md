# Migration Rehberi

FastPCB backend tarafinda migration islemleri manuel olarak yapilir.
Uygulama acilisinda otomatik migration calistirilmaz.

## Yeni Migration Olusturma

```powershell
cd backend/FastPCB.API
dotnet ef migrations add MigrationAdi --project ../FastPCB.Data --startup-project .
```

## Veritabanini Guncelleme

```powershell
cd backend/FastPCB.API
dotnet ef database update --project ../FastPCB.Data --startup-project .
```

## Ne Zaman Migration Gerekir?

- Yeni model eklendiginde
- Alan adi veya tablo adi degistiginde
- Iliski ya da index degisikligi yapildiginda
