using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    public class HistoryLog
    {
        private EconNoun type;
        private Dictionary<string, List<double>> log;

        public HistoryLog(EconNoun type)
        {
            this.type = type;
            log = new Dictionary<string, List<double>>();
        }

        /**
	     * Add a new entry to this log
	     * @param	name
	     * @param	amount
	     */
        public void Add(string name, double amount)
        {
            if (log.ContainsKey(name))
            {
                var list = log[name];
                list.Add(amount);
            }
        }

        /**
	     * Register a new category list in this log
	     * @param	name
	     */
        public void Register(string name)
        {
            if (!log.ContainsKey(name))
            {
                log[name] = new List<double>();
            }
        }

        /**
	     * Returns the average amount of the given category, looking backwards over a specified range
	     * @param	name the category of thing
	     * @param	range how far to look back
	     * @return
	     */
        public double Average(string name, int range)
        {
            if (log.ContainsKey(name))
            {
                var list = log[name];
                double amt = 0.0;
                var length = list.Count;
                if (length < range)
                {
                    range = length;
                }
                for (int i = 0; i < range; i++)
                {
                    amt += list[length - 1 - i];
                }
                if (range <= 0) return -1;
                return amt / range;
            }

            return 0;
        }
    }
}
