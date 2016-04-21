using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ManyClients
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
            this.button1.Enabled = false;
            int countNetClient = -1;
            int.TryParse(this.comboBox1.Text.Trim(), out countNetClient);
            if (countNetClient < 1) {
                countNetClient = 1;
                this.comboBox1.Text = "1";
            }
            Program.CreateClient(countNetClient);
            this.button1.Enabled = true;
		}
	}
}
