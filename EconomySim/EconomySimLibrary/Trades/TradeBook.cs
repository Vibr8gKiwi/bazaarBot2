using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class TradeBook
    {
	    public Dictionary<String, List<Offer>> bids;
	    public Dictionary<String, List<Offer>> asks;

        public DataTable dbook { get; set; }

	    public TradeBook()
	    {
		    bids = new Dictionary<String, List<Offer>>();
		    asks = new Dictionary<String, List<Offer>>();
            dbook = new DataTable("Book");
            dbook.Columns.Add(new DataColumn("bid"));
            dbook.Columns.Add(new DataColumn("ask"));
            dbook.Rows.Add(1.0,2.0);
        }

	    public void register(String name)
	    {
		    asks[name] = new List<Offer>();
		    bids[name] = new List<Offer>();
	    }

	    public bool bid(Offer offer)
	    {
		    if (!bids.ContainsKey(offer.good))
			    return false;

		    bids[offer.good].Add(offer);
		    return true;
	    }

	    public bool ask(Offer offer)
	    {
		    if (!bids.ContainsKey(offer.good))
			    return false;

		    asks[offer.good].Add(offer);
		    return true;
	    }
    }
}
