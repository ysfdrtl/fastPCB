# FastPCB.Services

Bu katman, projenin iş kurallarini tasir. Controller'lar dogrudan veritabanina gitmek yerine bu servislere cagirida bulunur.
## Katmanin Amaci

- Controller'lardaki kodu sade tutmak
- Tekrarlanan dogrulama kurallarini tek yerde toplamak
- API ile veri erisim katmani arasinda bir is mantigi ara yuzeyi saglamak



## Bu Katmanda Neler Var?

- `AuthService`: Kayit, giris ve JWT token uretimi.
- `ProjectService`: Proje olusturma, listeleme, teknik detay guncelleme ve dosya baglama islemleri.
- `CommentService`: Yorum ekleme, listeleme ve yorum silme akisi.
- `ProjectLikeService`: Proje begenme, begeniyi kaldirma ve begenilen projeleri listeleme.
- `ReportService`: Proje raporlama ve kullanicinin kendi raporlarini listeleme.

