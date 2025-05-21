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
    public partial class Izdavačpregled : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //objekti vezani za sql
        SqlDataAdapter adapter;
        SqlDataAdapter pretrazi;
        DataSet izdavac;
        SqlCommand pretraga;

        //stringovi
        string unosizdavaca = "select * from Izdavač";
        string Ime_izdavaca;
        public Izdavačpregled()
        {
            InitializeComponent();
        }

        private void Izdavačpregled_Load(object sender, EventArgs e)
        {
            //ucitavanje podataka u dataset
            adapter = new SqlDataAdapter(unosizdavaca, veza);
            izdavac = new DataSet();
            adapter.Fill(izdavac, "Izdavač");
            //postavljanje izvora podataka za datagridview
            dataGridView1.DataSource = izdavac.Tables["Izdavač"];
            //ucitavanje komande za pretragu
            pretraga = new SqlCommand("use Biblioteka select * from Izdavač where Ime_izdavača like @Ime_izdavaca", veza);
            pretrazi = new SqlDataAdapter(pretraga);
        }

        //izvrsava se kada god se promjeni tekst u textboxu za pretragu
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ime_izdavaca = "%" + textBox2.Text + "%";
            pretraga.Parameters.AddWithValue("@Ime_izdavaca", Ime_izdavaca);
            izdavac.Clear();
            pretrazi.Fill(izdavac, "Izdavač");
            dataGridView1.DataSource = izdavac.Tables["Izdavač"];
            pretraga.Parameters.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
