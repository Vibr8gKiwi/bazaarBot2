using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{

    /**
     * An agent that performs the basic logic from the Doran & Parberry article
     * @author
     */
    public class Agent : BasicAgent
    {

	    public static double MIN_PRICE = 0.01;		//lowest possible price

	    public Agent(int id, AgentData data) : base(id,data)
	    {
	    }


	    override public Offer createBid(Market bazaar, String good, double limit)
	    {
            var bidPrice = 0;// determinePriceOf(good);  bids are now made "at market", no price determination needed
		    var ideal = determinePurchaseQuantity(bazaar, good);

		    //can't buy more than limit
		    double quantityToBuy = ideal > limit ? limit : ideal;
		    if (quantityToBuy > 0)
		    {
			    return new Offer(id, good, quantityToBuy, bidPrice);
		    }
		    return null;
	    }

	    override public Offer createAsk(Market bazaar, String commodity_, double limit_)
	    {
		    var ask_price = _inventory.query_cost(commodity_) * 1.02; //asks are fair prices:  costs + small profit

            var quantity_to_sell = _inventory.query(commodity_);//put asks out for all inventory
            nProduct = quantity_to_sell;

		    if (quantity_to_sell > 0)
		    {
			    return new Offer(id, commodity_, quantity_to_sell, ask_price);
		    }
		    return null;
	    }

	    override public void generateOffers(Market bazaar, String commodity)
	    {
		    Offer offer;
		    double surplus = _inventory.surplus(commodity);
		    if (surplus >= 1)
		    {
			     offer = createAsk(bazaar, commodity, 1);
			     if (offer != null)
			     {
				    bazaar.ask(offer);
			     }
		    }
		    else
		    {
			    var shortage = _inventory.shortage(commodity);
			    var space = _inventory.getEmptySpace();
			    var unit_size = _inventory.getCapacityFor(commodity);

			    if (shortage > 0 && space >= unit_size)
			    {
				    double limit = 0;
				    if ((shortage * unit_size) <= space)	//enough space for ideal order
				    {
					    limit = shortage;
				    }
				    else									//not enough space for ideal order
				    {
                        limit = space; // Math.Floor(space / shortage);
				    }

				    if (limit > 0)
				    {
					    offer = createBid(bazaar, commodity, limit);
					    if (offer != null)
					    {
						    bazaar.bid(offer);
					    }
				    }
			    }
		    }
	    }

	    override public void updatePriceModel(Market bazaar, String act, String good, bool success, double unitPrice= 0)
	    {
		    List<double> observed_trades;

		    if (success)
		    {
			    //Add this to my list of observed trades
			    observed_trades = _observedTradingRange[good];
			    observed_trades.Add(unitPrice);
		    }

		    var public_mean_price = bazaar.getAverageHistoricalPrice(good, 1);

	    }
    }
}
