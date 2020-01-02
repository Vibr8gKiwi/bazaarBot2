using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public interface ISignalBankrupt
    {
        void SignalBankrupt(Market m, BasicAgent agent);
    }

    public class Market
    {
	    public String name;

	    /**Logs information about all economic activity in this market**/
	    public History history;

	    /**Signal fired when an agent's money reaches 0 or below**/
	    public ISignalBankrupt signalBankrupt;


	    /********PRIVATE*********/

	    private int _roundNum = 0;

	    private List<String> _goodTypes;		//list of string ids for all the legal commodities
	    public BindingList<BasicAgent> _agents;
	    public TradeBook _book;
	    private Dictionary<String, AgentData> _mapAgents;
	    private Dictionary<String, Good> _mapGoods;
        
        
        public Market(string name, ISignalBankrupt isb)
	    {
		    this.name = name;

		    history = new History();
		    _book = new TradeBook();
		    _goodTypes = new List<String>();
		    _agents = new BindingList<BasicAgent>();
		    _mapGoods = new Dictionary<String, Good>();
		    _mapAgents = new Dictionary<String, AgentData>();

		    signalBankrupt = isb;//new TypedSignal<Market->BasicAgent->Void>();
	    }

	    public void Init(MarketData data)
	    {
		    fromData(data);
	    }

	    public int numTypesOfGood()
	    {
		    return _goodTypes.Count;
	    }

	    public int numAgents()
	    {
		    return _agents.Count;
	    }

	    public void replaceAgent(BasicAgent oldAgent, BasicAgent newAgent)
	    {
		    newAgent.Id = oldAgent.Id;
		    _agents[oldAgent.Id] = newAgent;
		    oldAgent.Destroy();
		    newAgent.Init(this);
	    }

	    //@:access(bazaarbot.agent.BasicAgent)    //dfs stub ????
	    public void simulate(int rounds)
	    {
		    for (int round=0; round<rounds; round++)
		    {
			    foreach (var agent in _agents)
			    {
				    agent.MoneyLastRound = agent.Money;
				    agent.Simulate(this);

				    foreach (var commodity in _goodTypes)
				    {
					    agent.GenerateOffers(this, commodity);
				    }
			    }

			    foreach (var commodity in _goodTypes)
			    {
				    resolveOffers(commodity);
			    }
                var del = new List<BasicAgent>();
			    foreach (var agent in _agents)
			    {
                    if (agent.Money <= 0) del.Add(agent);  
			    }
                while (del.Count > 0)
                {
                    signalBankrupt.SignalBankrupt(this, del[0]); //signalBankrupt.dispatch(this, agent);
                    del.RemoveAt(0);
                }
                _roundNum++;
		    }
	    }

	    public void ask(Offer offer)
	    {
		    _book.ask(offer);
	    }

	    public void bid(Offer offer)
	    {
		    _book.bid(offer);
	    }

	    /**
	     * Returns the historical mean price of the given commodity over the last X rounds
	     * @param	commodity_ string id of commodity
	     * @param	range number of rounds to look back
	     * @return
	     */

	    public double GetAverageHistoricalPrice(String good, int range)
	    {
		    return history.Prices.Average(good, range);
	    }

	    /**
	     * Get the good with the highest demand/supply ratio over time
	     * @param   minimum the minimum demand/supply ratio to consider an opportunity
	     * @param	range number of rounds to look back
	     * @return
	     */

	    public String GetHottestGood(double minimum = 1.5, int range = 10)
	    {
		    string best_market = "";
            double best_ratio = -99999;// Math.NEGATIVE_INFINITY;
		    foreach (var good in _goodTypes)
		    {
			    var asks = history.Asks.Average(good, range);
			    var bids = history.Bids.Average(good, range);

			    double ratio = 0;
			    if (asks == 0 && bids > 0)
			    {
				    //If there are NONE on the market we artificially create a fake supply of 1/2 a unit to avoid the
				    //crazy bias that "infinite" demand can cause...

				    asks = 0.5;
			    }

			    ratio = bids / asks;

			    if (ratio > minimum && ratio > best_ratio)
			    {
				    best_ratio = ratio;
				    best_market = good;
			    }
		    }
		    return best_market;
	    }

	    /**
	     * Returns the good that has the lowest average price over the given range of time
	     * @param	range how many rounds to look back
	     * @param	exclude goods to exclude
	     * @return
	     */

	    public String getCheapestGood(int range, List<String> exclude = null)
	    {
            double best_price = -9999999;// Math.POSITIVE_INFINITY;
		    string best_good = "";
		    foreach (var g in _goodTypes)
		    {
			    if (exclude == null || !exclude.Contains(g))
			    {
				    double price = history.Prices.Average(g, range);
				    if (price < best_price)
				    {
					    best_price = price;
					    best_good = g;
				    }
			    }
		    }
		    return best_good;
	    }

	    /**
	     * Returns the good that has the highest average price over the given range of time
	     * @param	range how many rounds to look back
	     * @param	exclude goods to exclude
	     * @return
	     */

	    public String getDearestGood(int range, List<String> exclude= null)
	    {
		    double best_price = 0;
		    String best_good = "";
		    foreach (var g in _goodTypes)
		    {
			    if (exclude == null || !exclude.Contains(g))
			    {
				    var price = history.Prices.Average(g, range);
				    if (price > best_price)
				    {
					    best_price = price;
					    best_good = g;
				    }
			    }
		    }
		    return best_good;
	    }

	    /**
	     *
	     * @param	range
	     * @return
	     */
	    public String GetMostProfitableAgentClass(int range= 10)
	    {
            double best = -999999;// Math.NEGATIVE_INFINITY;
		    String bestClass="";
		    foreach (var className in _mapAgents.Keys)
		    {
			    double val = history.Profit.Average(className, range);
			    if (val > best)
			    {
				    bestClass = className;
				    best = val;
			    }
		    }
		    return bestClass;
	    }

	    public AgentData getAgentClass(String className)
	    {
		    return _mapAgents[className];
	    }

	    public List<String> getAgentClassNames()
	    {
		    var agentData = new List<String> ();
		    foreach (var key in _mapAgents.Keys)
		    {
			    agentData.Add(key);
		    }
		    return agentData;
	    }

	    public List<String> getGoods()
	    {
            return new List<String>(_goodTypes);
	    }

        // TODO: why is this unsafe????
	    public List<String> GetGoodsUnsafe()
	    {
		    return _goodTypes;
	    }

	    public Good getGoodEntry(String str)
	    {
		    if (_mapGoods.ContainsKey(str))
		    {
			    return _mapGoods[str].Copy();
		    }
		    return null;
	    }

	    /********REPORT**********/
	    public MarketReport get_marketReport(int rounds)
	    {
		    var mr = new MarketReport();
		    mr.strListGood = "Commod\n\n";
		    mr.strListGoodPrices = "Price\n\n";
		    mr.strListGoodTrades = "Trades\n\n";
		    mr.strListGoodAsks = "Supply\n\n";
		    mr.strListGoodBids = "Demand\n\n";

		    mr.strListAgent = "Classes\n\n";
		    mr.strListAgentCount = "Count\n\n";
		    mr.strListAgentProfit = "Profit\n\n";
		    mr.strListAgentMoney = "Money\n\n";

		    mr.arrStrListInventory = new List<String>();

		    foreach (var commodity in _goodTypes)
		    {
			    mr.strListGood += commodity + "\n";

			    var price = history.Prices.Average(commodity, rounds);
			    mr.strListGoodPrices += Quick.numStr(price, 2) + "\n";

			    var asks = history.Asks.Average(commodity, rounds);
			    mr.strListGoodAsks += (int)(asks) + "\n";

			    var bids = history.Bids.Average(commodity, rounds);
			    mr.strListGoodBids += (int)(bids) + "\n";

			    var trades = history.Trades.Average(commodity, rounds);
			    mr.strListGoodTrades += (int)(trades) + "\n";

			    mr.arrStrListInventory.Add(commodity + "\n\n");
		    }
		    foreach (var key in _mapAgents.Keys)
		    {
			    var inventory = new List<double>();
			    foreach (var str in _goodTypes)
			    {
				    inventory.Add(0);
			    }
			    mr.strListAgent += key + "\n";
			    var profit = history.Profit.Average(key, rounds);
			    mr.strListAgentProfit += Quick.numStr(profit, 2) + "\n";

			    double test_profit = 0;
			    var list = _agents; //var list = _agents.filter(function(a:BasicAgent):Bool { return a.className == key; } );  dfs stub wtf
			    int count = 0;
			    double money = 0;

			    foreach (var a in list)
			    {
                    if (a.ClassName==key)
                    {
                        count++;
				        money += a.Money;
				        for (int lic=0; lic<_goodTypes.Count; lic++)
				        {
					        inventory[lic] += a.QueryInventory(_goodTypes[lic]);
				        }
                    }
			    }

			    money /= count;
			    for (int lic =0; lic<_goodTypes.Count; lic++)
			    {
				    inventory[lic] /= count;
				    mr.arrStrListInventory[lic] += Quick.numStr(inventory[lic],1) + "\n";
			    }

			    mr.strListAgentCount += Quick.numStr(count, 0) + "\n";
			    mr.strListAgentMoney += Quick.numStr(money, 0) + "\n";
		    }
		    return mr;
	    }

	    /********PRIVATE*********/

	    private void fromData(MarketData data)
	    {
		    //Create commodity index
		    foreach (var g in data.goods)
		    {
			    _goodTypes.Add(g.Id);
			    _mapGoods[g.Id] = new Good(g.Id, g.Size);

                double v = 1.0;
                if (g.Id == "metal") v = 2.0;
                if (g.Id == "tools") v = 3.0;

			    history.Register(g.Id);
                history.Prices.Add(g.Id, v);	//start the bidding at $1!
                history.Asks.Add(g.Id, v);	//start history charts with 1 fake buy/sell bid
                history.Bids.Add(g.Id, v);
                history.Trades.Add(g.Id, v);

			    _book.register(g.Id);
		    }

		    _mapAgents = new Dictionary<String, AgentData>();

		    foreach (var aData in data.agentTypes)
		    {
			    _mapAgents[aData.ClassName] = aData;
			    history.Profit.Register(aData.ClassName);
		    }

		    //Make the agent list
		    _agents = new BindingList<BasicAgent>();

		    var agentIndex = 0;
		    foreach (var agent in data.agents)
		    {
			    agent.Id = agentIndex;
			    agent.Init(this);
			    _agents.Add(agent);
			    agentIndex++;
		    }

	    }

	    private void resolveOffers(String good= "")
	    {
		    var bids = _book.bids[good];
		    var asks = _book.asks[good];

		    bids = Quick.shuffle(bids);  
		    asks = Quick.shuffle(asks);  

            //bids.Sort(Quick.sortOfferDecending); //highest buying price first
            asks.Sort(Quick.sortOfferAcending); //lowest selling price first

		    int successfulTrades = 0;		//# of successful trades this round
		    double moneyTraded = 0;			//amount of money traded this round
		    double unitsTraded = 0;			//amount of goods traded this round
		    double avgPrice = 0;				//avg clearing price this round
		    double numAsks = 0;
		    double numBids = 0;

		    int failsafe = 0;

		    for (int i=0; i<bids.Count; i++)
		    {
			    numBids += bids[i].units;
		    }

		    for (int i=0; i<asks.Count; i++)
		    {
			    numAsks += asks[i].units;
		    }

		    //march through and try to clear orders
		    while (bids.Count > 0 && asks.Count > 0)		//while both books are non-empty
		    {
			    var buyer = bids[0];
			    var seller = asks[0];

			    var quantity_traded = (double)Math.Min(seller.units, buyer.units);
                var clearing_price = seller.unit_price; //Quick.avgf(seller.unit_price, buyer.unit_price);

                //if (buyer.unit_price < seller.unit_price)
                //    break;

			    if (quantity_traded > 0)
			    {
				    //transfer the goods for the agreed price
				    seller.units -= quantity_traded;
				    buyer.units -= quantity_traded;

				    transferGood(good, quantity_traded, seller.agent_id, buyer.agent_id, clearing_price);
				    transferMoney(quantity_traded * clearing_price, seller.agent_id, buyer.agent_id);

				    //update agent price beliefs based on successful transaction
				    var buyer_a = _agents[buyer.agent_id];
				    var seller_a = _agents[seller.agent_id];
				    buyer_a.UpdatePriceModel(this, "buy", good, true, clearing_price);
				    seller_a.UpdatePriceModel(this, "sell", good, true, clearing_price);

				    //log the stats
				    moneyTraded += (quantity_traded * clearing_price);
				    unitsTraded += quantity_traded;
				    successfulTrades++;
			    }

			    if (seller.units == 0)		//seller is out of offered good
			    {
				    asks.RemoveAt(0); //.splice(0, 1);		//remove ask
				    failsafe = 0;
			    }
			    if (buyer.units == 0)		//buyer is out of offered good
			    {
				    bids.RemoveAt(0);//.splice(0, 1);		//remove bid
				    failsafe = 0;
			    }

			    failsafe++;

			    if (failsafe > 1000)
			    {
				    Console.WriteLine("BOINK!");
			    }
		    }

		    //reject all remaining offers,
		    //update price belief models based on unsuccessful transaction
		    while (bids.Count > 0)
		    {
			    var buyer = bids[0];
			    var buyer_a = _agents[buyer.agent_id];
			    buyer_a.UpdatePriceModel(this,"buy",good, false);
			    bids.RemoveAt(0);//.splice(0, 1);
		    }
            while (asks.Count > 0)
		    {
			    var seller = asks[0];
			    var seller_a = _agents[seller.agent_id];
			    seller_a.UpdatePriceModel(this,"sell",good, false);
                asks.RemoveAt(0);// splice(0, 1);
		    }

		    //update history

		    history.Asks.Add(good, numAsks);
		    history.Bids.Add(good, numBids);
		    history.Trades.Add(good, unitsTraded);

		    if (unitsTraded > 0)
		    {
			    avgPrice = moneyTraded / (double)unitsTraded;
			    history.Prices.Add(good, avgPrice);
		    }
		    else
		    {
			    //special case: none were traded this round, use last round's average price
			    history.Prices.Add(good, history.Prices.Average(good, 1));
			    avgPrice = history.Prices.Average(good,1);
		    }

            List<BasicAgent> ag = _agents.ToList<BasicAgent>();
		    ag.Sort(Quick.sortAgentAlpha);

		    String curr_class = "";
		    String last_class = "";
		    List<double> list  = null;
		    double avg_profit = 0;

		    for (int i=0;i<ag.Count; i++)
		    {
			    var a = ag[i];		//get current agent
			    curr_class = a.ClassName;			//check its class
			    if (curr_class != last_class)		//new class?
			    {
				    if (list != null)				//do we have a list built up?
				    {
					    //log last class' profit
					    history.Profit.Add(last_class, Quick.listAvgf(list));
				    }
				    list = new List<double>();		//make a new list
				    last_class = curr_class;
			    }
			    list.Add(a.GetProfit());			//push profit onto list
		    }

		    //add the last class too
		    history.Profit.Add(last_class, Quick.listAvgf(list));

		    //sort by id so everything works again
		    //_agents.Sort(Quick.sortAgentId);

	    }

	    private void transferGood(String good, double units, int seller_id, int buyer_id, double clearing_price)
	    {
		    var seller = _agents[seller_id];
		    var  buyer = _agents[buyer_id];
		    seller.ChangeInventory(good, -units, 0);
		     buyer.ChangeInventory(good,  units, clearing_price);
	    }

	    private void transferMoney(double amount, int seller_id, int buyer_id)
	    {
		    var seller = _agents[seller_id];
		    var  buyer = _agents[buyer_id];
		    seller.Money += amount;
		     buyer.Money -= amount;
	    }

    }
}
