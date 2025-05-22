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
    public partial class Žanrpregled : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //objekti vezani za sql
        SqlDataAdapter adapter;
        SqlDataAdapter pretrazi;
        DataSet zanr;
        SqlCommand pretraga;

        //stringovi
        string unoszanra = "select * from Žanr";
        string Ime_zanra;
        public Žanrpregled()
        {
            InitializeComponent();
        }

        private void Žanrpregled_Load(object sender, EventArgs e)
        {
            //ucitavanje podataka u dataset
            adapter = new SqlDataAdapter(unoszanra, veza);
            zanr = new DataSet();
            adapter.Fill(zanr, "Žanr");
            //postavljanje izvora podataka za datagridview
            dataGridView1.DataSource = zanr.Tables["Žanr"];
            //ucitavanje komande za pretragu
            pretraga = new SqlCommand("use Biblioteka select * from Žanr where Ime_žanra like @Ime_zanra", veza);
            pretrazi = new SqlDataAdapter(pretraga);
        }

        //izvrsava se kada god se promjeni tekst u textboxu za pretragu
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ime_zanra = "%" + textBox2.Text + "%";
            pretraga.Parameters.AddWithValue("@Ime_zanra", Ime_zanra);
            zanr.Clear();
            pretrazi.Fill(zanr, "Žanr");
            dataGridView1.DataSource = zanr.Tables["Žanr"];
            pretraga.Parameters.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
