using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyNeedHealingDecision : IDecision
{
    public Unit target;

    private List<Unit> allies;

    public float healThreshold = 0.5f;

    public IDecision trueBranch;
    public IDecision falseBranch;

    public AllyNeedHealingDecision() { }

    public IDecision MakeDecision(Unit agent)
    {

        //  populate the allies list with the agents reference to its party
        allies = agent.partyManager.party;
        
        //  iterate through the list of allies
        foreach(Unit unit in allies)
        {
            //  if a target doesn't currently exist
            if (target == null)
            {
                //  if a unit has less/equal health than the threshold (as a percentage)
                if ((float)(unit.CurHealth / unit.MaxHealth) <= healThreshold)
                {
                    //  set this unit as the target
                    target = unit;
                }
            }
            //  otherwise, a target currently exists
            else
            {
                //  check if that unit has lower health than the target
                if ((float)(unit.CurHealth / unit.MaxHealth) < (float)(target.CurHealth / target.MaxHealth))
                {
                    target = unit;
                }
            }

        }
        //  if the target isn't null
        if (target != null)
        {
            //  assign the target to the agents target field
            agent.target = target;
            //  go down the true path
            //  aka try to heal the bois
            return trueBranch.MakeDecision(agent);
        }

        //  otherwise, the target doesn't exist
        else
        {
            //  go down the false path
            //  aka ATTACK THE BOIS
            return falseBranch.MakeDecision(agent);
        }
    }
}
