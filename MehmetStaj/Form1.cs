using Microsoft.VisualBasic.ApplicationServices;
using System.Data;
using System.Windows.Forms;

namespace MehmetStaj
{
    public partial class Form1 : Form
    {
        string dosyaAdi = "C:\\Users\\yasla\\source\\repos\\MehmetStaj\\MehmetStaj\\database.txt";

        public Form1()
        {
            InitializeComponent();
        }
        // D��meler
        // Ekle D��mesi
        public void button1_Click(object sender, EventArgs e)
        {
            // Girilen de�er kontrol�
            if (textBox1.Text == "")
            {
                MessageBox.Show("�sim girmelisiniz.");
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Soyad girmelisiniz.");
                return;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("Numara girmelisiniz.");
                return;
            }
            try
            {
                int sayi = Int32.Parse(textBox3.Text);
            }
            catch (FormatException)
            {
                Console.WriteLine("Numara k�sm�na sadece say� girmelisiniz!");
                return;
            }
            // Girilen de�erlerden yeni bir ogrenci olusturma ve ekleme
            Ogrenci yeniOgrenci = new Ogrenci
            {
                isim = textBox1.Text,
                soyad = textBox2.Text,
                Numara = Int32.Parse(textBox3.Text),
                exist = true,
            };
            Ekle(yeniOgrenci);
        }
        // Sil D��mesi
        public void button2_Click(object sender, EventArgs e)
        {
            // Se�ili sat�rdan ogrencinin id'sini al
            int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            int studentID = (int)dataGridView1.Rows[selectedRowIndex].Cells["ID"].Value;
            // Ogrenci ID'sinden ogrenciyi bul ve sil(gizle)
            var ogrenciler = TumOgrencileriGetir();
            var ogrenci = ogrenciler.FirstOrDefault(p => p.OgrenciID == studentID);
            ogrenci.exist = false;

            Guncelle(ogrenci);
        }
        // Listele D��mesi
        private void button4_Click(object sender, EventArgs e)
        {
            // T�m ogrencilerden gizlenmi� olanlar� ele
            var ogrenciler = TumOgrencileriGetir();
            ogrenciler = ogrenciler.Where(o => o.exist).ToList();
            // ��rencileri �nce isim sonra soyada bakarak alfabetik s�raya koy
            ogrenciler = ogrenciler.OrderBy(o => o.isim).ThenBy(o => o.soyad).ToList();
            // DataGrid'e ogrencileri listele
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Ad", typeof(string));
            dataTable.Columns.Add("Soyad", typeof(string));
            dataTable.Columns.Add("Numara", typeof(string));

            foreach (Ogrenci ogrenci in ogrenciler)
            {
                dataTable.Rows.Add(ogrenci.OgrenciID, ogrenci.isim, ogrenci.soyad, ogrenci.Numara);
            }
            dataGridView1.DataSource = dataTable;
        }
        // Arama D��mesi
        private void button5_Click(object sender, EventArgs e)
        {
            var ogrenciler = TumOgrencileriGetir();
            // Filtreleme
            if (textBox1.Text != "") ogrenciler = ogrenciler.Where(p => p.isim == textBox1.Text).ToList();
            if (textBox2.Text != "") ogrenciler = ogrenciler.Where(p => p.soyad == textBox2.Text).ToList();
            if (textBox3.Text != "") ogrenciler = ogrenciler.Where(p => p.Numara == Int32.Parse(textBox3.Text)).ToList();
            // �nce ad sonra soyada bakarak alfabetik s�ralama
            ogrenciler = ogrenciler.OrderBy(o => o.isim).ThenBy(o => o.soyad).ToList();
            // DataGrid'e ogrencileri listeleme
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Ad", typeof(string));
            dataTable.Columns.Add("Soyad", typeof(string));
            dataTable.Columns.Add("Numara", typeof(string));

            foreach (Ogrenci ogrenci in ogrenciler)
            {
                dataTable.Rows.Add(ogrenci.OgrenciID, ogrenci.isim, ogrenci.soyad, ogrenci.Numara);
            }
            dataGridView1.DataSource = dataTable;
        }





        // Veri G�ncelleme
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                int rowIndex = e.RowIndex;
                int columnIndex = e.ColumnIndex;

                int idColumnIndex = dataGridView1.Columns["ID"].Index;
                int isimColumnIndex = dataGridView1.Columns["Ad"].Index;
                int soyadColumnIndex = dataGridView1.Columns["Soyad"].Index;
                int numaraColumnIndex = dataGridView1.Columns["Numara"].Index;


                int studentID = (int)dataGridView1.Rows[rowIndex].Cells[idColumnIndex].Value;

                var ogrenciler = TumOgrencileriGetir();

                var ogrenci = ogrenciler.FirstOrDefault(p => p.OgrenciID == studentID);
                ogrenci.isim = dataGridView1.Rows[rowIndex].Cells[isimColumnIndex].Value.ToString();
                ogrenci.soyad = dataGridView1.Rows[rowIndex].Cells[soyadColumnIndex].Value.ToString();
                ogrenci.Numara = Int32.Parse(dataGridView1.Rows[rowIndex].Cells[numaraColumnIndex].Value.ToString());

                Guncelle(ogrenci);
            }
        }





        // Fonksiyonlar
        // Ekleme fonksiyonu
        public void Ekle(Ogrenci ogrenci)
        {
            List<Ogrenci> ogrenciler = TumOgrencileriGetir();

            // Ayn� numaraya sahip bir ��renci var m� kontrol et
            bool ayniNumaraVar = ogrenciler.Any(o => o.Numara == ogrenci.Numara);
            if (ayniNumaraVar)
            {
                Console.WriteLine("Hata: Bu numara zaten bir ��renci taraf�ndan kullan�l�yor.");
                return;
            }

            ogrenci.OgrenciID = ogrenciler.Count > 0 ? ogrenciler.Max(o => o.OgrenciID) + 1 : 1;
            ogrenciler.Add(ogrenci);
            DosyayaYaz(ogrenciler);
        }
        // G�ncelleme fonksiyonu
        public void Guncelle(Ogrenci ogrenci)
        {
            List<Ogrenci> ogrenciler = TumOgrencileriGetir();
            Ogrenci eskiOgrenci = ogrenciler.FirstOrDefault(o => o.OgrenciID == ogrenci.OgrenciID);
            if (eskiOgrenci != null)
            {
                eskiOgrenci.isim = ogrenci.isim;
                eskiOgrenci.soyad = ogrenci.soyad;
                eskiOgrenci.Numara = ogrenci.Numara;
                eskiOgrenci.exist = ogrenci.exist;
                DosyayaYaz(ogrenciler);
            }
        }





        // Veri Taban�
        // Veri taban�ndan t�m verileri �eker
        public List<Ogrenci> TumOgrencileriGetir()
        {
            List<Ogrenci> ogrenciler = new List<Ogrenci>();
            if (File.Exists(dosyaAdi))
            {
                using (StreamReader sr = new StreamReader(dosyaAdi))
                {
                    string satir;
                    while ((satir = sr.ReadLine()) != null)
                    {
                        string[] parcalar = satir.Split(',');
                        if (parcalar.Length == 5)
                        {
                            Ogrenci ogrenci = new Ogrenci
                            {
                                isim = parcalar[0],
                                soyad = parcalar[1],
                                Numara = Int32.Parse(parcalar[2]),
                                OgrenciID = Int32.Parse(parcalar[3]),
                                exist = bool.Parse(parcalar[4])
                            };
                            ogrenciler.Add(ogrenci);
                        }
                    }
                }
            }
            return ogrenciler;
        }
        // T�m verileri veri taban�na yazar
        private void DosyayaYaz(List<Ogrenci> ogrenciler)
        {
            ogrenciler = ogrenciler.OrderBy(o => o.isim).ThenBy(o => o.soyad).ToList();

            using (StreamWriter sw = new StreamWriter(dosyaAdi))
            {
                foreach (Ogrenci ogrenci in ogrenciler)
                {
                    sw.WriteLine($"{ogrenci.isim},{ogrenci.soyad},{ogrenci.Numara},{ogrenci.OgrenciID},{ogrenci.exist}");
                }
            }

            ogrenciler = ogrenciler.Where(o => o.exist).ToList();

            ogrenciler = ogrenciler.OrderBy(o => o.isim).ThenBy(o => o.soyad).ToList();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Ad", typeof(string));
            dataTable.Columns.Add("Soyad", typeof(string));
            dataTable.Columns.Add("Numara", typeof(string));

            foreach (Ogrenci ogrenci in ogrenciler)
            {
                dataTable.Rows.Add(ogrenci.OgrenciID, ogrenci.isim, ogrenci.soyad, ogrenci.Numara);
            }

            dataGridView1.DataSource = dataTable;
        }

    }
}