# FastPCB.Data

Bu katman, uygulamanin veritabanina nasil baglandigini ve tablolarin nasil sekillendigini tanimlar.
## Katmanin Gorevi

- Uygulama ile MySQL arasindaki teknik baglantiyi kurmak
- Domain modellerini tablo yapisina donusturmek
- Migration surecini yonetmek

## Icerik

- `FastPCBContext`: EF Core context sinifi.
- `Configurations`: Her entity icin tablo, kolon, iliski ve index ayarlari.
- `Migrations`: Veritabanina uygulanan sema degisikliklerinin gecmisi.
- `Seeders`: Ilk acilista veya migration sirasinda eklenen sabit ornek veriler.
- `Extensions`: DI container'a data katmanini eklemek icin yardimci extension metodlari.



