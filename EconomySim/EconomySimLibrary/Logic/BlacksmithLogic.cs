using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySim
{
    class BlacksmithLogic : Logic
    {
        override public void Perform(BasicAgent agent, Market market)
        {
            var food = agent.QueryInventory("food");
            var metal = agent.QueryInventory("metal");
            var tools = agent.QueryInventory("tools");
            var need_tools = tools < 4;

            var hasFood = food >= 1;
            var hasMetal = metal >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, "food", 1);//cost of living

            if (hasFood && hasMetal & need_tools)
            {
                //convert all metal into tools
                Consume(agent, "metal", metal);
                Produce(agent, "tools", metal);
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!hasFood && agent.GetInventoryFull())
                {
                    //make_room_for(agent, "food", 2); stub todo needed?
                }
            }
        }
    }
}
