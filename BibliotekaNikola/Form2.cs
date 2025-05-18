using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
//NAPOMENA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//ovo dvoje je potrebno dodati u svaku formu
//potrebni su za izvrsavanje funkcija i unos podataka u bazi
using System.Configuration;
using System.Data.SqlClient;

namespace BibliotekaNikola
{
    public partial class Form2 : Form
    {

        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //adapter,dataset,dataview,currencymanager i komanda za bazu
        //pakistanac nije dobro objasnio currencymanager
        SqlDataAdapter adapter;
        DataSet pisac;
        DataView pregled;
        CurrencyManager trenutni;
        string unospisaca = "select * from Pisac";
        SqlCommand unosubazu;
        SqlCommand promjeniubazi;

        //stringovi
        string ID_pisca;
        string Ime_pisca;

        public Form2()
        {
            InitializeComponent();
        }

        //ucitavanje stvari
        private void Form2_Load(object sender, EventArgs e)
        {
            //salje komandu biranja svih vrijednosti iz pisaca koristeci vezu
            adapter = new SqlDataAdapter(unospisaca, veza);
            //komande za unos u bazu
            unosubazu = new SqlCommand("insert into Pisac(ID_pisca,Ime_pisca) values(@ID_pisca,@Ime_pisca)", veza);
            promjeniubazi = new SqlCommand("update Pisac set Ime_pisca=@Ime_pisca where ID_pisca=@ID_pisca",veza);
            //dodavanje podataka iz adaptera u dataset pisac
            pisac = new DataSet();
            adapter.Fill(pisac,"Pisac");
            //pakistanac nije dobro objasnio ovaj dio, ako program prestane raditi vjerovatno je do ovoga
            pregled = new DataView(pisac.Tables["Pisac"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
            prikazpozicije();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            textBox1.DataBindings.Clear();
            textBox2.DataBindings.Clear();

            //stavimo nas databind koji upisuje podatke iz baze u textbox
            textBox1.DataBindings.Add("Text", pregled, "ID_pisca");
            textBox2.DataBindings.Add("Text", pregled, "Ime_pisca");
        }

        //validacija i unos podataka u bazu
        private void button1_Click(object sender, EventArgs e)
        {
            //pretvaranje unosa korisnika u stringove
            ID_pisca = textBox1.Text;
            Ime_pisca = textBox2.Text;
            //dodjeljivanje ovih vrijednosti parametrima unutar komandi
            unosubazu.Parameters.AddWithValue("@ID_pisca", ID_pisca);
            unosubazu.Parameters.AddWithValue("@Ime_pisca", Ime_pisca);
            promjeniubazi.Parameters.AddWithValue("@ID_pisca", ID_pisca);
            promjeniubazi.Parameters.AddWithValue("@Ime_pisca", Ime_pisca);

            //objasnjenje Regex.IsMatch:
            // ^ - oznacava pocetak stringa
            // \d - trazi unos broja
            // {4} - trazi tacno 4 broja da se unesu
            // $ - oznacava kraj stringa

            //ID_pisca - slucaj kada unos nije 4 broja
            //Luka(moj brat, ne Lazarov) mi je rekao da ovdje napisem "moj tata"...
            if (!Regex.IsMatch(ID_pisca, @"^\d{4}$"))
            {
                MessageBox.Show("Morate unijeti tačno 4 broja u polje ID_pisca!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_pisca - slucaj kada je prazno
            if(string.IsNullOrEmpty(Ime_pisca))
            {
                MessageBox.Show("Ime_pisca ne smije biti prazno!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_pisca - slucaj kada je predugo
            if (Ime_pisca.Length > 255)
            {
                MessageBox.Show("Ime_pisca je predugo!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if(Regex.IsMatch(ID_pisca, @"^\d{4}$") && !string.IsNullOrEmpty(Ime_pisca) && Ime_pisca.Length < 255)
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
                    }
                    //ID_pisca - slucaj kada je unesena redundantna vrijednost
                    catch (Exception redundantnost)
                    {
                        MessageBox.Show(redundantnost.Message, "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //u slucaju da mjenjamo postojecu vrijednost u bazi
                //PAZNJA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //OVO TREBA NAPRAVITI!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                else 
                {
                    try
                    {
                        veza.Open();
                        promjeniubazi.ExecuteNonQuery();
                        prikazpozicije();
                    }
                    //ID_pisca - slucaj kada vrijednost ne postoji
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
            label1.Text = trenutni.Position+1 + ". od " + trenutni.Count.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        //dugmici za navigaciju
        private void button2_Click(object sender, EventArgs e)
        {
            trenutni.Position = 0;
            prikazpozicije();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position - 1;
            prikazpozicije();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position + 1;
            prikazpozicije();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Count - 1;
            prikazpozicije();
        }
    }
}
