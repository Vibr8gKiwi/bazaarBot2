using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    class WorkerLogic : Logic
    {
        override public void Perform(BasicAgent agent, Market market)
        {
            var food = agent.QueryInventory("food");
            var hasFood = food >= 1;
            var work = agent.QueryInventory("work");
            var needWork = work < 1;

            Consume(agent, "food", 1);
            //_consume(agent, "money", 0.5);//cost of living/business

            if (needWork)
            {
                Produce(agent, "work", 1);
            }
        }
    }
}
