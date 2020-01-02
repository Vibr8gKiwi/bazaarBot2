using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class Logic
    {
	    /**
	     * Perform this logic on the given agent
	     * @param	agent
	     */
	    public virtual void Perform(BasicAgent agent, Market market)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

        protected void Produce(BasicAgent agent, String commodity, double amount, double chance = 1.0)
	    {
		    if (chance >= 1.0 || Quick.rnd.NextDouble() < chance)
		    {
			    agent.ProduceInventory(commodity, amount);
		    }
	    }

	    protected void Consume(BasicAgent agent, String commodity, double amount, double chance = 1.0)
	    {
            if (chance >= 1.0 || Quick.rnd.NextDouble() < chance)
		    {
                //if (commodity == "money")
                //{
                //    agent.changeInventory(comm
                //    agent.money -= amount;
                //}
                //else
                //{
				    agent.ConsumeInventory(commodity, -amount);
                //}
		    }
	    }
    } 
}
