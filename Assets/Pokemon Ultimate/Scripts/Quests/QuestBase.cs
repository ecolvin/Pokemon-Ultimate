using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Create new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string questName;
    [SerializeField] List<string> stageDescriptions;
    [SerializeField] bool showInQuestList;

    public string QuestName => questName;
    public List<string> StageDescriptions => stageDescriptions;
    public bool ShowInQuestList => showInQuestList;
}
