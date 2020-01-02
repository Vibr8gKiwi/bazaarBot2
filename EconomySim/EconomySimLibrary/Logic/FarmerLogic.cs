using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    public class FarmerLogic : Logic
    {
        override public void Perform(BasicAgent agent, Market market)
        {
            var wood = agent.QueryInventory("wood");
            var tools = agent.QueryInventory("tools");
            var food = agent.QueryInventory("food");
            var needFood = food < 10;
            var work = agent.QueryInventory("work");

            var hasWood = wood >= 1;
            var hasTools = tools >= 1;
            var hasWork = work >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business

            if (needFood)
            {
                if (hasWood && hasTools && hasWork)
                {
                    //produce 4 food, consume 1 wood, break tools with 10% chance
                    Consume(agent, "wood", 1, 1);
                    Consume(agent, "tools", 1, 0.1);
                    Consume(agent, "work", 1, 1);
                    Produce(agent, "food", 6, 1);
                }
                else if (hasWood && !hasTools && hasWork)
                {
                    //produce 2 food, consume 1 wood
                    Consume(agent, "wood", 1, 1);
                    Consume(agent, "work", 1, 1);
                    Produce(agent, "food", 3, 1);
                }
                else //no wood
                {
                    //produce 1 food, 
                    Produce(agent, "food", 1, 1);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
            }
        }
    }
}
