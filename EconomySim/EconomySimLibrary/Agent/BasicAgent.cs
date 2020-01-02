using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class BasicAgent
    {
	    public int Id;				//unique integer identifier
        public string ClassName { get; set; }	//string identifier, "famer", "woodcutter", etc.
        public double Money { get; set; }
        public double Space { get { return Inventory.GetEmptySpace(); } }
        public double NProduct { get; set; }

        //public var moneyLastRound(default, null):double;
        //public var profit(get, null):double;
        //public var inventorySpace(get, null):double;
        //public var inventoryFull(get, null):Bool;
        //public var destroyed(default, null):Bool;
        public bool Destroyed; //dfs stub  needed?
        public double MoneyLastRound; //dfs stub needed?
        public double Profit; //dfs stub needed?

        public double TrackCosts;

	    /********PRIVATE************/

	    private Logic logic;
	    protected Inventory Inventory;
	    protected Dictionary<string,List<double>> ObservedTradingRange;
	    private double profit = 0;	//profit from last round
	    private int lookback = 15;


	    public BasicAgent(int id, AgentData data)
	    {
		    Id = id;
		    ClassName = data.ClassName;
		    Money = data.Money;
		    Inventory = new Inventory();
		    Inventory.FromData(data.Inventory);
		    logic = data.Logic;

		    if (data.LookBack == null)
		    {
			    lookback = 15;
		    }
		    else
		    {
			    lookback = (int)data.LookBack;
		    }

		    ObservedTradingRange = new Dictionary<string, List<double>>();

            TrackCosts = 0;
	    }

	    public void Destroy()
	    {
		    Destroyed = true;
		    Inventory.Destroy();
		    foreach (string key in ObservedTradingRange.Keys)
		    {
			    var list = ObservedTradingRange[key];
                list.Clear();
		    }
            ObservedTradingRange.Clear();
		    ObservedTradingRange = null;
		    logic = null;
	    }

	    public void Init(Market market)
	    {
		    var listGoods = market.GetGoodsUnsafe();
		    foreach (string str in listGoods)
		    {
			    var trades = new List<double>();

                var price = 2;// market.getAverageHistoricalPrice(str, _lookback);
			    trades.Add(price * 1.0);
			    trades.Add(price * 3.0);	//push two fake trades to generate a range

			    //set initial price belief & observed trading range
			    ObservedTradingRange[str]=trades;
		    }
	    }

	    public void Simulate(Market market)
	    {
		    logic.Perform(this, market);
	    }

        public virtual void GenerateOffers(Market bazaar, string good)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

        public virtual void UpdatePriceModel(Market bazaar, string act, string good, bool success, double unitPrice = 0)
	    {
		    //no implementation -- provide your own in a subclass
	    }

	    public virtual Offer CreateBid(Market bazaar, string good, double limit)
	    {
		    //no implementation -- provide your own in a subclass
		    return null;
	    }

        public virtual Offer CreateAsk(Market bazaar, string commodity_, double limit_)
	    {
		    //no implementation -- provide your own in a subclass
		    return null;
	    }

	    public double QueryInventory(string good)
	    {
		    return Inventory.Query(good);
	    }

	    public void ProduceInventory(string good, double delta)
	    {
            if (TrackCosts < 1)
                TrackCosts = 1;

            double curunitcost = Inventory.Change(good, delta, TrackCosts / delta);
            TrackCosts = 0;
	    }

        public void ConsumeInventory(string good, double delta)
        {
            if (good == "money")
            {
                Money += delta;
                if (delta < 0)
                    TrackCosts += (-delta);
            }
            else
            {
                double curunitcost = Inventory.Change(good, delta, 0);
                if (delta < 0)
                    TrackCosts += (-delta) * curunitcost;
            }
        }

        public void ChangeInventory(string good, double delta, double unit_cost)
        {
            if (good == "money")
            {
                Money += delta;
            }
            else
            {
                Inventory.Change(good, delta, unit_cost);
            }
        }

        public double GetProfit()
        {
            return Money - MoneyLastRound;
        }

        public bool GetInventoryFull()
        {
            return Inventory.GetEmptySpace() == 0;
        }



        /********PRIVATE************/

        private double getInventorySpace()
	    {
		    return Inventory.GetEmptySpace();
	    }

        /********PROTECTED************/

        protected double DetermineSaleQuantity(Market bazaar, string commodity)
	    {
		    var mean = bazaar.GetAverageHistoricalPrice(commodity,lookback); //double
		    var tradingRange = observeTradingRange(commodity,10);//point

		    if (tradingRange != null && mean > 0)
		    {
			    var favorability= Quick.PositionInRange(mean, tradingRange.x, tradingRange.y);//double
			    //position_in_range: high means price is at a high point

			    double amountToSell = Math.Round(favorability * Inventory.Surplus(commodity)); //double
                amountToSell = Inventory.Query(commodity);

			    if (amountToSell < 1)
			    {
				    amountToSell = 1;
			    }
			    return amountToSell;
		    }

		    return 0;
	    }

        protected double DeterminePurchaseQuantity(Market bazaar, string commodity)
	    {
		    var mean = bazaar.GetAverageHistoricalPrice(commodity,lookback);//double
		    var tradingRange = observeTradingRange(commodity,10); //Point

		    if (tradingRange != null)
		    {
			    var favorability = Quick.PositionInRange(mean, tradingRange.x, tradingRange.y);//double
			    favorability = 1 - favorability;
			    //do 1 - favorability to see how close we are to the low end

			    double amountToBuy = Math.Round(favorability * Inventory.Shortage(commodity));//double
			    if (amountToBuy < 1)
			    {
				    amountToBuy = 1;
			    }
			    return amountToBuy;
		    }
		    return 0;
	    }

	    private Point observeTradingRange(string good, int window)
	    {
		    var a = ObservedTradingRange[good]; //List<double>
		    var pt = new Point(Quick.MinArr(a,window), Quick.MaxArr(a,window));
		    return pt;
	    }
    }

}
