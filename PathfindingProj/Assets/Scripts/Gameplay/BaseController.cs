using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public List<Unit> party;

    [SerializeField] protected TurnManager TurnManager;
    [SerializeField] protected UIController uiCon;

    public bool noMoreActions = false;

    public Unit activeUnit;

    private void Update()
    {
        if (uiCon.endTurnWish)
        {
            uiCon.endTurnWish = false;
            TurnManager.TurnEnd();
        }
    }
}
