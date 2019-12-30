using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class Offer
    {
	    public String good;	//the thing offered
	    public double units;			//how many units
	    public double unit_price;	//price per unit
	    public int agent_id;		//who offered this

	    public Offer(int agent_id_=-1,String commodity_="",double units_=1.0,double unit_price_=1.0)
	    {
		    agent_id = agent_id_;
		    good = commodity_;
		    units = units_;
		    unit_price = unit_price_;
	    }

	    public String toString()
	    {
		    return "("+agent_id + "): " + good + "x " + units + " @ " + unit_price;
	    }
    }
}
