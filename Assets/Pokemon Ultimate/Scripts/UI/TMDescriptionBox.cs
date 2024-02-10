using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMDescriptionBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmNos;
    [SerializeField] TextMeshProUGUI tmName;
    [SerializeField] Image typeIcon;
    [SerializeField] Image categoryIcon;
    [SerializeField] TextMeshProUGUI moveDescription;
    [SerializeField] TextMeshProUGUI movePowerText;
    [SerializeField] TextMeshProUGUI moveAccuracyText;
    [SerializeField] TextMeshProUGUI movePPText;

    public void Set(TM tm)
    {
        if(tm == null)
        {
            tmNos.text = "000";
            tmName.text = "";
            typeIcon.color = Color.clear;
            categoryIcon.color = Color.clear;
            moveDescription.text = "";
            movePowerText.text = "--";
            moveAccuracyText.text = "--";
            movePPText.text = "--";
        }
        tmNos.text = $"{tm.Number:000}";
        tmName.text = $"{tm.Move.MoveName}";
        typeIcon.color = Color.white;
        typeIcon.sprite = GlobalSpriteDictionary.Instance.TypeBars[tm.Move.MoveType];
        categoryIcon.color = Color.white;
        categoryIcon.sprite = GlobalSpriteDictionary.Instance.MoveCategorySprites[tm.Move.Category];
        moveDescription.text = $"{tm.Move.Description}";
        
        movePowerText.text = $"{tm.Move.BasePower}";
        if(tm.Move.BasePower <= 1)
        {
            movePowerText.text = "--";
        }

        moveAccuracyText.text = $"{tm.Move.Accuracy}";
        if(tm.Move.Accuracy == -1)
        {
            moveAccuracyText.text = "--";            
        }
        
        movePPText.text = $"{tm.Move.PP}";
    }
}
