using EconomySim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    /**
     * The most fundamental agent class, and has as little implementation as possible.
     * In most cases you should start by extending Agent instead of this.
     * @author larsiusprime
     */

    public class AgentData
    {
        public string ClassName { get; set; }
        public double Money;
        public InventoryData Inventory;
        public string logicName;
        public Logic Logic;
        public int? LookBack;

        public AgentData(string className, double money, string logicName)
        {
            this.ClassName = className;
            this.Money = money;
            this.logicName = logicName;
        }
    }
}
