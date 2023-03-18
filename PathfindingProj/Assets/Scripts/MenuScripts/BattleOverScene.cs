using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleOverScene : MonoBehaviour
{
    

    private List<int> stats = new List<int>();
    private struct UnitStats
    {
        public int classID;
        public int dmgDone;
        public int dmgTaken;
        public int healingDone;
        public int healingTaken;
        public bool IsDead;
    }

    private bool victory;
    UnitStats[] party = new UnitStats[3];
    public StatScreenPanel[] statPanels = new StatScreenPanel[3];
    [SerializeField] Text conditionText;
    [SerializeField] List<Sprite> portraits = new List<Sprite>();

    [SerializeField] AudioSource buttonSource;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuMusicEnd;

    private bool shouldExit = false;

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

        for (int i = 0; i < statPanels.Length; i++)
        {
            //  if the party member is dead
            if (party[i].IsDead)
            {
                //  assing the portrait to the gravestone
                statPanels[i].portrait.sprite = portraits[4];
            }
            //  otherwise:
            else
            {
                //  assign the portrait given the class ID
                statPanels[i].portrait.sprite = portraits[party[i].classID];
            }

            statPanels[i].DmgDone.text = "Dmg Done: " + party[i].dmgDone;
            statPanels[i].DmgTaken.text = "Dmg Taken: " + party[i].dmgTaken;
            statPanels[i].HealingDone.text = "Healing Done: " + party[i].healingDone;
            statPanels[i].HealingTaken.text = "Healing Taken: " + party[i].healingTaken;
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

    //  return to the main menu
    public void ReturnToMenu()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedLoad(menuMusicEnd, 0));
    }

    //  return to the party select screen to select a new party
    public void ReturnToPartySelect()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedLoad(menuMusicEnd, 1));
    }

    //  re-attempt the battle scene with your previous party
    public void Continue()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedLoad(menuMusicEnd, 2));
    }

    //Delayed load for a scene with parameters for an audio clip, and a scene index
    IEnumerator DelayedLoad(AudioClip clip, int sceneIndex)
    {
        if (!shouldExit)
        {

            shouldExit = true;

            audioSource.Stop();
            audioSource.PlayOneShot(clip);

            yield return new WaitForSecondsRealtime(clip.length);

            shouldExit = false;
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
