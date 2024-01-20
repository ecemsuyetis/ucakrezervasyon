using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace UcusRezervasyonKonsolUygulamasi
{
    public interface IMusteri
    {
        string Ad { get; set; }
        string Soyad { get; set; }
        int Yas { get; set; }
        string CepTelefonu { get; set; }
    }

    public interface IUcus
    {
        string UcusNumarasi { get; set; }
        Lokasyon KalkisLokasyon { get; set; }
        Lokasyon VarisLokasyon { get; set; }
        DateTime KalkisZamani { get; set; }
        DateTime VarisZamani { get; set; }
        Ucak Ucak { get; set; }
    }

    public interface IRezervasyon
    {
        IMusteri Musteri { get; set; }
        IUcus Ucus { get; set; }
        string RezervasyonNumarasi { get; set; }
    }

    public class Ucak
    {
        public string Model { get; set; }
        public string Marka { get; set; }
        public string SeriNo { get; set; }
        public int KoltukKapasitesi { get; set; }
    }

    public class Lokasyon
    {
        public string Ulke { get; set; }
        public string Sehir { get; set; }
        public string Havaalani { get; set; }
        public bool Aktif { get; set; }
    }

    public class Ucus : IUcus
    {
        public string UcusNumarasi { get; set; }
        public Lokasyon KalkisLokasyon { get; set; }
        public Lokasyon VarisLokasyon { get; set; }
        public DateTime KalkisZamani { get; set; }
        public DateTime VarisZamani { get; set; }
        public Ucak Ucak { get; set; }
    }

    public class Rezervasyon : IRezervasyon
    {
        public IMusteri Musteri { get; set; }
        public IUcus Ucus { get; set; }
        public string RezervasyonNumarasi { get; set; }
    }

    public class Uygulama
    {
        private List<IUcus> ucuslar = new List<IUcus>();
        private List<IRezervasyon> rezervasyonlar = new List<IRezervasyon>();

        public List<IUcus> Ucuslar => ucuslar;
        public List<IRezervasyon> Rezervasyonlar => rezervasyonlar;

        public void UcusEkle(IUcus ucus)
        {
            ucuslar.Add(ucus);
        }

        public void RezervasyonYap(IRezervasyon rezervasyon)
        {
            // Uçak kapasitesini kontrol et
            if (KapasiteKontrol(rezervasyon.Ucus.Ucak, rezervasyon))
            {
                rezervasyonlar.Add(rezervasyon);
                Console.WriteLine("Rezervasyon başarıyla yapıldı.");
            }
            else
            {
                Console.WriteLine("Üzgünüz, uçak kapasitesi dolu. Rezervasyon yapılamadı.");
            }
        }

        private bool KapasiteKontrol(Ucak ucak, IRezervasyon rezervasyon)
        {
            int rezervasyonSayisi = rezervasyonlar.Count(r => r.Ucus.UcusNumarasi == rezervasyon.Ucus.UcusNumarasi);
            return rezervasyonSayisi < ucak.KoltukKapasitesi;
        }

        public void DosyayaKaydet<T>(List<T> liste, string dosyaAdi, string dosyaFormati)
        {
            string dosyaYolu = $"{dosyaAdi}.{dosyaFormati}";

            using (StreamWriter sw = new StreamWriter(dosyaYolu))
            {
                switch (dosyaFormati)
                {
                    case "csv":
                        foreach (var item in liste)
                        {
                            string csvLine = "";

                            if (item is IUcus)
                            {
                                var ucus = item as IUcus;
                                csvLine = $"{ucus.UcusNumarasi},{ucus.KalkisLokasyon.Ulke},{ucus.KalkisLokasyon.Sehir},{ucus.KalkisLokasyon.Havaalani}," +
                                    $"{ucus.VarisLokasyon.Ulke},{ucus.VarisLokasyon.Sehir},{ucus.VarisLokasyon.Havaalani}," +
                                    $"{ucus.KalkisZamani},{ucus.VarisZamani},{ucus.Ucak.Model},{ucus.Ucak.Marka},{ucus.Ucak.SeriNo},{ucus.Ucak.KoltukKapasitesi}";
                            }
                            else if (item is IRezervasyon)
                            {
                                var rezervasyon = item as IRezervasyon;
                                csvLine = $"{rezervasyon.Musteri.Ad},{rezervasyon.Musteri.Soyad},{rezervasyon.Musteri.Yas},{rezervasyon.Musteri.CepTelefonu}," +
                                    $"{rezervasyon.Ucus.UcusNumarasi},{rezervasyon.Ucus.KalkisLokasyon.Ulke},{rezervasyon.Ucus.KalkisLokasyon.Sehir},{rezervasyon.Ucus.KalkisLokasyon.Havaalani}," +
                                    $"{rezervasyon.Ucus.VarisLokasyon.Ulke},{rezervasyon.Ucus.VarisLokasyon.Sehir},{rezervasyon.Ucus.VarisLokasyon.Havaalani}," +
                                    $"{rezervasyon.Ucus.KalkisZamani},{rezervasyon.Ucus.VarisZamani},{rezervasyon.Ucus.Ucak.Model},{rezervasyon.Ucus.Ucak.Marka},{rezervasyon.Ucus.Ucak.SeriNo},{rezervasyon.Ucus.Ucak.KoltukKapasitesi}," +
                                    $"{rezervasyon.RezervasyonNumarasi}";
                            }

                            sw.WriteLine(csvLine);
                        }
                        break;
                    case "json":
                        string json = JsonSerializer.Serialize(liste, new JsonSerializerOptions { WriteIndented = true });
                        sw.Write(json);
                        break;
                    case "xml":
                        var serializer = new XmlSerializer(typeof(List<T>));
                        serializer.Serialize(sw, liste);
                        break;
                    default:
                        Console.WriteLine("Geçersiz dosya formatı");
                        break;
                }
            }

            Console.WriteLine($"Veriler {dosyaYolu} dosyasına başarıyla kaydedildi.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Uygulama uygulama = new Uygulama();

            // ... Diğer kodlar

            // CSV dosyaya kaydet
            uygulama.DosyayaKaydet<IUcus>(uygulama.Ucuslar, "Ucuslar", "csv");
            uygulama.DosyayaKaydet<IRezervasyon>(uygulama.Rezervasyonlar, "Rezervasyonlar", "csv");
        }
    }
}



