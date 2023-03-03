using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanHealDecision : IDecision
{
    public bool healer = false;

    IDecision trueBranch;
    IDecision falseBranch;
    public CanHealDecision() { }

    public CanHealDecision(bool canHeal) 
    { 
        this.healer = canHeal;
    }

    //  evaluate the decision
    public IDecision MakeDecision(Unit agent)
    {
        //  if this unit is a healer
        if (agent.canHeal)
        {
            //  to healer branch
            return trueBranch.MakeDecision(agent);
        }

        //  otherwise, they cannot heal
        else
        {
            //  to combat branch
            return falseBranch.MakeDecision(agent);
        }
    }
}
