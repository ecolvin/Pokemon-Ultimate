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
        Debug.Log($"Status = {status}");
        if(!initialized)
        {
            Init();
        }
        UpdateColor(statusMap[status].IconColor);
        UpdateText(statusMap[status].IconText);
    }

    void UpdateColor(Color color)
    {
        this.gameObject.GetComponent<Image>().color = color;
    }

    void UpdateText(string s)
    {
        text.text = s;
    }
}

[System.Serializable]
public class StatusInfo
{
    [SerializeField] NonVolatileStatus status;
    public NonVolatileStatus Status{get{return status;}}
    [SerializeField] Color iconColor;
    public Color IconColor{get{return iconColor;}}
    [SerializeField] string iconText;
    public string IconText{get{return iconText;}}
}
