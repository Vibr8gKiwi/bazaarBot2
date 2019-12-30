using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class Logic
    {
	    private bool init = false;

        //public function new(?data:Dynamic)
        //{
        //    //no implemenation -- provide your own in a subclass
        //}

	    /**
	     * Perform this logic on the given agent
	     * @param	agent
	     */

	    public virtual void perform(BasicAgent agent, Market market)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

        protected void _produce(BasicAgent agent, String commodity, double amount, double chance = 1.0)
	    {
		    if (chance >= 1.0 || Quick.rnd.NextDouble() < chance)
		    {
			    agent.produceInventory(commodity, amount);
		    }
	    }

	    protected void _consume(BasicAgent agent, String commodity, double amount, double chance = 1.0)
	    {
            if (chance >= 1.0 || Quick.rnd.NextDouble() < chance)
		    {
                //if (commodity == "money")
                //{
                //    agent.changeInventory(comm
                //    agent.money -= amount;
                //}
                //else
                //{
				    agent.consumeInventory(commodity, -amount);
                //}
		    }
	    }

    }

    class LogicBlacksmith : Logic
    {
        override public void perform(BasicAgent agent, Market market)
        {
            var food = agent.queryInventory("food");
            var metal = agent.queryInventory("metal");
            var tools = agent.queryInventory("tools");
            var need_tools = tools < 4;

            var has_food = food >= 1;
            var has_metal = metal >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            _consume(agent, "food", 1);//cost of living

            if (has_food && has_metal & need_tools)
            {
                //convert all metal into tools
                _consume(agent, "metal", metal);
                _produce(agent, "tools", metal);
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent, "food", 2); stub todo needed?
                }
            }
        }
    }

    class LogicFarmer : Logic
    {
        override public void perform(BasicAgent agent, Market market)
        {
            var wood = agent.queryInventory("wood");
            var tools = agent.queryInventory("tools");
            var food = agent.queryInventory("food");
            var need_food = food < 10;
            var work = agent.queryInventory("work");

            var has_wood = wood >= 1;
            var has_tools = tools >= 1;
            var has_work = work >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business

            if (need_food)
            {
                if (has_wood && has_tools && has_work)
                {
                    //produce 4 food, consume 1 wood, break tools with 10% chance
                    _consume(agent, "wood", 1, 1);
                    _consume(agent, "tools", 1, 0.1);
                    _consume(agent, "work", 1, 1);
                    _produce(agent, "food", 6, 1);
                }
                else if (has_wood && !has_tools && has_work)
                {
                    //produce 2 food, consume 1 wood
                    _consume(agent, "wood", 1, 1);
                    _consume(agent, "work", 1, 1);
                    _produce(agent, "food", 3, 1);
                }
                else //no wood
                {
                    //produce 1 food, 
                    _produce(agent, "food", 1, 1);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
            }
        }
    }


    class LogicMiner : Logic
    {
        override public void perform(BasicAgent agent, Market market)
        {
            var food = agent.queryInventory("food");
            var tools = agent.queryInventory("tools");
            var ore = agent.queryInventory("ore");
            var need_ore = ore < 4;

            var has_food = food >= 1;
            var has_tools = tools >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            _consume(agent, "food", 1);//cost of living

            if (has_food && need_ore)
            {
                if (has_tools)
                {
                    //produce 4 ore, consume 1 food, break tools with 10% chance
                    _consume(agent, "food", 1);
                    _consume(agent, "tools", 1, 0.1);
                    _produce(agent, "ore", 4);
                }
                else
                {
                    //produce 2 ore, consume 1 food
                    _consume(agent, "food", 1);
                    _produce(agent, "ore", 2);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent,"food",2);
                }
            }
        }
    }

    class LogicRefiner : Logic
    {
        override public void perform(BasicAgent agent, Market market)
        {
            var food = agent.queryInventory("food");
            var tools = agent.queryInventory("tools");
            var ore = agent.queryInventory("ore");
            if (ore > 4) ore = 4;
            var metal = agent.queryInventory("metal");
            var need_metal = metal < 4;

            var has_food = food >= 1;
            var has_tools = tools >= 1;
            var has_ore = ore >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            _consume(agent, "food", 1);//cost of living

            if (has_food && has_ore && need_metal)
            {
                if (has_tools)
                {
                    //convert all ore into metal, consume 1 food, break tools with 10% chance
                    _consume(agent, "ore", ore);
                    _consume(agent, "food", 1);
                    _consume(agent, "tools", 1, 0.1);
                    _produce(agent, "metal", ore);
                }
                else
                {
                    //convert up to 2 ore into metal, consume 1 food
                    var max = agent.queryInventory("ore");
                    if (max > 2) { max = 2; }
                    _consume(agent, "ore", max);
                    _consume(agent, "food", 1);
                    _produce(agent, "metal", max);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent, "food", 2);
                }
            }
        }
    }

    class LogicWoodcutter : Logic
    {
        override public void perform(BasicAgent agent, Market market)
        {
            var food = agent.queryInventory("food");
            var tools = agent.queryInventory("tools");
            var wood = agent.queryInventory("wood");
            var need_wood = wood < 4;

            var has_food = food >= 1;
            var has_tools = tools >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            _consume(agent, "food", 1);//cost of living

            if (has_food && need_wood)
            {
                if (has_tools)
                {
                    //produce 2 wood, consume 1 food, break tools with 10% chance
                    _consume(agent, "food", 1);
                    _consume(agent, "tools", 1, 0.1);
                    _produce(agent, "wood", 2);
                }
                else
                {
                    //produce 1 wood, consume 1 food
                    _consume(agent, "food", 1);
                    _produce(agent, "wood", 1);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent, "food", 2);
                }
            }
        }
    }

    class LogicWorker : Logic
    {
        override public void perform(BasicAgent agent, Market market)
        {
            var food = agent.queryInventory("food");
            var has_food = food >= 1;
            var work = agent.queryInventory("work");
            var need_work = work < 1;

            _consume(agent, "food", 1);
            //_consume(agent, "money", 0.5);//cost of living/business

            if (need_work)
            {
                _produce(agent, "work", 1);
            }
        }
    }


}
