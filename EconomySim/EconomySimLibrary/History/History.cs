using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public enum EconNoun
    {
	    Price,
	    Ask,
	    Bid,
	    Trade,
	    Profit
    }
    
    public class History
    {
	    public HistoryLog Prices;
	    public HistoryLog Asks;
	    public HistoryLog Bids;
	    public HistoryLog Trades;
	    public HistoryLog Profit;

	    public History()
	    {
		    Prices = new HistoryLog(EconNoun.Price);
		    Asks   = new HistoryLog(EconNoun.Ask);
		    Bids   = new HistoryLog(EconNoun.Bid);
		    Trades = new HistoryLog(EconNoun.Trade);
		    Profit = new HistoryLog(EconNoun.Profit);
	    }

	    public void Register(string good)
	    {
		    Prices.Register(good);
		    Asks.Register(good);
		    Bids.Register(good);
		    Trades.Register(good);
		    Profit.Register(good);
	    }
    }
}
