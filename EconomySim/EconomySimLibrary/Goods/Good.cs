using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{

    public class Good
    {
	    public String id = "";		//string id of good
	    public double size = 1.0;	//inventory size taken up

	    public Good (String id_, double size_)
	    {
		    id = id_;
		    size = size_;
	    }

	    public Good copy()
	    {
		    return new Good(id, size);
	    }
    }
}
