using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSelectionController : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moves;
    
    [SerializeField] Color selectionColor;
    [SerializeField] Color defaultColor;

    public void SetMoves(List<PokemonMove> currentMoves, PokemonMove newMove)
    {
        for(int i = 0; i < currentMoves.Count; ++i)
        {
            moves[i].text = currentMoves[i].MoveBase.MoveName;
            Debug.Log($"Moves[{i}] = {moves[i].text}");
        }
        moves[currentMoves.Count].text = newMove.MoveBase.MoveName;
        Debug.Log($"Moves[{currentMoves.Count}] = {moves[currentMoves.Count].text}");
    }

    public void UpdateMoveSelection(int selection)
    {
        int i = 0;
        foreach(TextMeshProUGUI option in moves)
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
}
