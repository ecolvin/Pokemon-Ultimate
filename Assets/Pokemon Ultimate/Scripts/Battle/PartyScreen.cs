using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image cancelButton;
    [SerializeField] Color cancelDefaultColor;
    [SerializeField] Color cancelSelectionColor;
  
    //public event Action<Pokemon> OnClose;

    Party party;

    PartyMember[] members;
    int partySize = 0;
    int index = 0;

    public void Init()
    {
        members = GetComponentsInChildren<PartyMember>();
        party = Party.GetParty();
        SetPartyMembers();

        party.OnUpdated += SetPartyMembers;
    }

    public void SetPartyMembers()
    {
        List<Pokemon> pokemon = party.Pokemon;

        index = 0;
        partySize = pokemon.Count;
        Pokemon active = null;
        Queue<Pokemon> fainted = new Queue<Pokemon>();
        Queue<Pokemon> notFainted = new Queue<Pokemon>();

        for(int j = 0; j < partySize; j++)
        { 
            if(pokemon[j].IsActive)
            {
                active = pokemon[j];
            }
            else if(pokemon[j].Fainted)
            {
                fainted.Enqueue(pokemon[j]);
            }
            else
            {
                notFainted.Enqueue(pokemon[j]);
            }
        }

        if(active != null)
        {
            members[0].Set(active);
        }
        else
        {
            members[0].Set(notFainted.Dequeue());
        }  
        int i = 1;
        while(notFainted.Count > 0)
        {
            members[i].Set(notFainted.Dequeue());
            i++;
        }      
        while(fainted.Count > 0)
        {
            members[i].Set(fainted.Dequeue());
            i++;
        }        
        while(i < 6)
        {
            members[i].Disable();
            i++;
        }
        UpdateSelection();
    }

    public void ForcedSelection()
    {
        text.text = $"{members[0].Poke.Nickname} is unable to battle.";
    }

    public void SelectionFainted()
    {
        text.text = $"{members[index].Poke.Nickname} is unable to battle.";
    }

    public void SelectionActive()
    {
        text.text = $"{members[index].Poke.Nickname} is already in battle.";
    }

    public void InvalidSelection()
    {
        text.text = $"Please make a different selection.";
    }

    public void HandleInput(Action<Pokemon> onSelection, Action onClose)
    {       
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            index += 2;
            if(index >= partySize)
            {
                index = 6;
            }
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if(index == 6)
            {
                index = partySize - 1;
            }
            else
            {
                index -= 2;
            }
            if(index < 0)
            {
                index = 0;
            }
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if(index == 6)
            {
                index = partySize - 1;
            }
            else
            {
                index--;
            }
            if(index < 0)
            {
                index = 0;
            }
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            index++;
            if(index >= partySize)
            {
                index = 6;
            }
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            index = 6;
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            text.text = "";
            if(index == 6)
            {
                onClose?.Invoke();
            }
            else
            {
                onSelection?.Invoke(members[index].Poke);
            }
        }
    }

    void UpdateSelection()
    {
        foreach(PartyMember member in members)
        {
            member.Default();
        }
        cancelButton.color = cancelDefaultColor;

        if(index == 6)
        {
            cancelButton.color = cancelSelectionColor;
        }
        else
        {
            members[index].Select();
        }
    }
}