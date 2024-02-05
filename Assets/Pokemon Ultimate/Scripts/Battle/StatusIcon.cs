using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] List<StatusInfo> statusInfo;

    Dictionary<NonVolatileStatus, StatusInfo> statusMap = new Dictionary<NonVolatileStatus, StatusInfo>();
    public Dictionary<NonVolatileStatus, StatusInfo> StatusMap {get => statusMap;}

    bool initialized = false;

    public void Init() 
    {
        if(initialized)
        {
            return;
        }
        foreach(StatusInfo si in statusInfo)
        {
            statusMap.Add(si.Status, si);
        }
        initialized = true;
    }

    public void UpdateIcon(NonVolatileStatus status)
    {
        Debug.Log($"Updating Icon to {status}");
        if(!initialized)
        {
            Init();
        }
        GetComponent<Image>().sprite = statusMap[status].Sprite;
    }

    public void UpdateBattleIcon(NonVolatileStatus status)
    {
        if(!initialized)
        {
            Init();
        }
        UpdateColor(statusMap[status].IconColor);
        UpdateText(statusMap[status].IconText);
    }

    void UpdateColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    void UpdateText(string s)
    {
        if(text != null)
        {
            text.text = s;
        }
    }
}

[System.Serializable]
public class StatusInfo
{
    [SerializeField] NonVolatileStatus status;
    public NonVolatileStatus Status{get => status;}
    [SerializeField] Color iconColor;
    public Color IconColor{get => iconColor;}
    [SerializeField] string iconText;
    public string IconText{get => iconText;}
    [SerializeField] Sprite sprite;
    public Sprite Sprite{get => sprite;}
}
