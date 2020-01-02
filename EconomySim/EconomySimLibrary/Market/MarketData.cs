using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomySim
{
    public class MarketData
    {
	    public List<Good> goods;
	    public List<AgentData> agentTypes;
	    public List<BasicAgent> agents;

	    public MarketData(List<Good> goods, List<AgentData> agentTypes, List<BasicAgent> agents)
	    {
		    this.goods = goods;
		    this.agentTypes = agentTypes;
		    this.agents = agents;
	    }

	    /**
	     * Parse a market settings file to construct everything
	     * @param	data		the JSON file definition for your Market
	     * @param	getAgent	a function to create agents
	     */

	    public static MarketData fromJSON(string json, BasicAgent getAgent)
	    {
            //var goods:Array<Good> = [];

            ////Create goods index
            //var jsonGoods:Array<Dynamic> = json.goods;
            //for (g in jsonGoods)
            //{
            //    goods.push(new Good(g.id, g.size));
            //}

            //var agentTypes:Array<AgentData> = [];

            ////Create agent classes
            //var jsonAgents:Array<Dynamic> = json.agents;

            //for (a in jsonAgents)
            //{
            //    var agentData:AgentData =
            //    {
            //        className:a.id,
            //        money:a.money,
            //        inventory:InventoryData.fromJson(a.inventory),
            //        logicName:a.id,
            //        logic:null
            //    }

            //    for (g in goods)
            //    {
            //        agentData.inventory.size.set(g.id, g.size);
            //    }

            //    agentTypes.push(agentData);
            //}

            ////Make the agent list
            //var agents:Array<BasicAgent> = [];

            ////Get start conditions
            //var startConditions:Dynamic = json.start_conditions;
            //var starts = Reflect.fields(startConditions.agents);

            //var agentIndex:Int = 0;
            ////Make given number of each agent type

            //for (classStr in starts)
            //{
            //    var val:Int = Reflect.field(startConditions.agents, classStr);
            //    var agentData = null;
            //    for (i in 0...agentTypes.length) {
            //        if (agentTypes[i].className == classStr)
            //        {
            //            agentData = agentTypes[i];
            //            break;
            //        }
            //    }

            //    for (i in 0...val)
            //    {
            //        var a:BasicAgent = getAgent(agentData);
            //        a.id = agentIndex;
            //        agentIndex++;
            //        agents.push(a);
            //    }
            //}

            //return new MarketData(goods, agentTypes, agents);
            return null;
	    }
    }
}
