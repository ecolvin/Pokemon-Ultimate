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
        if(text == "")
        {
            yield break;
        }
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        yield return SlowText(text);
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
    }

    public IEnumerator ShowDialog(string text)
    {
        if(text == "")
        {
            yield break;
        }
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);

        yield return SlowText(text);
        yield return PauseAfterText();
    
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        if(dialog == null)
        {
            yield break;
        }
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
    
    public IEnumerator ObtainItem(ItemBase item, int qty)
    {
        if(item is TM)
        {                    
            TM tm = (TM) item;
            if(qty > 1)
            {
                yield return DialogManager.Instance.ShowDialog($"You obtained {qty} TM{tm.Number:000} {tm.Move.MoveName}!");
                yield return DialogManager.Instance.ShowDialog($"You put the TM{tm.Number:000}s in your Bag's TM pocket.");
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog($"You obtained the TM{tm.Number:000} {tm.Move.MoveName}!");
                yield return DialogManager.Instance.ShowDialog($"You put the TM{tm.Number:000} in your Bag's TM pocket.");
            }
        }
        else
        {
            if(qty > 1)
            {
                yield return DialogManager.Instance.ShowDialog($"You obtained {qty} {item.ItemName}s!");
                yield return DialogManager.Instance.ShowDialog($"You put the {item.ItemName}s in your Bag's {item.Tab} pocket.");                 
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog($"You obtained the {item.ItemName}!");
                yield return DialogManager.Instance.ShowDialog($"You put the {item.ItemName} in your Bag's {item.Tab} pocket.");   
            }             
        }
    }

    public IEnumerator AddPokemonParty(string pokemon)
    {
        yield return ShowDialog($"You received {pokemon}!");
        yield return ShowDialog($"{pokemon} was added to your party!");
    }

    public IEnumerator AddPokemonPC(string pokemon)
    {
        yield return ShowDialog($"You received {pokemon}!");
        yield return ShowDialog($"{pokemon} was sent to your PC!");
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
