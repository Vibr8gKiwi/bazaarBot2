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
	    public const double MIN_PRICE = 0.01;		//lowest possible price

	    public Agent(int id, AgentData data) : base(id,data)
	    {

	    }

        public override Offer CreateBid(Market bazaar, String good, double limit)
	    {
            var bidPrice = 0;// determinePriceOf(good);  bids are now made "at market", no price determination needed
		    var ideal = DeterminePurchaseQuantity(bazaar, good);

		    //can't buy more than limit
		    double quantityToBuy = ideal > limit ? limit : ideal;
		    if (quantityToBuy > 0)
		    {
			    return new Offer(Id, good, quantityToBuy, bidPrice);
		    }
		    return null;
	    }

        public override Offer CreateAsk(Market bazaar, String commodity, double limit)
	    {
		    var askPrice = Inventory.QueryCost(commodity) * 1.02; //asks are fair prices:  costs + small profit

            var quantityToSell = Inventory.Query(commodity);//put asks out for all inventory
            NProduct = quantityToSell;

		    if (quantityToSell > 0)
		    {
			    return new Offer(Id, commodity, quantityToSell, askPrice);
		    }
		    return null;
	    }

	    public override void GenerateOffers(Market bazaar, String commodity)
	    {
		    Offer offer;
		    double surplus = Inventory.Surplus(commodity);
		    if (surplus >= 1)
		    {
			     offer = CreateAsk(bazaar, commodity, 1);
			     if (offer != null)
			     {
				    bazaar.Ask(offer);
			     }
		    }
		    else
		    {
			    var shortage = Inventory.Shortage(commodity);
			    var space = Inventory.GetEmptySpace();
			    var unitSize = Inventory.GetCapacityFor(commodity);

			    if (shortage > 0 && space >= unitSize)
			    {
				    double limit = 0;
				    if ((shortage * unitSize) <= space)	//enough space for ideal order
				    {
					    limit = shortage;
				    }
				    else									//not enough space for ideal order
				    {
                        limit = space; // Math.Floor(space / shortage);
				    }

				    if (limit > 0)
				    {
					    offer = CreateBid(bazaar, commodity, limit);
					    if (offer != null)
					    {
						    bazaar.Bid(offer);
					    }
				    }
			    }
		    }
	    }

        public override void UpdatePriceModel(Market bazaar, String act, String good, bool success, double unitPrice = 0)
	    {
		    List<double> observed_trades;

		    if (success)
		    {
			    //Add this to my list of observed trades
			    observed_trades = ObservedTradingRange[good];
			    observed_trades.Add(unitPrice);
		    }

		    var publicMeanPrice = bazaar.GetAverageHistoricalPrice(good, 1);

	    }
    }
}
