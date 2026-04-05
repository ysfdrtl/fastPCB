# FastPCB.Models

Bu katman, sistemde saklanan temel veri modellerini icerir. Veritabanindaki tablolarin karsiligi olan siniflar burada yer alir.
## Neden Ayri Bir Katman?

- Tum projede kullanilan veri yapilari tek yerde toplanir
- Service ve Data katmanlari ayni modelleri ortak kullanir
- Iliskiler daha rahat gorulur


## Ana Modeller

- `User`: Platform kullanicisi
- `Project`: Paylasilan PCB projesi
- `Comment`: Projelere yazilan yorum
- `ProjectLike`: Proje begeni kaydi
- `Ticket`: Rapor kaydi icin kullanilan model


