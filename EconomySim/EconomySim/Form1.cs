using LiveCharts;
using LiveCharts.Wpf;
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

        private Timer autoStepTimer;

        public Form1()
        {
            InitializeComponent();

            SetupChart();
            SetupTimer();            
        }

        private void SetupChart()
        {
            lineChart.Series = new SeriesCollection
            {
                //Sample data...
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> {4, 6, 5, 2, 7}
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> {6, 7, 3, 4, 6},
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> {5, 2, 8, 3},
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };

            lineChart.AxisX.Add(new Axis
            {
                Title = "Month",
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
            });

            lineChart.AxisY.Add(new Axis
            {
                Title = "Sales",
                LabelFormatter = value => value.ToString("C")
            });

            lineChart.LegendLocation = LegendLocation.Right;
        }

        private void SetupTimer()
        {
            autoStepTimer = new Timer();
            autoStepTimer.Tick += AutoStepTimer_Tick;
            autoStepTimer.Interval = 1000;
            autoStepTimer.Enabled = true;
            autoStepTimer.Start();
        }

        private void AutoStepTimer_Tick(object sender, EventArgs e)
        {
            if (market != null && autoRunCbx.Checked)
                run(100);
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
            textBox1.Text = res.strListGood.Replace("\n", "\t") + Environment.NewLine;
            textBox1.Text += res.strListGoodPrices.Replace("\n", "\t") + Environment.NewLine;
            textBox1.Text += res.strListGoodTrades.Replace("\n", "\t") + Environment.NewLine;
            textBox1.Text += res.strListGoodBids.Replace("\n", "\t") + Environment.NewLine;
            textBox1.Text += res.strListGoodAsks.Replace("\n", "\t") + Environment.NewLine;
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
