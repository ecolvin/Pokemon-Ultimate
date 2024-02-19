using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestActionOption {None, Start, Progress, Complete}

public class QuestTrigger : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] List<QuestAction> questActions;

    QuestList questList;

    void Start()
    {
        questList = QuestList.GetQuestList();
    }

    public IEnumerator OnPlayerTriggered(PlayerController player)
    {
        yield return ResolveTrigger(player);        
    }

    IEnumerator ResolveTrigger(PlayerController player)
    {  
        Debug.Log($"Resolving quest trigger. Quest Actions contains {questActions.Count} items");
        foreach(QuestAction action in questActions)
        {
            Debug.Log($"Resolving action for {action.Quest.QuestName}: Stage {action.Stage}");
            if(questList.GetStage(action.Quest) == action.Stage)
            {
                Debug.Log($"Quest with the correct stage found! Performing action");
                yield return action.PerformAction(player);
                yield break;
            }
        }
    }
}