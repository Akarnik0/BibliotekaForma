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
    public partial class Članska_kartapregled : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //objekti vezani za sql
        SqlDataAdapter adapter;
        SqlDataAdapter pretraziclan;
        SqlDataAdapter pretraziknjigu;
        DataSet clanskakarta;
        SqlCommand pretragaclan;
        SqlCommand pretragaknjiga;

        //stringovi
        string unosclanskakarta = "SELECT Član_biblioteke.Ime_člana, Knjiga.Ime_knjige FROM Članska_karta INNER JOIN Član_biblioteke ON Članska_karta.ID_člana = Član_biblioteke.ID_člana INNER JOIN Knjiga ON Članska_karta.ID_knjige = Knjiga.ID_knjige";
        string Ime_clana;
        string Ime_knjige;
        public Članska_kartapregled()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ime_clana = "%" + textBox2.Text + "%";
            pretragaclan.Parameters.AddWithValue("@Ime_člana", Ime_clana);
            clanskakarta.Clear();
            pretraziclan.Fill(clanskakarta, "Članska_karta");
            dataGridView1.DataSource = clanskakarta.Tables["Članska_karta"];
            pretragaclan.Parameters.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Ime_knjige = "%" + textBox1.Text + "%";
            pretragaknjiga.Parameters.AddWithValue("@Ime_knjige", Ime_knjige);
            clanskakarta.Clear();
            pretraziknjigu.Fill(clanskakarta, "Članska_karta");
            dataGridView1.DataSource = clanskakarta.Tables["Članska_karta"];
            pretragaknjiga.Parameters.Clear();
        }

        private void Članska_kartapregled_Load(object sender, EventArgs e)
        {
            //ucitavanje podataka u dataset
            adapter = new SqlDataAdapter(unosclanskakarta, veza);
            clanskakarta = new DataSet();
            adapter.Fill(clanskakarta, "Članska_karta");
            //postavljanje izvora podataka za datagridview
            dataGridView1.DataSource = clanskakarta.Tables["Članska_karta"];
            //ucitavanje komande za pretragu
            pretragaclan = new SqlCommand("USE Biblioteka SELECT Član_biblioteke.Ime_člana, Knjiga.Ime_knjige FROM Članska_karta INNER JOIN Član_biblioteke ON Članska_karta.ID_člana = Član_biblioteke.ID_člana INNER JOIN Knjiga ON Članska_karta.ID_knjige = Knjiga.ID_knjige where Ime_člana like @Ime_člana", veza);
            pretraziclan = new SqlDataAdapter(pretragaclan);
            pretragaknjiga = new SqlCommand("USE Biblioteka SELECT Član_biblioteke.Ime_člana, Knjiga.Ime_knjige FROM Članska_karta INNER JOIN Član_biblioteke ON Članska_karta.ID_člana = Član_biblioteke.ID_člana INNER JOIN Knjiga ON Članska_karta.ID_knjige = Knjiga.ID_knjige where Ime_knjige like @Ime_knjige", veza);
            pretraziknjigu = new SqlDataAdapter(pretragaknjiga);
        }
    }
}
