# Yapilacaklar

Bu dosya projeye eklenecek yeni teknolojiler icin takip edilecek teknik isleri listeler.

## Kafka

- [x] Kafka'nin projede hangi olaylar icin kullanilacagini netlestir.
  - Ornek: proje yuklendi, proje raporlandi, rapor durumu degisti, kullanici admin yapildi.
- [x] Backend tarafinda Kafka producer servisi ekle.
- [x] Event modellerini tanimla.
  - `ProjectCreated`
  - `ProjectReported`
  - `ReportStatusChanged`
  - `UserRoleChanged`
- [x] Kafka topic isimlerini standartlastir.
  - `fastpcb.projects`
  - `fastpcb.reports`
  - `fastpcb.users`
- [x] Kritik islemlerden sonra event publish et.
- [x] Kafka baglanti ayarlarini `.env` ve appsettings uzerinden okunabilir yap.
- [x] Local gelistirme icin Docker Compose'a Kafka ve Zookeeper/KRaft servisi ekle.
- [x] Kafka kullanilamiyorsa uygulamanin tamamen dusmemesi icin hata yonetimi ekle.
- [x] Producer icin temel unit/integration testleri yaz.
- [x] Kafka kurulum ve kullanim adimlarini README'ye ekle.

## Redis

- [x] Redis'in projede hangi amaclarla kullanilacagini belirle.
  - Ornek: cache, rate limit, session/token blacklist, populer projeler.
- [x] Backend tarafina Redis connection ayari ekle.
- [x] Redis client paketini projeye ekle.
- [x] Cache servisi icin ortak interface olustur.
- [x] Sik okunan veriler icin cache stratejisi tasarla.
  - Proje listeleme
  - Proje detay
  - Admin dashboard istatistikleri
- [x] Cache invalidation kurallarini belirle.
  - Proje eklenince/guncellenince/silinince ilgili cache temizlenecek.
  - Rapor veya kullanici rolu degisince admin dashboard cache temizlenecek.
- [x] Redis baglantisi yoksa uygulamanin cache olmadan calismaya devam etmesini sagla.
- [x] Local gelistirme icin Docker Compose'a Redis servisi ekle.
- [x] Redis icin health check ekle.
- [x] Redis kullanimini README'ye ekle.

## Docker

- [x] Mevcut backend ve frontend Dockerfile'larini kontrol et.
- [x] Backend Dockerfile'ini production calisma sekline gore sadelestir.
- [x] Frontend Dockerfile'ini production build ve static servis icin duzenle.
- [x] Proje kokune `docker-compose.yml` ekle.
- [x] Docker Compose servislerini tanimla.
  - Backend API
  - Frontend
  - MySQL
  - Redis
  - Kafka
- [x] Servisler arasi network ayarlarini yap.
- [x] Ortam degiskenlerini `.env` uzerinden compose'a bagla.
- [x] Database migration calistirma akisini belirle.
  - Manuel komut
  - Container startup
  - CI/CD adimi
- [x] Upload dosyalari icin volume stratejisi belirle.
- [x] Docker health check'leri ekle.
- [x] Docker ile local calistirma adimlarini README'ye ekle.

## CI/CD

- [x] Kullanilacak CI/CD platformunu netlestir.
  - GitHub Actions onerilir.
- [x] Pull request icin build pipeline ekle.
- [x] Backend icin restore ve build adimlari ekle.
- [x] Frontend icin install ve build adimlari ekle.
- [x] Test komutlari eklendiginde pipeline'a test adimi koy.
- [x] Migration dosyalarinin build sirasinda dogrulanmasini sagla.
- [x] Docker image build adimi ekle.
- [x] Main branch'e merge sonrasi deploy akisini tasarla.
- [x] Production secret'larini repo disinda sakla.
  - Railway variables
  - GitHub Actions secrets
- [x] CI/CD icin gerekli environment variable listesini dokumante et.
- [x] Basarisiz build/deploy durumunda log takibi ve rollback planini yaz.

## Genel

- [x] `.env.example` dosyasi ekle ve gercek sifreleri koyma.
- [x] README'yi yeni servisler ve calistirma komutlariyla guncelle.
- [x] Gelistirme, staging ve production ortamlarini ayir.
- [x] Yeni servisler icin temel monitoring/logging ihtiyaclarini belirle.
- [x] Guvenlik kontrolu yap.
  - Secret'lar git'e girmeyecek.
  - Admin endpointleri sadece admin role ile calisacak.
  - Production connection string'leri appsettings icinde tutulmayacak.
