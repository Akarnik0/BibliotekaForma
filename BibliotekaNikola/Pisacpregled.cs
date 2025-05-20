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
        SqlDataAdapter adapter;
        DataSet pisac;

        string unospisaca = "select * from Pisac";
        public Pisacpregled()
        {
            InitializeComponent();
        }

        private void Pisacpregled_Load(object sender, EventArgs e)
        {
            adapter = new SqlDataAdapter(unospisaca, veza);
            pisac = new DataSet();
            adapter.Fill(pisac, "Pisac");
            dataGridView1.DataSource = pisac.Tables["Pisac"];
        }
    }
}
