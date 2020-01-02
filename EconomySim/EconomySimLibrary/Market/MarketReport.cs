using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class MarketReport
    {
	    public String strListGood = "";
	    public String strListGoodPrices= "";
	    public String strListGoodTrades = "";
	    public String strListGoodAsks = "";
	    public String strListGoodBids = "";

	    public String strListAgent = "";
	    public String strListAgentCount = "";
	    public String strListAgentMoney = "";
	    public String strListAgentProfit = "";

        public List<String> arrStrListInventory { get; set; }	    
    }
}
