using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    public class WoodcutterLogic : Logic
    {
        override public void Perform(BasicAgent agent, Market market)
        {
            var food = agent.QueryInventory("food");
            var tools = agent.QueryInventory("tools");
            var wood = agent.QueryInventory("wood");
            var needWood = wood < 4;

            var hasFood = food >= 1;
            var hasTools = tools >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, "food", 1);//cost of living

            if (hasFood && needWood)
            {
                if (hasTools)
                {
                    //produce 2 wood, consume 1 food, break tools with 10% chance
                    Consume(agent, "food", 1);
                    Consume(agent, "tools", 1, 0.1);
                    Produce(agent, "wood", 2);
                }
                else
                {
                    //produce 1 wood, consume 1 food
                    Consume(agent, "food", 1);
                    Produce(agent, "wood", 1);
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
