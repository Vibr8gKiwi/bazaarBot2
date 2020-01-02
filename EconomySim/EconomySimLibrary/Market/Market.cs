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
	    public string Name;

	    /**Logs information about all economic activity in this market**/
	    public History History;

	    /**Signal fired when an agent's money reaches 0 or below**/
	    public ISignalBankrupt SignalBankrupt;


	    /********PRIVATE*********/

	    private int roundNum = 0;

	    private List<string> goodTypes;		//list of string ids for all the legal commodities
	    public BindingList<BasicAgent> agents;
	    public TradeBook book;
	    private Dictionary<string, AgentData> mapAgents;
	    private Dictionary<string, Good> mapGoods;
        
        
        public Market(string name, ISignalBankrupt isb)
	    {
		    this.Name = name;

		    History = new History();
		    book = new TradeBook();
		    goodTypes = new List<string>();
		    agents = new BindingList<BasicAgent>();
		    mapGoods = new Dictionary<string, Good>();
		    mapAgents = new Dictionary<string, AgentData>();

		    SignalBankrupt = isb;//new TypedSignal<Market->BasicAgent->Void>();
	    }

	    public void Init(MarketData data)
	    {
		    fromData(data);
	    }

	    public int NumTypesOfGood()
	    {
		    return goodTypes.Count;
	    }

	    public int NumAgents()
	    {
		    return agents.Count;
	    }

	    public void ReplaceAgent(BasicAgent oldAgent, BasicAgent newAgent)
	    {
		    newAgent.Id = oldAgent.Id;
		    agents[oldAgent.Id] = newAgent;
		    oldAgent.Destroy();
		    newAgent.Init(this);
	    }

	    //@:access(bazaarbot.agent.BasicAgent)    //dfs stub ????
	    public void Simulate(int rounds)
	    {
		    for (int round=0; round<rounds; round++)
		    {
			    foreach (var agent in agents)
			    {
				    agent.MoneyLastRound = agent.Money;
				    agent.Simulate(this);

				    foreach (var commodity in goodTypes)
				    {
					    agent.GenerateOffers(this, commodity);
				    }
			    }

			    foreach (var commodity in goodTypes)
			    {
				    resolveOffers(commodity);
			    }
                var del = new List<BasicAgent>();
			    foreach (var agent in agents)
			    {
                    if (agent.Money <= 0) del.Add(agent);  
			    }
                while (del.Count > 0)
                {
                    SignalBankrupt.SignalBankrupt(this, del[0]); //signalBankrupt.dispatch(this, agent);
                    del.RemoveAt(0);
                }
                roundNum++;
		    }
	    }

	    public void Ask(Offer offer)
	    {
		    book.Ask(offer);
	    }

	    public void Bid(Offer offer)
	    {
		    book.Bid(offer);
	    }

	    /**
	     * Returns the historical mean price of the given commodity over the last X rounds
	     * @param	commodity_ string id of commodity
	     * @param	range number of rounds to look back
	     * @return
	     */

	    public double GetAverageHistoricalPrice(string good, int range)
	    {
		    return History.Prices.Average(good, range);
	    }

	    /**
	     * Get the good with the highest demand/supply ratio over time
	     * @param   minimum the minimum demand/supply ratio to consider an opportunity
	     * @param	range number of rounds to look back
	     * @return
	     */

	    public string GetHottestGood(double minimum = 1.5, int range = 10)
	    {
		    string bestMarket = "";
            double bestRatio = -99999;// Math.NEGATIVE_INFINITY;
		    foreach (var good in goodTypes)
		    {
			    var asks = History.Asks.Average(good, range);
			    var bids = History.Bids.Average(good, range);

			    double ratio = 0;
			    if (asks == 0 && bids > 0)
			    {
				    //If there are NONE on the market we artificially create a fake supply of 1/2 a unit to avoid the
				    //crazy bias that "infinite" demand can cause...

				    asks = 0.5;
			    }

			    ratio = bids / asks;

			    if (ratio > minimum && ratio > bestRatio)
			    {
				    bestRatio = ratio;
				    bestMarket = good;
			    }
		    }
		    return bestMarket;
	    }

	    /**
	     * Returns the good that has the lowest average price over the given range of time
	     * @param	range how many rounds to look back
	     * @param	exclude goods to exclude
	     * @return
	     */

	    public string GetCheapestGood(int range, List<string> exclude = null)
	    {
            double bestPrice = -9999999;// Math.POSITIVE_INFINITY;
		    string bestGood = "";
		    foreach (var g in goodTypes)
		    {
			    if (exclude == null || !exclude.Contains(g))
			    {
				    double price = History.Prices.Average(g, range);
				    if (price < bestPrice)
				    {
					    bestPrice = price;
					    bestGood = g;
				    }
			    }
		    }
		    return bestGood;
	    }

	    /**
	     * Returns the good that has the highest average price over the given range of time
	     * @param	range how many rounds to look back
	     * @param	exclude goods to exclude
	     * @return
	     */

	    public string GetDearestGood(int range, List<string> exclude= null)
	    {
		    double bestPrice = 0;
		    string bestGood = "";
		    foreach (var g in goodTypes)
		    {
			    if (exclude == null || !exclude.Contains(g))
			    {
				    var price = History.Prices.Average(g, range);
				    if (price > bestPrice)
				    {
					    bestPrice = price;
					    bestGood = g;
				    }
			    }
		    }
		    return bestGood;
	    }

	    /**
	     *
	     * @param	range
	     * @return
	     */
	    public string GetMostProfitableAgentClass(int range= 10)
	    {
            double best = -999999;// Math.NEGATIVE_INFINITY;
		    string bestClass="";
		    foreach (var className in mapAgents.Keys)
		    {
			    double val = History.Profit.Average(className, range);
			    if (val > best)
			    {
				    bestClass = className;
				    best = val;
			    }
		    }
		    return bestClass;
	    }

	    public AgentData GetAgentClass(string className)
	    {
		    return mapAgents[className];
	    }

	    public List<string> GetAgentClassNames()
	    {
		    var agentData = new List<string> ();
		    foreach (var key in mapAgents.Keys)
		    {
			    agentData.Add(key);
		    }
		    return agentData;
	    }

	    public List<string> GetGoods()
	    {
            return new List<string>(goodTypes);
	    }

        // TODO: why is this unsafe????
	    public List<string> GetGoodsUnsafe()
	    {
		    return goodTypes;
	    }

	    public Good GetGoodEntry(string str)
	    {
		    if (mapGoods.ContainsKey(str))
		    {
			    return mapGoods[str].Copy();
		    }
		    return null;
	    }

	    /********REPORT**********/
	    public MarketReport GetMarketReport(int rounds)
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

		    mr.arrStrListInventory = new List<string>();

		    foreach (var commodity in goodTypes)
		    {
			    mr.strListGood += commodity + "\n";

			    var price = History.Prices.Average(commodity, rounds);
			    mr.strListGoodPrices += Quick.NumToStr(price, 2) + "\n";

			    var asks = History.Asks.Average(commodity, rounds);
			    mr.strListGoodAsks += (int)(asks) + "\n";

			    var bids = History.Bids.Average(commodity, rounds);
			    mr.strListGoodBids += (int)(bids) + "\n";

			    var trades = History.Trades.Average(commodity, rounds);
			    mr.strListGoodTrades += (int)(trades) + "\n";

			    mr.arrStrListInventory.Add(commodity + "\n\n");
		    }
		    foreach (var key in mapAgents.Keys)
		    {
			    var inventory = new List<double>();
			    foreach (var str in goodTypes)
			    {
				    inventory.Add(0);
			    }
			    mr.strListAgent += key + "\n";
			    var profit = History.Profit.Average(key, rounds);
			    mr.strListAgentProfit += Quick.NumToStr(profit, 2) + "\n";

			    var list = agents; //var list = _agents.filter(function(a:BasicAgent):Bool { return a.className == key; } );  dfs stub wtf
			    int count = 0;
			    double money = 0;

			    foreach (var a in list)
			    {
                    if (a.ClassName==key)
                    {
                        count++;
				        money += a.Money;
				        for (int lic=0; lic<goodTypes.Count; lic++)
				        {
					        inventory[lic] += a.QueryInventory(goodTypes[lic]);
				        }
                    }
			    }

			    money /= count;
			    for (int lic =0; lic<goodTypes.Count; lic++)
			    {
				    inventory[lic] /= count;
				    mr.arrStrListInventory[lic] += Quick.NumToStr(inventory[lic],1) + "\n";
			    }

			    mr.strListAgentCount += Quick.NumToStr(count, 0) + "\n";
			    mr.strListAgentMoney += Quick.NumToStr(money, 0) + "\n";
		    }
		    return mr;
	    }

	    /********PRIVATE*********/

	    private void fromData(MarketData data)
	    {
		    //Create commodity index
		    foreach (var g in data.goods)
		    {
			    goodTypes.Add(g.Id);
			    mapGoods[g.Id] = new Good(g.Id, g.Size);

                double v = 1.0;
                if (g.Id == "metal") v = 2.0;
                if (g.Id == "tools") v = 3.0;

			    History.Register(g.Id);
                History.Prices.Add(g.Id, v);	//start the bidding at $1!
                History.Asks.Add(g.Id, v);	//start history charts with 1 fake buy/sell bid
                History.Bids.Add(g.Id, v);
                History.Trades.Add(g.Id, v);

			    book.Register(g.Id);
		    }

		    mapAgents = new Dictionary<string, AgentData>();

		    foreach (var aData in data.agentTypes)
		    {
			    mapAgents[aData.ClassName] = aData;
			    History.Profit.Register(aData.ClassName);
		    }

		    //Make the agent list
		    agents = new BindingList<BasicAgent>();

		    var agentIndex = 0;
		    foreach (var agent in data.agents)
		    {
			    agent.Id = agentIndex;
			    agent.Init(this);
			    agents.Add(agent);
			    agentIndex++;
		    }

	    }

        //TODO: do we need the default ""?
	    private void resolveOffers(string good= "")
	    {
		    var bids = book.bids[good];
		    var asks = book.asks[good];

		    bids = Quick.Shuffle(bids);  
		    asks = Quick.Shuffle(asks);  

            //bids.Sort(Quick.sortOfferDecending); //highest buying price first
            asks.Sort(Quick.SortOfferAcending); //lowest selling price first

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
                var clearing_price = seller.unitPrice; //Quick.avgf(seller.unit_price, buyer.unit_price);

                //if (buyer.unit_price < seller.unit_price)
                //    break;

			    if (quantity_traded > 0)
			    {
				    //transfer the goods for the agreed price
				    seller.units -= quantity_traded;
				    buyer.units -= quantity_traded;

				    transferGood(good, quantity_traded, seller.agentId, buyer.agentId, clearing_price);
				    transferMoney(quantity_traded * clearing_price, seller.agentId, buyer.agentId);

				    //update agent price beliefs based on successful transaction
				    var buyer_a = agents[buyer.agentId];
				    var seller_a = agents[seller.agentId];
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

			    if (failsafe > 1000) //lol, ok
			    {
				    Console.WriteLine("BOINK!");
			    }
		    }

		    //reject all remaining offers,
		    //update price belief models based on unsuccessful transaction
		    while (bids.Count > 0)
		    {
			    var buyer = bids[0];
			    var buyer_a = agents[buyer.agentId];
			    buyer_a.UpdatePriceModel(this,"buy",good, false);
			    bids.RemoveAt(0);//.splice(0, 1);
		    }
            while (asks.Count > 0)
		    {
			    var seller = asks[0];
			    var seller_a = agents[seller.agentId];
			    seller_a.UpdatePriceModel(this,"sell",good, false);
                asks.RemoveAt(0);// splice(0, 1);
		    }

		    //update history

		    History.Asks.Add(good, numAsks);
		    History.Bids.Add(good, numBids);
		    History.Trades.Add(good, unitsTraded);

		    if (unitsTraded > 0)
		    {
			    avgPrice = moneyTraded / (double)unitsTraded;
			    History.Prices.Add(good, avgPrice);
		    }
		    else
		    {
			    //special case: none were traded this round, use last round's average price
			    History.Prices.Add(good, History.Prices.Average(good, 1));
			    avgPrice = History.Prices.Average(good,1);
		    }

            List<BasicAgent> agentsList = agents.ToList<BasicAgent>();
		    agentsList.Sort(Quick.SortAgentAlpha);

		    string currClass = "";
		    string lastClass = "";
		    List<double> list  = null;

		    for (int i=0;i<agentsList.Count; i++)
		    {
			    var a = agentsList[i];		//get current agent
			    currClass = a.ClassName;			//check its class
			    if (currClass != lastClass)		//new class?
			    {
				    if (list != null)				//do we have a list built up?
				    {
					    //log last class' profit
					    History.Profit.Add(lastClass, Quick.AvgList(list));
				    }
				    list = new List<double>();		//make a new list
				    lastClass = currClass;
			    }
			    list.Add(a.GetProfit());			//push profit onto list
		    }

		    //add the last class too
		    History.Profit.Add(lastClass, Quick.AvgList(list));

		    //sort by id so everything works again
		    //_agents.Sort(Quick.sortAgentId);

	    }

	    private void transferGood(string good, double units, int sellerId, int buyerId, double clearingPrice)
	    {
		    var seller = agents[sellerId];
		    var  buyer = agents[buyerId];
		    seller.ChangeInventory(good, -units, 0);
		     buyer.ChangeInventory(good,  units, clearingPrice);
	    }

	    private void transferMoney(double amount, int sellerId, int buyerId)
	    {
		    var seller = agents[sellerId];
		    var  buyer = agents[buyerId];
		    seller.Money += amount;
		    buyer.Money -= amount;
	    }

    }
}
