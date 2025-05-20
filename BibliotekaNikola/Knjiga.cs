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

namespace BibliotekaNikola
{
    public partial class Knjiga : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
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
            //TREBA SE NAPRAVITI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


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
            comboBox1.DataSource = zanr.Tables["Žanr"];
            comboBox1.DisplayMember = "Ime_žanra";
            comboBox1.ValueMember = "ID_žanra";
            //lista podataka u combobox3
            comboBox1.DataSource = izdavac.Tables["Izdavač"];
            comboBox1.DisplayMember = "Ime_izdavača";
            comboBox1.ValueMember = "ID_izdavača";

            adapter.Fill(knjiga, "Knjiga");
            pregled = new DataView(knjiga.Tables["Knjiga"]);
            trenutni = (CurrencyManager)this.BindingContext[pregled];
            pregled.AddNew();
            Navigacija();
        }

        //navigacija po podacima iz baze
        private void Navigacija()
        {
            //sklonimo bilo koje prosle bindove sa tekst boxova
            comboBox1.DataBindings.Clear();
            comboBox2.DataBindings.Clear();
            comboBox3.DataBindings.Clear();
            //stavimo nas databind koji upisuje podatke iz baze u textbox
            comboBox1.DataBindings.Add("SelectedValue", pregled, "ID_pisca");
            comboBox2.DataBindings.Add("SelectedValue", pregled, "ID_žanra");
            comboBox3.DataBindings.Add("SelectedValue", pregled, "ID_izdavača");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
