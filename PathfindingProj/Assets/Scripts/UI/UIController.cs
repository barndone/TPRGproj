using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //  List of all buttons for controlling a unit
    [SerializeField] List<GameObject> unitButtons = new List<GameObject>();
    [SerializeField] public GameObject unitFrame;
    [SerializeField] public GameObject targetFrame;
    [SerializeField] Text turnTracker;
    [SerializeField] TurnManager TurnManager;

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

        /*
         * UnitButtons List:
         * 0 - attack button
         * 1 - skill button
         * 2 - wait button
         * 3 - cancel button
         * 4 - move button
         */

        //  if there is an active unit--
        if (gridManager.activeUnit != null)
        {
            //  cache the active unit
            Unit activeUnit = gridManager.activeUnit;

            switch (!activeUnit.hasMoved)
            {
                case false:
                    unitButtons[4].GetComponent<Button>().interactable = false;
                    break;
                case true:
                    unitButtons[4].GetComponent<Button>().interactable = true;
                    break;
            }

            switch (!activeUnit.hasActed)
            {
                case false:
                    unitButtons[0].GetComponent<Button>().interactable = false;
                    unitButtons[1].GetComponent<Button>().interactable = false;
                    break;

                case true:
                    unitButtons[0].GetComponent<Button>().interactable = true;
                    unitButtons[1].GetComponent<Button>().interactable = true;
                    break;
            }
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
}
