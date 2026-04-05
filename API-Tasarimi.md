# API Tasarimi - OpenAPI Specification

**OpenAPI Spesifikasyon Dosyasi:** [lamine.yaml](lamine.yaml)

Bu dokuman, FastPCB projesinin REST API yapisini ozetler. API tasarimi OpenAPI mantigina uygun sekilde dusunulmus ve temel kaynaklar `auth`, `projects`, `comments`, `likes` ve `reports` etrafinda kurgulanmistir.

## API'nin Temel Kaynaklari

- `auth`: Kayit ve giris islemleri
- `projects`: Proje olusturma, listeleme, detay ve dosya yukleme
- `comments`: Projelere yorum ekleme ve yorum silme
- `likes`: Proje begenme ve kullanicinin begenilerini listeleme
- `reports`: Proje raporlama ve kullanicinin kendi raporlarini gorme

## OpenAPI Specification Ozeti

```yaml
openapi: 3.0.3
info:
  title: FastPCB API
  version: 1.0.0
  description: PCB projelerinin yuklendigi ve paylasildigi topluluk platformu API'si

servers:
  - url: http://localhost:5000/api
    description: Development server

tags:
  - name: auth
  - name: projects
  - name: comments
  - name: likes
  - name: reports

paths:
  /Auth/register:
    post:
      summary: Yeni kullanici kaydi
  /Auth/login:
    post:
      summary: Kullanici girisi
  /projects:
    get:
      summary: Proje listesi
    post:
      summary: Yeni proje olusturma
  /projects/{projectId}:
    get:
      summary: Proje detayi
  /projects/{projectId}/details:
    post:
      summary: Teknik detay kaydetme
  /projects/{projectId}/files:
    post:
      summary: Proje dosyasi yukleme
  /projects/{projectId}/comments:
    get:
      summary: Yorumlari listeleme
    post:
      summary: Yorum ekleme
  /projects/{projectId}/like:
    post:
      summary: Projeyi begenme
    delete:
      summary: Begeniyi kaldirma
  /projects/{projectId}/report:
    post:
      summary: Projeyi raporlama
```

## API Tasarim Kararlari

1. JWT ile korunan endpointler yazma islemlerinde kullanilir.
2. Listeleme endpointlerinde filtreleme ve sayfalama desteklenir.
3. Dosya yukleme `multipart/form-data` ile yapilir.
4. Hata durumlarinda anlamli HTTP kodlari ve mesajlar donulur.

Detayli sema ve endpoint tanimlari icin [lamine.yaml](lamine.yaml) dosyasi referans alinmalidir.
