# FastPCB.API

Bu klasor, projenin HTTP giris noktasi olan Web API katmanidir.
## API Katmaninin Temel Gorevi Nedir?


- request alma
- request dogrulama
- kullanici kimligini okuma
- servis cagirma
- HTTP response uretme

islerini yapar.

## Bu Katmanda Ne Var?

- `Controllers/`
  - Dis dunyadan gelen HTTP isteklerini karsilar
  - Route tanimlarini tutar
  - Servis katmanina is delegasyonu yapar
  - Uygun HTTP cevaplarini doner

- `Program.cs`
  - Uygulamanin baslangic ayarlaridir
  - Swagger, JWT, CORS, static file ve DI kayitlari burada yapilir

- `Services/LocalProjectFileStorage.cs`
  - API tarafinda proje dosyalarini diske yazan local storage yardimcisidir

## Controller Sorumluluklari

### AuthController

- Kayit ve giris islemlerini yonetir
- Kullaniciya JWT token doner

### ProjectController

- Proje olusturma
- Proje listeleme
- Proje detay getirme
- Teknik detay kaydetme
- Dosya yukleme
- Proje silme

### CommentController

- Yorum listeleme
- Yorum ekleme
- Yorum silme

### ProjectLikeController

- Proje begenme
- Begeni kaldirma
- Kullanici begenilerini listeleme

### ReportController

- Proje raporlama
- Kullanicinin kendi raporlarini listeleme


## Ozet

Kisaca `FastPCB.API`, uygulamanin dis dunyaya acilan kapi katmanidir.
