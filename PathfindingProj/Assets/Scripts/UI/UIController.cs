using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    //  List of all buttons for controlling a unit
    [SerializeField] List<GameObject> unitButtons = new List<GameObject>();

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
