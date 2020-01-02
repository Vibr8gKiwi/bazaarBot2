using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{    
    public static class Quick
    {
        //TODO: improve this design
        public static Random rnd = new Random();

        public static double Avg(double a, double b)
        {
            return (a + b) / 2;
        }

        public static double AvgList(List<double> list)
        {
            double avg=0;
            for (int j = 0; j < list.Count; j++)
            {
                avg += list[j];
            }
            avg /= list.Count;
            return avg;
        }

        public static double MinArr(List<double> a, int window)
        {
            double min = 99999999;//Math.POSITIVE_INFINITY;
            if (window > a.Count) window = a.Count;
            for (int i = 0; i < window-1; i++)
            {
                var f = a[a.Count-1 - i];
                if (f < min) { min = f; }
            }

            return min;
        }

        public static double MaxArr(List<double> a, int window)
        {
            double max = -9999999;///Math.NEGATIVE_INFINITY;
            if (window > a.Count) window = a.Count;
            for (int i = 0; i < window - 1; i++)
            {
                var f = a[a.Count - 1 - i];
                if (f > max) { max = f; }
            }
            return max;
        }

        /**
         * Turns a number into a string with the specified number of decimal points
         * @param	num
         * @param	decimals
         * @return
         */
        public static string NumToStr(double num, int decimals)
        {
            string s = string.Format("{0:N"+decimals.ToString()+"}", num);
            return s;
        }
    //        num = Math.floor(num * tens) / tens;
    //        var str:string = Std.string(num);
    //        var split = str.split(".");
    //        if (split.length == 2)
    //        {
    //            if (split[1].length < decimals)
    //            {
    //                var diff:Int = decimals - split[1].length;
    //                for (i in 0...diff)
    //                {
    //                    str += "0";
    //                }
    //            }
    //            if (decimals > 0)
    //            {
    //                str = split[0] + "." + split[1].substr(0, decimals);
    //            }
    //            else
    //            {
    //                str = split[0];
    //            }
    //        }
    //        else
    //        {
    //            if (decimals > 0)
    //            {
    //                str += ".";
    //                for (i in 0...decimals)
    //                {
    //                    str += "0";
    //                }
    //            }
    //        }
    //        return str;
    //    }

        public static double PositionInRange(double value, double min, double max, bool clamp = true)
        {
            value -= min;
            max -= min;
            min = 0;
            value = (value / (max - min));
            if (clamp) {
                if (value < 0) { value = 0; }
                if (value > 1) { value = 1; }
            }
            return value;
        }

    //    public static inline function randomInteger(min:Int, max:Int):Int
    //    {
    //        return Std.int(Math.random() * cast(1 + max - min, Float)) + min;
    //    }

        public static double RandomRange(double a, double b)
        {
            double r = rnd.NextDouble();
            double min = a < b ? a : b;
            double max = a < b ? b : a;
            double range = max - min;
            return r * range + min;
        }

        public static List<Offer> Shuffle(List<Offer>list)
        {
            /*
            To shuffle an array a of n elements (indices 0..n-1):
            for i from n − 1 downto 1 do
                j ← random integer with 0 ≤ j ≤ i
                exchange a[j] and a[i]
             */
            for (int i=0; i<list.Count - 1; i++)
            {
                int ii = (list.Count - 1) - i;
                if (ii > 1)
                {
                    int j = rnd.Next(ii);
                    var temp = list[j];
                    list[j] = list[ii];
                    list[ii] = temp;
                }
            }
            return list;
        }

        //TODO: convert these to refs
        public static int SortAgentAlpha(BasicAgent a, BasicAgent b)
        {
            return string.Compare(a.ClassName,b.ClassName);
        }

        public static int SortAgentId(BasicAgent a, BasicAgent b)
        {
            if (a.Id < b.Id) return -1;
            if (a.Id > b.Id) return 1;
            return 0;
        }

        public static int SortOfferAcending(Offer a, Offer b)
        {
            if (a.unitPrice < b.unitPrice) return -1;
            if (a.unitPrice > b.unitPrice) return 1;
            return 0;
        }
        public static int SortOfferDecending(Offer a, Offer b)
        {
            if (a.unitPrice > b.unitPrice) return -1;
            if (a.unitPrice < b.unitPrice) return 1;
            return 0;
        }


    //    public static function sortDecreasingPrice(a:Offer, b:Offer):Int
    //    {
    //        //Decreasing means: highest first
    //        if (a.unit_price < b.unit_price) return 1;
    //        if (a.unit_price > b.unit_price) return -1;
    //        return 0;
    //    }

    //    public static function sortIncreasingPrice(a:Offer, b:Offer):Int
    //    {
    //        //Increasing means: lowest first
    //        if (a.unit_price > b.unit_price) return 1;
    //        if (a.unit_price < b.unit_price) return -1;
    //        return 0;
    //    }
    }
}
