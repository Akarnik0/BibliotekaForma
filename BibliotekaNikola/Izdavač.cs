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
    public partial class izdavac : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //adapter,dataset,dataview,currencymanager i komanda za bazu
        SqlDataAdapter adapter;
        DataSet Izdavač;
        DataView pregled;
        CurrencyManager trenutni;
        string unosizdavaca = "select * from Izdavač";
        SqlCommand unosubazu;
        SqlCommand promjeniubazi;

        //stringovi
        string ID_izdavača;
        string Ime_izdavača;
        string ID_izdavača_stari;
        public izdavac()
        {
            InitializeComponent();
        }

        private void izdavac_Load(object sender, EventArgs e)
        {
            //salje komandu biranja svih vrijednosti iz pisaca koristeci vezu
            adapter = new SqlDataAdapter(unosizdavaca, veza);
            //komande za unos u bazu
            unosubazu = new SqlCommand("insert into Izdavač(ID_izdavača,Ime_izdavača) values(@ID_izdavača,@Ime_izdavača)", veza);
            promjeniubazi = new SqlCommand("update Izdavač set Ime_izdavača=@Ime_izdavača, ID_izdavača=@ID_izdavača where ID_izdavača=@ID_izdavača_stari", veza);
            //dodavanje podataka iz adaptera u dataset izdavač
            Izdavač = new DataSet();
            adapter.Fill(Izdavač, "Izdavač");
            pregled = new DataView(Izdavač.Tables["Izdavač"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
            ID_izdavača_stari = textBox1.Text;
            prikazpozicije();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            textBox1.DataBindings.Clear();
            textBox2.DataBindings.Clear();

            //stavimo nas databind koji upisuje podatke iz baze u textbox
            textBox1.DataBindings.Add("Text", pregled, "ID_izdavača");
            textBox2.DataBindings.Add("Text", pregled, "Ime_izdavača");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pretvaranje unosa korisnika u stringove
            ID_izdavača = textBox1.Text;
            Ime_izdavača = textBox2.Text;
            //dodjeljivanje ovih parametara unutar komandi
            unosubazu.Parameters.AddWithValue("@ID_izdavača", ID_izdavača);
            unosubazu.Parameters.AddWithValue("@Ime_izdavača", Ime_izdavača);
            promjeniubazi.Parameters.AddWithValue("@ID_izdavača", ID_izdavača);
            promjeniubazi.Parameters.AddWithValue("@Ime_izdavača", Ime_izdavača);
            promjeniubazi.Parameters.AddWithValue("@ID_izdavača_stari", ID_izdavača_stari);

            //objasnjenje Regex.IsMatch:
            // ^ - oznacava pocetak stringa
            // \d - trazi unos broja
            // {4} - trazi tacno 4 broja da se unesu
            // $ - oznacava kraj stringa

            //ID_izdavača - slucaj kada unos nije 4 broja
            if (!Regex.IsMatch(ID_izdavača, @"^\d{4}$"))
            {
                MessageBox.Show("Morate unijeti tačno 4 broja u polje ID_izdavača!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_izdavača - slucaj kada je prazno
            if (string.IsNullOrEmpty(Ime_izdavača))
            {
                MessageBox.Show("Ime_izdavača ne smije biti prazno!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_izdavača - slucaj kada je predugo
            if (Ime_izdavača.Length > 255)
            {
                MessageBox.Show("Ime_izdavača je predugo!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (Regex.IsMatch(ID_izdavača, @"^\d{4}$") && !string.IsNullOrEmpty(Ime_izdavača) && Ime_izdavača.Length < 255)
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
                    //ID_izdavača - slucaj kada je unesena redundantna vrijednost
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
                    //ID_izdavača - slucaj kada vrijednost ne postoji
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
            ID_izdavača_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position - 1;
            ID_izdavača_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position + 1;
            ID_izdavača_stari = textBox1.Text;
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
    }
}
