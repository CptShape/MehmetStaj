using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;

namespace MehmetStaj
{
    public partial class Form1 : Form
    {
        string dosyaYolu = "C:\\Users\\yasla\\source\\repos\\MehmetStaj\\MehmetStaj\\database.txt";
        static string metin;
        static List<string> veriler = new List<string>();
        static List<string> isimler = new List<string>();
        static List<string> soyadlar = new List<string>();
        static List<string> numaralar = new List<string>();
        static List<Ogrenci> ogrenciler = new List<Ogrenci>();

        public Form1()
        {
            InitializeComponent();
        }

        public void GetData()
        {
            metin = File.ReadAllText(dosyaYolu);

            isimler.Clear();
            soyadlar.Clear();
            numaralar.Clear();

            veriler = new List<string>(metin.Split('\n'));
            veriler.RemoveAt(veriler.Count - 1);
            veriler.Sort();
            foreach (var veri in veriler)
            {
                List<string> veri2 = new List<string>(veri.Split(','));
                isimler.Add(veri2[0]);
                soyadlar.Add(veri2[1]);
                numaralar.Add(veri2[2]);
            }
        }

        public void ReloadDataGrid()
        {
            ogrenciler.Clear();
            if (dataGridView1.Visible == false)
            {
                return;
            }

            GetData();

            foreach (var veri in veriler)
            {
                List<string> veri2 = new List<string>(veri.Split(','));
                var ogrenci = new Ogrenci()
                {
                    isim = veri2[0],
                    soyad = veri2[1],
                    numara = Int32.Parse(veri2[2]),
                };
                ogrenciler.Add(ogrenci);
            }

            dataGridView1.Rows.Clear();

            foreach (var ogrenci in ogrenciler)
            {
                dataGridView1.Rows.Add(ogrenci.isim, ogrenci.soyad, ogrenci.numara);
            }
        }

        //Ekleme
        private void button1_Click(object sender, EventArgs e)
        {
            var isim = textBox1.Text;
            var soyad = textBox2.Text;
            var numara = textBox3.Text;

            if (isim == "")
            {
                MessageBox.Show("Lütfen bir isim girin.");
                return;
            }
            if (soyad == "")
            {
                MessageBox.Show("Lütfen bir soyad girin.");
                return;
            }
            if (numara == "")
            {
                MessageBox.Show("Lütfen bir numara girin.");
                return;
            }

            GetData();

            foreach (var temp in numaralar)
            {
                if (temp == textBox3.Text + "\r")
                {
                    MessageBox.Show("Bu öðrenci zaten kayýtlý.");
                    return;
                }
            }
            metin += isim + "," + soyad + "," + numara;

            using (StreamWriter writer = new StreamWriter(dosyaYolu))
            {
                writer.WriteLine(metin);
            }
            MessageBox.Show("Ekleme Baþarýlý");

            ReloadDataGrid();
        }

        //Listeleme
        private void button4_Click(object sender, EventArgs e)
        {
            ogrenciler.Clear();
            if (dataGridView1.Visible == true)
            {
                dataGridView1.Visible = false;
                return;
            }

            dataGridView1.Visible = true;

            GetData();

            foreach (var veri in veriler)
            {
                List<string> veri2 = new List<string>(veri.Split(','));
                var ogrenci = new Ogrenci()
                {
                    isim = veri2[0],
                    soyad = veri2[1],
                    numara = Int32.Parse(veri2[2]),
                };
                ogrenciler.Add(ogrenci);
            }

            dataGridView1.Rows.Clear();

            foreach (var ogrenci in ogrenciler)
            {
                dataGridView1.Rows.Add(ogrenci.isim, ogrenci.soyad, ogrenci.numara);
            }
        }

        //Silme
        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewRow seciliSatir = dataGridView1.SelectedRows[0];

            GetData();

            veriler.RemoveAt(seciliSatir.Index);

            string yenimetin = string.Empty;

            using (StreamWriter writer = new StreamWriter(dosyaYolu))
            {
                foreach (var veri in veriler)
                {
                    yenimetin += veri + "\n";
                }
                if (yenimetin != "") yenimetin = yenimetin.Substring(0, yenimetin.Length - 1);
                writer.WriteLine(yenimetin);
            }

            ReloadDataGrid();
        }

        //Güncelleme
        private void button3_Click(object sender, EventArgs e)
        {
            string collectedData = string.Empty;
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                {
                    collectedData += cell.Value.ToString() + ",";
                }
                collectedData = collectedData.Substring(0, collectedData.Length - 1);
                collectedData += "\n";
            }

            if (!string.IsNullOrEmpty(collectedData))
            {
                collectedData = collectedData.Substring(0, collectedData.Length - 1);
            }

            using (StreamWriter writer = new StreamWriter(dosyaYolu))
            {
                writer.WriteLine(collectedData);
            }
        }

        // Arama
        private void button5_Click(object sender, EventArgs e)
        {
            GetData();

            foreach (var veri in veriler)
            {
                List<string> veri2 = new List<string>(veri.Split(','));
                var ogrenci = new Ogrenci()
                {
                    isim = veri2[0],
                    soyad = veri2[1],
                    numara = Int32.Parse(veri2[2]),
                };
                ogrenciler.Add(ogrenci);
            }

            var sonuc = new List<Ogrenci>();

            if (textBox1.Text != "") sonuc.AddRange(ogrenciler.Where(p => p.isim == textBox1.Text).ToList());
            if (textBox2.Text != "") sonuc.AddRange(ogrenciler.Where(p => p.soyad == textBox2.Text).ToList());
            if (textBox3.Text != "") sonuc.AddRange(ogrenciler.Where(p => p.numara == Int32.Parse(textBox3.Text)).ToList());

            List<Ogrenci> asilSonuc = sonuc
            .GroupBy(ogrenci => new { ogrenci.isim, ogrenci.soyad, ogrenci.numara })
            .Select(grup => grup.First())
            .ToList();

            dataGridView1.Rows.Clear();

            foreach (var sonuc2 in asilSonuc)
            {
                dataGridView1.Rows.Add(sonuc2.isim, sonuc2.soyad, sonuc2.numara);
            }

        }
    }
}