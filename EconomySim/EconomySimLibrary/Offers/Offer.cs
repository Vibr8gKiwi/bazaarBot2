using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class Offer
    {
	    public string commodity;	//the thing offered
	    public double units;			//how many units
	    public double unitPrice;	//price per unit
	    public int agentId;		//who offered this

	    public Offer(int agentId = -1, string commodity = "", double units = 1.0, double unitPrice = 1.0)
	    {
		    this.agentId = agentId;
		    this.commodity = commodity;
		    this.units = units;
		    this.unitPrice = unitPrice;
	    }

	    public override string ToString()
	    {
		    return "("+agentId + "): " + commodity + "x " + units + " @ " + unitPrice;
	    }
    }
}
