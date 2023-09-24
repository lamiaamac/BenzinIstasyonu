using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BenzinIstasyonu
{
    public class Urun
    {
        public string Adi { get; set; }
        public int Stok { get; set; }
        public double Fiyat { get; set; }
    }

    public class Market
    {
        public List<Urun> Urunler { get; set; }

        public void UrunleriExcelDosyasinaKaydet()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Urunler");

                // Başlık satırını ekle
                worksheet.Cells["A1"].Value = "Ürün Adı";
                worksheet.Cells["B1"].Value = "Stok";
                worksheet.Cells["C1"].Value = "Fiyat";

                // Verileri doldur
                for (int i = 0; i < Urunler.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = Urunler[i].Adi;
                    worksheet.Cells[i + 2, 2].Value = Urunler[i].Stok;
                    worksheet.Cells[i + 2, 3].Value = Urunler[i].Fiyat;
                }

                // Dosyayı kaydet
                var excelFile = new FileInfo("Urunler.xlsx");
                package.SaveAs(excelFile);
            }
        }
    }

    public class Kasa
    {
        public List<Satis> Satislar { get; set; }

        public Kasa()
        {
            Satislar = new List<Satis>();
        }

        public void SatisYap(Urun urun, int adet)
        {
            Satis satis = new Satis { Urun = urun, Adet = adet };
            Satislar.Add(satis);

            // Excel dosyasını güncelle
            SatislariExcelDosyasinaKaydet();
        }

        public void SatislariExcelDosyasinaKaydet()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Satislar");

                // Başlık satırını ekle
                worksheet.Cells["A1"].Value = "Ürün Adı";
                worksheet.Cells["B1"].Value = "Adet";

                // Verileri doldur
                for (int i = 0; i < Satislar.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = Satislar[i].Urun.Adi;
                    worksheet.Cells[i + 2, 2].Value = Satislar[i].Adet;
                }

                // Dosyayı kaydet
                var excelFile = new FileInfo("Satislar.xlsx");
                package.SaveAs(excelFile);
            }
        }

        public void SatisBilgileriniGoruntule()
        {
            if (Satislar.Count == 0)
            {
                Console.WriteLine("Satış Yok!");
            }
            else
            {
                Console.WriteLine("Satışlar: ");
                foreach (var satis in Satislar)
                {
                    Console.WriteLine($"{satis.Urun.Adi} - Adet : {satis.Adet}");
                }
            }
        }

        public void ToplamSatisFiyatiniGoruntule()
        {
            double toplamFiyat = 0;
            if (Satislar.Count == 0)
            {
                Console.WriteLine("Satış Yok!");
            }
            else
            {
                foreach (var satis in Satislar)
                {
                    toplamFiyat += satis.Urun.Fiyat * satis.Adet;
                }
                Console.WriteLine($"Toplam Satış Fiyatı: {toplamFiyat}");
            }
        }
    }

    public class Satis
    {
        public Urun Urun { get; set; }
        public int Adet { get; set; }
    }

    public class Kullanici
    {
        public string Mail { get; set; }
        public string Sifre { get; set; }

        public bool GirisYap(string mail, string sifre)
        {
            if (this.Mail == mail && this.Sifre == sifre)
            {
                return true;
            }
            return false;
        }
    }

    public class Kasiyer : Kullanici
    {
        public Kasa Kasa { get; set; }

        public Kasiyer(Kasa kasa)
        {
            Kasa = kasa;
        }

        public void Menu(Kasa kasa, Market market)
        {
            bool devam = true;

            while (devam)
            {
                Console.WriteLine("Kasiyer Menüsü");
                Console.WriteLine("1. Satış Yap");
                Console.WriteLine("2. Satış Bilgilerini Görüntüle");
                Console.WriteLine("3. Toplam Satış Fiyatını Görüntüle");
                Console.WriteLine("4. Çıkış");

                int secim = Convert.ToInt32(Console.ReadLine());

                switch (secim)
                {
                    case 1:
                        SatistanSorumluOl(kasa, market);
                        break;
                    case 2:
                        Kasa.SatisBilgileriniGoruntule();
                        break;
                    case 3:
                        Kasa.ToplamSatisFiyatiniGoruntule();
                        break;
                    case 4:
                        devam = false;
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçenek. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        public void SatistanSorumluOl(Kasa kasa, Market market)
        {
            if (market.Urunler == null)
            {
                Console.WriteLine("Ürün yok!");
            }
            else
            {

                Console.WriteLine("Satış yapılacak ürünler:");
                foreach (var urunIsmi in market.Urunler)
                {
                    Console.WriteLine($"{urunIsmi.Adi} - Stok: {urunIsmi.Stok} - Fiyat: {urunIsmi.Fiyat}");
                }

                Console.WriteLine("Ürün adını girin:");
                string urunAdi = Console.ReadLine();
                Console.WriteLine("Adet girin:");
                int adet = Convert.ToInt32(Console.ReadLine());

                Urun urun = market.Urunler.Find(u => u.Adi == urunAdi);
                if (urun != null && urun.Stok >= adet)
                {
                    Console.WriteLine("Ürün Satışı Başladı!");
                    kasa.SatisYap(urun, adet);
                    urun.Stok -= adet;
                    Console.WriteLine($"{adet} adet {urun.Adi} satıldı.");

                    // Marketi ve Excel dosyasını güncelle
                    market.UrunleriExcelDosyasinaKaydet();
                }
                else
                {
                    Console.WriteLine("Ürün bulunamadı veya yeterli stok yok.");
                }
            }
        }
    }

    public class PetrolSahibi : Kullanici
    {

        public Kasa Kasa { get; set; }

        public PetrolSahibi(Kasa kasa)
        {
            Kasa = kasa;
        }
        public void Menu(Kasa kasa, Market market)
        {
            bool devam = true;

            while (devam)
            {
                Console.WriteLine("Petrol Sahibi Menüsü");
                Console.WriteLine("1. Ürünleri Listele");
                Console.WriteLine("2. Satış Bilgilerini Görüntüle");
                Console.WriteLine("3. Toplam Satış Fiyatını Görüntüle");
                Console.WriteLine("4. Çıkış");

                int secim = Convert.ToInt32(Console.ReadLine());

                switch (secim)
                {
                    case 1:
                        UrunleriListele(market);
                        break;
                    case 2:
                        Kasa.SatisBilgileriniGoruntule();
                        break;
                    case 3:
                        Kasa.ToplamSatisFiyatiniGoruntule();
                        break;
                    case 4:
                        devam = false;
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçenek. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        public void UrunleriListele(Market market)
        {
            if (market.Urunler == null)
            {
                Console.WriteLine("Ürün yok!");
            }
            else
            {
                Console.WriteLine("Market ürünleri:");
                foreach (var urun in market.Urunler)
                {
                    Console.WriteLine($"{urun.Adi} - Stok: {urun.Stok} - Fiyat: {urun.Fiyat}");
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            bool devam = true;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Kasa kasa = new Kasa();

            Market market = new Market();
            market.Urunler = new List<Urun> {
                new Urun { Adi = "Çikolata", Stok = 100, Fiyat = 10 },
                new Urun { Adi = "Bisküvi", Stok = 50, Fiyat = 20 },
                new Urun { Adi = "İçecek", Stok = 30, Fiyat = 15 },
                new Urun { Adi = "Sandviç", Stok = 70, Fiyat = 25 }
            };
            market.UrunleriExcelDosyasinaKaydet();

            while (devam)
            {
                Console.WriteLine("Mail: ");
                string mail = Console.ReadLine();
                Console.WriteLine("Şifre: ");
                string sifre = Console.ReadLine();

                Kasiyer kasiyer = new Kasiyer(kasa);
                kasiyer.Mail = "kasiyer@mail.com";
                kasiyer.Sifre = "kasiyer123";


                PetrolSahibi petrolSahibi = new PetrolSahibi(kasa);
                petrolSahibi.Mail = "petrolsahibi@mail.com";
                petrolSahibi.Sifre = "sahibi123";

                if (kasiyer.GirisYap(mail, sifre))
                {
                    kasiyer.Menu(kasa, market);
                }
                else if (petrolSahibi.GirisYap(mail, sifre))
                {
                    petrolSahibi.Menu(kasa, market);
                }
                else
                {
                    Console.WriteLine("Hatalı giriş.");
                }

                Console.WriteLine("Devam etmek istiyor musunuz? (Evet/Hayır)");
                string cevap = Console.ReadLine();
                if (cevap.ToLower() != "evet")
                {
                    devam = false;
                }
            }

            Console.WriteLine("Hoşçakalın :)");
            Console.ReadKey();
        }
    }
}
