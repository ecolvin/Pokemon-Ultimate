using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState {Intro, ActionSelection, MoveSelection, Bag, Pokemon, Switching, Battle}
public enum Weather {None, Sun, HarshSun, Rain, HeavyRain, Sand, Hail, Snow, Shadow, Fog, Wind}
public enum Terrain {None, Electric, Misty, Psychic, Grassy}
public enum FieldEffect {None, Spikes, ToxicSpikes, Rocks, Webs, Reflect, LightScreen, Tailwind, LuckyChant}

public class Battle : MonoBehaviour
{
    [SerializeField] Party player;

    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] MainBoxController mainBox;
    [SerializeField] SideBoxController sideBox;
    [SerializeField] PartyScreen partyScreen;


    //TODO: Replace with parameters
    [SerializeField] Image background;
    [SerializeField] BattleSprite enemySprite;
    [SerializeField] BattleSprite playerSprite;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int curBattleOption = 0;
    int curMoveOption = 0;

    int fleeAttempts = 0;

    bool switchBecauseFainted = false;

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
        partyScreen.OnClose += ClosePokemon;
    }

    public void StartBattle(Pokemon wildPokemon)
    {   
        playerPokemon = player.GetLeadPokemon();
        if(playerPokemon == null)
        {
            Debug.Log("The player is out of useable pokemon!");
            return;
        }
        enemyPokemon = wildPokemon;
        state = BattleState.Intro;
        InitField();
        StartCoroutine(InitBattle(playerPokemon, enemyPokemon));
    }

    //public void StartTrainerBattle(Trainer trainer){}

    public void HandleUpdate()
    {
        HandleInput();
    }

    void InitField()
    {
        foreach(int i in Enum.GetValues(typeof(FieldEffect)))
        {
            FieldEffect fe = (FieldEffect) i;
            if(!playerField.ContainsKey(fe))
            {
                playerField.Add(fe, -1);
            }
            else
            {
                playerField[fe] = -1;
            }

            if(!enemyField.ContainsKey(fe))
            {
                enemyField.Add(fe, -1);
            }
            else
            {
                enemyField[fe] = -1;
            }
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
        state = BattleState.Pokemon;
        partyScreen.SetPartyMembers(player.PartyPokemon);
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePokemon(Pokemon pokemon)
    {
        if(switchBecauseFainted)
        {
            if(pokemon != null && !pokemon.Fainted)
            {
                partyScreen.gameObject.SetActive(false);
                StartCoroutine(SwitchPokemon(pokemon, true));
                switchBecauseFainted = false;
            }
        }
        else
        {
            if(pokemon == null)
            {
                partyScreen.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            }
            else if(!pokemon.Fainted)
            {
                partyScreen.gameObject.SetActive(false);
                StartCoroutine(SwitchPokemon(pokemon, false));
            }
        }
    }

    void Run()
    {
        StartCoroutine(RunAttempt());
    }

//----------------------Timed Coroutines (and helper functions)------------------------

    public IEnumerator InitBattle(Pokemon playerPokemon, Pokemon enemyPokemon)
    {
        sideBox.Clear();
        
        playerSprite.Set(playerPokemon);
        enemySprite.Set(enemyPokemon);
        playerPokemon.IsActive = true;
        
        partyScreen.Init();

        StartCoroutine(enemySprite.Entrance());
        yield return mainBox.WildPokemonIntro(enemyPokemon.Species.SpeciesName);
        yield return mainBox.PauseAfterText();

        enemyHUD.GenerateBar(enemyPokemon);
        enemyHUD.gameObject.SetActive(true);
        StartCoroutine(playerSprite.Entrance());
        yield return mainBox.PlayerPokemonIntro(playerPokemon.Species.SpeciesName);
        yield return mainBox.PauseAfterText();

        playerHUD.GenerateBar(playerPokemon);
        playerHUD.gameObject.SetActive(true);
        PlayerSelection();
    }

    IEnumerator ChooseMove(PokemonMove move)
    {
        sideBox.Clear();
        state = BattleState.Battle;

        PokemonMove enemyMove = getEnemyMove();
        int playerPriority = move.MoveBase.Priority;
        int enemyPriority = enemyMove.MoveBase.Priority;
        int playerSpeed = GetEffectiveSpeed(playerPokemon, playerField[FieldEffect.Tailwind] > 0);
        int enemySpeed = GetEffectiveSpeed(enemyPokemon, enemyField[FieldEffect.Tailwind] > 0);       
        
        bool playerFirst = false;

        if(playerPriority > enemyPriority)
        {
            playerFirst = true;
        }
        else if(playerPriority < enemyPriority)
        {
            playerFirst = false;
        }
        else if((playerSpeed > enemySpeed && !trickRoom) || (playerSpeed < enemySpeed && trickRoom))
        {
            playerFirst = true;
        }
        else if((playerSpeed < enemySpeed && !trickRoom) || (playerSpeed > enemySpeed && trickRoom))
        {
            playerFirst = false;
        }
        else if(UnityEngine.Random.Range(0,2) == 0)
        {            
            playerFirst = true;
        }
        else
        {
            playerFirst = false;
        }

        if(playerFirst)
        {
            yield return UseMove(move, playerPokemon, enemyPokemon, true);
            yield return UseMove(enemyMove, enemyPokemon, playerPokemon, false);
        }
        else
        {
            yield return UseMove(enemyMove, enemyPokemon, playerPokemon, false);
            if(!switchBecauseFainted)
            {
                yield return UseMove(move, playerPokemon, enemyPokemon, true);
            }
        }

        yield return EndRound();

        if(switchBecauseFainted)
        {
            if(player.GetLeadPokemon() == null)
            {
                yield return mainBox.WhiteOut();
                yield return mainBox.PauseAfterText();
                CleanupBattle();
                OnBattleOver(false);
            }
            Pokemon();
        }
        else
        {
            PlayerSelection();
        }
    }

    int GetEffectiveSpeed(Pokemon pokemon, bool tailwind)
    {
        int speed = pokemon.Stats.Spe;
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
        if(attacker.Fainted)
        {
            yield break;
        }

        yield return mainBox.UseMove(attacker, move);
        yield return mainBox.PauseAfterText();

        //Failure check
        //Accuracy check

        float type = CalculateTypeAdvantage(move.MoveBase.MoveType, move, defender, attacker);
        Debug.Log($"Move: {move.MoveBase.MoveName}; Type: {move.MoveBase.MoveType}; Defender Types: {defender.Species.Type1}/{defender.Species.Type2}; Effectiveness: {type}");        
        if(type == 0f)
        {
            yield return mainBox.Immune();
            yield return mainBox.PauseAfterText();
            yield break;
        }

        if(isPlayerAttacking)
        {
            yield return playerSprite.Attack();
            yield return playerSprite.Attack();
            yield return enemySprite.Hit();
            yield return enemySprite.Hit();
        }
        else
        {
            yield return enemySprite.Attack();
            yield return enemySprite.Attack();
            yield return playerSprite.Hit();
            yield return playerSprite.Hit();
        }

        if(move.MoveBase.Category != MoveCategory.Status)
        {
            yield return CalculateDamage(move, attacker, defender, isPlayerAttacking);
        }
        yield return CalculateEffects(move, attacker, defender, isPlayerAttacking);
        //yield return CalculateOther()          -Thinks like contact for abilities and whatnot

        if(defender.Fainted)
        {
            if(isPlayerAttacking)
            {
                yield return enemySprite.Faint();
                yield return mainBox.Fainted(enemyPokemon.Nickname);
                yield return mainBox.PauseAfterText();
                CleanupBattle();
                OnBattleOver(true);
            }
            else
            {
                yield return playerSprite.Faint();
                yield return mainBox.Fainted(playerPokemon.Nickname);
                yield return mainBox.PauseAfterText();
                switchBecauseFainted = true;
            }
        } 
    }

    IEnumerator CalculateEffects(PokemonMove move, Pokemon attacker, Pokemon defender, bool isPlayerAttacking)
    {
        if(!defender.Fainted)
        {
            yield return mainBox.HandlingEffects();
            yield return mainBox.PauseAfterText();
        }
    }

    IEnumerator UseItem(string itemName)
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

    IEnumerator EnemySoloTurn()
    {
        state = BattleState.Battle;
        PokemonMove enemyMove = getEnemyMove();
        yield return UseMove(enemyMove, enemyPokemon, playerPokemon, false);        
        yield return EndRound();

        if(switchBecauseFainted)
        {            
            if(player.GetLeadPokemon() == null)
            {
                yield return mainBox.WhiteOut();
                yield return mainBox.PauseAfterText();
                CleanupBattle();
                OnBattleOver(false);
            }
            Pokemon();
        }
        else
        {
            PlayerSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon pokemon, bool isFree)
    {
        state = BattleState.Switching;

        sideBox.Clear();

        playerHUD.gameObject.SetActive(false);
        if(!playerPokemon.Fainted)
        {
            yield return mainBox.PlayerPokemonReturn(playerPokemon.Nickname);
            yield return playerSprite.Return();
            yield return mainBox.PauseAfterText();
        }

        playerPokemon.IsActive = false;
        playerPokemon = pokemon;
        playerPokemon.IsActive = true;

        playerSprite.Set(playerPokemon);
        yield return mainBox.PlayerPokemonIntro(playerPokemon.Species.SpeciesName);
        yield return playerSprite.Entrance();
        yield return mainBox.PauseAfterText();

        playerHUD.GenerateBar(playerPokemon);
        playerHUD.gameObject.SetActive(true);
    
        if(!isFree)
        {       
            yield return EnemySoloTurn();
        }
        else
        {     
            PlayerSelection();
        }
    }

    IEnumerator RunAttempt()
    {
        if(playerPokemon.Species.Type1 == PokemonType.Ghost || playerPokemon.Species.Type1 == PokemonType.Ghost)
        {
            yield return RunSuccess();
        }
        if(enemyPokemon.Ability == "Arena Trap")
        {
            //yield return mainBox.FleePrevention()
            yield return mainBox.PauseAfterText();
        }

        if(playerPokemon.Stats.Spe >= enemyPokemon.Stats.Spe)
        {
            yield return RunSuccess();
        }
        else
        {
            //ToDo: Rework to check if run is successful or not
            yield return RunSuccess();
            yield return EnemySoloTurn();
        }        
    }

    IEnumerator RunSuccess()
    {
        yield return mainBox.Run();
        yield return mainBox.PauseAfterText();
        CleanupBattle();
        OnBattleOver(false);
        yield return null;
    }

    IEnumerator RunFailure()
    {
        yield return null;
    }

    void CleanupBattle()
    {
        foreach(int i in Enum.GetValues(typeof(FieldEffect)))
        {
            FieldEffect fe = (FieldEffect) i;
            playerField[fe] = -1;
            enemyField[fe] = -1;
        }
        weather = Weather.None;
        weatherCounter = -1;
        terrain = Terrain.None;
        terrainCounter = -1;
        trickRoom = false;
        trickRoomCounter = -1;
        magicRoom = false;
        magicRoomCounter = -1;
        wonderRoom = false;
        wonderRoomCounter = -1;
        gravity = false;
        gravityCounter = -1;
        fairyLock = false;
        fairyLockCounter = -1;
        ionDeluge = false;

        fleeAttempts = 0;

        playerHUD.gameObject.SetActive(false);
        enemyHUD.gameObject.SetActive(false);

        enemyPokemon.IsActive = false;
        playerPokemon.IsActive = false;
    }
//----------------------Damage Calculations-----------------------

    IEnumerator CalculateDamage(PokemonMove move, Pokemon attacker, Pokemon defender, bool isPlayerAttacking)
    {   
        Debug.Log($"Attacker is {attacker.Nickname}");
        attacker.Stats.print();
        attacker.Ivs.print();
        Debug.Log("-------------");
        Debug.Log($"Defender is {defender.Nickname}");
        defender.Stats.print();
        defender.Ivs.print();

        PokemonType moveType = move.MoveBase.MoveType;
        int level = attacker.Level;
        int power = move.MoveBase.BasePower;
        int A = 0;
        int D = 1;
        if(move.MoveBase.Category == MoveCategory.Physical)
        {
            A = attacker.Stats.Atk; //Physical Attack
            D = defender.Stats.Def;
        }
        else if(move.MoveBase.Category == MoveCategory.Special)
        {
            A = attacker.Stats.SpA; //Special Attack
            D = defender.Stats.SpD;
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

        Debug.Log($"Level = {level}; Power = {power}; A = {A}; D = {D}; Crit = {critical}");

        //-----------------Modifiers-------------------------------
        float targets = 1f;    //Implement with double/triple battles

        float pb = 1f;         //Implement with Parental Bond ability

        float weather = CalculateWeatherBoost(move, moveType);
        if(weather == 0f)
        {
            yield return mainBox.Failed();
            yield return mainBox.PauseAfterText();
            yield break;
        }

        float glaiveRush = 1f; //Implement with Glaive Rush Attack\

        float random = (UnityEngine.Random.Range(85,101))/100f;

        float stab = CalculateStab(moveType, attacker);

        float type = CalculateTypeAdvantage(moveType, move, defender, attacker);

        float burn = 1f;
        if(attacker.Status == NonVolatileStatus.Burn && attacker.Ability != "Guts" && move.MoveBase.Category == MoveCategory.Physical && move.MoveBase.MoveName != "Facade")
        {
            burn = 0.5f;
        }

        float other = 1f; //CalculateOther(move, attacker, defender);
        float zMove = 1f;
        float teraShield = 1f;

        Debug.Log($"Targets: {targets}; PB: {pb}; Weather: {weather}; Glaive Rush: {glaiveRush}; Random: {random}; STAB: {stab}; Type: {type}; Burn: {burn}; Other: {other}; Zmove: {zMove}; Tera Shield: {teraShield}");

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

        Debug.Log($"Level Calc: {levelCalc}; Top: {top}; Base Damage: {baseDamage}; Damage: {damage}");
    
        if(isPlayerAttacking)
        {
            int startHP = enemyPokemon.CurHP;
            enemyPokemon.TakeDamage(damage);
            yield return enemyHUD.UpdateHP();
        }
        else
        {
            int startHP = playerPokemon.CurHP;
            playerPokemon.TakeDamage(damage);
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
            typeAdv *= TypeChart.GetTypeEffectiveness(moveType, t1);
        }
        if(t2 != PokemonType.None)
        {
            typeAdv *= TypeChart.GetTypeEffectiveness(moveType, t2);
        }
        if(defender.TrickOrTreat)
        {
            typeAdv *= TypeChart.GetTypeEffectiveness(moveType, PokemonType.Ghost);
        }   
        else if(defender.ForestsCurse)
        {
            typeAdv *= TypeChart.GetTypeEffectiveness(moveType, PokemonType.Ghost);
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
        if(defender.HeldItem == "Ring Target" && TypeChart.GetTypeEffectiveness(moveType, type) == 0.0f)
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
            return TypeChart.GetTypeEffectiveness(moveType, type) * TypeChart.GetTypeEffectiveness(PokemonType.Flying, type);
        }
        if(type == PokemonType.Flying && weather == Weather.Wind && TypeChart.GetTypeEffectiveness(moveType, type) > 1f)
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
        if(state == BattleState.Pokemon)
        {
            partyScreen.HandleInput(switchBecauseFainted);
            return;
        }
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