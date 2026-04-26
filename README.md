# FastPCB

---

## Proje Hakkinda

![Urun Tanitim Gorseli](Product.png)

**Proje Tanimi:**
> FastPCB, kullanicilarin PCB tasarimlarini yukleyip toplulukla paylastigi bir platformdur. Sistem uzerinden kullanicilar hesap olusturabilir, proje ekleyebilir, teknik detay girebilir, tasarim dosyalarini yukleyebilir, diger projeleri kesfedebilir, yorum yapabilir, begeni birakabilir ve uygunsuz icerikleri raporlayabilir. Projenin amaci PCB toplulugu icin sade, anlasilir ve etkilesimli bir paylasim ortami sunmaktir.

**Proje Kategorisi:**
> Sosyal Medya / Teknik Icerik Paylasim Platformu

**Referans Uygulama:**
> [Hackaday.io](https://hackaday.io/)

---

## Proje Linkleri

- **REST API Adresi:** `http://localhost:5000/swagger`
- **Web Frontend Adresi:** `http://localhost:5173`

---

## Proje Ekibi

**Grup Adi:**
> FastPCB

**Ekip Uyeleri:**
- Yusuf Doruatli

---

## Dokumantasyon

Proje dokumantasyonuna asagidaki linklerden erisebilirsiniz:

1. [Gereksinim Analizi](Gereksinim-Analizi.md)
2. [REST API Tasarimi](API-Tasarimi.md)
3. [REST API](Rest-API.md)
4. [Web Front-End](WebFrontEnd.md)
5. [Mobil Front-End](MobilFrontEnd.md)
6. [Mobil Backend](MobilBackEnd.md)
7. [Video Sunum](Sunum.md)

---


## Notlar

- Backend `.NET 8 Web API`, frontend ise `React + Vite + TypeScript` ile gelistirilmistir.
- Veritabani olarak `MySQL` kullanilmaktadir.
- Kimlik dogrulama `JWT` ile saglanmaktadir.
- Proje artik siparis ve odeme mantiginda degil, PCB paylasim platformu mantiginda ilerlemektedir.

---

## Local Ortam ve Secret Yonetimi

1. `.env.example` dosyasini `.env` olarak kopyalayin.
2. Ornek degerleri kendi local degerlerinizle degistirin.
3. Gercek production secret, sifre veya connection string repo icine eklenmemelidir.

Onemli environment variable listesi:

- `ConnectionStrings__DefaultConnection` veya `MYSQL_URL`: Backend MySQL baglantisi.
- `Jwt__SecretKey`, `Jwt__Issuer`, `Jwt__Audience`: JWT imzalama ve dogrulama ayarlari.
- `Kafka__Enabled`, `Kafka__BootstrapServers`, `Kafka__ClientId`: Kafka producer ayarlari.
- `Kafka__Topics__Projects`, `Kafka__Topics__Reports`, `Kafka__Topics__Users`: Topic isimleri.
- `Redis__Enabled`, `Redis__ConnectionString`, `Redis__InstanceName`: Redis cache ayarlari.
- `VITE_API_BASE_URL`: Frontend build sirasinda kullanilan API adresi.
- `FileStorage__ProjectUploadsPath`: Upload dosyalari icin kalici klasor.

Development, staging ve production ayrimi appsettings dosyalari ve environment variable override'lari ile yapilir. Production degerleri GitHub Actions secrets, Railway variables veya kullanilan platformun secret store'u uzerinden verilmelidir.

---

## Docker ile Calistirma

Docker Compose su servisleri kaldirir: backend API, frontend, MySQL, Redis ve KRaft tabanli tek-node Kafka. Upload dosyalari, MySQL, Redis ve Kafka verisi volume ile kalici tutulur.

```bash
cd fastPCB
copy .env.example .env
docker compose up --build
```

Adresler:

- Backend Swagger: `http://localhost:5000/swagger`
- Backend health: `http://localhost:5000/health`
- Frontend: `http://localhost:3000`
- Kafka local dis port: `localhost:9094`
- Redis local port: `localhost:6379`

Migration icin onerilen secenekler:

- Manuel local: `dotnet ef database update --project backend/FastPCB.Data --startup-project backend/FastPCB.API`
- Container startup: sadece staging gibi kontrollu ortamlarda ayri bir migration komutu/container olarak calistirin; API boot akisini migration'a baglamayin.
- CI/CD: deploy oncesi migration adimi ekleyin ve hata alinirsa yeni image'i production'a cikarmayin.

---

## Kafka

Backend su eventleri publish eder:

- `ProjectCreated` -> `fastpcb.projects`
- `ProjectReported` -> `fastpcb.reports`
- `ReportStatusChanged` -> `fastpcb.reports`
- `UserRoleChanged` -> `fastpcb.users`

Kafka opsiyoneldir. `Kafka__Enabled=false` ise publish atlanir. Kafka acik ama broker kapaliysa producer hatayi loglar ve kullanicinin proje/rapor/admin islemi tamamlanmaya devam eder.

---

## Redis Cache

Redis su anda cache amaciyla kullanilir; ileride rate limit, token blacklist/session kontrolu, populer projeler ve admin dashboard metrikleri icin genisletilebilir.

Cache stratejisi:

- Proje listeleme ve proje detay cevaplari kisa TTL ile cache'lenir.
- Admin dashboard istatistikleri cache'lenir.
- Proje ekleme, guncelleme, dosya yukleme veya silme sonrasi proje cache'i temizlenir.
- Rapor durumu, kullanici rolu ve admin istatistiklerini etkileyen islemler sonrasi dashboard cache'i temizlenir.

Redis opsiyoneldir. Redis kapaliysa backend cache olmadan calisir. `/health` endpointi Redis durumunu gosterir.

---

## CI/CD

GitHub Actions workflow dosyasi `.github/workflows/ci.yml` altindadir. Pull request ve `main` push icin su adimlari calistirir:

- `dotnet restore`, `dotnet build`, `dotnet test`
- Migration kaynaklarinin derlenmesi icin `FastPCB.Data` build kontrolu
- Frontend `npm ci` ve `npm run build`
- Backend ve frontend Docker image build kontrolu

Main branch sonrasi deploy icin workflow'da temel handoff job'u vardir. Production deployment'ta registry push, platform deploy ve migration adimi kullanilan ortama gore eklenmelidir.

Rollback plani:

- Her deploy image'i commit SHA veya versiyon tag'i ile yayinlayin.
- Basarisiz deployda onceki saglikli image tag'ine geri donun.
- Backend loglarinda Kafka/Redis warning'lerini, DB migration hatalarini ve `/health` durumunu kontrol edin.
- Railway veya benzeri platformlarda environment variables gecmisini ve deployment loglarini saklayin.

Admin endpointleri `api/admin/*` altinda `[Authorize(Roles = Admin)]` ile korunur.

Monitoring ihtiyaclari:

- API request/error loglari
- MySQL yavas sorgu ve migration loglari
- Redis hit/miss ve baglanti warning'leri
- Kafka publish warning'leri ve broker health durumu
