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

namespace BibliotekaNikola
{
    public partial class Žanr : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //adapter,dataset,dataview,currencymanager i komanda za bazu
        SqlDataAdapter adapter;
        DataSet žanr;
        DataView pregled;
        CurrencyManager trenutni;
        string unoszanra = "select * from Žanr";
        SqlCommand unosubazu;
        SqlCommand promjeniubazi;

        //stringovi
        string ID_žanra;
        string Ime_žanra;
        string ID_žanra_stari;
        public Žanr()
        {
            InitializeComponent();
        }

        private void Žanr_Load(object sender, EventArgs e)
        {
            label2.Text = null;
            //salje komandu biranja svih vrijednosti iz pisaca koristeci vezu
            adapter = new SqlDataAdapter(unoszanra, veza);
            //komande za unos u bazu
            unosubazu = new SqlCommand("insert into Žanr(ID_žanra,Ime_žanra) values(@ID_žanra,@Ime_žanra)", veza);
            promjeniubazi = new SqlCommand("update Žanr set Ime_žanra=@Ime_žanra, ID_žanra=@ID_žanra where ID_žanra=@ID_žanra_stari", veza);
            //dodavanje podataka iz adaptera u dataset žanr
            žanr = new DataSet();
            adapter.Fill(žanr, "Žanr");
            pregled = new DataView(žanr.Tables["Žanr"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
            ID_žanra_stari = textBox1.Text;
            prikazpozicije();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            textBox1.DataBindings.Clear();
            textBox2.DataBindings.Clear();

            //stavimo nas databind koji upisuje podatke iz baze u textbox
            textBox1.DataBindings.Add("Text", pregled, "ID_žanra");
            textBox2.DataBindings.Add("Text", pregled, "Ime_žanra");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pretvaranje unosa korisnika u stringove
            ID_žanra = textBox1.Text;
            Ime_žanra = textBox2.Text;
            //dodjeljivanje ovih parametara unutar komandi
            unosubazu.Parameters.AddWithValue("@ID_žanra", ID_žanra);
            unosubazu.Parameters.AddWithValue("@Ime_žanra", Ime_žanra);
            promjeniubazi.Parameters.AddWithValue("@ID_žanra", ID_žanra);
            promjeniubazi.Parameters.AddWithValue("@Ime_žanra", Ime_žanra);
            promjeniubazi.Parameters.AddWithValue("@ID_žanra_stari", ID_žanra_stari);

            //objasnjenje Regex.IsMatch:
            // ^ - oznacava pocetak stringa
            // \d - trazi unos broja
            // {4} - trazi tacno 4 broja da se unesu
            // $ - oznacava kraj stringa

            //ID_žanra - slucaj kada unos nije 4 broja
            if (!Regex.IsMatch(ID_žanra, @"^\d{4}$"))
            {
                MessageBox.Show("Morate unijeti tačno 4 broja u polje ID_žanra!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_žanra - slucaj kada je prazno
            if (string.IsNullOrEmpty(Ime_žanra))
            {
                MessageBox.Show("Ime_žanra ne smije biti prazno!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_žanra - slucaj kada je predugo
            if (Ime_žanra.Length > 255)
            {
                MessageBox.Show("Ime_žanra je predugo!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (Regex.IsMatch(ID_žanra, @"^\d{4}$") && !string.IsNullOrEmpty(Ime_žanra) && Ime_žanra.Length < 255)
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
                        label2.Text = "Podaci su uneseni u bazu";
                    }
                    //ID_žanra - slucaj kada je unesena redundantna vrijednost
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
                        label2.Text = "Podaci su izmjenjeni u bazi";
                    }
                    //ID_žanra - slucaj kada vrijednost ne postoji
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
            ID_žanra_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position - 1;
            ID_žanra_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position + 1;
            ID_žanra_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Count - 1;
            prikazpozicije();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
