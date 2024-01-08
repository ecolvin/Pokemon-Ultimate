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

    [SerializeField] float textDelay = .02f;     //Dynamically change in gameSettings eventually
    [SerializeField] float pauseDuration = 1f;

    bool typing = false;

//---------------Dialog Box Options------------------

    void OnEnable()
    {
        typing = false;
        battleText.text = "";
    }
    //-----------------Pokemon I/O---------------
    public IEnumerator WildPokemonIntro(string pokemonName)
    {
        yield return SlowText($"A wild {pokemonName} has appeared!");
    }

    //Confirmed
    public IEnumerator PlayerPokemonIntro(string pokemonName)
    {
        string [] intros = {
            $"Go, {pokemonName}!", 
            $"Go for it, {pokemonName}!", 
            $"You're in charge, {pokemonName}!"
        };
        yield return SlowText(intros[Random.Range(0, intros.Length)]);
    }    

    //Confirmed
    public IEnumerator PlayerPokemonReturn(string pokemonName)
    {
        yield return SlowText($"{pokemonName}, come back!");
    }

    //Confirmed
    public IEnumerator Fainted(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} fainted!");
    }

    //-!-UNCONFIRMED-!-
    public IEnumerator WhiteOut()
    {
        yield return SlowText("You have run out of useable pokemon.");
        yield return PauseAfterText();
        yield return SlowText("You whited out.");
    }

    public IEnumerator Run()
    {
        yield return SlowText("Got away safely!");
    }
    
    //---GET CORRECT TEXT---
    public IEnumerator RunFailure()
    {
        yield return SlowText("Failed to Run!");
    }
    //--------------------------------------------

    //--------------Move Success and Type Matchups--------------
    //Confirmed
    public IEnumerator UseMove(Pokemon pokemon, PokemonMove move)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} used {move.MoveBase.MoveName}!");
    }

    //Confirmed
    public IEnumerator CriticalHit()
    {
        yield return SlowText($"A critical hit!");
    }
    
    //Confirmed
    public IEnumerator SuperEffective()
    {
        yield return SlowText($"It's super effective!");
    }

    //Confirmed
    public IEnumerator NotVeryEffective()
    {
        yield return SlowText($"It's not very effective...");
    }

    //Confirmed
    public IEnumerator Immune(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"It doesn't affect {pokemonName}");
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
    //-----------------------------------------

    //---------------Move Effects--------------
    //Confirmed
    public IEnumerator StatIncrease(Pokemon pokemon, string stat, bool sharp)
    {
        string sharply = sharp ? "sharply " : "";
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName}'s {stat} {sharply}rose!");
    }

    //Confirmed
    public IEnumerator StatDecrease(Pokemon pokemon, string stat, bool harsh)
    {
        string harshly = harsh ? "harshly " : "";
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName}'s {stat} {harshly}fell!");
    }

    //Double Check Text
    public IEnumerator CantGoHigher(Pokemon pokemon, string stat)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName}'s {stat} can't go any higher!"); 
    }

    //Double Check Text
    public IEnumerator CantGoLower(Pokemon pokemon, string stat)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName}'s {stat} can't go any lower!");
        
    }
    
    //Update for different conditions
    public IEnumerator ReceiveCondition(Pokemon pokemon, NonVolatileStatus NVStatus)
    {      
        string conditioned = "was " + NVStatus.ToString().ToLower() + "ed";
        if(NVStatus == NonVolatileStatus.Paralysis)
        {
            conditioned = "is paralyzed, so it may be unable to move";  //Verified
        }
        if(NVStatus == NonVolatileStatus.Freeze)
        {
            conditioned = "was frozen";
        }
        if(NVStatus == NonVolatileStatus.BadlyPoisoned)
        {
            conditioned = "was badly poisoned";
        }
        if(NVStatus == NonVolatileStatus.Sleep)
        {
            conditioned = "fell asleep";
        }

        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} {conditioned}!");        
    }

    //Confirmed
    public IEnumerator PoisonDamage(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} was hurt by its poisoning!");
    }    
    
    //Confirmed
    public IEnumerator BurnDamage(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} was hurt by its burn!");
    }

    //Confirmed
    public IEnumerator FullParalysis(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} couldn't move because it's paralyzed!");
    }

    public IEnumerator SleepTurn(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} is fast asleep!");
    }

    public IEnumerator WakeUp(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} woke up!");
    }

    public IEnumerator Thaw(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} thawed out!");
    }

    //Confirmed
    public IEnumerator FreezeTurn(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} is frozen solid!");
    }
    //----------------------------------------

    //Confirmed
    public IEnumerator NaturalWeather(Weather weather)
    {
        if(weather == Weather.Sun)
        {
            yield return SlowText("The sunlight is harsh!");
        }
        else if(weather == Weather.Rain)
        {
            yield return SlowText("It's raining!");
        }
        else if(weather == Weather.Sand)
        {
            yield return SlowText("The sandstorm is raging!");
        }
        else if(weather == Weather.Snow)
        {
            yield return SlowText("It's snowing!");
        }
        else if(weather == Weather.Fog)
        {
            yield return SlowText("Mist swirls around the battlefield!");
        }
        else if(weather == Weather.HarshSun)
        {
            yield return SlowText("The sunlight is extremely harsh!");
        }
        else if(weather == Weather.HeavyRain)
        {
            yield return SlowText("It's raining heavily!");
        }
        else if(weather == Weather.Wind)
        {
            yield return SlowText("Mysterious strong winds are protecting Flying-type Pokemon!");
        }
        else
        {
            yield return SlowText("This type of weather is not currently implemented!");
        }
    }

    //Confirmed
    public IEnumerator CreateWeather(Weather weather)
    {
        if(weather == Weather.Sun)
        {
            yield return SlowText("The sunlight turned harsh!");
        }
        else if(weather == Weather.Rain)
        {
            yield return SlowText("It started to rain!");
        }
        else if(weather == Weather.Sand)
        {
            yield return SlowText("A sandstorm kicked up!");
        }
        else if(weather == Weather.Snow)
        {
            yield return SlowText("It started to snow!");
        }
        else if(weather == Weather.Fog)
        {
            yield return SlowText("Mist swirls around the battlefield!");
        }
        else if(weather == Weather.HarshSun)
        {
            yield return SlowText("The sunlight turned extremely harsh!");
        }
        else if(weather == Weather.HeavyRain)
        {
            yield return SlowText("A heavy rain began to fall!");
        }
        else if(weather == Weather.Wind)
        {
            yield return SlowText("Mysterious strong winds are protecting Flying-type Pokemon!");
        }
        else
        {
            yield return SlowText("This type of weather is not currently implemented!");
        }
    }

    //Confirmed
    public IEnumerator WeatherExpire(Weather weather)
    {
        if(weather == Weather.Sun)
        {
            yield return SlowText("The harsh sunlight faded!");
        }
        else if(weather == Weather.Rain)
        {
            yield return SlowText("The rain stopped!");
        }
        else if(weather == Weather.Sand)
        {
            yield return SlowText("The sandstorm subsided!");
        }
        else if(weather == Weather.Snow)
        {
            yield return SlowText("The snow stopped!");
        }
        else if(weather == Weather.HarshSun)
        {
            yield return SlowText("The harsh sunlight faded!");
        }
        else if(weather == Weather.HeavyRain)
        {
            yield return SlowText("The heavy rain has lifted!");
        }
        else if(weather == Weather.Wind)
        {
            yield return SlowText("The mysterious strong winds have dissipated!");
        }
        else
        {
            yield return SlowText("This type of weather is not currently implemented!");
        }
    }

    //Confirmed
    public IEnumerator CreateTerrain(Terrain terrain)
    {
        if(terrain == Terrain.Grassy)
        {
            yield return SlowText("Grass grew to cover the battlefield!");
        }
        else if(terrain == Terrain.Electric)
        {
            yield return SlowText("An electric current ran across the battlefield!");
        }
        else if(terrain == Terrain.Psychic)
        {
            yield return SlowText("The battlefield got weird!");
        }
        else if(terrain == Terrain.Misty)
        {
            yield return SlowText("Mist swirled around the battlefield!");
        }
        yield return null;
    }

    public IEnumerator TerrainExpire(Terrain terrain)
    {
        string terrainText = "terrain";
        if(terrain == Terrain.Grassy)
        {
            terrainText = "grass";
        }
        else if(terrain == Terrain.Electric)
        {
            terrainText = "electricity";
        }
        else if(terrain == Terrain.Psychic)
        {
            terrainText = "weirdness";
        }
        else if(terrain == Terrain.Misty)
        {
            terrainText = "mist";
        }
        yield return SlowText($"The {terrainText} disappeared from the battlefield!");
    }

    //Confirmed
    public IEnumerator WeatherFail(Weather weather)
    {
        if(weather == Weather.HarshSun)
        {
            yield return SlowText("The Water-type attack evaporated in the harsh sunlight!");
        }
        else if(weather == Weather.HeavyRain)
        {
            yield return SlowText("The Fire-type attack fizzled out in the heavy rain!");
        }
        else
        {            
            yield return SlowText("The weather caused the move to fail somehow.");
        }
    }

    //Confirmed
    public IEnumerator SandDamage(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} is buffeted by the sandstorm!");
    }

    public IEnumerator PokemonHealing(Pokemon pokemon)
    {
        string pokemonName = pokemon.GetBattleMessageName();
        yield return SlowText($"{pokemonName} had its HP restored.");
    }

    //---------Helper Functions------------
    public void NotImplemented()
    {
        battleText.text = "This feature has not been implemented yet.";
        battleText.enabled = true;
    }

    public IEnumerator SetText(string text)
    {
        yield return SlowText(text);
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
    //-------------------------------------

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
        UpdateMoveSelection(selection);
        moveOptions.SetActive(true);
    }

//------------------Dynamic UI Updates-------------------------

    public void UpdateMoveSelection(int selection)
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


//___ sent out ___!
//Go! _____!
//___ used ___!
//A critical hit!
//The opposing ___ was ___!  ***(poisoned, burned)***
//___ went back to ___!    ***(U-turn used)***
//Go for it, ___!
//The opposing ___ used ___!
//The opposing ___ was hurt by the Rocky Helmet!
//The opposing ___ was hurt by its poisoning!
//___ withdrew ___!     ***(Opponent switching)***
//___ sent out ___!     ***(Opponent switching)***
//It started to snow!
//___ knocked off the opposing ___'s ___!
//___, come back!
//You're in charge, ___!
//It's not very effective...
//The opposing ___'s ___ harshly fell!
//The snow stopped.
//It's super effective!
//___ fainted!
//The opposing ___ fainted!
//The opposing ___'s ___ weakened the Sp. Def of all surrounding Pokemon! ***(Chi Yu's Beads of Ruin)***
//___ made the opposing team stronger against physical and special moves! ***(Aurora Veil)***
//The opposing ___ lost some of its HP! ***(Life Orb)***
//It doesn't affect the opposing ___...
//The opposing team's ___ wore off!!! ***(Aurora Veil)***
//___'s ___ fell! ***(User stat drop)***
//The opposing ___ restored a little HP using its Leftovers!
//The opposing ___ was hurt by its burn!
//It doesn't affect ___...
//The opposing ___'s ___ fell!
//The opposing ___'s ___ rose!
//The opposing ___ became fully charged due to its bond with its Trainer!
//You defeated ___!
//The ___ Berry weakened the damage to ___!


//Grassy Terrain Healing:
//___ had its HP restored.