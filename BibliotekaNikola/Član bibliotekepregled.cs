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
    public partial class Član_bibliotekepregled : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //objekti vezani za sql
        SqlDataAdapter adapter;
        SqlDataAdapter pretrazi;
        DataSet clan;
        SqlCommand pretraga;

        //stringovi
        string unosclana = "select * from Član_biblioteke";
        string Ime_clana;
        public Član_bibliotekepregled()
        {
            InitializeComponent();
        }

        private void Član_bibliotekepregled_Load(object sender, EventArgs e)
        {
            //ucitavanje podataka u dataset
            adapter = new SqlDataAdapter(unosclana, veza);
            clan = new DataSet();
            adapter.Fill(clan, "Član_biblioteke");
            //postavljanje izvora podataka za datagridview
            dataGridView1.DataSource = clan.Tables["Član_biblioteke"];
            //ucitavanje komande za pretragu
            pretraga = new SqlCommand("use Biblioteka select * from Član_biblioteke where Ime_člana like @Ime_clana", veza);
            pretrazi = new SqlDataAdapter(pretraga);
        }

        //izvrsava se kada god se promjeni tekst u textboxu za pretragu
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ime_clana = "%" + textBox2.Text + "%";
            pretraga.Parameters.AddWithValue("@Ime_clana", Ime_clana);
            clan.Clear();
            pretrazi.Fill(clan, "Član_biblioteke");
            dataGridView1.DataSource = clan.Tables["Član_biblioteke"];
            pretraga.Parameters.Clear();
        }
    }
}
