using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibliotekaNikola
{
    public partial class Form1 : Form
    {
        //deklarisanje formi
        Form2 pisac = new Form2();
        Knjiga knjiga = new Knjiga();
        Žanr žanr = new Žanr();
        izdavac izdavac = new izdavac();
        Član_biblioteke član = new Član_biblioteke();
        Članska_karta karta = new Članska_karta();
        Pisacpregled pisacpregled = new Pisacpregled();
        Knjigapregled knjigapregled = new Knjigapregled();
        Žanrpregled žanrpregled = new Žanrpregled();
        Izdavačpregled izdavačpregled = new Izdavačpregled();
        Član_bibliotekepregled članpregled = new Član_bibliotekepregled();
        Članska_kartapregled kartapregled = new Članska_kartapregled();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //funkcije za navigaciju formi
        //nmp gdje button1 pobjegao
        private void button2_Click(object sender, EventArgs e)
        {
            knjiga.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pisac.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            žanr.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            izdavac.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            član.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            karta.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            knjigapregled.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pisacpregled.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            žanrpregled.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            izdavačpregled.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            članpregled.ShowDialog();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            kartapregled.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
