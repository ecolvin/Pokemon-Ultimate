using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class QuestList : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();

    public List<Quest> Quests => quests;

    public static QuestList GetQuestList()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestList>();
    }

    public void Set(List<Quest> quests)
    {
        this.quests = quests;
    }

    public int GetStage(QuestBase questBase)
    {        
        Quest quest = quests.FirstOrDefault(q => q.Base == questBase);
        if(quest == null)
        {
            return 0;
        }
        return quest.Stage;
    }

    public void StartQuest(QuestBase questBase)
    {
        Quest quest = quests.FirstOrDefault(q => q.Base == questBase);
        if(quest != null)
        {
            Debug.LogError($"Tried to start quest ({questBase.QuestName}) but it already exists.");
        }
        quest = new Quest(questBase);
        quests.Add(quest);
    }

    public void ProgressQuest(QuestBase questBase)
    {
        Quest quest = quests.FirstOrDefault(q => q.Base == questBase);
        if(quest == null)
        {
            Debug.LogError($"Quest progression failed. Quest ({questBase.QuestName}) could not be found.");
        }
        quest.ProgressQuest();
    }

    public void CompleteQuest(QuestBase questBase)
    {
        Quest quest = quests.FirstOrDefault(q => q.Base == questBase);
        if(quest == null)
        {
            quest = new Quest(questBase);
            quest.CompleteQuest();   
            quests.Add(quest);  
        }
        else
        {
            quest.CompleteQuest();  
        }  
    }
}
