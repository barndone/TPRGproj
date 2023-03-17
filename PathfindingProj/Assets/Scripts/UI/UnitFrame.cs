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

    [SerializeField] List<Sprite> portraitOptions = new List<Sprite>();

    //  change the portrait on the unit frame
    public void SetPortrait(int classIndex)
    {
        portrait.sprite = portraitOptions[classIndex];
    }

    //  Change the health text on the unit frame
    public void SetHealthText(int CurHealth, int MaxHealth)
    {
        hpText.text = "Health: " + CurHealth + " / " + MaxHealth;
    }

    //  Change the attack stat on the unit frame
    public void SetAtkText(int atk)
    {
        atkText.text = "Atk: " + atk;
    }

    //  Change the defence stat on the unit frame
    public void SetDefText(int def)
    {
        defText.text = "Def: " + def;
    }

}
