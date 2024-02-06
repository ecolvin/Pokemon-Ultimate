using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokemonInvUI : MonoBehaviour
{
    [SerializeField] Image sprite;    
    [SerializeField] StatusIcon statusIcon;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image heldItemSprite;
    [SerializeField] Image useableIcon;
    [SerializeField] TextMeshProUGUI useableText;
    [SerializeField] Image heldItemBubble;
    [SerializeField] TextMeshProUGUI heldItemText;
    [SerializeField] HPBar expBar;
        
    public Image Sprite {get => sprite;}
    public Image UseableIcon {get => useableIcon;}
    public TextMeshProUGUI UseableText {get => useableText;}
    // public StatusIcon StatusIcon {get => statusIcon;}
    // public HPBar HpBar {get => hpBar;}
    // public Image HeldItemSprite {get => heldItemSprite;}

    public bool IsMoving {get; set;}

    Pokemon pokemon;

    InvUIState curState = InvUIState.Default;

    InventoryUI inventory;

    public void Awake()
    {
        inventory = GetComponentInParent<InventoryUI>();
    }

    public void Set(Pokemon p)
    {   
        pokemon = p;
        if(p == null)
        {
            sprite.gameObject.SetActive(false);
            statusIcon.gameObject.SetActive(false);
            heldItemSprite.gameObject.SetActive(false);
            hpBar.gameObject.SetActive(false);
            return;
        }
        p.OnDataChange += UpdateUI;
        sprite.gameObject.SetActive(true);
        sprite.sprite = p.Sprite;
        
        hpBar.SetHP(p.CurHP, p.Stats.HP);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if(pokemon == null)
        {
            return;
        }

        if(pokemon.Status == NonVolatileStatus.None || curState != InvUIState.Healing)
        {
            statusIcon.gameObject.SetActive(false);
        }
        else
        {
            statusIcon.gameObject.SetActive(true);
            statusIcon.UpdateIcon(pokemon.Status);
        }

        if(pokemon.HeldItem == null)
        {
            heldItemSprite.gameObject.SetActive(false);
        }
        else
        {
            heldItemSprite.gameObject.SetActive(true);
            heldItemSprite.sprite = pokemon.HeldItem.Icon;
        }

        if(hpBar.gameObject.activeInHierarchy)
        {
            StartCoroutine(hpBar.SetHPSmoothly(pokemon.CurHP, pokemon.Stats.HP));
        }
        else
        {
            hpBar.SetHP(pokemon.CurHP, pokemon.Stats.HP);
        }
    }

    public void SetState(InvUIState state)
    {
        curState = state;
        if(pokemon == null)
        {
            return;
        }
        switch(state)
        {
            case InvUIState.Default:
                statusIcon.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
                useableIcon.gameObject.SetActive(false);
                heldItemBubble.gameObject.SetActive(false);
                expBar.gameObject.SetActive(false);
                break;
            case InvUIState.Healing:
                if(pokemon.Status != NonVolatileStatus.None)
                {
                    statusIcon.gameObject.SetActive(true);
                    statusIcon.UpdateIcon(pokemon.Status);
                }
                hpBar.gameObject.SetActive(true);
                useableIcon.gameObject.SetActive(false);
                heldItemBubble.gameObject.SetActive(false); 
                expBar.gameObject.SetActive(false);
                break;
            case InvUIState.Useable:
                statusIcon.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
                useableIcon.gameObject.SetActive(true);
                heldItemBubble.gameObject.SetActive(false); 
                expBar.gameObject.SetActive(false);
                break;
            case InvUIState.Learnable:
                statusIcon.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
                useableIcon.gameObject.SetActive(true);
                heldItemBubble.gameObject.SetActive(false); 
                expBar.gameObject.SetActive(false);
                break;
            case InvUIState.HeldItem:
                statusIcon.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
                useableIcon.gameObject.SetActive(false);
                if(pokemon.HeldItem != null)
                {
                    heldItemText.text = pokemon.HeldItem.ItemName;
                    heldItemBubble.gameObject.SetActive(true);                    
                }
                expBar.gameObject.SetActive(false);
                break;
            case InvUIState.Experience:
                statusIcon.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
                useableIcon.gameObject.SetActive(false);
                heldItemBubble.gameObject.SetActive(false); 
                expBar.gameObject.SetActive(true);
                break;
            default:
                statusIcon.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
                useableIcon.gameObject.SetActive(false);
                heldItemBubble.gameObject.SetActive(false); 
                expBar.gameObject.SetActive(false);
                break;
        }
    }
}
