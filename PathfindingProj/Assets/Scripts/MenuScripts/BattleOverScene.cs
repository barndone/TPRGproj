using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOverScene : MonoBehaviour
{
    [SerializeField] List<Sprite> portraits = new List<Sprite>();

    private List<int> stats = new List<int>();

    //  1.  class - unit id (0 - 3)
    //  2.  dmg done
    //  3.  dmg taken
    //  4.  healing done
    //  5.  healing taken
    //  6.  alive/dead (0/1)

    public struct UnitStats
    {
        public int classID;
        public int dmgDone;
        public int dmgTaken;
        public int healingDone;
        public int healingTaken;
        public bool IsDead;
    }

    UnitStats[] party = new UnitStats[3];

    private bool victory;

    [SerializeField] Text conditionText;

    private void Awake()
    {
        stats = SaveSystem.instance.LoadStats();

        //  hacky lazy way to initialize each struct

        party[0].classID = stats[0];
        party[0].dmgDone = stats[1];
        party[0].dmgTaken = stats[2];
        party[0].healingDone = stats[3];
        party[0].healingTaken = stats[4];
        if (stats[5] == 1)  { party[0].IsDead = true; }
        else { party[0].IsDead = false; }

        party[1].classID = stats[6];
        party[1].dmgDone = stats[7];
        party[1].dmgTaken = stats[8];
        party[1].healingDone = stats[9];
        party[1].healingTaken = stats[10];
        if (stats[11] == 1) { party[1].IsDead = true; }
        else { party[1].IsDead = false; }

        party[2].classID = stats[12];
        party[2].dmgDone = stats[13];
        party[2].dmgTaken = stats[14];
        party[2].healingDone = stats[15];
        party[2].healingTaken = stats[16];
        if (stats[17] == 1) { party[2].IsDead = true; }
        else { party[2].IsDead = false; }

        victory = CheckVictoryStatus();

        if (victory)
        {
            conditionText.text = "Victory!";
        }
        else
        {
            conditionText.text = "Defeat..";
        }
    }

    private bool CheckVictoryStatus()
    {
        foreach (UnitStats unit in party)
        {
            if (unit.IsDead == true)
            {
                return false;
            }
        }

        return true;
    }
}
