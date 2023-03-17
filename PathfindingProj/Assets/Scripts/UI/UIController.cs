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

    Button moveButton;
    Button attackButton;
    Button skillButton;

    [SerializeField] Button endTurnButton;

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
                    skillButton.interactable = false;
                    break;

                case true:
                    attackButton.interactable = true;
                    skillButton.interactable = true;
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
}
