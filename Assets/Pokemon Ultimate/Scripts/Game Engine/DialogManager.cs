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
    
    Action onDialogFinished;

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

    public IEnumerator ShowDialog(string text, Action onFinished=null)
    {
        Debug.Log("Invoking OnShowDialog");
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        onDialogFinished = onFinished;

        Debug.Log($"Calling SlowText(\"{text}\")");

        yield return SlowText(text);
        yield return PauseAfterText();
    
        Debug.Log("Invoking OnCloseDialog");
        dialogBox.SetActive(false);
        onDialogFinished?.Invoke();
        OnCloseDialog?.Invoke();
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished=null)
    {
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        onDialogFinished = onFinished;

        foreach(string line in dialog.Lines)
        {
            yield return SlowText(line);
            yield return PauseAfterText();
        }

        dialogBox.SetActive(false);
        onDialogFinished?.Invoke();
        OnCloseDialog?.Invoke();
    }

    public IEnumerator PauseAfterText()
    {  
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
        Debug.Log($"Slow Text = {line}");
        while(typing)
        {
            Debug.Log("Typing");
            yield return null;
        }
        typing = true;  
        dialogText.text = "";
        foreach(char letter in line)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textDelay);
            Debug.Log($"{dialogText.text}");
        }
        typing = false;
    }
}
