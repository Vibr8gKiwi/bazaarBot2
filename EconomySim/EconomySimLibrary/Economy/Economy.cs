using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class Economy : ISignalBankrupt
    {
	    private List<Market> _markets;

        public Economy()
	    {
		    _markets = new List<Market>();
	    }

	    public void addMarket(Market m)
	    {
		    if (_markets.IndexOf(m) == -1)
		    {
			    _markets.Add(m);
		    }
	    }

	    public Market getMarket(String name)
	    {
		    foreach (var m in _markets)
		    {
			    if (m.name == name) return m;
		    }
		    return null;
	    }

	    public void simulate(int rounds)
	    {
		    foreach (var m in _markets)
		    {
			    m.simulate(rounds);
		    }
	    }


	    public virtual void signalBankrupt(Market m, BasicAgent a)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

    }

}
