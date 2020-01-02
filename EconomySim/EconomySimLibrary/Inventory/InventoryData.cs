using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class InventoryData
    {
	    public double MaxSize;
	    public Dictionary<String, double> Ideal;
	    public Dictionary<String, double> Start;
	    public Dictionary<String, double> Size;

	    public InventoryData(double maxSize, Dictionary<String,double> ideal, Dictionary<String,double> start, Dictionary<String,double> size)
	    {
		    this.MaxSize = maxSize;
		    this.Ideal = ideal;
		    this.Start = start;
		    this.Size = size;

            if (this.Size == null)
            {
                this.Size = new Dictionary<string, double>();
                foreach (KeyValuePair<String, double> entry in start)
                {
                    this.Size[entry.Key] = 1;
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
