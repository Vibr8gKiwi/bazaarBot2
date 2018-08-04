using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EconomySim
{
    public partial class Form1 : Form
    {

        private Economy economy;
	    private Market market;
//	    private MarketDisplay display;
//	    private TextField txt_benchmark;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            economy = new DoranAndParberryEconomy();

            market = economy.getMarket("default");

            dataGridView1.DataSource = market._agents;
            //dataGridView2.DataSource = market._book.dbook;

        }

        private void run(int rounds)
        {
            market.simulate(rounds);
            var res = market.get_marketReport(rounds);
            dataGridView1.Refresh();
            //dataGridView2.DataSource = res.arrStrListInventory;
            textBox1.Clear();
            textBox1.Text = res.strListGood.Replace("\n", "  ") + Environment.NewLine;
            textBox1.Text += res.strListGoodPrices.Replace("\n", "  ") + Environment.NewLine;
            textBox1.Text += res.strListGoodTrades.Replace("\n", "  ") + Environment.NewLine;
            textBox1.Text += res.strListGoodBids.Replace("\n", "  ") + Environment.NewLine;
            textBox1.Text += res.strListGoodAsks.Replace("\n", "  ") + Environment.NewLine;
            //textBox1.Lines = res.arrStrListInventory.ToArray<string>();
            //dataGridView1.DataSource = market._agents;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            run(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            run(20);
        }
    }
}
