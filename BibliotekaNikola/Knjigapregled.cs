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
    public partial class Knjigapregled : Form
    {
        //stvaranje veze sa bazom
        SqlConnection veza = new SqlConnection(@"Data Source =.\sqlexpress; Initial Catalog = Biblioteka; Integrated Security = True; Encrypt = False");
        //objekti vezani za sql
        SqlDataAdapter adapter;
        SqlDataAdapter pretrazi;
        DataSet knjiga;
        SqlCommand pretraga;

        //stringovi
        string unosknjiga = "SELECT Knjiga.ID_knjige, Knjiga.Ime_knjige, Pisac.Ime_pisca, Žanr.Ime_žanra, Izdavač.Ime_izdavača FROM Knjiga INNER JOIN Pisac ON Knjiga.ID_pisca = Pisac.ID_pisca INNER JOIN Žanr ON Knjiga.ID_žanra = Žanr.ID_žanra INNER JOIN Izdavač ON Knjiga.ID_izdavača = Izdavač.ID_izdavača";
        string Ime_knjige;
        public Knjigapregled()
        {
            InitializeComponent();
        }

        private void Knjigapregled_Load(object sender, EventArgs e)
        {
            //ucitavanje podataka u dataset
            adapter = new SqlDataAdapter(unosknjiga, veza);
            knjiga = new DataSet();
            adapter.Fill(knjiga, "Knjiga");
            //postavljanje izvora podataka za datagridview
            dataGridView1.DataSource = knjiga.Tables["Knjiga"];
            //ucitavanje komande za pretragu
            pretraga = new SqlCommand("USE Biblioteka SELECT Knjiga.ID_knjige, Knjiga.Ime_knjige, Pisac.Ime_pisca, Žanr.Ime_žanra, Izdavač.Ime_izdavača FROM Knjiga INNER JOIN Pisac ON Knjiga.ID_pisca = Pisac.ID_pisca INNER JOIN Žanr ON Knjiga.ID_žanra = Žanr.ID_žanra INNER JOIN Izdavač ON Knjiga.ID_izdavača = Izdavač.ID_izdavača where Ime_knjige like @Ime_knjige", veza);
            pretrazi = new SqlDataAdapter(pretraga);
        }

        //izvrsava se kada god se promjeni tekst u textboxu za pretragu
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ime_knjige = "%" + textBox2.Text + "%";
            pretraga.Parameters.AddWithValue("@Ime_knjige", Ime_knjige);
            knjiga.Clear();
            pretrazi.Fill(knjiga, "Knjiga");
            dataGridView1.DataSource = knjiga.Tables["Knjiga"];
            pretraga.Parameters.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
