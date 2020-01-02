using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    public class MinerLogic : Logic
    {
        override public void Perform(BasicAgent agent, Market market)
        {
            var food = agent.QueryInventory("food");
            var tools = agent.QueryInventory("tools");
            var ore = agent.QueryInventory("ore");
            var needOre = ore < 4;

            var hasFood = food >= 1;
            var hasTools = tools >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, "food", 1);//cost of living

            if (hasFood && needOre)
            {
                if (hasTools)
                {
                    //produce 4 ore, consume 1 food, break tools with 10% chance
                    Consume(agent, "food", 1);
                    Consume(agent, "tools", 1, 0.1);
                    Produce(agent, "ore", 4);
                }
                else
                {
                    //produce 2 ore, consume 1 food
                    Consume(agent, "food", 1);
                    Produce(agent, "ore", 2);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!hasFood && agent.GetInventoryFull())
                {
                    //make_room_for(agent,"food",2);
                }
            }
        }
    }
}
