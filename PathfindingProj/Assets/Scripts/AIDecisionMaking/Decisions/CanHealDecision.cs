using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanHealDecision : IDecision
{
    public bool healer = false;

    public IDecision trueBranch;
    public IDecision falseBranch;
    public CanHealDecision() { }

    public CanHealDecision(Unit unit) 
    { 
        this.healer = unit.canHeal;
    }

    //  evaluate the decision
    public IDecision MakeDecision(Unit agent)
    {

        if (agent.partyManager.activeUnit == null)
        {
            if (!agent.waiting)
            {
                agent.isSelected = !agent.isSelected;
                agent.gridManager.activeUnit = agent;
                agent.partyManager.activeUnit = agent;

                //  update the selected animation depending on isSelected being T/F
                agent.animator.SetBool("selected", agent.isSelected);
            }
            return null;
        }
        else
        {
            if (agent.partyManager.activeUnit == agent)
            {
                if (agent.acting)
                {
                    return null;
                }

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
            else
            {
                return null;
            }
        }
    }
}
