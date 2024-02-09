using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgetMoveOption : MonoBehaviour
{
    [SerializeField] Image typeIcon;
    [SerializeField] TextMeshProUGUI moveName;
    [SerializeField] Image ppBackground;
    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] Color ppDefaultColor;
    [SerializeField] Color ppSelectColor;

    Image image;

    PokemonMove move;
    public PokemonMove Move {get => move;}

    void Awake()
    {
        if(image == null)
        {
            image = gameObject.GetComponent<Image>();
        }
    }

    public void SetMove(PokemonMove move)
    {
        this.move = move;
        //typeIcon = move.Type.Icon;
        moveName.text = move.MoveBase.MoveName;
        ppText.text = $"{move.CurPP}/{move.MaxPP}";
    }

    public void Select()
    {
        ppBackground.color = ppSelectColor;
        moveName.color = Color.black;
        if(image == null)
        {
            image = gameObject.GetComponent<Image>();
        }
        image.color = GlobalSettings.Instance.SelectedBarColor;
    }

    public void Unselect()
    {
        ppBackground.color = ppDefaultColor;
        moveName.color = Color.white;
        if(image == null)
        {
            image = gameObject.GetComponent<Image>();
        }
        image.color = Color.clear;
    }
}
