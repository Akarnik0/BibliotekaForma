using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BibliotekaNikola
{
    public partial class Član_biblioteke : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //adapter,dataset,dataview,currencymanager i komanda za bazu
        SqlDataAdapter adapter;
        DataSet član;
        DataView pregled;
        CurrencyManager trenutni;
        string unosčlana = "select * from Član_biblioteke";
        SqlCommand unosubazu;
        SqlCommand promjeniubazi;

        //stringovi
        string ID_člana;
        string Ime_člana;
        string ID_člana_stari;
        public Član_biblioteke()
        {
            InitializeComponent();
        }

        private void Član_biblioteke_Load(object sender, EventArgs e)
        {
            //salje komandu biranja svih vrijednosti iz članabiblioteke koristeci vezu
            adapter = new SqlDataAdapter(unosčlana, veza);
            //komande za unos u bazu
            unosubazu = new SqlCommand("insert into Član_biblioteke(ID_člana,Ime_člana) values(@ID_člana,@Ime_člana)", veza);
            promjeniubazi = new SqlCommand("update Član_biblioteke set Ime_člana=@Ime_člana, ID_člana=@ID_člana where ID_člana=@ID_člana_stari", veza);
            //dodavanje podataka iz adaptera u dataset član
            član = new DataSet();
            adapter.Fill(član, "Član_biblioteke");
            pregled = new DataView(član.Tables["Član_biblioteke"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
            ID_člana_stari = textBox1.Text;
            prikazpozicije();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            textBox1.DataBindings.Clear();
            textBox2.DataBindings.Clear();

            //stavimo nas databind koji upisuje podatke iz baze u textbox
            textBox1.DataBindings.Add("Text", pregled, "ID_člana");
            textBox2.DataBindings.Add("Text", pregled, "Ime_člana");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pretvaranje unosa korisnika u stringove
            ID_člana = textBox1.Text;
            Ime_člana = textBox2.Text;
            //dodjeljivanje ovih parametara unutar komandi
            unosubazu.Parameters.AddWithValue("@ID_člana", ID_člana);
            unosubazu.Parameters.AddWithValue("@Ime_člana", Ime_člana);
            promjeniubazi.Parameters.AddWithValue("@ID_člana", ID_člana);
            promjeniubazi.Parameters.AddWithValue("@Ime_člana", Ime_člana);
            promjeniubazi.Parameters.AddWithValue("@ID_člana_stari", ID_člana_stari);

            //objasnjenje Regex.IsMatch:
            // ^ - oznacava pocetak stringa
            // \d - trazi unos broja
            // {4} - trazi tacno 4 broja da se unesu
            // $ - oznacava kraj stringa

            //ID_člana - slucaj kada unos nije 4 broja
            if (!Regex.IsMatch(ID_člana, @"^\d{4}$"))
            {
                MessageBox.Show("Morate unijeti tačno 4 broja u polje ID_člana!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_člana - slucaj kada je prazno
            if (string.IsNullOrEmpty(Ime_člana))
            {
                MessageBox.Show("Ime_člana ne smije biti prazno!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_člana - slucaj kada je predugo
            if (Ime_člana.Length > 255)
            {
                MessageBox.Show("Ime_člana je predugo!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (Regex.IsMatch(ID_člana, @"^\d{4}$") && !string.IsNullOrEmpty(Ime_člana) && Ime_člana.Length < 255)
            {
                //u slucaju da unosimo novu vrijednost u bazu
                if (trenutni.Position == trenutni.Count - 1)
                {
                    try
                    {
                        veza.Open();
                        unosubazu.ExecuteNonQuery();
                        pregled.AddNew();
                        prikazpozicije();
                        veza.Close();
                        //brisanje parametara
                        unosubazu.Parameters.Clear();
                        promjeniubazi.Parameters.Clear();
                    }
                    //ID_člana - slucaj kada je unesena redundantna vrijednost
                    catch (Exception redundantnost)
                    {
                        MessageBox.Show(redundantnost.Message, "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //u slucaju da mjenjamo postojecu vrijednost u bazi
                else
                {
                    try
                    {
                        veza.Open();
                        promjeniubazi.ExecuteNonQuery();
                        prikazpozicije();
                        veza.Close();
                        //brisanje parametara
                        unosubazu.Parameters.Clear();
                        promjeniubazi.Parameters.Clear();
                    }
                    //ID_člana - slucaj kada vrijednost ne postoji
                    catch (Exception ne_postoji)
                    {
                        MessageBox.Show(ne_postoji.Message, "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //prikazuje trenutnu poziciju unutar tabele pomocu label-a
        private void prikazpozicije()
        {
            label1.Text = trenutni.Position + 1 + ". od " + trenutni.Count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trenutni.Position = 0;
            ID_člana_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position - 1;
            ID_člana_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position + 1;
            ID_člana_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Count - 1;
            prikazpozicije();
        }
    }
}
