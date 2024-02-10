using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] float textDelay = .02f;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    
    public static DialogManager Instance {get; private set;}

    bool typing = false;

    void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        //Do things on update (update selected option if dialog has options)
    }

    public IEnumerator DisplayText(string text)
    {
        dialogBox.SetActive(true);
        yield return SlowText(text);
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
    }

    public IEnumerator ShowDialog(string text)
    {
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);

        yield return SlowText(text);
        yield return PauseAfterText();
    
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);

        foreach(string line in dialog.Lines)
        {
            yield return SlowText(line);
            yield return PauseAfterText();
        }

        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
    }

    public IEnumerator PauseAfterText()
    {  
        //Make triangle appear in bottom right corner to indicate more pending text
        if(GlobalSettings.Instance.WaitForInputAfterText)
        {   
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape));
        }
        else
        {
            yield return new WaitForSeconds(GlobalSettings.Instance.PauseDuration);
        }
    }

    IEnumerator SlowText(string line)
    {
        while(typing)
        {
            yield return null;
        }
        typing = true;  
        dialogText.text = "";
        foreach(char letter in line)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textDelay);
        }
        typing = false;
    }
}
