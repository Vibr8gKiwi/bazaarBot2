using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class Inventory
    {
	    public double maxSize = 0;

	    //private static var _index:Map<String, Commodity>;

	    private Dictionary<String, Point> stuff;		// key:commodity_id, val:amount, original_cost
	    private Dictionary<String, double> ideal;		// ideal counts for each thing
	    private Dictionary<String, double> sizes;		// how much space each thing takes up


	    public Inventory()
	    {
		    sizes = new Dictionary<String, double>();
		    stuff = new Dictionary<String, Point>();
		    ideal = new Dictionary<String, double>();
		    maxSize = 0;
	    }

	    public void FromData(InventoryData data)
	    {
		    var sizes = new List<string>();
		    var amountsp = new List<Point>();
		    foreach (string key in data.Start.Keys)
		    {
			    sizes.Add(key);
			    amountsp.Add(new Point(data.Start[key],0));
		    }
		    SetStuff(sizes, amountsp);
		    sizes = new List<string>();
		    var amounts = new List<double>();
		    foreach (string key in data.Size.Keys)
		    {
			    sizes.Add(key);
			    amounts.Add(data.Size[key]);
		    }
		    SetSizes(sizes, amounts);
		    sizes = new List<string>();
		    amounts = new List<double>();
		    foreach (string key in data.Ideal.Keys)
		    {
			    sizes.Add(key);
			    amounts.Add(data.Ideal[key]);
			    SetIdeal(sizes, amounts);
		    }
		    maxSize = data.MaxSize;
	    }

	    public Inventory Copy()
	    {
		    var i = new Inventory();
            //TODO: improve these names
		    var stufff = new List<Point>();
		    var stuffi = new List<String>();
		    var idealf = new List<double>();
		    var ideali = new List<String>();
		    var sizesf = new List<double>();
		    var sizesi = new List<String>();
		    foreach (string key in stuff.Keys)
		    {
			    stufff.Add(stuff[key]);
			    stuffi.Add(key);
		    }
		    foreach (string key in ideal.Keys)
		    {
			    idealf.Add(ideal[key]);
			    ideali.Add(key);
		    }
		    foreach (string key in sizes.Keys)
		    {
			    sizesf.Add(sizes[key]);
			    sizesi.Add(key);
		    }
		    i.SetStuff(stuffi, stufff);
		    i.SetIdeal(ideali, idealf);
		    i.SetSizes(sizesi, sizesf);
		    i.maxSize = maxSize;
		    return i;
	    }

	    public void Destroy()
	    {
            stuff.Clear();
            ideal.Clear();
            sizes.Clear();
		    stuff = null;
		    ideal = null;
		    sizes = null;
	    }

	    /**
	     * Set amounts of various commodities
	     * @param	stuff_
	     * @param	amounts_
	     */

	    public void SetStuff(List<String>stuff, List<Point>amounts)
	    {
		    for (int i=0; i<stuff.Count; i++)
		    {
			    this.stuff[stuff[i]] = amounts[i];
		    }
	    }

	    /**
	     * Set how much of each commodity to stockpile
	     * @param	stuff_
	     * @param	amounts_
	     */

	    public void SetIdeal(List<String>ideal, List<double>amounts)
	    {
		    for (int i=0; i<ideal.Count; i++)
		    {
			    this.ideal[ideal[i]] = amounts[i];
		    }
	    }

	    public void SetSizes(List<String>sizes, List<double>amounts)
	    {
		    for(int i=0; i<sizes.Count; i++)
		    {
			    this.sizes[sizes[i]] = amounts[i];
		    }
	    }

	    /**
	     * Returns how much of this
	     * @param	commodity_ string id of commodity
	     * @return
	     */

	    public double Query(String good)
	    {
		    if (stuff.ContainsKey(good))
		    {
			    return stuff[good].x;
		    }
		    return 0;
	    }
        public double QueryCost(String good)
        {
            if (stuff.ContainsKey(good))
            {
                return stuff[good].y;
            }
            return 0;
        }

	    public double Ideal(String good)
	    {
		    if (ideal.ContainsKey(good))
		    {
			    return ideal[good];
		    }
		    return 0;
	    }

	    public double GetEmptySpace()
	    {
		    return maxSize - GetUsedSpace();
	    }

	    public double GetUsedSpace()
	    {
		    double space_used = 0;
		    foreach (string key in stuff.Keys)
		    {
                if (!sizes.ContainsKey(key)) continue;
			    space_used += stuff[key].x * sizes[key];
		    }
		    return space_used;
	    }

	    public double GetCapacityFor(string good)
	    {
		    if (sizes.ContainsKey(good))
		    {
			    return sizes[good];
		    }
		    return -1;
	    }

	    /**
	     * Change the amount of the given commodity by delta
	     * @param	commodity_ string id of commodity
	     * @param	delta_ amount added
	     */

	    public double Change(string good, double delta, double unit_cost)
	    {
		    Point result = new Point(0,0);

		    if (stuff.ContainsKey(good))
		    {
			    var amount = stuff[good];
                if (unit_cost > 0)
                {
                    if (amount.x <= 0)
                    {
                        result.x = delta;
                        result.y = unit_cost;
                    }
                    else
                    {
                        result.y = (amount.x * amount.y + delta * unit_cost) / (amount.x + delta);
                        result.x = amount.x + delta;
                    }
                }
                else
                {
                    result.x = amount.x + delta;
                    result.y = amount.y; //just copy from old value?
                }
		    }
		    else
		    {
			    result.x = delta;
                result.y = unit_cost;
		    }

		    if (result.x < 0)
		    {
			    result.x = 0;
                result.y = 0;
		    }

		    stuff[good] = result;
            return result.y; //return current unit cost
	    }

	    /**
	     * Returns # of units above the desired inventory level, or 0 if @ or below
	     * @param	commodity_ string id of commodity
	     * @return
	     */

	    public double Surplus(string good)
	    {
		    var amt = Query(good);
            double ideal = 0;
            if (this.ideal.ContainsKey(good))
                ideal = this.ideal[good];
		    if (amt > ideal)
		    {
			    return (amt - ideal);
		    }
		    return 0;
	    }

	    /**
	     * Returns # of units below the desired inventory level, or 0 if @ or above
	     * @param	commodity_
	     * @return
	     */

	    public double Shortage(string good)
	    {
		    if (!stuff.ContainsKey(good))
		    {
			    return 0;
		    }
		    var amt = Query(good);
            double ideal = 0;
            if (this.ideal.ContainsKey(good))
                ideal = this.ideal[good];
		    if (amt < ideal)
		    {
			    return (ideal - amt);
		    }
		    return 0;
	    }

    }
}
