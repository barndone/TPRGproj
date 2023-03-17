using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitFrame : MonoBehaviour
{
    [SerializeField] Text atkText;
    [SerializeField] Text defText;
    [SerializeField] Text hpText;

    [SerializeField] Image portrait;
    [SerializeField] Image background;

    [SerializeField] List<Sprite> backgroundOptions = new List<Sprite>();
    [SerializeField] List<Sprite> portraitOptions = new List<Sprite>();

    public Unit target;

    //  change the portrait on the unit frame
    void SetPortrait(int classIndex)
    {
        portrait.sprite = portraitOptions[classIndex];
    }

    //  Change the health text on the unit frame
    void SetHealthText(int CurHealth, int MaxHealth)
    {
        hpText.text = "Health: " + CurHealth + " / " + MaxHealth;
    }

    //  Change the attack stat on the unit frame
    void SetAtkText(int atk)
    {
        atkText.text = "Atk: " + atk;
    }

    //  Change the defence stat on the unit frame
    void SetDefText(int def)
    {
        defText.text = "Def: " + def;
    }

    public void UpdateUnitFrame(Unit unit)
    {
        target = unit;

        SetPortrait(unit.unitID);
        SetHealthText(unit.CurHealth, unit.MaxHealth);
        SetAtkText(unit.Attack);
        SetDefText(unit.Defence);

        if (unit.ally)
        {
            background.sprite = backgroundOptions[0];
        }
        else
        {
            background.sprite = backgroundOptions[1];
        }
    }

    void FixedUpdate()
    {
        //  if this gameobject is active
        if (this.gameObject.activeSelf)
        {
            //  if the target is not null
            if (target != null)
            {
                //  update the frame with the target (so that hp updates!)
                UpdateUnitFrame(target);
            }
        }
    }
}
