using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public enum ForgetMoveState {MoveSelection, Confirmation, Busy}

public class ForgetMoveScreen : MonoBehaviour
{
    [SerializeField] List<ForgetMoveOption> moveOptions;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image categoryIcon;
    [SerializeField] TextMeshProUGUI movePowerText;
    [SerializeField] TextMeshProUGUI moveAccuracyText;

    [SerializeField] GameObject choiceBox;
    [SerializeField] Image yesBox;
    [SerializeField] Image noBox;

    int selectedMove = 0;
    bool choice = true;
    ForgetMoveState state = ForgetMoveState.MoveSelection;
    Action<int> onClose;

    public void OpenScreen(Pokemon pokemon, PokemonMove move, Action<int> onClose)
    {
        this.onClose = onClose;
        state = ForgetMoveState.MoveSelection;
        for(int i = 0; i < moveOptions.Count - 1; i++)
        {
            if(i < pokemon.Moves.Count)
            {
                moveOptions[i].SetMove(pokemon.Moves[i]);
            }
            else
            {
                gameObject.SetActive(false);
                onClose?.Invoke(pokemon.Moves.Count);
                return;
            }
        }
        moveOptions[moveOptions.Count-1].SetMove(move);
        UpdateMoveSelection();
        gameObject.SetActive(true);
    }

    public void HandleUpdate()
    {
        if(state == ForgetMoveState.MoveSelection)
        {
            HandleMoveInput();
        }
        else if(state == ForgetMoveState.Confirmation)
        {
            HandleConfirmationInput();
        }
        //else busy
    }

    void HandleMoveInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)
        || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedMove--;
            if(selectedMove < 0)
            {
                selectedMove += moveOptions.Count;
            }
            UpdateMoveSelection();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) 
        || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selectedMove++;
            if(selectedMove >= moveOptions.Count)
            {
                selectedMove -= moveOptions.Count;
            }
            UpdateMoveSelection();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            selectedMove = moveOptions.Count - 1;
            UpdateMoveSelection();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(OpenConfirmationBox());
        }    
    }

    void HandleConfirmationInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)
        || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)
        || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) 
        || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            choice = !choice;
            UpdateChoiceSelection();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            choice = false;
            UpdateChoiceSelection();            
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            state = ForgetMoveState.MoveSelection;
            choiceBox.SetActive(false);
            DialogManager.Instance.CloseDialog();
            if(choice)
            {                  
                gameObject.SetActive(false);  
                onClose?.Invoke(selectedMove);
            }
        }
    }

    void UpdateMoveSelection()
    {
        for(int i = 0; i < moveOptions.Count; i++)
        {
            if(i == selectedMove)
            {
                moveOptions[i].Select();
                UpdateMoveDetails(moveOptions[i].Move);
            }
            else
            {
                moveOptions[i].Unselect();
            }
        }
    }

    void UpdateMoveDetails(PokemonMove move)
    {
        descriptionText.text = move.MoveBase.Description;
        //categoryIcon.sprite = ;
        string power = $"{move.MoveBase.BasePower}";
        if(move.MoveBase.BasePower == 1)
        {
            power = "--";
        }
        movePowerText.text = power;

        string accuracy = $"{move.MoveBase.Accuracy}";
        if(move.MoveBase.Accuracy == -1)
        {
            accuracy = "--";
        }
        moveAccuracyText.text = accuracy;
    }

    void UpdateChoiceSelection()
    {
        if(choice)
        {
            yesBox.color = GlobalSettings.Instance.SelectedBarColor;
            noBox.color = Color.clear;
        }
        else
        {
            yesBox.color = Color.clear;
            noBox.color = GlobalSettings.Instance.SelectedBarColor;
        }
    }

    IEnumerator OpenConfirmationBox()
    {
        state = ForgetMoveState.Busy;
        if(selectedMove == moveOptions.Count - 1)
        {
            yield return DialogManager.Instance.DisplayText($"Do you want to give up on having your Pokemon learn {moveOptions[selectedMove].Move.MoveBase.MoveName}?");
        }
        else
        {
            yield return DialogManager.Instance.DisplayText($"Is it OK to forget {moveOptions[selectedMove].Move.MoveBase.MoveName} in order to learn {moveOptions[moveOptions.Count-1].Move.MoveBase.MoveName}?");
        }
        UpdateChoiceSelection();
        choiceBox.SetActive(true);
        state = ForgetMoveState.Confirmation;
        yield return null;
    }
}
