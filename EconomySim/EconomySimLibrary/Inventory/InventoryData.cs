using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{

    /**
     */
    public class InventoryData
    {
	    public double maxSize;
	    public Dictionary<String, double> ideal;
	    public Dictionary<String, double> start;
	    public Dictionary<String, double> size;

	    public InventoryData(double maxSize, Dictionary<String,double>ideal, Dictionary<String,double>start, Dictionary<String,double>size)
	    {
		    this.maxSize = maxSize;
		    this.ideal = ideal;
		    this.start = start;
		    this.size = size;
            if (this.size == null)
            {
                this.size = new Dictionary<string, double>();
                foreach (KeyValuePair<String, double> entry in start)
                {
                    this.size[entry.Key] = 1;
                }
            }
	    }

	    public InventoryData(string data)
	    {
            //var maxSize:Int = data.max_size;
            //var ideal = new Map<String, Float>();
            //var start = new Map<String, Float>();
            //var size  = new Map<String, Float>();

            //var startArray = Reflect.fields(data.start);
            //if (startArray != null)
            //{
            //    for (s in startArray)
            //    {
            //        start.set(s, cast Reflect.field(data.start, s));
            //        size.set(s, 1);	//initialize size of every item to 1 by default
            //    }
            //}
            //var idealArray = Reflect.fields(data.ideal);
            //if (idealArray != null)
            //{
            //    for (i in idealArray)
            //    {
            //        ideal.set(i, cast Reflect.field(data.ideal, i));
            //    }
            //}

            //return new InventoryData(maxSize, ideal, start, size);
	    }
    }
}
