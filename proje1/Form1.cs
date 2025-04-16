using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Microsoft.Win32;
using System.Globalization;

namespace proje1
{
    public partial class Form1 : Form
    {
        private SQLiteConnection sqliteConnection;
        public Form1()
        {
            InitializeComponent();
            string databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "veritabani.db");
            Console.WriteLine($"Database Path: {databasePath}");
            sqliteConnection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            sqliteConnection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Kullanici (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Ad TEXT,
                    Soyad TEXT,
                    Boy INTEGER,
                    Kilo INTEGER,
                    Gun INTEGER,
                    Ay TEXT,
                    Yil INTEGER
                );";

            using (var command = new SQLiteCommand(createTableQuery, sqliteConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqliteConnection != null)
            {
                sqliteConnection.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string ad = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string soyad = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string tarih = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private float HesaplaVKI(float kilo, float boy)
        {
            return kilo / (boy * boy);
        }

        private string VKIYorumu(float vki)
        {
            if (vki < 18.5) return "Zayıf";
            else if (vki < 25) return "Normal";
            else if (vki < 30) return "Fazla kilolu";
            else return "Obez";
        }

        private string BurcHesapla(int gun, string ay)
        {
            if ((gun >= 21 && ay == "Mart") || (gun <= 20 && ay == "Nisan")) return "Koç";
            if ((gun >= 21 && ay == "Nisan") || (gun <= 20 && ay == "Mayıs")) return "Boğa";
            if ((gun >= 21 && ay == "Mayıs") || (gun <= 20 && ay == "Haziran")) return "İkizler";
            if ((gun >= 21 && ay == "Haziran") || (gun <= 22 && ay == "Temmuz")) return "Yengeç";
            if ((gun >= 23 && ay == "Temmuz") || (gun <= 22 && ay == "Ağustos")) return "Aslan";
            if ((gun >= 23 && ay == "Ağustos") || (gun <= 22 && ay == "Eylül")) return "Başak";
            if ((gun >= 23 && ay == "Eylül") || (gun <= 22 && ay == "Ekim")) return "Terazi";
            if ((gun >= 23 && ay == "Ekim") || (gun <= 21 && ay == "Kasım")) return "Akrep";
            if ((gun >= 22 && ay == "Kasım") || (gun <= 21 && ay == "Aralık")) return "Yay";
            if ((gun >= 22 && ay == "Aralık") || (gun <= 19 && ay == "Ocak")) return "Oğlak";
            if ((gun >= 20 && ay == "Ocak") || (gun <= 18 && ay == "Şubat")) return "Kova";
            return "Balık";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Lütfen ad ve soyad bilgilerini giriniz.");
                return;
            }

            string dogumTarihiStr = textBox3.Text.Trim();
            string[] tarihParcalari = dogumTarihiStr.Split(' ');

            if (tarihParcalari.Length != 3)
            {
                MessageBox.Show("Lütfen doğum tarihini '31 Ocak 2004' formatında giriniz.");
                return;
            }

            if (!int.TryParse(tarihParcalari[0], out int gun))
            {
                MessageBox.Show("Lütfen geçerli bir gün değeri giriniz.");
                return;
            }

            string ay = tarihParcalari[1];
            if (!int.TryParse(tarihParcalari[2], out int yil))
            {
                MessageBox.Show("Lütfen geçerli bir yıl değeri giriniz.");
                return;
            }

            string ad = textBox1.Text;
            string soyad = textBox2.Text;
            int boy = 0;
            int kilo = 0;

            string selectQuery = @"
        SELECT Boy, Kilo 
        FROM Kullanici 
        WHERE Ad = @Ad AND Soyad = @Soyad AND Gun = @Gun AND Ay = @Ay AND Yil = @Yil;";

            using (var command = new SQLiteCommand(selectQuery, sqliteConnection))
            {
                command.Parameters.AddWithValue("@Ad", ad);
                command.Parameters.AddWithValue("@Soyad", soyad);
                command.Parameters.AddWithValue("@Gun", gun);
                command.Parameters.AddWithValue("@Ay", ay);
                command.Parameters.AddWithValue("@Yil", yil);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        boy = Convert.ToInt32(reader["Boy"]);
                        kilo = Convert.ToInt32(reader["Kilo"]);
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı bilgileri bulunamadı.");
                        return;
                    }
                }
            }

            float boyMetre = boy / 100f;

            string burc = BurcHesapla(gun, ay);
            float vki = HesaplaVKI(kilo, boyMetre);
            string vkiYorum = VKIYorumu(vki);

            string burcYorum;
            switch (burc)
            {
                case "Koç":
                    burcYorum = "Koç burcu cesur ve enerjik bir yapıya sahiptir.";
                    break;
                case "Boğa":
                    burcYorum = "Boğa burcu sabırlı ve güvenilir bir yapıya sahiptir.";
                    break;
                case "İkizler":
                    burcYorum = "İkizler burcu meraklı ve iletişim becerileri yüksektir.";
                    break;
                case "Yengeç":
                    burcYorum = "Yengeç burcu duygusal ve koruyucu bir yapıya sahiptir.";
                    break;
                case "Aslan":
                    burcYorum = "Aslan burcu liderlik özellikleriyle öne çıkar.";
                    break;
                case "Başak":
                    burcYorum = "Başak burcu titiz ve çalışkan bir yapıya sahiptir.";
                    break;
                case "Terazi":
                    burcYorum = "Terazi burcu adaletli ve uyumlu bir yapıya sahiptir.";
                    break;
                case "Akrep":
                    burcYorum = "Akrep burcu tutkulu ve kararlı bir yapıya sahiptir.";
                    break;
                case "Yay":
                    burcYorum = "Yay burcu özgür ruhlu ve maceracı bir yapıya sahiptir.";
                    break;
                case "Oğlak":
                    burcYorum = "Oğlak burcu disiplinli ve sorumluluk sahibidir.";
                    break;
                case "Kova":
                    burcYorum = "Kova burcu yenilikçi ve bağımsız bir yapıya sahiptir.";
                    break;
                case "Balık":
                    burcYorum = "Balık burcu hayalperest ve sezgisel bir yapıya sahiptir.";
                    break;
                default:
                    burcYorum = "Burç yorumu bulunamadı.";
                    break;
            }

            richTextBox3.Text = vkiYorum;
            richTextBox1.Text = burcYorum;
            BurcResminiGoster(burc);
        }

        private void buttonEkle_Click_1(object sender, EventArgs e)
        {
            // Ad ve soyad kontrolü
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Lütfen ad ve soyad bilgilerini giriniz.");
                return;
            }

            // Boy kontrolü
            if (!int.TryParse(textBox4.Text, out int boy) || boy <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir boy değeri giriniz (ör. 175).");
                return;
            }

            // Kilo kontrolü
            if (!int.TryParse(textBox5.Text, out int kilo) || kilo <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir kilo değeri giriniz (ör. 70).");
                return;
            }

            // Tarih kontrolü ve parse işlemi
            string dogumTarihiStr = textBox3.Text.Trim();
            string[] tarihParcalari = dogumTarihiStr.Split(' ');

            if (tarihParcalari.Length != 3)
            {
                MessageBox.Show("Lütfen doğum tarihini '31 Ocak 2004' formatında giriniz.");
                return;
            }

            if (!int.TryParse(tarihParcalari[0], out int gun))
            {
                MessageBox.Show("Lütfen geçerli bir gün değeri giriniz.");
                return;
            }

            string ay = tarihParcalari[1];
            if (!int.TryParse(tarihParcalari[2], out int yil))
            {
                MessageBox.Show("Lütfen geçerli bir yıl değeri giriniz.");
                return;
            }

            // Veritabanına ekleme işlemi
            string ad = textBox1.Text;
            string soyad = textBox2.Text;

            string insertQuery = @"
        INSERT INTO Kullanici (Ad, Soyad, Boy, Kilo, Gun, Ay, Yil)
        VALUES (@Ad, @Soyad, @Boy, @Kilo, @Gun, @Ay, @Yil);";

            using (var command = new SQLiteCommand(insertQuery, sqliteConnection))
            {
                command.Parameters.AddWithValue("@Ad", ad);
                command.Parameters.AddWithValue("@Soyad", soyad);
                command.Parameters.AddWithValue("@Boy", boy);
                command.Parameters.AddWithValue("@Kilo", kilo);
                command.Parameters.AddWithValue("@Gun", gun);
                command.Parameters.AddWithValue("@Ay", ay);
                command.Parameters.AddWithValue("@Yil", yil);

                command.ExecuteNonQuery();
            }

            MessageBox.Show("Kullanıcı bilgileri başarıyla eklendi.");
        }



        private void BurcResminiGoster(string burc)
        {
            string resimDizini = "Data";
            string resimDosyasi = $"{resimDizini}/{burc.ToLower()}.jpg";
            
            pictureBox1.Image = Image.FromFile(resimDosyasi);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
