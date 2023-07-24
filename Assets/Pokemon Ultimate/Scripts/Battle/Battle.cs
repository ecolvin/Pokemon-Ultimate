using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState {Intro, Ready, ActionSelection, MoveSelection, Bag, Pokemon, Battle}
public enum Weather {None, Sun, HarshSun, Rain, HeavyRain, Sand, Hail, Snow, Shadow, Fog, Wind}
public enum Terrain {None, Electric, Misty, Psychic, Grassy}
public enum FieldEffect {None, Spikes, ToxicSpikes, Rocks, Webs, Reflect, LightScreen, Tailwind, LuckyChant}

public class Battle : MonoBehaviour
{
    public float[][] TYPE_CHART = 
    {
        new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f},
        new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,0.0f,1.0f,1.0f,0.5f,1.0f},
        new float[19]{1.0f,1.0f,0.5f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,1.0f,2.0f,1.0f},
        new float[19]{1.0f,1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f,1.0f},
        new float[19]{1.0f,1.0f,0.5f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,0.5f,1.0f,0.5f,1.0f},
        new float[19]{1.0f,1.0f,1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f},
        new float[19]{1.0f,1.0f,0.5f,0.5f,2.0f,1.0f,0.5f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f},
        new float[19]{1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f,0.5f,0.5f,2.0f,0.0f,1.0f,2.0f,2.0f,0.5f},
        new float[19]{1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,0.0f,2.0f},
        new float[19]{1.0f,1.0f,2.0f,1.0f,0.5f,2.0f,1.0f,1.0f,2.0f,1.0f,0.0f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f},
        new float[19]{1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,1.0f},
        new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,0.0f,0.5f,1.0f},
        new float[19]{1.0f,1.0f,0.5f,1.0f,2.0f,1.0f,1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,0.5f,1.0f,2.0f,0.5f,0.5f},
        new float[19]{1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f},
        new float[19]{1.0f,0.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f},
        new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,0.0f},
        new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f},
        new float[19]{1.0f,1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,2.0f},
        new float[19]{1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,0.5f,1.0f}
    };

    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] MainBoxController mainBox;
    [SerializeField] SideBoxController sideBox;

    //TODO: Replace with parameters
    [SerializeField] Image background;
    [SerializeField] Image enemySprite;
    [SerializeField] Image playerSprite;

    //TODO: Replace with parameters
    [SerializeField] PokemonSpecies enemySpecies;
    [SerializeField] PokemonSpecies playerSpecies;

    BattleState state;
    int curBattleOption = 0;
    int curMoveOption = 0;

    Pokemon playerPokemon;
    Pokemon enemyPokemon;

    Dictionary<FieldEffect, int> playerField = new Dictionary<FieldEffect, int>(), enemyField = new Dictionary<FieldEffect, int>();

    //--------------Battle Conditions----------------------
    Weather weather = Weather.None;
    int weatherCounter = -1;
    Terrain terrain = Terrain.None;
    int terrainCounter = -1;
    bool trickRoom = false;
    int trickRoomCounter = -1;
    bool magicRoom = false;
    int magicRoomCounter = -1;
    bool wonderRoom = false;
    int wonderRoomCounter = -1;
    bool gravity = false;
    int gravityCounter = -1;
    bool fairyLock = false;
    int fairyLockCounter = -1;
    bool ionDeluge = false; //only ever lasts 1 turn

    //Until switched out
    //mudSport - last until the user switches out
    //waterSport - last until the user switches out
    bool airLock = false; //Whether or not pokemon with Cloud Nine/Air Lock is on the field

    void Start() 
    {   
        playerPokemon = new Pokemon(playerSpecies, 5, true, false);
        enemyPokemon = new Pokemon(enemySpecies, 5, true, false);
        state = BattleState.Intro;
        InitField();
        StartCoroutine(InitBattle(playerPokemon, enemyPokemon));
    }

    void Update() 
    {
        HandleInput();
    }

    void InitField()
    {
        foreach(int i in Enum.GetValues(typeof(FieldEffect)))
        {
            FieldEffect fe = (FieldEffect) i;
            playerField.Add(fe, -1);
            enemyField.Add(fe,-1);
        }
    }

//-----------------------Battle States--------------------------

    void PlayerSelection()
    {
        state = BattleState.ActionSelection;
        mainBox.PlayerSelection();
        sideBox.PlayerSelection(curBattleOption);
    }

    void Fight()
    {
        state = BattleState.MoveSelection;
        mainBox.Fight(curMoveOption, playerPokemon);
        sideBox.Fight(playerPokemon.Moves[curMoveOption]);
    }

    void Bag()
    {
        mainBox.NotImplemented();
    }

    void Pokemon()
    {
        mainBox.NotImplemented();
    }

    void Run()
    {
        mainBox.NotImplemented();
    }

//----------------------Timed Coroutines (and helper functions)------------------------

    public IEnumerator InitBattle(Pokemon playerPokemon, Pokemon enemyPokemon)
    {
        playerSprite.sprite = playerPokemon.Sprite;
        enemySprite.sprite = enemyPokemon.Sprite;
        enemyHUD.GenerateBar(enemyPokemon);
        playerHUD.GenerateBar(playerPokemon);

        yield return mainBox.WildPokemonIntro(enemyPokemon.Species.SpeciesName);
        yield return mainBox.PauseAfterText();
        state = BattleState.Ready;
        enemyHUD.gameObject.SetActive(true);
        yield return mainBox.PlayerPokemonIntro(playerPokemon.Species.SpeciesName);
        playerSprite.gameObject.SetActive(true);
        yield return mainBox.PauseAfterText();
        playerHUD.gameObject.SetActive(true);
        PlayerSelection();
    }

    IEnumerator ChooseMove(PokemonMove move)
    {
        state = BattleState.Battle;

        PokemonMove enemyMove = getEnemyMove();
        int playerPriority = move.MoveBase.Priority;
        int enemyPriority = enemyMove.MoveBase.Priority;
        int playerSpeed = GetEffectiveSpeed(playerPokemon, playerField[FieldEffect.Tailwind] > 0);
        int enemySpeed = GetEffectiveSpeed(enemyPokemon, enemyField[FieldEffect.Tailwind] > 0);       
        
        if(playerPriority > enemyPriority)
        {
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
            yield return UseMove(move, enemyPokemon, playerPokemon, false);
        }
        else if(playerPriority < enemyPriority)
        {
            yield return UseMove(move, enemyPokemon, playerPokemon, false);
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
        }
        else if((playerSpeed > enemySpeed && !trickRoom) || (playerSpeed < enemySpeed && trickRoom))
        {
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
            yield return UseMove(move, enemyPokemon, playerPokemon, false);
        }
        else if((playerSpeed < enemySpeed && !trickRoom) || (playerSpeed > enemySpeed && trickRoom))
        {
            yield return UseMove(move, enemyPokemon, playerPokemon, false);
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
        }
        else if(UnityEngine.Random.Range(0,2) == 0)
        {            
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
            yield return UseMove(move, enemyPokemon, playerPokemon, false);
        }
        else
        {
            yield return UseMove(move, enemyPokemon, playerPokemon, false);
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
        }

        yield return EndRound();

        PlayerSelection();
    }

    int GetEffectiveSpeed(Pokemon pokemon, bool tailwind)
    {
        int speed = pokemon.Stats[5];
        if(tailwind)
        {
            speed *= 2;
        }
        if(pokemon.Status == NonVolatileStatus.Paralysis)
        {
            speed /= 2;
        }
        return speed;
    }

    IEnumerator UseMove(PokemonMove move, Pokemon attacker, Pokemon defender, bool isPlayerAttacking)
    {
        yield return mainBox.UseMove(attacker, move);
        yield return mainBox.PauseAfterText();

        bool fail = false;
        bool miss = false;
        bool immune = false;
        //Accuracy check
        //Immunity check
        if(move.MoveBase.Category != MoveCategory.Status)
        {
            //Refactor to make CalculateDamage an IEnumerator
            yield return CalculateDamage(move, attacker, defender, isPlayerAttacking);
        }
        yield return CalculateEffects(move, attacker, defender, isPlayerAttacking);
        //yield return CalculateOther()          -Thinks like contact for abilities and whatnot
    }

    IEnumerator CalculateEffects(PokemonMove move, Pokemon attacker, Pokemon defender, bool isPlayerAttacking)
    {
        yield return mainBox.HandlingEffects();
        yield return mainBox.PauseAfterText();

    }

    IEnumerator UseItem(string itemName)
    {
        yield return null;
    }

    IEnumerator SwapPokemon(Pokemon newPokemon)
    {
        yield return null;
    }

    IEnumerator EndRound()
    {
        if(weather != Weather.None)
        {
            weatherCounter--;
            if(weatherCounter < 0)
            {
                yield return mainBox.WeatherExpire(weather);
                yield return mainBox.PauseAfterText();
                weather = Weather.None;
            }
        }
        if(terrain != Terrain.None)
        {
            terrainCounter--;
            if(terrainCounter < 0)
            {
                yield return mainBox.TerrainExpire(terrain);
                yield return mainBox.PauseAfterText();
                terrain = Terrain.None;
            }
        }
        if(trickRoom)
        {
            trickRoomCounter--;
            if(trickRoomCounter < 0)
            {
                //yield return mainBox.Expire();
                trickRoom = false;
            }
        }
        if(magicRoom)
        {
            magicRoomCounter--;
            if(magicRoomCounter < 0)
            {
                //yield return mainBox.Expire();
                magicRoom = false;
            }
        }
        if(wonderRoom)
        {
            wonderRoomCounter--;
            if(wonderRoomCounter < 0)
            {
                //yield return mainBox.Expire();
                wonderRoom = false;
            }
        }
        if(gravity)
        {
            gravityCounter--;
            if(gravityCounter < 0)
            {
                //yield return mainBox.Expire();
                gravity = false;
            }
        }
        if(fairyLock)
        {
            fairyLockCounter--;
            if(fairyLockCounter < 0)
            {
                //yield return mainBox.Expire();
                fairyLock = false;
            }
        }
        if(ionDeluge)
        {
            //yield return mainBox.Expire();
            ionDeluge = false;
        }


        foreach(KeyValuePair<FieldEffect, int> effect in playerField)
        {
            //Decrement the values of the field effects that have a duration (screens, tailwind, lucky chant, etc...)    
        }
        foreach(KeyValuePair<FieldEffect, int> effect in enemyField)
        {

        }
    }

//----------------------Damage Calculations-----------------------

    IEnumerator CalculateDamage(PokemonMove move, Pokemon attacker, Pokemon defender, bool isPlayerAttacking)
    {
        PokemonType moveType = move.MoveBase.MoveType;
        int level = attacker.Level;
        int power = move.MoveBase.BasePower;
        int A = 0;
        int D = 1;
        if(move.MoveBase.Category == MoveCategory.Physical)
        {
            A = attacker.Stats[1]; //Physical Attack
            D = defender.Stats[2];
        }
        else if(move.MoveBase.Category == MoveCategory.Special)
        {
            A = attacker.Stats[3]; //Special Attack
            D = defender.Stats[4];
        }
        
        //Calculate variable power moves

        //Calculate moves that use other stats for attack/defense

        //Calculate crit (ignoring stat changes)
        
        float critical = 1f;
        bool crit = DetermineCrit(move, attacker, defender, isPlayerAttacking);
        if(crit)
        {
            critical = 1.5f;
            //ignore stat changes
            //figure out how to print the critical hit message
        }      

        //Calclate variable move type (HP, Tera Blast, Weather Ball, etc...)

        //-----------------Modifiers-------------------------------
        float targets = 1f;    //Implement with double/triple battles

        float pb = 1f;         //Implement with Parental Bond ability

        float weather = CalculateWeatherBoost(move, moveType);
        if(weather == 0f)
        {
            //return -1;
            yield return mainBox.Failed();
            yield return mainBox.PauseAfterText();
            yield break;
        }

        float glaiveRush = 1f; //Implement with Glaive Rush Attack\

        float random = (UnityEngine.Random.Range(85,101))/100f;
        Debug.Log($"Random factor: {random}");

        float stab = CalculateStab(moveType, attacker);

        float type = CalculateTypeAdvantage(moveType, move, defender, attacker);
        if(type == 0f)
        {
            yield return mainBox.Immune();
            yield return mainBox.PauseAfterText();
            yield break;
        }

        float burn = 1f;
        if(attacker.Status == NonVolatileStatus.Burn && attacker.Ability != "Guts" && move.MoveBase.Category == MoveCategory.Physical && move.MoveBase.MoveName != "Facade")
        {
            burn = 0.5f;
        }

        float other = 1f; //CalculateOther(move, attacker, defender);
        float zMove = 1f;
        float teraShield = 1f;


        //-----------------Formula---------------------------

        int levelCalc = Mathf.FloorToInt((2*level)/5) + 2;
        int top = Mathf.FloorToInt(levelCalc*power*A/D);
        int baseDamage = Mathf.FloorToInt((top/50)+2);

        int damage = Round(baseDamage * targets);
        damage = Round(damage * pb);
        damage = Round(damage * weather);
        damage = Round(damage * glaiveRush);
        damage = Round(damage * critical);
        damage = Round(damage * random);
        damage = Round(damage * stab);
        damage = Round(damage * type);
        damage = Round(damage * burn);
        damage = Round(damage * other);
        damage = Round(damage * zMove);
        damage = Round(damage * teraShield);

        if(damage == 0)
        {
            damage = 1;
        }
        
        bool ko = false;

        if(isPlayerAttacking)
        {
            int startHP = enemyPokemon.CurHP;
            ko = enemyPokemon.TakeDamage(damage);
            yield return enemyHUD.UpdateHP();
        }
        else
        {
            int startHP = playerPokemon.CurHP;
            ko = playerPokemon.TakeDamage(damage);
            yield return playerHUD.UpdateHP();
        }

        yield return mainBox.PauseAfterText();    //Pause after damage is dealt

        if(crit)
        {
            yield return mainBox.CriticalHit();
            yield return mainBox.PauseAfterText();
        }

        if(type > 1f)
        {
            yield return mainBox.SuperEffective();
            yield return mainBox.PauseAfterText();
        }
        else if(type < 1f)
        {
            yield return mainBox.NotVeryEffective();
            yield return mainBox.PauseAfterText();
        }       

        if(ko)
        {
            if(isPlayerAttacking)
            {

            }
            else
            {

            }
        } 
    }

    int Round(float f)  //Round to nearest with .5 rounding down
    {
        if(f < 0)
        {
            return (int) Math.Floor(f + 0.5f);
        }
        return (int) Math.Ceiling(f - 0.5f);
    }

    bool DetermineCrit(PokemonMove move, Pokemon att, Pokemon def, bool isPlayerAttacking)
    {        
        if(def.Ability == "Battle Armor" || def.Ability == "Shell Armor") //Implement Lucky Chant
        {
            return false;
        }
        if(att.LaserFocus || att.Ability == "Merciless" && (def.Status == NonVolatileStatus.Poison || def.Status == NonVolatileStatus.BadlyPoisoned))
        {
            return true;
        }
        if(isPlayerAttacking)
        {
            if(enemyField[FieldEffect.LuckyChant] > 0)
            {
                return false;
            }
        }
        else
        {
            if(playerField[FieldEffect.LuckyChant] > 0)
            {
                return false;
            }
        }

        //TODO: Implement other items/other abilities/Spacial Rend by Origin Forme Palkia

        float critChance = -1f;
        if(move.MoveBase.CritRate == 0)
        {
            critChance = 1f/24f;
        }
        else if(move.MoveBase.CritRate == 1)
        {
            critChance = 1f/8f;
        }
        else if(move.MoveBase.CritRate == 2)
        {
            critChance = 1f/2f;
        }
        else if(move.MoveBase.CritRate >= 3)
        {
            return true;
        }

        System.Random r = new System.Random();
        return r.NextDouble() < critChance;
    }

    float CalculateWeatherBoost(PokemonMove move, PokemonType type)
    {
        if(airLock)
        {
            return 1f;
        }
        if(weather == Weather.Sun)
        {
            if(type == PokemonType.Fire || move.MoveBase.MoveName == "Hydro Steam")
            {
                return 1.5f;
            }
            if(type == PokemonType.Water)
            {
                return 0.5f;
            }
        }
        if(weather == Weather.HarshSun)
        {
            if(type == PokemonType.Water)
            {
                return 0f;
            }
        }
        if(weather == Weather.Rain)
        {

        }
        if(weather == Weather.HeavyRain)
        {

        }
        return 1f;        
    }

    float CalculateStab(PokemonType type, Pokemon pokemon)
    {
        PokemonType t1 = pokemon.Species.Type1;
        PokemonType t2 = pokemon.Species.Type2;
        PokemonType tt = pokemon.TeraType;
        bool tera = pokemon.IsTera;
        if(tera)
        {
            if(tt == t1 || tt == t2)
            {
                if(tt == type)
                {
                    if(pokemon.Ability == "Adaptability" && !pokemon.NGas)
                    {
                        return 2.25f;
                    }
                    else
                    {
                        return 2f;
                    }
                }
            }
            else if (tt == type)
            {
                if(pokemon.Ability == "Adaptability" && !pokemon.NGas)
                {
                    return 2f;
                }
                else
                {
                    return 1.5f;
                }
            }
        }
        if(t1 == type || t2 == type)
        {
            if(pokemon.Ability == "Adaptability" && !pokemon.NGas)
            {
                return 2f;
            }
            else
            {
                return 1.5f;
            }
        }
        return 1f;
    }

    float CalculateTypeAdvantage(PokemonType moveType, PokemonMove move, Pokemon defender, Pokemon attacker)
    {
        PokemonType t1 = defender.Species.Type1;
        PokemonType t2 = defender.Species.Type2;
        if(defender.Soak)
        {
            t1 = PokemonType.Water;
            t2 = PokemonType.None;
        }
        if(defender.IsTera)
        {
            t1 = defender.TeraType;
            t2 = PokemonType.None;
        }        
        if(t1 == PokemonType.None && t2 == PokemonType.None)
        {
            Debug.Log($"{defender.Nickname} does not have a type.");
            return 1f;
        }

        float typeAdv = 1f;
        if(t1 != PokemonType.None)
        {
            CheckExceptions(move, moveType, t1, defender, attacker);
            typeAdv *= TYPE_CHART[(int) moveType][(int) t1];
        }
        if(t2 != PokemonType.None)
        {
            typeAdv *= TYPE_CHART[(int) moveType][(int) t2];
        }
        if(defender.TrickOrTreat)
        {
            typeAdv *= TYPE_CHART[(int) moveType][(int) PokemonType.Ghost];
        }   
        else if(defender.ForestsCurse)
        {
            typeAdv *= TYPE_CHART[(int) moveType][(int) PokemonType.Ghost];
        }
        return typeAdv;
    }

    float CheckExceptions(PokemonMove move, PokemonType moveType, PokemonType type, Pokemon defender, Pokemon attacker)
    {
        if(moveType == PokemonType.Ground && type == PokemonType.Flying)
        {
            if(move.MoveBase.MoveName == "Thousand Arrows" || defender.VolatileStatus[VolatileStatus.Grounded] >= 0 || gravity)
            {
                return 1f;
            }
        }
        if(defender.HeldItem == "Ring Target" && TYPE_CHART[(int) moveType][(int) type] == 0.0f)
        {
            return 1f;
        }
        if(type == PokemonType.Ghost && attacker.Ability == "Scrappy" && (moveType == PokemonType.Normal || moveType == PokemonType.Fighting))
        {
            return 1f;
        }
        if(move.MoveBase.MoveName == "Freeze-Dry" && type == PokemonType.Water)
        {
            return 2f;
        }
        if(move.MoveBase.MoveName == "Flying Press")
        {
            return TYPE_CHART[(int)moveType][(int)type] * TYPE_CHART[(int)PokemonType.Flying][(int)type];
        }
        if(type == PokemonType.Flying && weather == Weather.Wind && TYPE_CHART[(int)moveType][(int)type] > 1f)
        {
            return 1f;
        }
        if(defender.TarShot && moveType == PokemonType.Fire)
        {
            return 2f;
        }
        return 0f;
    }


//------------------------Battle Functions--------------------------------------

    PokemonMove getEnemyMove()
    {
        //Random Algorithm
        int numMoves = 0;
        foreach(PokemonMove move in enemyPokemon.Moves)
        {
            if(move != null)
            {
                numMoves++;
            }
        }
        return enemyPokemon.Moves[UnityEngine.Random.Range(0, numMoves)];
    }

//---------------------------Input Handling---------------------------------------    

    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if(curBattleOption < 2 && state == BattleState.ActionSelection)
            {
                curBattleOption+=2;
            }
            if(curMoveOption < 2 && state == BattleState.MoveSelection)
            {
                curMoveOption+=2;
            }
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if(curBattleOption > 1 && state == BattleState.ActionSelection)
            {
                curBattleOption-=2;
            }
            if(curMoveOption > 1 && state == BattleState.MoveSelection)
            {
                curMoveOption-=2;
            }
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if(curBattleOption % 2 == 0 && state == BattleState.ActionSelection)
            {
                curBattleOption++;
            }
            if(curMoveOption % 2 == 0 && state == BattleState.MoveSelection)
            {
                curMoveOption++;
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if(curBattleOption % 2 == 1 && state == BattleState.ActionSelection)
            {
                curBattleOption--;
            }
            if(curMoveOption % 2 == 1 && state == BattleState.MoveSelection)
            {
                curMoveOption--;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if(state == BattleState.ActionSelection)
            {
                if(curBattleOption == 0)
                {
                    int totalPP = 0;
                    foreach(PokemonMove move in playerPokemon.Moves)
                    {
                        if(move != null)
                        {
                            totalPP += move.CurPP;
                        }
                    }
                    if(totalPP > 0)
                    {
                        Fight();
                    }
                    else
                    {
                        //mainBox.SetText("Struggle!");
                    }
                }
                else if(curBattleOption == 1)
                {
                    Bag();
                }
                else if(curBattleOption == 2)
                {
                    Pokemon();
                }
                else if(curBattleOption == 3)
                {
                    Run();    
                }
            }
            else if(state == BattleState.MoveSelection)
            {
                if(playerPokemon.Moves[curMoveOption] != null && playerPokemon.Moves[curMoveOption].CurPP > 0)
                {
                    playerPokemon.Moves[curMoveOption].DecrementPP(false);
                    StartCoroutine(ChooseMove(playerPokemon.Moves[curMoveOption]));
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(state == BattleState.ActionSelection)
            {
                curBattleOption = 3;
            }
            else if(state == BattleState.MoveSelection)
            {
                PlayerSelection();
            }
        }

        if(state == BattleState.ActionSelection)
        {
            sideBox.updateBattleSelection(curBattleOption);
        }
        if(state == BattleState.MoveSelection)
        {
            mainBox.updateMoveSelection(curMoveOption);
            sideBox.updateMoveDetails(playerPokemon.Moves[curMoveOption]);
        }
    }
}


/*
Fully Implemented:
-Move order calculation (tailwind, trick room, paralysis, speed ties, and priority all taken into account)
-Adaptability in STAB calculations (May need to be updated with the ability rework in the future)

Mostly Implemented:
-Damage Calculation (the function has more comments for what is needed) 
---(also refactor into an IEnumerator that outputs messages based on effectiveness/failure/crits/etc...)
---Have the pokemon take damage at the end of the function instead of back in the previous function

TODO:
-Accuracy/Failure checks
-Move effects (stat changes, status conditions)
-Implement full weather/terrain effects
-Many corner cases for everything
*/