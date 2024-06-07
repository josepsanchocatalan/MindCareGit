using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindCare
{
    public partial class ControlVersiones : Form
    {
        public ControlVersiones()
        {
            InitializeComponent();

            textBox1.Text = "MindCare Inc.";
            textBox2.Text = "1.2.15.3";

            textBox1.Enabled = false;
            textBox2.Enabled = false;

            disenyoInicial();

        }

        private void disenyoInicial()
        {
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
