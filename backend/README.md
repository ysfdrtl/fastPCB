# Backend Katmani

Bu klasor, FastPCB platformunun sunucu tarafini barindirir. Backend yapisi katmanli mimari ile kurulmustur ve her proje belirli bir sorumluluga sahiptir.

## Alt Katmanlar

- `FastPCB.API`: HTTP endpointleri, authentication ayarlari, Swagger, middleware ve dosya servisleri burada yer alir.
- `FastPCB.Services`: Is kurallari, dogrulama kontrolleri ve controller'larin çağırdığı ana servis siniflari burada bulunur.
- `FastPCB.Data`: `DbContext`, entity konfigurasyonlari, migration'lar ve seed islemleri bu katmanda tutulur.
- `FastPCB.Models`: Veritabaninda saklanan ana domain modelleri burada tanimlanir.

## Veri Akisi

1. Istek `FastPCB.API` icindeki controller'a gelir.
2. Controller ilgili islemi `FastPCB.Services` katmanina devreder.
3. Servisler veriyi `FastPCB.Data` icindeki context uzerinden okur veya yazar.
4. Kullanilan entity siniflari `FastPCB.Models` icinden gelir.

Bu ayirim sayesinde backend daha okunabilir, test edilebilir ve sunum sirasinda daha rahat anlatilabilir hale gelir.

## Altyapi Servisleri

- Kafka producer ve event modelleri `FastPCB.Services/Infrastructure/Kafka` altindadir. Kafka kapaliysa veya broker'a ulasilamazsa is akisi devam eder, hata loglanir.
- Redis cache servisleri `FastPCB.Services/Infrastructure/Cache` altindadir. Redis kapaliysa cache no-op gibi davranir ve veri dogrudan veritabanindan okunur.
- Redis health check `FastPCB.Services/Infrastructure/Health` altindadir ve API'de `/health` endpointine baglanir.
- Admin controller `UserRole.Admin` rolu ile korunur; admin istatistik cache'i rol/rapor/proje durum degisimlerinde temizlenir.
