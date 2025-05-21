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
    public partial class Pisacpregled : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //objekti vezani za sql
        SqlDataAdapter adapter;
        SqlDataAdapter pretrazi;
        DataSet pisac;
        SqlCommand pretraga;

        //stringovi
        string unospisaca = "select * from Pisac";
        string Ime_pisca;
        public Pisacpregled()
        {
            InitializeComponent();
        }

        private void Pisacpregled_Load(object sender, EventArgs e)
        {
            //ucitavanje podataka u dataset
            adapter = new SqlDataAdapter(unospisaca, veza);
            pisac = new DataSet();
            adapter.Fill(pisac, "Pisac");
            //postavljanje izvora podataka za datagridview
            dataGridView1.DataSource = pisac.Tables["Pisac"];
            //ucitavanje komande za pretragu
            pretraga = new SqlCommand("use Biblioteka select * from Pisac where Ime_pisca like @Ime_pisca",veza);
            pretrazi = new SqlDataAdapter(pretraga);
        }

        //izvrsava se kada god se promjeni tekst u textboxu za pretragu
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //Ime_pisca mora izgledati ovako jer SqlCommand gleda sve unutar '' bukvalno
            //tj kada se napise %@Ime_pisca% ono ce bukvalno shvatiti da moraju imati "@Ime_pisca" negdje unutar imena
            Ime_pisca = "%" + textBox2.Text + "%"; ;
            pretraga.Parameters.AddWithValue("@Ime_pisca", Ime_pisca);
            pisac.Clear();
            pretrazi.Fill(pisac, "Pisac");
            dataGridView1.DataSource = pisac.Tables["Pisac"];
            pretraga.Parameters.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
