using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace BibliotekaNikola
{
    public partial class Knjiga : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source=.\sqlexpress01;Initial Catalog=Biblioteka;Integrated Security=True;Encrypt=False;");
        //adapter,dataset,dataview,currencymanager i komanda za bazu
        SqlDataAdapter adapter;
        SqlDataAdapter adapterpisac;
        SqlDataAdapter adapterzanr;
        SqlDataAdapter adapterizdavac;

        DataSet knjiga;
        DataSet pisac;
        DataSet zanr;
        DataSet izdavac;

        DataView pregled;

        CurrencyManager trenutni;

        string unosknjiga = "select * from Knjiga";
        string unospisac = "select * from Pisac";
        string unoszanr = "select * from Žanr";
        string unosizdavac = "select * from Izdavač";

        SqlCommand unosubazu;
        SqlCommand promjeniubazi;

        //stringovi koji se koriste u komandama
        string ID_knjige;
        string Ime_knjige;
        string ID_pisca;
        string ID_žanra;
        string ID_izdavača;
        string ID_knjige_stari;

        //stringovi koji se koriste pri mjenjanju vrijednosti
        public Knjiga()
        {
            InitializeComponent();
        }

        private void Knjiga_Load(object sender, EventArgs e)
        {
            //salje komandu biranja svih potrebnih vrijednosti iz baze koristeci vezu
            adapter = new SqlDataAdapter(unosknjiga, veza);
            adapterpisac = new SqlDataAdapter(unospisac, veza);
            adapterzanr = new SqlDataAdapter(unoszanr, veza);
            adapterizdavac = new SqlDataAdapter(unosizdavac, veza);

            //komande za unos u bazu
            unosubazu = new SqlCommand("insert into Knjiga(ID_knjige,Ime_knjige,ID_pisca,ID_žanra,ID_izdavača) values(@ID_knjige,@Ime_knjige,@ID_pisca,@ID_žanra,@ID_izdavača)", veza);
            promjeniubazi = new SqlCommand("update Knjiga set ID_knjige=@ID_knjige, Ime_knjige=@Ime_knjige, ID_pisca=@ID_pisca, ID_žanra=@ID_žanra, ID_izdavača=@ID_izdavača where ID_knjige=@ID_knjige_stari", veza);


            //dodavanje podataka iz adaptera u dataset pisac
            knjiga = new DataSet();
            pisac = new DataSet();
            zanr = new DataSet();
            izdavac = new DataSet();
            
            adapterpisac.Fill(pisac,"Pisac");
            adapterzanr.Fill(zanr, "Žanr");
            adapterizdavac.Fill(izdavac, "Izdavač");

            //lista podataka u combobox1
            comboBox1.DataSource = pisac.Tables["Pisac"];
            comboBox1.DisplayMember = "Ime_pisca";
            comboBox1.ValueMember = "ID_pisca";
            //lista podataka u combobox2
            comboBox2.DataSource = zanr.Tables["Žanr"];
            comboBox2.DisplayMember = "Ime_žanra";
            comboBox2.ValueMember = "ID_žanra";
            //lista podataka u combobox3
            comboBox3.DataSource = izdavac.Tables["Izdavač"];
            comboBox3.DisplayMember = "Ime_izdavača";
            comboBox3.ValueMember = "ID_izdavača";

            adapter.Fill(knjiga, "Knjiga");
            pregled = new DataView(knjiga.Tables["Knjiga"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
            ID_knjige_stari = textBox1.Text;
            prikazpozicije();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            textBox1.DataBindings.Clear();
            textBox2.DataBindings.Clear();
            comboBox1.DataBindings.Clear();
            comboBox2.DataBindings.Clear();
            comboBox3.DataBindings.Clear();
            //stavimo nas databind koji upisuje podatke iz baze u textbox
            textBox1.DataBindings.Add("Text", pregled, "ID_knjige");
            textBox2.DataBindings.Add("Text", pregled, "Ime_knjige");
            comboBox1.DataBindings.Add("SelectedValue", pregled, "ID_pisca");
            comboBox2.DataBindings.Add("SelectedValue", pregled, "ID_žanra");
            comboBox3.DataBindings.Add("SelectedValue", pregled, "ID_izdavača");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pretvaranje unosa korisnika u stringove
            ID_knjige = textBox1.Text;
            Ime_knjige = textBox2.Text;
            //ukratko: pojavi se error ako ne stoji ova provjera jer selectedvalue moze biti null... puno objasnjenje bi bilo predugo
            if (!string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(comboBox2.Text) && !string.IsNullOrEmpty(comboBox3.Text))
            {
                ID_pisca = comboBox1.SelectedValue.ToString();
                ID_žanra = comboBox2.SelectedValue.ToString();
                ID_izdavača = comboBox3.SelectedValue.ToString();
            }
            //dodjeljivanje ovih parametara unutar komandi
            unosubazu.Parameters.AddWithValue("@ID_knjige", ID_knjige);
            unosubazu.Parameters.AddWithValue("@Ime_knjige", Ime_knjige);
            unosubazu.Parameters.AddWithValue("ID_pisca", ID_pisca);
            unosubazu.Parameters.AddWithValue("ID_žanra", ID_žanra);
            unosubazu.Parameters.AddWithValue("ID_izdavača", ID_izdavača);
            promjeniubazi.Parameters.AddWithValue("@ID_knjige", ID_knjige);
            promjeniubazi.Parameters.AddWithValue("@Ime_knjige", Ime_knjige);
            promjeniubazi.Parameters.AddWithValue("@ID_pisca", ID_pisca);
            promjeniubazi.Parameters.AddWithValue("@ID_žanra", ID_žanra);
            promjeniubazi.Parameters.AddWithValue("@ID_izdavača", ID_izdavača);
            //ID_knjige - slucaj kada unos nije 4 broja
            if (!Regex.IsMatch(ID_knjige, @"^\d{4}$"))
            {
                MessageBox.Show("Morate unijeti tačno 4 broja u polje ID_knjige!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_knjige - slucaj kada je prazno
            if (string.IsNullOrEmpty(Ime_knjige))
            {
                MessageBox.Show("Ime_knjige ne smije biti prazno!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Ime_knjige - slucaj kada je predugo
            if (Ime_knjige.Length > 255)
            {
                MessageBox.Show("Ime_knjige je predugo!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //ID_pisca - slucaj kada je prazno
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("ID_pisca ne smije biti prazan!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //ID_žanra - slucaj kada je prazno
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                MessageBox.Show("ID_žanra ne smije biti prazan!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //ID_izdavača - slucaj kada je prazno
            if (string.IsNullOrEmpty(comboBox3.Text))
            {
                MessageBox.Show("ID_izdavača ne smije biti prazan!", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if(Regex.IsMatch(ID_knjige, @"^\d{4}$") && !string.IsNullOrEmpty(Ime_knjige) && Ime_knjige.Length < 255 && !string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(comboBox2.Text) && !string.IsNullOrEmpty(comboBox3.Text))
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

        private void prikazpozicije()
        {
            label1.Text = trenutni.Position + 1 + ". od " + trenutni.Count.ToString();
        }

        //za navigaciju
        private void button2_Click(object sender, EventArgs e)
        {
            trenutni.Position = 0;
            ID_knjige_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position - 1;
            ID_knjige_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Position + 1;
            ID_knjige_stari = textBox1.Text;
            prikazpozicije();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            trenutni.Position = trenutni.Count - 1;
            prikazpozicije();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
