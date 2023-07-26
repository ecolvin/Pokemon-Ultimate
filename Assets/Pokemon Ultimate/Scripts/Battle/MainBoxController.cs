using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainBoxController : MonoBehaviour
{
    [SerializeField] Color selectionColor;
    [SerializeField] Color defaultColor;

    [SerializeField] SideBoxController sideBox;

    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] GameObject moveOptions;
    
    [SerializeField] List<TextMeshProUGUI> moveOptionText;

    [SerializeField] float textDelay = .04f;     //Dynamically change in gameSettings eventually
    [SerializeField] float pauseDuration = 1f;

    bool typing = false;

//---------------Dialog Box Options------------------

    void OnEnable()
    {
        typing = false;
        battleText.text = "";
    }

    public IEnumerator WildPokemonIntro(string pokemonName)
    {
        yield return SlowText($"A wild {pokemonName} has appeared!");
    }

    public IEnumerator PlayerPokemonIntro(string pokemonName)  //Randomize message???
    {
        yield return SlowText($"{pokemonName}, I choose you!!!");
    }    

    public IEnumerator PlayerPokemonReturn(string pokemonName)
    {
        yield return SlowText($"{pokemonName}, return!");
    }

    public IEnumerator Fainted(string pokemonName)
    {
        yield return SlowText($"{pokemonName} fainted...");   //Get exact text
    }

    public IEnumerator WhiteOut()
    {
        yield return SlowText("You have run out of useable pokemon.");
        yield return PauseAfterText();
        yield return SlowText("You whited out.");
    }

    public IEnumerator UseMove(Pokemon pokemon, PokemonMove move)
    {
        yield return SlowText($"{pokemon.Nickname} used {move.MoveBase.MoveName}");
    }

    public IEnumerator CriticalHit()
    {
        yield return SlowText($"A critical hit!");
    }
    
    public IEnumerator SuperEffective()
    {
        yield return SlowText($"It's super effective!");
    }

    public IEnumerator NotVeryEffective()
    {
        yield return SlowText($"It's not very effective...");
    }

    public IEnumerator Immune()
    {
        yield return SlowText($"But it had no effect...");    //Get exact text
    }

    public IEnumerator Failed()
    {
        yield return SlowText($"But it failed...");    //Get exact text
    }

    public IEnumerator Missed()
    {
        yield return SlowText($"But the attack missed...");   //Get exact text
    }

    public IEnumerator OHKO()
    {
        yield return SlowText($"It's a one-hit KO!");
    }
        
    public IEnumerator WeatherExpire(Weather weather)
    {
        yield return SlowText($"The {weather} has worn off.");
    }

    public IEnumerator TerrainExpire(Terrain terrain)
    {
        yield return SlowText($"The {terrain} has worn off.");
    }

    public IEnumerator HandlingEffects()
    {
        yield return SlowText($"Handling Effects Now!");
    }

    public IEnumerator Run()
    {
        yield return SlowText("Got away safely!");
    }

    public void NotImplemented()
    {
        battleText.text = "This feature has not been implemented yet.";
        battleText.enabled = true;
    }

    public IEnumerator SetText(string text)
    {
        yield return StartCoroutine(SlowText(text));
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

    IEnumerator SlowText(string text)
    {
        if(typing)
        {
            yield break;
        }
        typing = true;  
        moveOptions.SetActive(false);
        battleText.text = "";
        battleText.enabled = true;
        foreach(char letter in text)
        {
            battleText.text += letter;
            yield return new WaitForSeconds(textDelay);
        }
        typing = false;
    }

//----------Battle States--------------    

    public void PlayerSelection()
    {
        battleText.enabled = false;
        moveOptions.SetActive(false);
    }

    public void Fight(int selection, Pokemon pokemon)
    {
        battleText.enabled = false;
        UpdateMoveNames(pokemon.Moves);
        updateMoveSelection(selection);
        moveOptions.SetActive(true);
    }

//------------------Dynamic UI Updates-------------------------

    public void updateMoveSelection(int selection)
    {
        int i = 0;
        foreach(TextMeshProUGUI option in moveOptionText)
        {
            if(i == selection)
            {
                option.color = selectionColor;
            }
            else
            {
                option.color = defaultColor;
            }
            i++;
        }
    }

    public void UpdateMoveNames(PokemonMove[] moves)
    {
        for(int i = 0; i < 4; i++)
        {
            if(moves[i] != null)
            {
                moveOptionText[i].text = moves[i].MoveBase.MoveName;
            }
            else
            {
                moveOptionText[i].text = "-----";
            }
        }
    }
}
