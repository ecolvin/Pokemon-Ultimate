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
        //Global setting for if text auto plays after a delay or if user input is needed     
        if(true)
        {   
            yield return WaitForInput(new List<KeyCode>(){KeyCode.Space,KeyCode.Return,KeyCode.Escape});
        }
        else
        {
            //yield return new WaitForSeconds(pauseDuration);
        }
    }

    IEnumerator WaitForInput(List<KeyCode> keyCodes)
    {
        bool keyDown = false;
        while(!keyDown)
        {
            foreach(KeyCode kc in keyCodes)
            {
                if(Input.GetKeyDown(kc))
                {
                    keyDown = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator SlowText(string line)
    {
        if(typing)
        {
            yield break;
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
