# Araç Kiralama Yönetim Sistemi (Vehicle Rental Management System)

**Geliştirici:** Ayşe Özge Bakır — Okul No: 25906045
**Ders:** BGY203 / YBS223 – Nesne Tabanlı Programlama (Dr. Öğr. Üyesi Tohid YOUSEFİ)
**GitHub:** https://github.com/KULLANICI/OKULNO_AdSoyad_VehicleRentalSystem (Private)

## Proje Açıklaması ve Amaç
Araç, müşteri, kiralama ve iade işlemlerini yöneten C# Windows Forms masaüstü uygulaması. Amaç; sınıf, kapsülleme, kalıtım, çok biçimlilik ve soyutlama ilkelerini çalışan bir uygulamada göstermektir.

## Temel Özellikler
- Araç ekleme / listeleme / güncelleme / silme (plaka tekrarı engellenir, kiralama kaydı olan araç silinemez)
- Müşteri ekleme / listeleme / güncelleme / silme (aktif kiralaması olan müşteri silinemez)
- Kiralama: yalnızca müsait araçlar, tarih kontrolü, gün × günlük ücret (araç türüne göre polimorfik hesap)
- İade: aktif kiralamalar listelenir, iade sonrası araç tekrar "Müsait" olur
- Ana sayfada koleksiyonlardan hesaplanan özet sayılar
- Veriler bellekte `List<T>` ile tutulur; açılışta örnek kayıtlar yüklenir

## Teknoloji ve Geliştirme Ortamı
C# 12, .NET 8, Windows Forms, Visual Studio 2022, Git/GitHub

## Çalıştırma
1. **Hazır uygulama:** `02_Application/Release/VehicleRentalSystem.exe` dosyasına çift tıklayın (.NET kurulumu gerekmez).
2. **Kaynaktan:** `01_SourceCode/VehicleRentalSystem.sln` dosyasını Visual Studio 2022 ile açın (".NET desktop development" iş yükü gerekir), `F5` ile çalıştırın.

## Klasör Yapısı
```
01_SourceCode/VehicleRentalSystem/
   Models/    Vehicle (abstract), Car, Truck, Motorcycle, Customer, Rental, IRentable, Enums
   Services/  RentalManager (iş mantığı, List<T>), RentalException
   Forms/     MainForm, DashboardControl, VehicleControl, CustomerControl, RentalControl, ReturnControl, UiHelper
   Program.cs
02_Application/Release/   çalıştırılabilir çıktı
03_Report/                proje raporu
04_GitHub/                README ve repository bağlantısı
```

## OOP Özeti
| İlke | Uygulama |
|---|---|
| Kapsülleme | private alanlar + doğrulamalı property'ler (`DailyPrice > 0`, telefon, plaka...) |
| Kalıtım | `Vehicle` → `Car`, `Truck`, `Motorcycle` |
| Çok biçimlilik | `CalculateRentalPrice(int days)` her türde override (Car: 7+ gün %10 indirim, Truck: +%20, Motorcycle: %15 indirim) |
| Soyutlama | `abstract class Vehicle` (soyut üyeler) + `interface IRentable` |

## Ekran Görüntüleri
Ekran görüntüleri `03_Report/` klasöründeki raporda yer almaktadır.
