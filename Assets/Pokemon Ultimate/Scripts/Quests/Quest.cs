using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus {None, Started, Completed}

[System.Serializable]
public class Quest
{
    [SerializeField] QuestBase questBase;
    [SerializeField] int stage;

    public QuestBase Base => questBase;
    public int Stage => stage;

    public Quest(QuestBase _base)
    {
        questBase = _base;
        stage = 1;
    }

    public void ProgressQuest()
    {
        stage++;
    }

    public void CompleteQuest()
    {
        stage = -1;
    }
}

[System.Serializable]
public class QuestAction
{
    [SerializeField] QuestBase quest;
    [SerializeField] int stage;
    [SerializeField] Dialog initialDialog;
    [SerializeField] List<Vector2> movement;

    [SerializeField] QuestActionOption actionToPerform;
    [SerializeField] List<QuestRequirement> questReqs;
    [SerializeField] List<ItemSlot> itemReqs;
    [SerializeField] Dialog successDialog;
    [SerializeField] Dialog failureDialog;

    [SerializeField] List<ItemSlot> itemsGiven = new List<ItemSlot>();
    [SerializeField] List<PartyPokemon> pokemonGiven = new List<PartyPokemon>();


    public QuestBase Quest => quest;
    public int Stage => stage;

    public IEnumerator PerformAction(PlayerController player)
    {
        QuestList questList = player.GetComponent<QuestList>();
        Inventory inventory = player.GetComponent<Inventory>();
        Party party = player.GetComponent<Party>();
        yield return DialogManager.Instance.ShowDialog(initialDialog);
        yield return player.ScriptedMovement(movement);
        if(RequirementsMet(player))
        {
            switch(actionToPerform)
            {
                case QuestActionOption.Start:
                    questList.StartQuest(quest);
                    break;
                case QuestActionOption.Progress:
                    questList.ProgressQuest(quest);
                    break;
                case QuestActionOption.Complete:
                    questList.CompleteQuest(quest);
                    break;
                default:
                    yield break;
            }

            foreach(ItemSlot item in itemsGiven)
            {
                if(inventory.AddItem(item.Item, item.Quantity))
                {
                    yield return DialogManager.Instance.ObtainItem(item.Item, item.Quantity);
                }
                else
                {
                    //item bag is full
                }
            }

            foreach(PartyPokemon pokemon in pokemonGiven)
            {
                if(party.AddPokemon(pokemon.GetPokemon()))
                {
                    yield return DialogManager.Instance.AddPokemonParty(pokemon.Species.SpeciesName);
                }
                else if(PC.Instance.AddPokemon(pokemon.GetPokemon()))
                {
                    yield return DialogManager.Instance.AddPokemonPC(pokemon.Species.SpeciesName);
                }                
                else if(pokemon != null)
                {
                    yield return DialogManager.Instance.ShowDialog("Oh dear, it seems your party and PC are full. Please come back and claim the Pokemon once you've made some room!");
                    yield break;
                }
                else
                {
                    Debug.Log($"hasPokemon is true but giftPokemon is null for NPC #(id)");
                    yield break;
                }
            }
            
            yield return DialogManager.Instance.ShowDialog(successDialog);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(failureDialog);
        }
    }

    bool RequirementsMet(PlayerController player)
    {
        QuestList questList = player.GetComponent<QuestList>();
        Inventory inventory = player.GetComponent<Inventory>();

        foreach(QuestRequirement questReq in questReqs)
        {
            if(!questReq.RequirementMet(questList.GetStage(questReq.RequiredQuest)))
            {
                return false;
            }
        }

        foreach(ItemSlot itemReq in itemReqs)
        {
            if(!inventory.HasItem(itemReq.Item, itemReq.Quantity))
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class QuestRequirement
{
    [SerializeField] QuestBase requiredQuest;
    [SerializeField] int requiredStage;

    public QuestBase RequiredQuest => requiredQuest;
    public int RequiredStage => requiredStage;

    public bool RequirementMet(int curStage)
    {
        return curStage == -1 || (requiredStage != -1 && curStage >= requiredStage);
    }
}
