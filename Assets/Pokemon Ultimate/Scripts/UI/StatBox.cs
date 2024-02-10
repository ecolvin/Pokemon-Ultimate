using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statText;
    [SerializeField] Image natureIcon;
    [SerializeField] Sprite natPlusIcon;
    [SerializeField] Sprite natMinusIcon;
    [SerializeField] Sprite natNeutralIcon;

    public void SetHP(int curHP, int maxHP)
    {
        statText.text = $"{curHP}/{maxHP}";
    }

    public void SetStat(int stat)
    {
        statText.text = $"{stat}";
    }

    public void SetNature(int value)
    {
        if(value > 0)
        {
            natureIcon.color = Color.red;
            //natureIcon.sprite = natPlusIcon;
        }
        else if(value < 0)
        {
            natureIcon.color = Color.blue;
            //natureIcon.sprite = natMinusIcon;
        }
        else
        {
            natureIcon.color = Color.grey;
            //natureIcon.sprite = natNeutralIcon;
        }
    }
}
