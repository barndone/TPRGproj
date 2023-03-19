using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //  List of all buttons for controlling a unit
    [SerializeField] List<GameObject> unitButtons = new List<GameObject>();
    [SerializeField] public GameObject unitFrame;
    [SerializeField] public GameObject targetFrame;
    [SerializeField] Text turnTracker;
    [SerializeField] TurnManager TurnManager;

    [SerializeField] Text helpText;

    [SerializeField] GridManager gridManager;

    //  flag for if a unit is selected
    //  if true -> show unitButtons
    //  otherwise -> hide unitBottons
    public bool unitSelected = false;

    //  flags for corresponding UI options
    public bool moveWish = false;
    public bool attackWish = false;
    public bool skillWish = false;
    public bool waitWish = false;
    public bool cancelWish = false;
    public bool endTurnWish = false;


    public bool showDangerZone = false;

    Button moveButton;
    Button attackButton;
    Button skillButton;

    [SerializeField] Button endTurnButton;

    [SerializeField] Text skillButtonText;

    [SerializeField] AudioSource buttonSource;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuMusicEnd;

    private bool shouldExit = false;

    void Start()
    {
        foreach(GameObject button in unitButtons)
        {
            button.SetActive(false);
        }

        unitFrame.SetActive(false);
        targetFrame.SetActive(false);

        TurnManager = FindObjectOfType<TurnManager>();
        gridManager = FindObjectOfType<GridManager>();

        /*
        *    UnitButtons List:
        *    0 - attack button
        *    1 - skill button
        *    2 - wait button
        *    3 - cancel button
        *    4 - move button
        */

        moveButton = unitButtons[4].GetComponent<Button>();
        attackButton = unitButtons[0].GetComponent<Button>();
        skillButton = unitButtons[1].GetComponent<Button>();

        helpText.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //  on unitSelected changing states
        switch (unitSelected)
        {
            //  if false:
            case false:
                //  hide all the unit action buttons
                foreach (GameObject button in unitButtons)
                {
                    button.SetActive(false);
                }
                break;
            //  if true:
            case true:
                //  show all the unit action buttons
                foreach (GameObject button in unitButtons)
                {
                    button.SetActive(true);
                }
                break;
        }

        switch (TurnManager.playerTurn)
        {
            case true:
                turnTracker.text = "Player Turn";
                turnTracker.color = Color.blue;
                break;
            case false:
                turnTracker.text = "Enemy Turn";
                turnTracker.color = Color.red;
                break;
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Time.timeScale = 15;
        }



        //  if there is an active unit--
        if (gridManager.activeUnit != null)
        {

            endTurnButton.interactable = false;
            //  cache the active unit
            Unit activeUnit = gridManager.activeUnit;

            switch (!activeUnit.hasMoved)
            {
                case false:
                    moveButton.interactable = false;
                    break;
                case true:
                    moveButton.interactable = true;
                    break;
            }

            switch (!activeUnit.hasActed)
            {
                case false:
                    attackButton.interactable = false;
                    break;

                case true:
                    attackButton.interactable = true;
                    break;
            }

            if (activeUnit.canUseSkill && !activeUnit.hasActed)
            {
                skillButton.interactable = true;
            }
            else
            {
                skillButton.interactable = false;
            }

            switch (activeUnit.unitID)
            {
                case 0:
                    //  unitID of 0 == warrior
                    skillButtonText.text = "Shove a unit one tile away.\nIf there is a collision, each unit takes 1 dmg.";
                    break;
                case 1:
                    //  unitId of 1 == Cleric
                    Cleric cleric = (Cleric)activeUnit;
                    skillButtonText.text = "Heal an ally by "+ cleric.Attack + ".\nRemaining uses: " + (cleric.maxHeals - cleric.healCount) + " of " + cleric.maxHeals;
                    break;
                case 2:
                    //  unitID of 2 == rogue
                    Rogue rogue = (Rogue)activeUnit;
                    skillButtonText.text = "Stun an enemy.\nRemaining uses: " + (rogue.maxStuns - rogue.stunCount) + " of " + rogue.maxStuns;
                    break;
                case 3:
                    //  unitID of 3 == ranger
                    skillButtonText.text = "Fire a powerful shot.\nUnit cannot move the same turn as using this skill.";
                    break;
            }

        }

        else
        {
            endTurnButton.interactable = true;
        }
    }

    public void MoveButton()
    {
        moveWish = !moveWish;

        attackWish = false;
        skillWish = false;
        waitWish = false;
        cancelWish = false;
        endTurnWish = false;
    }

    public void AttackButton()
    {
        attackWish = !attackWish;

        moveWish = false;
        skillWish = false;
        waitWish = false;
        cancelWish = false;
        endTurnWish = false;
    }

    public void SkillButton()
    {
        skillWish = !skillWish;

        attackWish = false;
        moveWish = false;
        waitWish = false;
        cancelWish = false;
        endTurnWish = false;
    }

    public void WaitButton()
    {
        waitWish = !waitWish;

        attackWish = false;
        skillWish = false;
        moveWish = false;
        cancelWish = false;
        endTurnWish = false;
    }

    public void CancelButton()
    {
        cancelWish = !cancelWish;

        attackWish = false;
        skillWish = false;
        waitWish = false;
        moveWish = false;
        endTurnWish = false;
    }

    public void EndTurnButton()
    {
        endTurnWish = !endTurnWish;

        attackWish = false;
        skillWish = false;
        waitWish = false;
        cancelWish = false;
        moveWish = false;
    }

    public void ResetFlags()
    {
        attackWish = false;
        skillWish = false;
        waitWish = false;
        cancelWish = false;
        moveWish = false;
        endTurnWish = false;
    }

    public void Help()
    { 
        if (helpText.gameObject.activeSelf)
        {
            helpText.gameObject.SetActive(false);
        }
        else
        {
            helpText.gameObject.SetActive(true);
        }
    }

    public void DangerRadius()
    {
        showDangerZone = !showDangerZone;

        if (showDangerZone)
        {
            gridManager.CalculateDangerZone(TurnManager.cpu.party);
        }

        else
        {
            //  invert the thing
            foreach (Unit enemy in TurnManager.cpu.party)
            {
                gridManager.HideAccessibleTiles(enemy, enemy.accessibleTiles);
            }
        }
    }

    public void Quit()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedQuit(menuMusicEnd));
    }

    //  return to the main menu
    public void ReturnToMenu()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedLoad(menuMusicEnd, 0));
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

    IEnumerator DelayedQuit(AudioClip clip)
    {
        if (!shouldExit)
        {
            shouldExit = true;

            audioSource.Stop();
            audioSource.PlayOneShot(clip);

            yield return new WaitForSecondsRealtime(clip.length);

            shouldExit = false;
            Application.Quit();
        }
    }
}
