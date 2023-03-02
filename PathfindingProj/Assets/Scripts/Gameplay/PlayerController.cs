using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    [SerializeField] private List<int> unitIds = new List<int>();

    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject clericPrefab;
    [SerializeField] private GameObject roguePrefab;
    [SerializeField] private GameObject rangerPrefab;

    void Awake()
    {
        unitIds = SaveSystem.instance.LoadParty();
        for (int i = 0; i < unitIds.Count; i++)
        {
            //  lazy initialization
            if (unitIds[i] == 0)
            {
                GameObject newUnit = Instantiate(warriorPrefab);
                party.Add(newUnit.GetComponent<Warrior>());
            }
            else if (unitIds[i] == 1)
            {
                GameObject newUnit = Instantiate(clericPrefab);
                party.Add(newUnit.GetComponent<Cleric>());
            }
            else if (unitIds[i] == 2)
            {
                GameObject newUnit = Instantiate(roguePrefab);
                party.Add(newUnit.GetComponent<Rogue>());
            }
            else if (unitIds[i] == 3)
            {
                GameObject newUnit = Instantiate(rangerPrefab);
                party.Add(newUnit.GetComponent<Ranger>());
            }
        }

        //  fan out the units!
        party[0].mapPosition = new Vector2(0, 0);
        party[1].mapPosition = new Vector2(0, 1);
        party[2].mapPosition = new Vector2(0, 2);

        //  asign start positions:
        party[0].StartOfTurnPos = party[0].mapPosition;
        party[1].StartOfTurnPos = party[1].mapPosition;
        party[2].StartOfTurnPos = party[2].mapPosition;

        //  assign the units party to this (player controller)
        //  used for managing death state of units
        party[0].partyManager = this;
        party[1].partyManager = this;
        party[2].partyManager = this;

    }
}
