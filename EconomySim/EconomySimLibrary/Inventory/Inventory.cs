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

	    private Dictionary<String, Point>_stuff;		// key:commodity_id, val:amount, original_cost
	    private Dictionary<String, double>_ideal;		// ideal counts for each thing
	    private Dictionary<String, double>_sizes;		// how much space each thing takes up


	    public Inventory()
	    {
		    _sizes = new Dictionary<String, double>();
		    _stuff = new Dictionary<String, Point>();
		    _ideal = new Dictionary<String, double>();
		    maxSize = 0;
	    }

	    public void fromData(InventoryData data)
	    {
		    var sizes = new List<string>();
		    var amountsp = new List<Point>();
		    foreach (string key in data.start.Keys)
		    {
			    sizes.Add(key);
			    amountsp.Add(new Point(data.start[key],0));
		    }
		    setStuff(sizes, amountsp);
		    sizes = new List<string>();
		    var amounts = new List<double>();
		    foreach (string key in data.size.Keys)
		    {
			    sizes.Add(key);
			    amounts.Add(data.size[key]);
		    }
		    setSizes(sizes, amounts);
		    sizes = new List<string>();
		    amounts = new List<double>();
		    foreach (string key in data.ideal.Keys)
		    {
			    sizes.Add(key);
			    amounts.Add(data.ideal[key]);
			    setIdeal(sizes, amounts);
		    }
		    maxSize = data.maxSize;
	    }

	    public Inventory copy()
	    {
		    var i = new Inventory();
		    var stufff = new List<Point>();
		    var stuffi = new List<String>();
		    var idealf = new List<double>();
		    var ideali = new List<String>();
		    var sizesf = new List<double>();
		    var sizesi = new List<String>();
		    foreach (string key in _stuff.Keys)
		    {
			    stufff.Add(_stuff[key]);
			    stuffi.Add(key);
		    }
		    foreach (string key in _ideal.Keys)
		    {
			    idealf.Add(_ideal[key]);
			    ideali.Add(key);
		    }
		    foreach (string key in _sizes.Keys)
		    {
			    sizesf.Add(_sizes[key]);
			    sizesi.Add(key);
		    }
		    i.setStuff(stuffi, stufff);
		    i.setIdeal(ideali, idealf);
		    i.setSizes(sizesi, sizesf);
		    i.maxSize = maxSize;
		    return i;
	    }

	    public void destroy()
	    {
            _stuff.Clear();
            _ideal.Clear();
            _sizes.Clear();
		    _stuff = null;
		    _ideal = null;
		    _sizes = null;
	    }

	    /**
	     * Set amounts of various commodities
	     * @param	stuff_
	     * @param	amounts_
	     */

	    public void setStuff(List<String>stuff, List<Point>amounts)
	    {
		    for (int i=0; i<stuff.Count; i++)
		    {
			    _stuff[stuff[i]] = amounts[i];
		    }
	    }

	    /**
	     * Set how much of each commodity to stockpile
	     * @param	stuff_
	     * @param	amounts_
	     */

	    public void setIdeal(List<String>ideal, List<double>amounts)
	    {
		    for (int i=0; i<ideal.Count; i++)
		    {
			    _ideal[ideal[i]] = amounts[i];
		    }
	    }

	    public void setSizes(List<String>sizes, List<double>amounts)
	    {
		    for(int i=0; i<sizes.Count; i++)
		    {
			    _sizes[sizes[i]] = amounts[i];
		    }
	    }

	    /**
	     * Returns how much of this
	     * @param	commodity_ string id of commodity
	     * @return
	     */

	    public double query(String good)
	    {
		    if (_stuff.ContainsKey(good))
		    {
			    return _stuff[good].x;
		    }
		    return 0;
	    }
        public double query_cost(String good)
        {
            if (_stuff.ContainsKey(good))
            {
                return _stuff[good].y;
            }
            return 0;
        }

	    public double ideal(String good)
	    {
		    if (_ideal.ContainsKey(good))
		    {
			    return _ideal[good];
		    }
		    return 0;
	    }

	    public double getEmptySpace()
	    {
		    return maxSize - getUsedSpace();
	    }

	    public double getUsedSpace()
	    {
		    double space_used = 0;
		    foreach (string key in _stuff.Keys)
		    {
                if (!_sizes.ContainsKey(key)) continue;
			    space_used += _stuff[key].x * _sizes[key];
		    }
		    return space_used;
	    }

	    public double getCapacityFor(string good)
	    {
		    if (_sizes.ContainsKey(good))
		    {
			    return _sizes[good];
		    }
		    return -1;
	    }

	    /**
	     * Change the amount of the given commodity by delta
	     * @param	commodity_ string id of commodity
	     * @param	delta_ amount added
	     */

	    public double change(string good, double delta, double unit_cost)
	    {
		    Point result = new Point(0,0);

		    if (_stuff.ContainsKey(good))
		    {
			    var amount = _stuff[good];
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

		    _stuff[good] = result;
            return result.y; //return current unit cost
	    }

	    /**
	     * Returns # of units above the desired inventory level, or 0 if @ or below
	     * @param	commodity_ string id of commodity
	     * @return
	     */

	    public double surplus(string good)
	    {
		    var amt = query(good);
            double ideal = 0;
            if (_ideal.ContainsKey(good))
                ideal = _ideal[good];
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

	    public double shortage(string good)
	    {
		    if (!_stuff.ContainsKey(good))
		    {
			    return 0;
		    }
		    var amt = query(good);
            double ideal = 0;
            if (_ideal.ContainsKey(good))
                ideal = _ideal[good];
		    if (amt < ideal)
		    {
			    return (ideal - amt);
		    }
		    return 0;
	    }

    }
}
