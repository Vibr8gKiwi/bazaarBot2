using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{

    public class Good
    {
        public string Id { get; set; }
        public double Size { get; set; }

	    public Good (String id, double size)
	    {
		    Id = id;
		    Size = size;
	    }

	    public Good Copy()
	    {
		    return new Good(Id, Size);
	    }
    }
}
