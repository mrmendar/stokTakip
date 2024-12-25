using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Forms.ComboBox;
using TextBox = System.Windows.Forms.TextBox;

namespace sql3
{
    public partial class frmUrunEkle : Form
    {
        public frmUrunEkle()
        {
            InitializeComponent();
        }
        
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432; Database=Stok_Takip; user Id=postgres; password=admin123; ");
        bool durum;
        private void barkodKontrol()
        {
            durum = true;
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("select *from urun", baglanti);
            NpgsqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                if (txtBarkodNo.Text == read["barkodno"].ToString()||txtBarkodNo.Text=="")
                {
                    durum = false;
                }
            }
            baglanti.Close();
        }
        private void kategorigetir()
        {
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("select *from kategoribilgileri", baglanti);
            NpgsqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                comboKategori.Items.Add(read["kategori"].ToString());
            }
            baglanti.Close();
        }
        private void frmUrunEkle_Load(object sender, EventArgs e)
        {
            kategorigetir();
        }

        private void comboKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboMarka.Items.Clear();
            comboMarka.Text = "";
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("select *from markabilgileri where kategori ='"+comboKategori.SelectedItem+"'", baglanti);
            NpgsqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                comboMarka.Items.Add(read["marka"].ToString());
            }
            baglanti.Close();
        }

        private void btnYeniEkle_Click(object sender, EventArgs e)
        {

            barkodKontrol();
            if (durum == true)
            {
                baglanti.Open();
                NpgsqlCommand komut = new NpgsqlCommand("insert into urun(barkodno,kategori,marka,urunadi,miktari,alisfiyati,satisfiyati,tarih) values(@barkodno,@kategori,@marka,@urunadi,@miktari,@alisfiyati,@satisfiyati,@tarih)", baglanti);
                komut.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                komut.Parameters.AddWithValue("@kategori", comboKategori.Text);
                komut.Parameters.AddWithValue("@marka", comboMarka.Text);
                komut.Parameters.AddWithValue("@urunadi", txtUrunAdi.Text);
                komut.Parameters.AddWithValue("@miktari", int.Parse(txtMiktari.Text));
                komut.Parameters.AddWithValue("@alisfiyati", int.Parse(txtAlisfiyati.Text));
                komut.Parameters.AddWithValue("@satisfiyati", int.Parse(txtSatisFiyati.Text));
                komut.Parameters.AddWithValue("tarih", DateTime.Now.ToString());
                komut.ExecuteNonQuery();

                baglanti.Close();
                MessageBox.Show("Ürün eklendi");
            }
            else
            {
                MessageBox.Show("Böyle bir barkodNo var.");
            }
            
            
            comboMarka.Items.Clear();

            foreach(Control item in groupBox1.Controls)
            {
                if(item is  TextBox)
                {
                    item.Text = "";
                }
                if(item is ComboBox)
                {
                    item.Text = "";
                }
            }

        }

        private void textBarkod_TextChanged(object sender, EventArgs e)
        {
            if (textBarkod.Text == "")
            {
                lblMiktar.Text = "";
                foreach (Control item in groupBox2.Controls)
                {
                    if(item is System.Windows.Forms.TextBox)
                    {
                        item.Text = "";
                    }
                }
            }
            
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("select *from urun where barkodno like'"+textBarkod.Text+"'", baglanti);
            NpgsqlDataReader read = komut.ExecuteReader();
            while(read.Read())
            {
                textKategori.Text = read["kategori"].ToString();
                textMarka.Text = read["marka"].ToString();
                textUrunAdi.Text = read["urunadi"].ToString();
                textMiktari.Text = read["miktari"].ToString();
                textAlisFiyati.Text = read["alisfiyati"].ToString();
                textSatisFiyati.Text = read["satisfiyati"].ToString();
            }
            baglanti.Close();
        }

        private void btnVarOlan_Click(object sender, EventArgs e)
        {
            
            
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("update urun set miktari=miktari+'" + int.Parse(textMiktari.Text) + "'where barkodno='" + textBarkod.Text + "'", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close() ;
            foreach (Control item in groupBox2.Controls)
            {
                if (item is TextBox)
                {
                    item.Text = "";
                }
            }
            MessageBox.Show("Var olan ürüne ekleme yapıldı"); 
        }
    }
}
