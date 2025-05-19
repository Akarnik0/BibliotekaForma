using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BibliotekaNikola
{
    public partial class Članska_karta : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //adapter,dataset,dataview,currencymanager i komanda za bazu
        SqlDataAdapter adapter;
        SqlDataAdapter adapterknjiga;
        SqlDataAdapter adapterkarta;

        DataSet član;
        DataSet knjiga;
        DataSet karta;

        DataView pregled;

        CurrencyManager trenutni;

        string unosčlan = "select * from Član_biblioteke";
        string unosknjiga = "select * from Knjiga";
        string unoskarta = "select * from Članska_karta";

        SqlCommand unosubazu;
        SqlCommand promjeniubazi;

        //stringovi koji se koriste pri mjenjanju vrijednosti
        string ID_člana;
        string ID_knjige;
        string ID_člana_stari;
        string ID_knjige_stari;
        public Članska_karta()
        {
            InitializeComponent();
        }

        private void Članska_karta_Load(object sender, EventArgs e)
        {
            //salje komandu biranja svih potrebnih vrijednosti iz baze koristeci vezu
            adapter = new SqlDataAdapter(unosčlan, veza);
            adapterknjiga = new SqlDataAdapter(unosknjiga, veza);
            adapterkarta = new SqlDataAdapter(unoskarta, veza);

            //komande za unos u bazu
            unosubazu = new SqlCommand("insert into Članska_karta(ID_člana,ID_knjige) values(@ID_člana,@ID_knjige)", veza);
            promjeniubazi = new SqlCommand("update Članska_karta set ID_člana=@ID_člana, ID_knjige=@ID_knjige where ID_člana=@ID_člana_stari and ID_knjige=@ID_knjige_stari", veza);

            //dodavanje podataka iz adaptera u dataset pisac
            član = new DataSet();
            knjiga = new DataSet();
            karta = new DataSet();

            adapter.Fill(član, "Član_biblioteke");
            adapterknjiga.Fill(knjiga, "Knjiga");

            //lista podataka u combobox1
            comboBox1.DataSource = član.Tables["Član_biblioteke"];
            comboBox1.DisplayMember = "Ime_člana";
            comboBox1.ValueMember = "ID_člana";
            //lista podataka u combobox2
            comboBox2.DataSource = knjiga.Tables["Knjiga"];
            comboBox2.DisplayMember = "Ime_knjige";
            comboBox2.ValueMember = "ID_knjige";

            adapterkarta.Fill(karta, "Članska_karta");
            pregled = new DataView(karta.Tables["Članska_karta"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
            ID_člana_stari = comboBox1.SelectedValue.ToString();
            ID_knjige_stari = comboBox2.SelectedValue.ToString();
            prikazpozicije();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            comboBox1.DataBindings.Clear();
            comboBox2.DataBindings.Clear();
            //stavimo nas databind koji upisuje podatke iz baze u textbox
            comboBox1.DataBindings.Add("SelectedValue", pregled, "ID_člana");
            comboBox2.DataBindings.Add("SelectedValue", pregled, "ID_knjige");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ukratko: pojavi se error ako ne stoji ova provjera jer selectedvalue moze biti null... puno objasnjenje bi bilo predugo
            if (!string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(comboBox2.Text))
            {
                //pretvaranje unosa korisnika u stringove
                ID_člana = comboBox1.SelectedValue.ToString();
                ID_knjige = comboBox2.SelectedValue.ToString();
            }

            //dodjeljivanje ovih parametara unutar komandi
            unosubazu.Parameters.AddWithValue("@ID_člana", ID_člana);
            unosubazu.Parameters.AddWithValue("@ID_knjige", ID_knjige);
            promjeniubazi.Parameters.AddWithValue("@ID_člana", ID_člana);
            promjeniubazi.Parameters.AddWithValue("@ID_knjige", ID_knjige);
            promjeniubazi.Parameters.AddWithValue("@ID_člana_stari", ID_člana_stari);
            promjeniubazi.Parameters.AddWithValue("@ID_knjige_stari", ID_knjige_stari);

            //ID_člana - slucaj kada je prazno
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("ID_člana ne smije biti prazan!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //ID_knjige - slucaj kada je prazno
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                MessageBox.Show("ID_knjige ne smije biti prazan!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (!string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(comboBox2.Text))
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
                    //ID_pisca - slucaj kada je unesena redundantna vrijednost
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
            label1.Text = trenutni.Position + 1 + ". od " + trenutni.Count.ToString();
        }

        //za navigaciju
        private void button2_Click(object sender, EventArgs e)
        {
            trenutni.Position = 0;
            ID_člana_stari = comboBox1.SelectedValue.ToString();
            ID_knjige_stari = comboBox2.SelectedValue.ToString();
            prikazpozicije();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position - 1;
            ID_člana_stari = comboBox1.SelectedValue.ToString();
            ID_knjige_stari = comboBox2.SelectedValue.ToString();
            prikazpozicije();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position + 1;
            //ukratko: pojavi se error ako ne stoji ova provjera jer selectedvalue moze biti null... puno objasnjenje bi bilo predugo
            if (!string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(comboBox2.Text))
            {
                ID_člana_stari = comboBox1.SelectedValue.ToString();
                ID_knjige_stari = comboBox2.SelectedValue.ToString();
            }
            prikazpozicije();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Count - 1;
            prikazpozicije();
        }
    }
}
