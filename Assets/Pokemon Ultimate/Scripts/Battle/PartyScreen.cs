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
 
    public event Action<Pokemon> OnClose;

    PartyMember[] members;
    int partySize = 0;
    int index = 0;

    public void Init()
    {
        members = GetComponentsInChildren<PartyMember>();
    }

    public void SetPartyMembers(List<Pokemon> party)
    {
        index = 0;
        partySize = party.Count;
        Pokemon active = null;
        Queue<Pokemon> fainted = new Queue<Pokemon>();
        Queue<Pokemon> notFainted = new Queue<Pokemon>();

        for(int j = 0; j < partySize; j++)
        { 
            if(party[j].IsActive)
            {
                active = party[j];
            }
            else if(party[j].Fainted)
            {
                fainted.Enqueue(party[j]);
            }
            else
            {
                notFainted.Enqueue(party[j]);
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
    }

    public void HandleInput(bool switchBecauseFainted)
    {       
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            index += 2;
            if(index >= partySize)
            {
                index = 6;
            }
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
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            index++;
            if(index >= partySize)
            {
                index = 6;
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            index = 6;
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if(index == 6)
            {
                if(!switchBecauseFainted)
                {
                    OnClose(null);
                }
                else
                {
                    text.text = $"{members[0].Poke.Nickname} is unable to battle.";
                }
            }
            else
            {
                if(index == 0)
                {
                    text.text = $"{members[index].Poke.Nickname} is already in battle.";
                }
                else if(!members[index].Poke.Fainted)
                {
                    OnClose(members[index].Poke);
                }
                else
                {
                    text.text = $"{members[index].Poke.Nickname} is unable to battle.";
                }
            }
        }

        UpdateSelection();
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