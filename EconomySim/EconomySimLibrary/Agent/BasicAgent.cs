using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    /**
     * The most fundamental agent class, and has as little implementation as possible.
     * In most cases you should start by extending Agent instead of this.
     * @author larsiusprime
     */

    public class AgentData {
	    public string className { get; set;}
	    public double money;
	    public InventoryData inventory;
	    public string logicName;
	    public  Logic logic;
	    public int? lookBack;

        public AgentData(string className, double money, string logicName)
        {
            this.className = className;
            this.money = money;
            this.logicName = logicName;
        }

    }

    public class Point
    {
        public double x;
        public double y;
        public Point(double x,double y)
        {
            this.x = x;
            this.y = y;
        }
    }


    public class BasicAgent
    {
	    public int id;				//unique integer identifier
        public string className { get; set; }	//string identifier, "famer", "woodcutter", etc.
        public double money { get; set; }
        public double Space { get { return _inventory.getEmptySpace(); } }
        public double nProduct { get; set; }

        //public var moneyLastRound(default, null):double;
        //public var profit(get, null):double;
        //public var inventorySpace(get, null):double;
        //public var inventoryFull(get, null):Bool;
        //public var destroyed(default, null):Bool;
        public bool destroyed; //dfs stub  needed?
        public double moneyLastRound; //dfs stub needed?
        public double profit; //dfs stub needed?

        public double trackcosts;

	    /********PRIVATE************/

	    private Logic _logic;
	    protected Inventory _inventory;
	    protected Dictionary<string,List<double>> _observedTradingRange;
	    private double _profit = 0;	//profit from last round
	    private int _lookback = 15;


	    public BasicAgent(int id, AgentData data)
	    {
		    this.id = id;
		    className = data.className;
		    money = data.money;
		    _inventory = new Inventory();
		    _inventory.fromData(data.inventory);
		    _logic = data.logic;

		    if (data.lookBack == null)
		    {
			    _lookback = 15;
		    }
		    else
		    {
			    _lookback = (int)data.lookBack;
		    }

		    _observedTradingRange = new Dictionary<String, List<double>>();

            trackcosts = 0;
	    }

	    public void destroy()
	    {
		    destroyed = true;
		    _inventory.destroy();
		    foreach (string key in _observedTradingRange.Keys)
		    {
			    var list = _observedTradingRange[key];
                list.Clear();
		    }
            _observedTradingRange.Clear();
		    _observedTradingRange = null;
		    _logic = null;
	    }

	    public void init(Market market)
	    {
		    var listGoods = market.getGoods_unsafe();//List<String>
		    foreach (string str in listGoods)
		    {
			    var trades = new List<double>();

                var price = 2;// market.getAverageHistoricalPrice(str, _lookback);
			    trades.Add(price * 1.0);
			    trades.Add(price * 3.0);	//push two fake trades to generate a range

			    //set initial price belief & observed trading range
			    _observedTradingRange[str]=trades;
		    }
	    }

	    public void simulate(Market market)
	    {
		    _logic.perform(this, market);
	    }

        public virtual void generateOffers(Market bazaar, string good)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

        public virtual void updatePriceModel(Market bazaar, String act, String good, bool success, double unitPrice = 0)
	    {
		    //no implementation -- provide your own in a subclass
	    }

	    public virtual Offer createBid(Market bazaar, String good, double limit)
	    {
		    //no implementation -- provide your own in a subclass
		    return null;
	    }

        public virtual Offer createAsk(Market bazaar, String commodity_, double limit_)
	    {
		    //no implementation -- provide your own in a subclass
		    return null;
	    }

	    public double queryInventory(String good)
	    {
		    return _inventory.query(good);
	    }

	    public void produceInventory(String good, double delta)
	    {
            if (trackcosts < 1) trackcosts = 1;
            double curunitcost = _inventory.change(good, delta, trackcosts / delta);
            trackcosts = 0;
	    }

        public void consumeInventory(String good, double delta)
        {
            if (good == "money")
            {
                money += delta;
                if (delta < 0)
                    trackcosts += (-delta);
            }
            else
            {
                double curunitcost = _inventory.change(good, delta, 0);
                if (delta < 0)
                    trackcosts += (-delta) * curunitcost;
            }
        }

        public void changeInventory(String good, double delta, double unit_cost)
        {
            if (good == "money")
            {
                money += delta;
            }
            else
            {
                _inventory.change(good, delta, unit_cost);
            }
        }

	    /********PRIVATE************/


	    private double get_inventorySpace()
	    {
		    return _inventory.getEmptySpace();
	    }

	    public bool get_inventoryFull()
	    {
		    return _inventory.getEmptySpace() == 0;
	    }

	    public double get_profit()
	    {
		    return money - moneyLastRound;
	    }

	    protected double determineSaleQuantity(Market bazaar, String commodity_)
	    {
		    var mean = bazaar.getAverageHistoricalPrice(commodity_,_lookback); //double
		    var trading_range = observeTradingRange(commodity_,10);//point
		    if (trading_range != null && mean>0)
		    {
			    var favorability= Quick.positionInRange(mean, trading_range.x, trading_range.y);//double
			    //position_in_range: high means price is at a high point

			    double amount_to_sell = Math.Round(favorability * _inventory.surplus(commodity_)); //double
amount_to_sell = _inventory.query(commodity_);
			    if (amount_to_sell < 1)
			    {
				    amount_to_sell = 1;
			    }
			    return amount_to_sell;
		    }
		    return 0;
	    }

        protected double determinePurchaseQuantity(Market bazaar, String commodity_)
	    {
		    var mean = bazaar.getAverageHistoricalPrice(commodity_,_lookback);//double
		    var trading_range = observeTradingRange(commodity_,10); //Point
		    if (trading_range != null)
		    {
			    var favorability = Quick.positionInRange(mean, trading_range.x, trading_range.y);//double
			    favorability = 1 - favorability;
			    //do 1 - favorability to see how close we are to the low end

			    double amount_to_buy = Math.Round(favorability * _inventory.shortage(commodity_));//double
			    if (amount_to_buy < 1)
			    {
				    amount_to_buy = 1;
			    }
			    return amount_to_buy;
		    }
		    return 0;
	    }

	    private Point observeTradingRange(String good, int window)
	    {
		    var a = _observedTradingRange[good]; //List<double>
		    var pt = new Point(Quick.minArr(a,window), Quick.maxArr(a,window));
		    return pt;
	    }
    }

}
