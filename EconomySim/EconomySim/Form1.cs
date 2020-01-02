using EconomySim.Models;
using LiveCharts;
using LiveCharts.Configurations;
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
        private Timer autoStepTimer;
        private int iterationCount = 0;

        public ChartValues<PriceModel> FoodPriceValues { get; set; }

        public Form1()
        {
            InitializeComponent();

            SetupChart();
            SetupTimer();            
        }

        private void SetupChart()
        {
            FoodPriceValues = new ChartValues<PriceModel>();

            var mapper = Mappers.Xy<PriceModel>()
                .X(model => model.Iteration)        //use accumulated iteration count X
                .Y(model => model.Price);           //use the Price property as Y

            //lets save the mapper globally.
            Charting.For<PriceModel>(mapper);

            lineChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Food Price",
                    Values = FoodPriceValues
                }
            };

            //lineChart.AxisX.Add(new Axis
            //{
            //    Title = "Iterations",
            //    Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
            //});

            //lineChart.AxisY.Add(new Axis
            //{
            //    Title = "Price",
            //    LabelFormatter = value => value.ToString("C")
            //});
            

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

            market = economy.GetMarket("default");

            dataGridView1.DataSource = market.agents;
            //dataGridView2.DataSource = market._book.dbook;

        }

        private void run(int rounds)
        {
            iterationCount += rounds; 

            market.Simulate(rounds);
            var res = market.GetMarketReport(rounds);
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

            //TODO: parse out the results and add to graph
            FoodPriceValues.Add(new PriceModel
            {
                Iteration = iterationCount,
                Price = Double.Parse(res.strListGoodPrices.Split('\n')[2])
            });
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
