using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SideBoxController : MonoBehaviour
{
    [SerializeField] Color selectionColor;
    [SerializeField] Color defaultColor;
    [SerializeField] Color outOfPPColor;
    [SerializeField] Color defaultPPColor;

    [SerializeField] MainBoxController mainBox;

    [SerializeField] GameObject battleOptions;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<TextMeshProUGUI> battleOptionText;
    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI typeText;

//----------------------Battle States-----------------------

    public void Clear()
    {
        battleOptions.SetActive(false);
        moveDetails.SetActive(false);
    }

    public void PlayerSelection(int selection)
    {
        updateBattleSelection(selection);
        battleOptions.SetActive(true);
        moveDetails.SetActive(false);
    }

    public void Fight(PokemonMove move)
    {
        battleOptions.SetActive(false);
        updateMoveDetails(move);
        moveDetails.SetActive(true);
    }

//-----------------------Dynamic UI Updates-------------------------

    public void updateBattleSelection(int selection)
    {
        int i = 0;
        foreach(TextMeshProUGUI option in battleOptionText)
        {
            if(i == selection)
            {
                option.color = selectionColor;
            }
            else
            {
                option.color = defaultColor;
            }
            i++;
        }
    }

    public void updateMoveDetails(PokemonMove move)
    {
        if(move == null)
        {
            ppText.color = defaultPPColor;
            ppText.text = "PP: ??/??";
            typeText.text = "TYPE: None";
            return;
        }
        if(move.CurPP <= 0)
        {
            ppText.color = outOfPPColor;
        }
        else
        {
            ppText.color = defaultPPColor;
        }
        ppText.text = $"PP: {move.CurPP}/{move.MaxPP}";
        typeText.text = $"TYPE: {move.MoveBase.MoveType}";
    }

}
