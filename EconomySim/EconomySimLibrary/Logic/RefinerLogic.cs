using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    public class RefinerLogic : Logic
    {
        override public void Perform(BasicAgent agent, Market market)
        {
            var food = agent.QueryInventory("food");
            var tools = agent.QueryInventory("tools");
            var ore = agent.QueryInventory("ore");
            if (ore > 4) ore = 4;
            var metal = agent.QueryInventory("metal");
            var needMetal = metal < 4;

            var hasFood = food >= 1;
            var hasTools = tools >= 1;
            var hasOre = ore >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, "food", 1);//cost of living

            if (hasFood && hasOre && needMetal)
            {
                if (hasTools)
                {
                    //convert all ore into metal, consume 1 food, break tools with 10% chance
                    Consume(agent, "ore", ore);
                    Consume(agent, "food", 1);
                    Consume(agent, "tools", 1, 0.1);
                    Produce(agent, "metal", ore);
                }
                else
                {
                    //convert up to 2 ore into metal, consume 1 food
                    var max = agent.QueryInventory("ore");
                    if (max > 2) { max = 2; }
                    Consume(agent, "ore", max);
                    Consume(agent, "food", 1);
                    Produce(agent, "metal", max);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!hasFood && agent.GetInventoryFull())
                {
                    //make_room_for(agent, "food", 2);
                }
            }
        }
    }
}
