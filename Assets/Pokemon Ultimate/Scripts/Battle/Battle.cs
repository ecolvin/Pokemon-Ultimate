using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public enum BattleState {Intro, ActionSelection, MoveSelection, Bag, Pokemon, Busy, Choice, Battle, ForgetMove}
public enum Weather {None, Sun, HarshSun, Rain, HeavyRain, Sand, Hail, Snow, Shadow, Fog, Wind}
public enum Terrain {None, Electric, Misty, Psychic, Grassy}
public enum FieldEffect {None, Spikes, ToxicSpikes, Rocks, Webs, Reflect, LightScreen, Tailwind, LuckyChant}
public enum BattleChoice {Move, Item, Switch, Run}
public enum SwitchReason {None, BattleSelection, Fainted, OppSwitch, SwitchMove}

public class Battle : MonoBehaviour
{
    
    [SerializeField] PlayerController player;
    [SerializeField] Party playerParty;

    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] PartyHUD enemyPartyHUD;
    [SerializeField] PartyHUD playerPartyHUD;
    [SerializeField] MainBoxController mainBox;
    [SerializeField] SideBoxController sideBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionController moveSelection;


    //TODO: Replace with parameters
    [SerializeField] Image background;
    [SerializeField] BattleSprite enemySprite;
    [SerializeField] BattleSprite playerSprite;
    [SerializeField] CharacterBattleSprite playerImage;
    [SerializeField] CharacterBattleSprite trainerImage;

    [SerializeField] PokeBallSprite ballSprite;

    public event Action<bool> OnBattleOver;

    BattleState state;

    int curBattleOption = 0;
    int curMoveOption = 0;
    bool choice = true;
    int moveChoice = 0;

    int numRunAttempts = 0;

    bool isTrainerBattle = false;
    SwitchReason switchReason = SwitchReason.None;
    //bool switchBecauseFainted = false;
    //bool switch

    Pokemon playerPokemon;
    Pokemon enemyPokemon;

    Party trainerParty;

    TrainerController trainer;

    Dictionary<FieldEffect, int> playerField = new Dictionary<FieldEffect, int>(); 
    Dictionary<FieldEffect, int> enemyField = new Dictionary<FieldEffect, int>();

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
        partyScreen.OnClose += (Pokemon p) => 
        {
            if(p != null)
            {
                state = BattleState.Busy;
            }
            partyScreen.gameObject.SetActive(false);
            StartCoroutine(ClosePokemon(p));
        };
    }

    public void StartBattle(Pokemon wildPokemon)
    {   
        isTrainerBattle = false;
        curBattleOption = 0;
        curMoveOption = 0;
        playerPokemon = playerParty.GetLeadPokemon();
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

    public void StartTrainerBattle(Party trainerParty)
    {   
        this.trainerParty = trainerParty;
        trainer = trainerParty.GetComponent<TrainerController>();
        isTrainerBattle = true;
        curBattleOption = 0;
        curMoveOption = 0;
        playerPokemon = playerParty.GetLeadPokemon();
        if(playerPokemon == null)
        {
            Debug.Log("The player is out of useable pokemon!");
            return;
        }
        enemyPokemon = trainerParty.GetLeadPokemon();
        state = BattleState.Intro;
        InitField();
        StartCoroutine(InitBattle(playerPokemon, enemyPokemon));
    }

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
        weather = Weather.None;   //Add dynamic battle weather based on environment
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
        if(isTrainerBattle)
        {
            StartCoroutine(mainBox.CantCatchTrainer());
            return;
        }
        StartCoroutine(ResolveTurn(BattleChoice.Item, null, null));
    }

    void Pokemon()
    {
        state = BattleState.Pokemon;
        
        partyScreen.SetPartyMembers(playerParty.Pokemon);
        partyScreen.gameObject.SetActive(true);
    }

    IEnumerator ClosePokemon(Pokemon pokemon)
    {
        if(switchReason == SwitchReason.Fainted)
        {
            if(pokemon != null && !pokemon.Fainted)
            {
                yield return SwitchPokemon(pokemon);
                switchReason = SwitchReason.None;
                yield return CheckIfOppFainted();
                PlayerSelection();
            }
            else
            {
                Debug.Log("Switching due to fainted Pokemon but no Pokemon selected. Party Screen shouldn't let you get here.");
            }
        }
        else if(switchReason == SwitchReason.OppSwitch)
        {
            if(pokemon != null && !pokemon.Fainted)
            {
                yield return SwitchPokemon(pokemon);
            }
            switchReason = SwitchReason.None;
        }
        else
        {
            if(pokemon == null)
            {
                state = BattleState.ActionSelection;
                switchReason = SwitchReason.None;
            }
            else if(!pokemon.Fainted)
            {
                yield return ResolveTurn(BattleChoice.Switch, null, pokemon);
            }
        }
    }

    void Run()
    {
        if(isTrainerBattle)
        {
            StartCoroutine(mainBox.RunFromTrainer());
            return;
        }
        StartCoroutine(ResolveTurn(BattleChoice.Run, null, null));
    }

//----------------------Timed Coroutines (and helper functions)------------------------

    public IEnumerator InitBattle(Pokemon playerPokemon, Pokemon enemyPokemon)
    {
        sideBox.Clear();
        partyScreen.Init();            
        
        playerSprite.gameObject.SetActive(true);
        enemySprite.gameObject.SetActive(true);
        playerSprite.Set(playerPokemon);
        enemySprite.Set(enemyPokemon);
        playerPokemon.IsActive = true; 
        
        numRunAttempts = 0;

        if(!isTrainerBattle)
        {
            playerImage.gameObject.SetActive(false);
            trainerImage.gameObject.SetActive(false);
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
        else
        {
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.Set(player.Sprite);
            trainerImage.Set(trainer.Sprite);
            StartCoroutine(playerImage.Entrance());
            yield return StartCoroutine(trainerImage.Entrance());
            
            playerPartyHUD.Set(playerParty);
            enemyPartyHUD.Set(trainerParty);
            playerPartyHUD.gameObject.SetActive(true);
            enemyPartyHUD.gameObject.SetActive(true);

            yield return mainBox.TrainerIntro(trainer.TrainerName);
            yield return mainBox.PauseAfterText();


            enemyPartyHUD.gameObject.SetActive(false);
            StartCoroutine(trainerImage.Exit());
            StartCoroutine(enemySprite.Entrance());
            yield return mainBox.TrainerPokemonIntro(trainer.TrainerName, enemyPokemon.Species.SpeciesName);
            yield return mainBox.PauseAfterText();
            enemyHUD.GenerateBar(enemyPokemon);
            enemyHUD.gameObject.SetActive(true);

            playerPartyHUD.gameObject.SetActive(false);
            StartCoroutine(playerImage.Exit());
            StartCoroutine(playerSprite.Entrance());
            yield return mainBox.PlayerPokemonIntro(playerPokemon.Species.SpeciesName);
            yield return mainBox.PauseAfterText();
            playerHUD.GenerateBar(playerPokemon);
            playerHUD.gameObject.SetActive(true);

            PlayerSelection(); 
        }
    }

    IEnumerator ChooseMove(PokemonMove move)
    {
        yield return ResolveTurn(BattleChoice.Move, move, null);
    }

    //choice: 0-use a move, 1-use an item, 2-switch pokemon, 3-run attempt
    IEnumerator ResolveTurn(BattleChoice choice, PokemonMove move, Pokemon pokemon)
    {
        sideBox.Clear();
        state = BattleState.Battle;

        BattleChoice enemyChoice = getEnemyChoice();
        PokemonMove enemyMove = getEnemyMove();
        
        //Quick Claw/Custap Berry announcement

        int playerSpeed = GetEffectiveSpeed(playerPokemon, playerField[FieldEffect.Tailwind] > 0);
        int enemySpeed = GetEffectiveSpeed(enemyPokemon, enemyField[FieldEffect.Tailwind] > 0);       
        
        bool playerFirst = false;


        if((playerSpeed > enemySpeed && !trickRoom) || (playerSpeed < enemySpeed && trickRoom))
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

        
        if(choice == BattleChoice.Run) //Run Away
        {
            yield return RunAttempt();
        }
        if(enemyChoice == BattleChoice.Run)
        {
            yield return RunAttempt(); //Update to use a different enemy run function
        }

        if(choice == BattleChoice.Switch && enemyChoice == BattleChoice.Switch)
        {
            //Switch in speed order
        }
        else if(choice == BattleChoice.Switch) //Switch Pokemon
        {
            yield return SwitchPokemon(pokemon);
            switchReason = SwitchReason.None;            
        }
        else if(enemyChoice == BattleChoice.Switch)
        {
            //Switch enemy
        }

        CheckLoss();
        CheckVictory();

        //Handle Rotation
        
        if(choice == BattleChoice.Item && enemyChoice == BattleChoice.Item) //Use an Item
        {

        }
        else if(choice == BattleChoice.Item)
        {
            if(isTrainerBattle)
            {
                Debug.Log("You shouldn't be able to select bag against a trainer currently...");
            }
            yield return ThrowPokeball(1);
        }
        else if (enemyChoice == BattleChoice.Item)
        {

        }

        //Mega Evolution/Ultra Burst/Dynamax/Terastalization
        //Focus Punch/Beak Blast/Shell Trap charging effects

        if(choice == BattleChoice.Move && enemyChoice == BattleChoice.Move)
        {
            int playerPriority = move.MoveBase.Priority;
            int enemyPriority = enemyMove.MoveBase.Priority;
            if(playerPriority > enemyPriority)
            {
                playerFirst = true;
            }
            else if(playerPriority < enemyPriority)
            {
                playerFirst = false;
            }
            if(playerFirst)
            {
                yield return StartCoroutine(UseMove(move, playerPokemon, enemyPokemon));
                if(!enemyPokemon.Fainted)
                {
                    yield return StartCoroutine(UseMove(enemyMove, enemyPokemon, playerPokemon));
                }
            }
            else
            {
                yield return StartCoroutine(UseMove(enemyMove, enemyPokemon, playerPokemon));
                if(!playerPokemon.Fainted)
                {
                    yield return StartCoroutine(UseMove(move, playerPokemon, enemyPokemon));
                }
            }
        }
        else if(choice == BattleChoice.Move)
        {
            yield return StartCoroutine(UseMove(move, playerPokemon, enemyPokemon));            
        }
        else if(enemyChoice == BattleChoice.Move)
        {
            yield return StartCoroutine(UseMove(enemyMove, enemyPokemon, playerPokemon));
        }

        yield return CheckLoss();
        yield return CheckVictory();

        yield return EndRound();

        yield return CheckLoss();
        yield return CheckVictory();

        if(playerPokemon.Fainted)
        {
            switchReason = SwitchReason.Fainted;
            Pokemon();
        }
        else
        {      
            yield return CheckIfOppFainted();            
            PlayerSelection();
        }
    }

    IEnumerator CheckVictory()
    {
        if(isTrainerBattle)
        {
            if(trainerParty.GetLeadPokemon() == null)
            {
                yield return mainBox.Victory(trainer.TrainerName);
                yield return mainBox.PauseAfterText();
                yield return trainerImage.Entrance();
                yield return mainBox.SayDialog(trainer.LossDialog);
                //int money = GetMoneyEarned();
                //yield return mainBox.EarnedMoney(money);
                CleanupBattle();
                OnBattleOver(true);
            }
        }
        else
        {
            if(enemyPokemon.Fainted)
            {
                CleanupBattle();
                OnBattleOver(true);
            }
        }
    }

    IEnumerator CheckLoss()
    {
        if(playerParty.GetLeadPokemon() == null)
        {
            yield return mainBox.WhiteOut();
            yield return mainBox.PauseAfterText();
            CleanupBattle();
            OnBattleOver(false);
        }
    }

    IEnumerator CheckIfOppFainted()
    {
        if(!enemyPokemon.Fainted)
        {
            yield break;
        }
        CheckVictory();
        if(isTrainerBattle)
        {   
            yield return EnemyNewPokemon(trainerParty.GetLeadPokemon());
        }
        else
        {
            Debug.Log("Battle should be over due to a fainted wild pokemon. Something went wrong.");
        }
    }

    IEnumerator EnemyNewPokemon(Pokemon newPokemon)
    {           
        enemyPokemon = newPokemon;
        enemySprite.Set(enemyPokemon);
        
        if(GlobalSettings.Instance.BattleMode == BattleMode.Switch && playerParty.NumHealthyPokemon() > 1) //&& Not trainer switching mid-turn
        {
            yield return TrainerSwitching();
        }        

        sideBox.Clear();
        enemyHUD.gameObject.SetActive(false);
        enemyPartyHUD.Set(trainerParty);
        enemyPartyHUD.gameObject.SetActive(true);
        yield return mainBox.PauseAfterText();
        yield return mainBox.TrainerPokemonIntro(trainer.TrainerName, enemyPokemon.Species.SpeciesName);
        yield return enemySprite.Entrance();
        yield return mainBox.PauseAfterText();
        enemyHUD.GenerateBar(enemyPokemon);
        enemyHUD.gameObject.SetActive(true);
        enemyPartyHUD.gameObject.SetActive(false);
    }

    IEnumerator TrainerSwitching()
    {
        yield return mainBox.TrainerNewPokemon(trainer.TrainerName, enemyPokemon.Nickname);             
        state = BattleState.Choice;
        mainBox.EnableChoiceBox(true);

        while(state == BattleState.Choice)
        {
            yield return null;
        }

        if(choice)
        {
            switchReason = SwitchReason.OppSwitch;
            Pokemon();
            while(switchReason == SwitchReason.OppSwitch)
            {
                yield return null;
            }
        }
    }

    int GetEffectiveSpeed(Pokemon pokemon, bool tailwind)
    {
        int speed = pokemon.Stats.Spe;

        int modifier = 4096;
        //Swift Swim/Chlorophyll/Slush Rush/Sand Rush/Surge Surger/Unburden *2 modifier
        //Quick Feet *1.5 modifier
        //Slow Start (still active) *0.5 modifier
        //Quickpowder & species is Ditto *2 modifier
        //Choice Scarf *1.5 modifier
        //Iron Ball/Macho Brace/Power EV item *0.5 modifier
        if(tailwind)
        {
            modifier *= 2;
        }
        //Pledge Swamp *0.25 modifier
        
        speed *= modifier;
        if(pokemon.Status == NonVolatileStatus.Paralysis) //&& !Quick Feet
        {
            speed /= 2;
        }
        if(speed > 10000)
        {
            speed = 10000;
        }
        if(trickRoom)
        {
            speed = 10000 - speed;
        }
        if(speed > 8191)
        {
            speed -= 8192;
        }
        return speed;
    }

    bool IsPlayerFaster() 
    {
        int playerSpeed = GetEffectiveSpeed(playerPokemon, playerField[FieldEffect.Tailwind] > 0);
        int enemySpeed = GetEffectiveSpeed(enemyPokemon, enemyField[FieldEffect.Tailwind] > 0); 
        
        return playerSpeed > enemySpeed;
    }

    bool MissCheck(PokemonMove move, Pokemon attacker, Pokemon defender)
    {
        int accuracy = move.MoveBase.Accuracy;
        if(accuracy == -1)
        {
            return false;
        }
        
        float modifier = 4096;
        if(gravity)
        {
            modifier = RoundUp(modifier * 6840/4096);
        }
        //Tangled Feet .5
        //Hustle 3277/4096
        //Sand Veil 3277/4096
        //Snow Cloak 3277/4096
        //Vicory Star 4506/4096 (can be applied multiple times)
        //Compound Eyes 5325/4096
        //Bright Powder 3686/4096
        //Lax Incense 3686/4096
        //Wide Lens 4505/4096
        //Zoom Lens 4915/4096
        
        int accuracyModified = Round(accuracy * modifier/4096);

        //Factor in simple/foresight
        int adjustedStages = attacker.Accuracy - defender.Evasion;
        int top = 3, bottom = 3;
        if(adjustedStages > 0)
        {
            top += adjustedStages;
        }
        if(adjustedStages < 0)
        {
            bottom += adjustedStages;
        }
        accuracyModified = Round((float)accuracyModified * (float)top/(float)bottom);
        return UnityEngine.Random.Range(1,101) > accuracyModified;
    }

    IEnumerator UseMove(PokemonMove move, Pokemon attacker, Pokemon defender) 
    {
        if(attacker.Fainted)
        {
            yield break;
        }

        //Recharge message

        if(attacker.Status == NonVolatileStatus.Sleep)  //Sleep Check
        {
            //Rest Flag
            if(attacker.SleepCounter > 0)
            {
                attacker.SleepCounter--;
                if(attacker.IsPlayer)
                {
                    StartCoroutine(playerSprite.Sleep());
                }
                else
                {
                    StartCoroutine(enemySprite.Sleep());
                }
                yield return mainBox.SleepTurn(attacker);
                yield return mainBox.PauseAfterText();
                yield break;
            }
            else
            {
                yield return mainBox.WakeUp(attacker);
                attacker.ClearNVStatus();
                if(attacker.IsPlayer)
                {
                    playerHUD.UpdateStatus();
                }
                else
                {
                    enemyHUD.UpdateStatus();
                }
                yield return mainBox.PauseAfterText();
            }
        }

        if(attacker.Status == NonVolatileStatus.Freeze && !move.MoveBase.Defrosts)    //Freeze Check
        {
            if(UnityEngine.Random.Range(0,5) > 0)
            {
                if(attacker.IsPlayer)
                {
                    StartCoroutine(playerSprite.Freeze());
                }
                else
                {
                    StartCoroutine(enemySprite.Freeze());
                }
                yield return mainBox.FreezeTurn(attacker);
                yield return mainBox.PauseAfterText();
                yield break;
            }
            else
            {
                yield return mainBox.Thaw(attacker);
                attacker.ClearNVStatus();
                if(attacker.IsPlayer)
                {
                    playerHUD.UpdateStatus();
                }
                else
                {
                    enemyHUD.UpdateStatus();
                }
                yield return mainBox.PauseAfterText();
            }
        }

        //PP Check
        //Disobedience
        //Truant
        //Focus Punch loses focus
        //Flinch

        //Disabled
        //Heal Block
        //Gravity/Throat Chop
        //Choice Locked
        //Taunt
        //Imprison

        //Confusion Self-Hit

        if(attacker.Status == NonVolatileStatus.Paralysis)
        {
            //yield return mainBox.ParaCheck(attacker);
            if(UnityEngine.Random.Range(0,4) == 0)
            {
                if(attacker.IsPlayer)
                {
                    StartCoroutine(playerSprite.Paralyzed());
                }
                else
                {
                    StartCoroutine(enemySprite.Paralyzed());
                }
                yield return mainBox.FullParalysis(attacker);
                yield return mainBox.PauseAfterText();
                yield break;
            }
        }

        //Infatuation Check
        //Z-Move Dance and Z-effect if status move
        //--Move Disabled//Heal Block//Gravity//Throat Chop//Choice//Taunt//Imprison checks to be after z-dance

        //Sleep Talk/Snore sleep announcement
        //Submoves
        //--announce the submove is occurring: "[Pokemon] used [move]!". Repeat check for Heal Block/Gravity/Throat Chop; if it passes, continue. Submove announcement guarantees PP deduction.
        //----Copycat
        //----Metronome
        //----Nature Power
        //----Sleep Talk as a submove
        //----Sleep Talk with no eligible moves to call will fail at this point
        //Autothaw Freeze (Flame Wheel, Sacred Fire, Flare Blitz, Fusion Flare, Scald, Steam Eruption, Burn Up, Pyro Ball, Scorching Sands, and Matcha Gotcha)
        
        if(move.MoveBase.Defrosts)
        {
            yield return mainBox.Thaw(attacker);
            attacker.ClearNVStatus();
            if(attacker.IsPlayer)
            {
                playerHUD.UpdateStatus();
            }
            else
            {
                enemyHUD.UpdateStatus();
            }
            yield return mainBox.PauseAfterText();
        }
        
        //Stance Change

        //Announce Move (Locked in through PP Deduction)
        
        yield return mainBox.UseMove(attacker, move);
        yield return mainBox.PauseAfterText();

        //----------Use UsedMove Class for type changes--------------
        //|Check for move type change from abilities like Pixilate
        //|Set Move type for Terrain Pulse/Hidden Power/Judgment/Multi-Attack/Natural Gift/Revelation Dance/Techno Blast/Weather Ball/Z-Weather Ball/Tera Blast
        //|Check for move type change from Electrify/Ion Deluge/Plasma Fists
        //|Check again for move type change from abilities like Pixilate
        //|Set random target for any of the following
        //|--Multi-turn moves (Outrage)
        //|--Single-target moves called by a submove
        //|--Ghost-type Curse
        //|--Single-target move if player timed out in move selection
        //|Set target redirection via Lightning Rod/Storm Drain
        //|Set target redirection via Follow Me/Rage Powder/Spotlight/Z-Destiny Bond/Z-Grudge
        //|Deduct the appropriate amount of PP

        move.DecrementPP(false);  //calculate value for Pressure dynamically

        //Set Choice lock or Gorilla Tactics locks
        //Desolate Land/Primordial Sea blocking damaging moves
        //Micle Berry's accuracy check consumed
        //Move failure checks, part 1
        //--Aura Wheel/Hypserspace Fury/Dark Void (when not an eligible pokemon)
        //--Aurora Veil (when not hailing)
        //--Bide (No energy to unleash)
        //--Burn Up (not fire type)
        //--Clangorous Soul (when not enough HP)
        //--Counter/Mirror Coat/Metal Burst (when user hasn't been damaged)
        //--Destiny Bond (when user has already been Destiny Bonded)
        //--Encore (target hasn't used a move/has no PP left/move is exempt)
        //--Fake Out/First Impression/Mat Block (After first turn)
        //--Fling/Natural Gift (with no item or if Embargo/Magic Room)
        //--Follow Me/Rage Powder (in singles)
        //--Future Sight/Doom Desire (when a future attack is already pending)
        //--Last Resort (when not the last move used)
        //--No Retreat (when Can't Escape flag set from No Retreat)
        //--Poltergeist (when target has no item)
        //--Protect moves (successive use checks/other move already used)
        //--Rest (if already asleep/at full hp/has insomnia or vital spirit)
        //--Sucker Punch (target doesn't have eligible move pending)
        //--Sleep Talk/Snore (not asleep)
        //--Steel Roller (no terrain)
        //--Stockpile (3 stockpiles already)
        //--Stuff Cheeks (user has no berry)
        //--Swallow/Spit UP (w/ 0 stockpiles)
        //--Teleport (w/ no other pokemon)
        //--Weight Moves into Dynamax
        //Ability Failures Pt 1
        //--Damp
        //--Dazzling
        //--Queenly Majesty
        //Interruptable Moves
        //--Future Sight/Doom Desire (move checks end here)
        //--Pledge Move Combos (the user's move check ends here, and the partner picks up at the start of the checks again)
        //Libero/Protean
        //Charge move message
        //Set flag to lose 100% HP from Explosion/Self Destruct
        //Failure due to no target for move or target is self when it shouldn't be
        //Set flag to lose 50% HP from Steel Beam
        //Semi-invulnerability
        //Psychic Terrain
        //Teammate Protection Effects
        //--Quick Guard
        //--Wide Guard
        //--Crafty Shield
        //Protect and equivalents (no Max Guard)
        //Mat Block
        //Max Guard
        //Magic Coat
        //Magic Bounce
        //Ability Failures Pt 2
        //--Dry Skin
        //--Flash Fire
        //--Lightning Rod
        //--Overcoat
        //--Sap Sipper
        //--Soundproof
        //--Storm Drain
        //--Telepathy
        //--Volt Absorb
        //--Water Absorb
        //--Wonder Guard
        float type = CalculateTypeAdvantage(move.MoveBase.MoveType, move, defender, attacker);
        Debug.Log($"Move: {move.MoveBase.MoveName}; Type: {move.MoveBase.MoveType}; Defender Types: {defender.Type1}/{defender.Type2}; Effectiveness: {type}");        
        if(type == 0f)
        {
            yield return mainBox.Immune(defender);
            yield return mainBox.PauseAfterText();
            yield break;
        }
        //Levitate Ground-type immunity
        //Air Balloon/Magnet Rise Ground-type immunity
        //Safety Goggles
        //Ability Failures Pt 3
        //--Bulletproof
        //--Sticky Hold (against Trick/Switcheroo/Corrosive Gas)
        //Type-based move immunities Pt 1
        //--Dark Type Prankster immunity
        //--Ghost Type immunity to trapping moves
        //--Grass Type powder immunity
        //--Ice Type immunity to Sheer Cold
        //Move Failure Checks Pt 2
        //--Attract (when target is same gender)
        //--Torment (into Dynamax)
        //--Venom Drench (when target not poisoned)
        //Move Failure Checks Pt 3
        //--Attract (if already infatuated)
        //--Clangorous Soul/No Retreat (with max stats already)
        //--Coaching (in singles or with no ally available in doubles)
        //--Dream Eater (target is awake)
        //--Endeavor (target had equal or less HP than the user)
        //--Ingrain (user already ingrained)
        //--Leech Seed (target already seeded)
        //--OHKO (target with higher level/dynamaxed)
        //--Perish Song (all targets already have Perish Song)
        //--Status move (into target with that status already)
        //--Status move (into target with another status already)
        //--Stat change (can't go any higher)
        //--Stat change (can't go any lower)
        //--Stuff Cheeks (w/ max Def)
        //--Torment (target already Tormented)
        //--Worry Seed (target has Insomnia/Truant)
        //--Yawn (target w/ status condition/already yawned)
        //Type-Based condition immunities Pt 2
        //--Electric type paralysis
        //--Fire type burn
        //--Grass type Leech Seed
        //--Poison/Steel type Poison/Badly Poisoned
        //Uproar stopping a sleep move
        //Safeguard
        //Electric Terrain/Misty Terrain blocking a status
        //Substitute blocking stat drop/decorate
        //Mist
        //Ability Failures Pt 4
        //--Stat based failures
        //----Big Pecks
        //----Clear Body
        //----Flower Veil
        //----Full Metal Body
        //----Hyper Cutter
        //----Keen Eye
        //----White Smoke
        //--Status condition based failures
        //----Comatose
        //----Flower Veil
        //----Immunity
        //----Insomnia
        //----Leaf Guard
        //----Limber
        //----Oblivious
        //----Own Tempo
        //----Pastel Veil
        //----Shields Down
        //----Sweet Veil
        //----Vital Spirit
        //----Water Bubble
        //----Water Veil
        //--Other
        //----Aroma Veil (Attract/Torment)
        //----Sturdy (against OHKO moves)
        
        if(MissCheck(move, attacker, defender))
        {
            yield return mainBox.Missed();
            yield return mainBox.PauseAfterText();
            //Blunder Policy check
            yield break;
        }


        //Substitute blocking effect other than stat drop
        //Mirror Armor
        //Roar/Whirlwind into D-Max target
        //Roar/Whirlwind into Suction Cups target
        //Roar/Whirlwind into Ingrained target
        //Move Failure Checks Pt 4
        //--Ability Failure
        //----Entrainment when target is Dynamaxed, target has same Ability as user, or target / user have an Ability that can't be passed / passed from
        //----Gastro Acid into already has Gastro Acid / unchangeable ability
        //----Role Play when target has same ability as user or cannot take target's Ability
        //----Simple Beam into target that has Simple / unreplaceable Ability
        //----Skill Swap when target or user cannot swap Ability / target is Dynamaxed
        //----Worry Seed when target has unreplaceable Ability
        //--Full HP failures
        //----Heal Pulse / Floral Healing into target at full HP
        //----Life Dew into target at full HP
        //----Jungle Healing when both user and ally are at full HP and lack status conditions
        //----Pollen Puff into target at full HP
        //----Self-target recovery moves when user has full HP
        //--Stat-related failures
        //----Belly Drum when user is at less than 50% HP or already at +6 Attack
        //----Flower Shield with no Grass-type targets
        //----Strength Sap into target at -6 Attack
        //----Swagger / Flatter when target is already at +6 and confused
        //----Topsy-Turvy when target has no stat changes
        //--Type failures
        //----Conversion when user is already the same type as its first move
        //----Conversion 2 into target that hasn't moved, or into target that most recently used Struggle
        //----Reflect Type when user is same type as target
        //----Soak into a pure Water-type / Magic Powder into a pure Psychic-type / Soak | Magic Powder into Silvally with RKS System
        //----Trick-or-Treat into a Ghost-type / Forest's Curse into a Grass-type
        //--Failures that play additional text
        //----Aromatherapy / Heal Bell while no party Pokemon have status conditions
        //----Teatime when no Pokemon on the field have Berries to eat
        //--Redundancy failures
        //----Attempting to create a weather / Terrain that already exists / field effect that already exists
        //----Aqua Ring while the user already has Aqua Ring
        //----Baton Pass / Healing Wish / Lunar Dance with no remaining Pokemon to pass to
        //----Curse (user Ghost-type) into target that already has Curse
        //----Entry hazard when that entry hazard is already fully set
        //----Focus Energy when user already has "Critical Hit Boost" Y-info volatile
        //----Helping Hand when target has already moved
        //----Lock-On / Mind Reader when user already has "Lock-On" Y-info volatile
        //----Magnet Rise when user already has Magnet Rise
        //----Substitute when user already has a Substitute
        //----Taunt into target already taunted
        //----Trapping move while target already has "Can't Escape" Y-info volatile
        //----Wish while a Wish is already active
        //--Other
        //----After You / Electrify / Quash into a target that has already moved
        //----Corrosive Gas
        //----Copycat with no moves to copy
        //----Court Change when neither side has field effects
        //----Disable failures
        //----Encore failures
        //----Helping Hand / Ally Switch / Aromatic Mist / Hold Hands in singles or when there is no ally target available in doubles
        //----Instruct failures
        //----Mimic into target that hasn't moved / target used unMimicable move
        //----Primal weather active and trying to set a regular weather
        //----Psycho Shift when user has no status condition / target has same status condition / target has different status condition
        //----Purify when target does not have a status condition
        //----Roar / Whirlwind into target with no Pokemon remaining to switch into
        //----Transform while user / target is already Transformed
        //----Trick / Switcheroo with neither having items or has unTrickable item
        //Move failure checks, part 5
        //--Psycho Shift when target is immune to status condition
        //--Substitute when user lacks enough HP to execute the move
        //Aroma Veil (Disable, Encore, Taunt)
        
        
        //Damage Calculation        
        //Perform the move
        if(attacker.IsPlayer)
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
            yield return CalculateDamage(move, attacker, defender);
        }

        List<Pokemon> pokemonOrder = new List<Pokemon>();
        pokemonOrder.Add(defender);
        pokemonOrder.Add(attacker);
        yield return CheckForFainted(pokemonOrder);

        yield return CalculateEffects(move, attacker, defender);
        //yield return CalculateOther()          -Things like contact for abilities and whatnot
    }

    IEnumerator CalculateEffects(PokemonMove move, Pokemon attacker, Pokemon defender)
    {
        yield return ApplyWeatherChanges(move, false); //update to check attacker's held item
        yield return ApplyTerrainChanges(move);
        yield return ApplyScreens(move, attacker.IsPlayer);
        yield return ApplySecondaryHealing(move, attacker);
        if(!defender.Fainted)
        {
            yield return ApplyStatChanges(move, attacker, defender);
            yield return ApplyNonVolatileStatus(move, defender);
            yield return ApplyVolatileStatus(move, defender);
            yield return mainBox.PauseAfterText();
        }
    }

    IEnumerator ApplyStatChanges(PokemonMove move, Pokemon attacker, Pokemon defender)
    {
        if(RandomPercent() <= move.MoveBase.SelfStatChangeChance)
        {
            StatBlock changes = move.MoveBase.SelfStatChanges;
            int acc = move.MoveBase.SelfAccuracyChange;
            int eva = move.MoveBase.SelfEvasionChange;
            yield return StatChangeMessageHandling(attacker, changes, acc, eva);
            attacker.ChangeStats(changes);
            attacker.ChangeAccuracy(acc);
            attacker.ChangeEvasion(eva);
        }
        if(RandomPercent() <= move.MoveBase.EnemyStatChangeChance)
        {
            StatBlock changes = move.MoveBase.EnemyStatChanges;
            int acc = move.MoveBase.EnemyAccuracyChange;
            int eva = move.MoveBase.EnemyEvasionChange;
            yield return StatChangeMessageHandling(defender, changes, acc, eva);
            defender.ChangeStats(changes);
            attacker.ChangeAccuracy(acc);
            attacker.ChangeEvasion(eva);
        }
        yield return null;
    }

    IEnumerator StatChangeMessageHandling(Pokemon target, StatBlock changes, int acc, int eva)
    {
        if(acc > 0)
        {
            if(target.Accuracy == 6)
            {
                yield return mainBox.CantGoHigher(target, "accuracy");
                yield return mainBox.PauseAfterText();
            }
            else
            {
                yield return mainBox.StatIncrease(target, "accuracy", acc == 2);
                yield return mainBox.PauseAfterText();
            }
        }
        else if(acc < 0)
        {
            if(target.Accuracy == -6)
            {
                yield return mainBox.CantGoLower(target, "accuracy");
                yield return mainBox.PauseAfterText();
            }
            else
            {
                yield return mainBox.StatDecrease(target, "accuracy", acc == -2);
                yield return mainBox.PauseAfterText();                
            }
        }
        
        if(eva > 0)
        {
            if(target.Evasion == 6)
            {
                yield return mainBox.CantGoHigher(target, "evasion");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatIncrease(target, "evasion", acc == 2);
                yield return mainBox.PauseAfterText();                
            }
        }
        else if(eva < 0)
        {
            if(target.Evasion == -6)
            {
                yield return mainBox.CantGoLower(target, "evasion");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatDecrease(target, "evasion", acc == -2);
                yield return mainBox.PauseAfterText();                
            }
        }

        if(changes.Atk > 0)
        {
            if(target.StatChanges.Atk == 6)
            {
                yield return mainBox.CantGoHigher(target, "Attack");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatIncrease(target, "Attack", changes.Atk == 2);
                yield return mainBox.PauseAfterText();                
            }
        }
        else if(changes.Atk < 0)
        {
            if(target.StatChanges.Atk == -6)
            {
                yield return mainBox.CantGoLower(target, "Attack");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatDecrease(target, "Attack", changes.Atk == -2);
                yield return mainBox.PauseAfterText();                
            }
        }

        if(changes.Def > 0)
        {
            if(target.StatChanges.Def == 6)
            {
                yield return mainBox.CantGoHigher(target, "Defense");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatIncrease(target, "Defense", changes.Def == 2);
                yield return mainBox.PauseAfterText();                
            }
        }
        else if(changes.Def < 0)
        {
            if(target.StatChanges.Def == -6)
            {
                yield return mainBox.CantGoLower(target, "Defense");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatDecrease(target, "Defense", changes.Def == -2);
                yield return mainBox.PauseAfterText();                
            }
        }

        if(changes.SpA > 0)
        {
            if(target.StatChanges.SpA == 6)
            {
                yield return mainBox.CantGoHigher(target, "Sp. Atk");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatIncrease(target, "Sp. Atk", changes.SpA == 2);
                yield return mainBox.PauseAfterText();                
            }
        }
        else if(changes.SpA < 0)
        {
            if(target.StatChanges.SpA == -6)
            {
                yield return mainBox.CantGoLower(target, "Sp. Atk");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatDecrease(target, "Sp. Atk", changes.SpA == -2);
                yield return mainBox.PauseAfterText();                
            }
        }

        if(changes.SpD > 0)
        {
            if(target.StatChanges.SpD == 6)
            {
                yield return mainBox.CantGoHigher(target, "Sp. Def");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatIncrease(target, "Sp. Def", changes.SpD == 2);
                yield return mainBox.PauseAfterText();                
            }
        }
        else if(changes.SpD < 0)
        {
            if(target.StatChanges.SpD == -6)
            {
                yield return mainBox.CantGoLower(target, "Sp. Def");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatDecrease(target, "Sp. Def", changes.SpD == -2);
                yield return mainBox.PauseAfterText();                
            }
        }

        if(changes.Spe > 0)
        {
            if(target.StatChanges.Spe == 6)
            {
                yield return mainBox.CantGoHigher(target, "Speed");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatIncrease(target, "Speed", changes.Spe == 2);
                yield return mainBox.PauseAfterText();                
            }
        }
        else if(changes.Spe < 0)
        {
            if(target.StatChanges.Spe == -6)
            {
                yield return mainBox.CantGoLower(target, "Speed");
                yield return mainBox.PauseAfterText();                
            }
            else
            {
                yield return mainBox.StatDecrease(target, "Speed", changes.Spe == -2);
                yield return mainBox.PauseAfterText();                
            }
        }

        yield return null;
    }

    IEnumerator ApplyNonVolatileStatus(PokemonMove move, Pokemon defender)
    {
        if(RandomPercent() <= move.MoveBase.EnemyNVStatusChance)
        {
            List<NonVolatileStatus> statuses = move.MoveBase.EnemyNonVolatileStatuses;
            NonVolatileStatus status = statuses[UnityEngine.Random.Range(0, statuses.Count)];
            int success = defender.ApplyNVStatus(status);
            if(success == 0)
            {
                yield return mainBox.ReceiveCondition(defender, status);
            }
            else if(success == -1)
            {
                yield return mainBox.SetText("UPDATE ME! Already Statused");
            }
            else if(success == -2)
            {
                yield return mainBox.SetText("UPDATE ME! Type Immune to Status");
            }
            else
            {
                //How???
                yield return mainBox.SetText("DEBUGGING PURPOSES ONLY! THIS LINE SHOULD NEVER BE REACHED!");
            }
            playerHUD.UpdateStatus();
            enemyHUD.UpdateStatus();
        }
        yield return null;
    }

    IEnumerator ApplyVolatileStatus(PokemonMove move, Pokemon defender)
    {
        if(RandomPercent() <= move.MoveBase.EnemyNVStatusChance)
        {
            List<VolatileStatus> statuses = move.MoveBase.EnemyVolatileStatuses;
            foreach(VolatileStatus status in statuses)
            {
                int success = defender.ApplyVolatileStatus(status);
                //Message Handling
            }
        }
        yield return null;
    }

    IEnumerator ApplyWeatherChanges(PokemonMove move, bool rock)
    {
        if(RandomPercent() <= move.MoveBase.WeatherChance)
        {
            yield return ChangeWeather(move.MoveBase.Weather, rock); 
        }
        yield return null;
    }

    IEnumerator ApplyTerrainChanges(PokemonMove move)
    {
        if(RandomPercent() <= move.MoveBase.TerrainChance)
        {
            yield return ChangeTerrain(move.MoveBase.Terrain);
        }
        yield return null;
    }

    IEnumerator ApplyScreens(PokemonMove move, bool isPlayerAttacking)
    {
        if(move.MoveBase.LightScreen)
        {
            yield return CreateLightScreen(isPlayerAttacking);
        }
        if(move.MoveBase.Reflect)
        {
            yield return CreateReflect(isPlayerAttacking);
        }
        yield return null;
    }

    IEnumerator ApplySecondaryHealing(PokemonMove move, Pokemon attacker)
    {
        int healing = -1 * Mathf.FloorToInt(attacker.Stats.HP * move.MoveBase.HealthBasedHealing); //-1 to represent healing (negative damage)
        playerPokemon.TakeDamage(healing);
        yield return playerHUD.UpdateHP();
        yield return null;
    }

    IEnumerator CreateLightScreen(bool player)
    {   
        int screenTurns = 5;

        //
        // if(lightClay) screenTurns = 8;
        //

        if(player)
        {
            if(playerField[FieldEffect.LightScreen] <= 0)
            {
                playerField[FieldEffect.LightScreen] = screenTurns;
                //yield return mainBox.LightScreen();
            }
            else
            {
                yield return mainBox.Failed();
                yield return mainBox.PauseAfterText();
            }
        }
        else
        {
            if(enemyField[FieldEffect.LightScreen] <= 0)
            {
                enemyField[FieldEffect.LightScreen] = screenTurns;
                //yield return mainBox.LightScreen();
            }
            else
            {                
                yield return mainBox.Failed();
                yield return mainBox.PauseAfterText();
            }       
        }
        yield return null;
    }

    IEnumerator CreateReflect(bool player)
    {   
        int screenTurns = 5;

        //
        // if(lightClay) screenTurns = 8;
        //

        if(player)
        {
            if(playerField[FieldEffect.Reflect] <= 0)
            {
                playerField[FieldEffect.Reflect] = screenTurns;
                //yield return mainBox.Reflect();
            }
            else
            {
                yield return mainBox.Failed();
                yield return mainBox.PauseAfterText();
            }
        }
        else
        {
            if(enemyField[FieldEffect.Reflect] <= 0)
            {
                enemyField[FieldEffect.Reflect] = screenTurns;
                //yield return mainBox.Reflect();
            }
            else
            {                
                yield return mainBox.Failed();
                yield return mainBox.PauseAfterText();
            }       
        }
        yield return null;
    }

    IEnumerator ChangeWeather(Weather w, bool rock)
    {
        if(weather == w)
        {
            yield return mainBox.SetText("That weather is already set!");
            yield break;
        }
        weather = w;
        weatherCounter = 5;
        if(rock)
        {
            weatherCounter = 8;
        }
        yield return mainBox.CreateWeather(w);
        yield return mainBox.PauseAfterText();
        yield return null;
    }

    IEnumerator ChangeTerrain(Terrain t)
    {
        terrain = t;
        terrainCounter = 5;
        //if Terrain Extender {terrainCounter = 8;}

        yield return mainBox.CreateTerrain(terrain);
        yield return mainBox.PauseAfterText();
        yield return null;
    }

    IEnumerator UseItem(string itemName)
    {
        yield return null;
    }

    List <Pokemon> UpdateSpeedOrder()
    {
        List <Pokemon> order = new List<Pokemon>();
        int playerSpeed = GetEffectiveSpeed(playerPokemon, playerField[FieldEffect.Tailwind] > 0);
        int enemySpeed = GetEffectiveSpeed(enemyPokemon, enemyField[FieldEffect.Tailwind] > 0); 
        if(playerSpeed > enemySpeed)
        {            
            order.Add(playerPokemon);
            order.Add(enemyPokemon);
        }
        else if(enemySpeed > playerSpeed)
        {
            order.Add(enemyPokemon);
            order.Add(playerPokemon);
        }
        else
        {
            if(UnityEngine.Random.Range(0,2) == 0)
            {
                order.Add(playerPokemon);
                order.Add(enemyPokemon);
            }
            else
            {
                order.Add(enemyPokemon);
                order.Add(playerPokemon);
            }
        }
        return order;
    }

    bool CheckForSandDmg(Pokemon p)
    {
        return p.Type1 == PokemonType.Ground || p.Type2 == PokemonType.Ground ||
               p.Type1 == PokemonType.Rock   || p.Type2 == PokemonType.Rock   ||
               p.Type1 == PokemonType.Steel  || p.Type2 == PokemonType.Steel;
    }
    
    IEnumerator EOTSand()
    {
        bool playerSand = CheckForSandDmg(playerPokemon);
        bool enemySand = CheckForSandDmg(enemyPokemon);
        if(playerSand && enemySand)
        {
            if(IsPlayerFaster())
            {
                yield return SandDamage(playerPokemon);
                yield return SandDamage(enemyPokemon);
            }
            else
            {
                yield return SandDamage(enemyPokemon);
                yield return SandDamage(playerPokemon);
            }
        }
        else if(playerSand)
        {
            yield return SandDamage(playerPokemon);
        }
        else if(enemySand)
        {
            yield return SandDamage(enemyPokemon);
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator SandDamage(Pokemon p)
    {
        int sandDamage = Mathf.Max(1, p.Stats.HP/16);
        yield return mainBox.SandDamage(p);
        p.TakeDamage(sandDamage);
        if(p.IsPlayer)
        {
            yield return playerSprite.Hit();
            yield return playerSprite.Hit();
            yield return playerHUD.UpdateHP();
        }
        else
        {
            yield return enemySprite.Hit();
            yield return enemySprite.Hit();
            yield return enemyHUD.UpdateHP();
        }
        yield return mainBox.PauseAfterText();
    }

    bool IsGrounded(Pokemon p)
    {
        if(gravity)
        {
            return true;
        }
        if(p.Type1 == PokemonType.Flying || p.Type2 == PokemonType.Flying)
        {
            return false;
        }
        //Levitate Ability
        //Air Balloon
        //Magnet Rise/Telekinesis
        return true;
    }

    IEnumerator GrassyTerrainHeal(Pokemon p)
    {
        if(IsGrounded(p))
        {
            if(p.CurHP < p.Stats.HP)
            {
                yield return mainBox.PokemonHealing(p);
                int healing = p.Stats.HP/16;
                p.TakeDamage(-1 * healing);
                if(p.IsPlayer)
                {
                    yield return playerHUD.UpdateHP();
                }
                else
                {
                    yield return enemyHUD.UpdateHP();
                }
                yield return mainBox.PauseAfterText();
            }
        }
        yield return null;
    }

    IEnumerator CheckForFainted(List<Pokemon> pokemonOrder)
    {
        bool playerWinsTie = false;
        bool enemyWinsTie = false;
        foreach(Pokemon p in pokemonOrder)
        {
            if(p.Fainted)
            {
                if(p.IsPlayer)
                {
                    yield return playerSprite.Faint();
                    yield return mainBox.Fainted(playerPokemon);
                    yield return mainBox.PauseAfterText();
                    if(!playerWinsTie)
                    {
                        enemyWinsTie = true;
                    }
                }
                else
                {
                    yield return enemySprite.Faint();
                    yield return mainBox.Fainted(enemyPokemon);
                    yield return mainBox.PauseAfterText();
                    yield return GainExp();
                    if(!enemyWinsTie)
                    {
                        playerWinsTie = true;
                    }
                }
            }
        }

        if(playerWinsTie)
        {
            CheckVictory();
            CheckLoss();
        }
        if(enemyWinsTie)
        {
            CheckLoss();
            CheckVictory();
        }
    }

    IEnumerator EndRound()
    {
        List<Pokemon> pokemonOrder = UpdateSpeedOrder();

        if(weather != Weather.None)
        {
            if(--weatherCounter <= 0)
            {
                yield return mainBox.WeatherExpire(weather);
                yield return mainBox.PauseAfterText();
                weather = Weather.None;
            }
            else if(weather == Weather.Hail)
            {
                //Ice Body
                //Hail Damage not implemented due to snow instead
            }
            else if(weather == Weather.Sand)
            {
                yield return EOTSand();
            }
            else if(weather == Weather.Rain)
            {
                //Rain Dish/Dry Skin/
            }
        }        

        CheckForFainted(pokemonOrder);
        
        //Emergency Exit/Wimp Out switches from Weather

        //Affection shrug off status
        //Future Sight/Doom Desire/Wish (Queue from when set, not determined by speed)
        //---Block A--- (First pokemon does all 4, then second pokemon does all 4)
        //G-Max Chip/Sea of Fire (Grass+Fire Pledges) (Queue from when set, not determined by speed)
        //Grassy Terrain Heal
        //Healer/Hydration/Shed Skin
        //Leftovers/Black Sludge
        //-------------
        //Emergency Exit/Wimp Out switches from Block A
        //Aqua Ring
        //Ingrain
        //Leech Seed
        
        foreach(Pokemon p in pokemonOrder)
        {
            if(p.Status == NonVolatileStatus.Poison)
            {
                //Toxic Heal
                int poisonDamage = Mathf.Max(1, p.Stats.HP/8);
                yield return mainBox.PoisonDamage(p);
                yield return mainBox.PauseAfterText();
                p.TakeDamage(poisonDamage);
                if(p.IsPlayer)
                {
                    yield return playerSprite.Poison();
                    yield return playerSprite.Poison();
                    yield return playerHUD.UpdateHP();
                }
                else
                {
                    yield return enemySprite.Poison();
                    yield return enemySprite.Poison();
                    yield return enemyHUD.UpdateHP();
                }
                yield return mainBox.PauseAfterText();
            }
            if(p.Status == NonVolatileStatus.BadlyPoisoned)
            {
                //Toxic Heal
                int poisonDamage = Mathf.Max(1, p.PoisonCounter++ * (p.Stats.HP/16));
                yield return mainBox.PoisonDamage(p);
                yield return mainBox.PauseAfterText();
                p.TakeDamage(poisonDamage);
                if(p.IsPlayer)
                {
                    yield return playerSprite.Poison();
                    yield return playerSprite.Poison();
                    yield return playerHUD.UpdateHP();
                }
                else
                {
                    yield return enemySprite.Poison();
                    yield return enemySprite.Poison();
                    yield return enemyHUD.UpdateHP();
                }
                yield return mainBox.PauseAfterText();
            }
        }
        foreach(Pokemon p in pokemonOrder)
        {
            if(p.Status == NonVolatileStatus.Burn)
            {
                int burnDamage = Mathf.Max(1, p.Stats.HP/16);
                yield return mainBox.BurnDamage(p);
                yield return mainBox.PauseAfterText();
                p.TakeDamage(burnDamage);
                if(p.IsPlayer)
                {
                    yield return playerSprite.Burn();
                    yield return playerSprite.Burn();
                    yield return playerHUD.UpdateHP();
                }
                else
                {
                    yield return enemySprite.Burn();
                    yield return enemySprite.Burn();
                    yield return enemyHUD.UpdateHP();
                }
                yield return mainBox.PauseAfterText();         
            }
        }
        //Poison Heal

        CheckForFainted(pokemonOrder);

        //Nightmare
        //Curse
        //Bind/Clamp/Fire Spin/Infestation/Magma Storm/Sand Tomb/Whirlpool/Wrap (Binding Moves - both damage and freeing)
        //Octolock
        //Taunt fading
        //Torment ending
        //Encore fading
        //Disable fading
        //Magnet Rise fading
        //Telekinesis fading
        //Heal Block fading
        //Embargo fading
        //Yawn
        //Perish Count
        //Roost fading
        //Emergency Exit/Wimp Out Checkpoint
        //---Block B---
        //Reflect dissipating
        //Light Screen dissipating
        //Safeguard dissipating
        //Mist dissipating
        //Tailwind dissipating
        //Lucky Chant dissipating
        //Rainbow (Water+Fire Pledges) dissipating
        //Sea of Fire (Fire+Grass Pledges) dissipating
        //Swamp (Grass+Water Pledges) dissipating
        //Aurora Veil dissipating
        //-------------

        if(trickRoom)
        {
            trickRoomCounter--;
            if(trickRoomCounter <= 0)
            {
                //yield return mainBox.Expire();
                trickRoom = false;
            }
        }
        
        //Water Sport dissipating
        //Mud Sport dissipating
        
        if(gravity)
        {
            gravityCounter--;
            if(gravityCounter <= 0)
            {
                //yield return mainBox.Expire();
                gravity = false;
            }
        }
        if(wonderRoom)
        {
            wonderRoomCounter--;
            if(wonderRoomCounter <= 0)
            {
                //yield return mainBox.Expire();
                wonderRoom = false;
            }
        }
        if(magicRoom)
        {
            magicRoomCounter--;
            if(magicRoomCounter <= 0)
            {
                //yield return mainBox.Expire();
                magicRoom = false;
            }
        }
        if(terrain != Terrain.None)
        {
            if(terrain == Terrain.Grassy)
            {
                if(IsPlayerFaster())
                {
                    yield return GrassyTerrainHeal(playerPokemon);
                    yield return GrassyTerrainHeal(enemyPokemon);
                }
                else
                {
                    yield return GrassyTerrainHeal(enemyPokemon);
                    yield return GrassyTerrainHeal(playerPokemon);
                }
            }

            if(--terrainCounter <= 0)
            {
                yield return mainBox.TerrainExpire(terrain);
                yield return mainBox.PauseAfterText();
                terrain = Terrain.None;
            }
        }
        //---Block C---
        //Uproar (active/ending)
        //Bad Dreams/Ball Fetch/Harvest/Moody/Pickup/Slow Start/Speed Boost
        //Flame Orb/Sticky Barb/Toxic Orb/White Herb
        //-------------
        //Emergency Exit/Wimp Out Checkpoint
        //Power Construct/Schooling/Shields Down/Zen Mode
        //---Block D---
        //Hunger Switch
        //Eject Pack
        //-------------

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
    }

    int CalculateExperience()
    {
        float b = enemyPokemon.Species.BaseXPYield;
        float e = 1f;
        //if(Lucky Egg)
        // e = 1.5f;
        float f = 1f;
        //Change F based on affection
        float l = enemyPokemon.Level;
        float lp = playerPokemon.Level;
        float p = 1;
        //Increase if EXP point power active
        float s = 1;
        //s = 2 if EXP Share && pokemon didn't participate
        float t = 1;
        //t = 1.5 if not same OT; t = 1.7 if not same OT and not same Language
        float v = 1;
        //v = 4915f/4096f if past evolution level without evolving

        return Mathf.RoundToInt(((((b*l)/5) * (1/s) * Mathf.Pow(((2*l) + 10)/(l+lp+10), 2.5f)) + 1) * t * e * v * f * p);
    }

    IEnumerator ForgetAMove(Pokemon p, PokemonMove newMove)
    {
        moveSelection.gameObject.SetActive(true);
        List<PokemonMove> moves = new List<PokemonMove>();
        moves.AddRange(p.Moves);
        moveSelection.SetMoves(moves, newMove);
        state = BattleState.ForgetMove;

        while(state == BattleState.ForgetMove)
        {
            yield return null;
        }

        if(moveChoice >= 0 && moveChoice < moves.Count)
        {
            yield return mainBox.MoveConfirm(moves[moveChoice].MoveBase.MoveName, newMove.MoveBase.MoveName);
            yield return mainBox.PauseAfterText();

            state = BattleState.Choice;
            mainBox.EnableChoiceBox(true);

            while(state == BattleState.Choice)
            {
                yield return null;
            }

            if(choice)
            {
                p.LearnMove(newMove, moveChoice);

                yield return mainBox.ForgottenMove(p.Nickname, moves[moveChoice].MoveBase.MoveName, newMove.MoveBase.MoveName);
                yield return mainBox.PauseAfterText();
            }
            else
            {
                yield return ForgetAMove(p, newMove);
            }
        }
        else if(moveChoice == moves.Count)
        {
            yield return mainBox.SkipConfirm(newMove.MoveBase.MoveName);
            yield return mainBox.PauseAfterText();

            state = BattleState.Choice;
            mainBox.EnableChoiceBox(true);

            while(state == BattleState.Choice)
            {
                yield return null;
            }

            if(!choice)
            {
                yield return ForgetAMove(p, newMove);
            }
        }
        else
        {
            yield return mainBox.NotAValidChoice();
            yield return ForgetAMove(p, newMove);
            //Can theoretically get here if somehow move selection is opened
            //for a Pokemon with less than 4 moves leading to an empty move
            //slot that if selected would lead here, but that should not be
            //possible if everything is coded properly.
        }

    }

    IEnumerator LearnNewMove(Pokemon p, PokemonMove newMove)
    {
        Debug.Log("New Move Learnable");
        int slot = p.AvailableMoveSlot();
        if(slot == -1)
        {
            yield return mainBox.NewMove(p.Nickname, newMove.MoveBase.MoveName);
            yield return mainBox.PauseAfterText();

            state = BattleState.Choice;
            mainBox.EnableChoiceBox(true);

            while(state == BattleState.Choice)
            {
                yield return null;
            }

            if(choice)
            {
                yield return mainBox.WhichMove(newMove.MoveBase.MoveName);
                yield return mainBox.PauseAfterText();
                yield return ForgetAMove(p, newMove);
            }  
        }
        else
        {
            p.LearnMove(newMove, slot);
            yield return mainBox.MoveLearned(p.Nickname, newMove.MoveBase.MoveName);
            yield return mainBox.PauseAfterText();
        }
    }

    IEnumerator GainExp()
    {
        int exp = CalculateExperience();
        playerPokemon.GainExp(exp);
        playerPokemon.ModifyEVs(enemyPokemon.Species.EvYield);
        yield return mainBox.GainExp(playerPokemon, exp);
        yield return mainBox.PauseAfterText();
        
        while(playerPokemon.LevelUp())
        {
            yield return playerHUD.LevelUp();
            yield return mainBox.LevelUp(playerPokemon, playerPokemon.Level);
            yield return mainBox.PauseAfterText();
            List<PokemonMove> newMoves = playerPokemon.NewMoveAtCurLevel();
            if(newMoves.Count > 0)
            {
                foreach(PokemonMove m in newMoves)
                {
                    yield return LearnNewMove(playerPokemon, m);
                }
            }
        }

        yield return playerHUD.UpdateXP(playerPokemon.GetExpPercent());
        yield return mainBox.PauseAfterText();
    }

    IEnumerator SwitchPokemon(Pokemon pokemon)
    {
        sideBox.Clear();

        playerHUD.gameObject.SetActive(false);
        if(switchReason != SwitchReason.Fainted)
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
    }

    //-------------Pokemon Catching Methods-----------------

    IEnumerator ThrowPokeball(float ballMultiplier) //Add parameter for type of ball
    {
        yield return mainBox.UsedBall("Poke Ball");
        ballSprite.gameObject.SetActive(true);
        yield return ballSprite.Thrown();
        
        yield return enemySprite.Return();


        int success = TryCapture(ballMultiplier);
        Debug.Log($"Catch success = {success}");
        if(success < 0)
        {
            yield return ballSprite.Critical();
            if(success == -1)
            {
                yield return ballSprite.Shake();
                yield return ballSprite.BreakOut();
                yield return enemySprite.SendOut();
                yield return mainBox.BreakOut(0);
                yield return mainBox.PauseAfterText();
            }
            else if(success == -2)
            {
                yield return ballSprite.Shake();
                yield return ballSprite.Catch();
                yield return mainBox.Caught(enemyPokemon.Nickname);
                yield return mainBox.PauseAfterText();
                yield return ballSprite.BreakOut(); //Get rid of the ball sprite
                yield return CatchSuccess();
            }
            else
            {
                Debug.Log($"How did you get {success} from TryCapture()?!?!?!");
            }
        }
        else if(success >= 0 && success <= 4)
        {            
            yield return ballSprite.Fall();
            for(int i = 0; i < Mathf.Min(success, 3); i++)
            {
                yield return ballSprite.Shake();
            }
            if(success == 4)
            {
                yield return ballSprite.Catch();
                yield return mainBox.Caught(enemyPokemon.Nickname);
                yield return mainBox.PauseAfterText();
                yield return ballSprite.BreakOut(); //Get rid of the ball sprite
                yield return CatchSuccess();
            }
            else
            {
                yield return ballSprite.BreakOut();
                yield return enemySprite.SendOut();
                yield return mainBox.BreakOut(success);
                yield return mainBox.PauseAfterText();
            }
        }
        else
        {
            Debug.Log($"How did you get {success} from TryCapture()?!?!?!");
        }
    }

    int TryCapture(float ballMultiplier)
    {
        //if Route 1 
        //success

        float modCatchRate = (3 * enemyPokemon.Stats.HP) - (2 * enemyPokemon.CurHP);
        Debug.Log($"Catch Rate 1: {modCatchRate}");
        modCatchRate *= 4096;
        modCatchRate += 0.5f;
        modCatchRate = Mathf.FloorToInt(modCatchRate);
        Debug.Log($"Catch Rate 2: {modCatchRate}");
        //Dark Grass modifier (see wiki if/when you implement dark grass)
        //if(Heavy Ball Used)
        //int heavyBall = Heavy Ball modifier
        modCatchRate *= enemyPokemon.Species.CatchRate; //+ heavyBall;
        modCatchRate *= ballMultiplier;
        //Calculate badgePenalty based on badges missing
        modCatchRate = modCatchRate/(3 * enemyPokemon.Stats.HP);  //(modCatchRate * BadgePenalty)
        if(enemyPokemon.Level <= 13)
        {
            modCatchRate = ((36-(2*enemyPokemon.Level)) * modCatchRate)/10;
        }
        modCatchRate = Mathf.FloorToInt(modCatchRate);
        Debug.Log($"Catch Rate 3: {modCatchRate}");
        modCatchRate *= GetStatusCatchBonus(enemyPokemon);
        //modCatchRate *= (410/4096) if wild pokemon's level > player pokemon's level && <8 gym badges
        //modCatchRate *= miscellaneous (2 for a backstrike, various for capture powers of some sort)
        modCatchRate = Mathf.Min(modCatchRate, 1044480);
        Debug.Log($"Catch Rate 4: {modCatchRate}");

        float critModifier = 1f;
        //int catchingCharm = 2;
        int critRate = Mathf.FloorToInt(715827883f * modCatchRate * critModifier/(4294967296f*4096f));
        Debug.Log($"Crit Rate = {critRate}");
        
        int shakeProb = Mathf.FloorToInt(65536 / Mathf.Pow(1044480/modCatchRate, .1875f));
        Debug.Log($"Shake Prob = {shakeProb}");

        if(UnityEngine.Random.Range(0, 256) < critRate)
        {
            if(ShakeCheck(shakeProb))
            {
                return -2; //Successful Crit Capture
            }
            else
            {
                return -1; //Failed Crit Capture
            }
        }
        else
        {
            for(int i = 0; i < 4; i++)
            {
                if(!ShakeCheck(shakeProb))
                {
                    return i; //Return # of shakes
                }
            }
            return 4; //Successful catch
        }
    }

    bool ShakeCheck(int shakeProb)
    {
        return UnityEngine.Random.Range(0, 65536) < shakeProb;
    }

    float GetStatusCatchBonus(Pokemon p)
    {
        if(p.Status == NonVolatileStatus.Sleep || p.Status == NonVolatileStatus.Freeze)
        {
            return 2.5f;
        }
        if(p.Status == NonVolatileStatus.Paralysis || p.Status == NonVolatileStatus.Poison || p.Status == NonVolatileStatus.BadlyPoisoned || p.Status == NonVolatileStatus.Burn)
        {
            return 1.5f;
        }
        return 1f;
    }

    IEnumerator CatchSuccess()
    {
        enemyPokemon.IsPlayer = true;
        if(playerParty.AddPokemon(enemyPokemon))
        {
            yield return mainBox.AddedToParty(enemyPokemon.Nickname);
        }
        else if(PC.Instance.AddPokemon(enemyPokemon))
        {            
            yield return mainBox.PartyFull(enemyPokemon.Nickname);
        }
        else
        {
            Debug.Log("No room in party or PC");
        }
        //Experience gain
        CleanupBattle();
        OnBattleOver(true);
        yield return null;
    }

    //-----------------------------------------------------------

    IEnumerator RunAttempt()
    {
        if(playerPokemon.Type1 == PokemonType.Ghost || playerPokemon.Type2 == PokemonType.Ghost)
        {
            yield return RunSuccess();
        }

        // if(enemyPokemon.Ability == "Arena Trap") //Shadow Tag & Magnet Pull if Steel Type ; Binding moves and Ingrain
        // {
        //     //yield return mainBox.FleePrevention()
        //     yield return mainBox.PauseAfterText();
        // }

        // if(Smoke Ball or Run Away)
        // {
        //     Smoke Ball message???
        //     yield return RunSuccess();
        // }

        if(playerPokemon.Stats.Spe >= enemyPokemon.Stats.Spe)
        {
            yield return RunSuccess();
        }
        else
        {
            numRunAttempts++;
            float escapeOdds = (Mathf.Floor((playerPokemon.Stats.Spe * 32)/(enemyPokemon.Stats.Spe / 4)) + 30) * numRunAttempts;
            Debug.Log($"Run Attempt #{numRunAttempts}: Player Speed = {playerPokemon.Stats.Spe}; Enemy Speed = {enemyPokemon.Stats.Spe}; Escape Odds = {escapeOdds}");

            if(UnityEngine.Random.Range(0, 256) < escapeOdds)
            {
                yield return RunSuccess();
            }
            else
            {
                yield return RunFailure();
            }
        }        
    }

    IEnumerator RunSuccess()
    {
        yield return mainBox.Run();
        yield return mainBox.PauseAfterText();
        CleanupBattle();
        OnBattleOver(false);
        yield break;
    }

    IEnumerator RunFailure()
    {
        yield return mainBox.RunFailure();
        yield return mainBox.PauseAfterText();
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

        numRunAttempts = 0;

        playerHUD.gameObject.SetActive(false);
        enemyHUD.gameObject.SetActive(false);

        enemyPokemon.EndBattle();

        playerParty.EndBattle();

        enemyPokemon.IsActive = false;
        playerPokemon.IsActive = false;
    }
//----------------------Damage Calculations-----------------------

    IEnumerator CalculateDamage(PokemonMove move, Pokemon attacker, Pokemon defender)
    {   
        attacker.Stats.print();
        attacker.Ivs.print();
        defender.Stats.print();
        defender.Ivs.print();

        PokemonType moveType = move.MoveBase.MoveType;
        int level = attacker.Level;
        int power = GetPower(move, attacker, defender);
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

        //Calculate moves that use other stats for attack/defense (update weather boost as well)
        D = Mathf.FloorToInt(BoostDefForWeather(move, defender) * D);

        //Calculate crit (ignoring stat changes)
        
        float critical = 1f;
        bool crit = DetermineCrit(move, attacker, defender);
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

        float weat = CalculateWeatherBoost(move, moveType);
        if(weat == 0f)
        {
            yield return mainBox.WeatherFail(weather);
            yield return mainBox.PauseAfterText();
            yield break;
        }

        float glaiveRush = 1f; //Implement with Glaive Rush Attack\

        float random = (UnityEngine.Random.Range(85,101))/100f;

        float stab = CalculateStab(moveType, attacker);

        float type = CalculateTypeAdvantage(moveType, move, defender, attacker);

        float burn = 1f;
        if(attacker.Status == NonVolatileStatus.Burn && move.MoveBase.Category == MoveCategory.Physical) //&& attacker.Ability != "Guts" && move.MoveBase.MoveName != "Facade")
        {
            burn = 0.5f;
        }

        float other = 1f; //CalculateOther(move, attacker, defender);
        float zMove = 1f;
        float teraShield = 1f;

        Debug.Log($"Targets: {targets}; PB: {pb}; Weather: {weat}; Glaive Rush: {glaiveRush}; Random: {random}; STAB: {stab}; Type: {type}; Burn: {burn}; Other: {other}; Zmove: {zMove}; Tera Shield: {teraShield}");

        //-----------------Formula---------------------------

        int levelCalc = Mathf.FloorToInt((2*level)/5) + 2;
        int top = Mathf.FloorToInt(levelCalc*power*A/D);
        int baseDamage = Mathf.FloorToInt((top/50)+2);

        int damage = Round(baseDamage * targets);
        damage = Round(damage * pb);
        damage = Round(damage * weat);
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
    
        if(attacker.IsPlayer)
        {
            enemyPokemon.TakeDamage(damage);
            yield return enemyHUD.UpdateHP();
        }
        else
        {
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

        //Damage Based Healing and Recoil calculations 
    }

    bool DetermineCrit(PokemonMove move, Pokemon att, Pokemon def)
    {        
        if(def.Ability == "Battle Armor" || def.Ability == "Shell Armor") //Implement Lucky Chant
        {
            return false;
        }
        if(att.LaserFocus || att.Ability == "Merciless" && (def.Status == NonVolatileStatus.Poison || def.Status == NonVolatileStatus.BadlyPoisoned))
        {
            return true;
        }
        if(att.IsPlayer)
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

    float BoostDefForWeather(PokemonMove move, Pokemon defender)
    {
        if(weather == Weather.Sand && 
          (defender.Type1 == PokemonType.Rock || defender.Type2 == PokemonType.Rock) &&
          move.MoveBase.Category == MoveCategory.Special)
        {
            return 1.5f;
        }
        if(weather == Weather.Snow && 
          (defender.Type1 == PokemonType.Ice || defender.Type2 == PokemonType.Ice) &&
          move.MoveBase.Category == MoveCategory.Physical)
        {
            return 1.5f;
        }
        return 1f;
    }

    int GetPower(PokemonMove move, Pokemon att, Pokemon def)
    {
        //https://bulbapedia.bulbagarden.net/wiki/Power
        int basePower = move.MoveBase.BasePower;

        //Variable Power Calculations

        int modifier = 4096;

        //2x Facade/Brine/Venoshock/Barb Barrage/Retaliate/Fusion Flare/Fusion Bolt/Lash Out
        //0.5x Solar Beam/Solar Blade in rain/sand/snow & no cloud nine/air lock
        //1.5 Knock Off/Grav Apple/Misty Explosion/Expanding Force/Psyblade
        //2x Charge && Electric Move
        //1.5x Me First
        //1.5x Helping Hand (multiple can stack)
        //1352/4096x Mud Sport/Water Sport
        //0.5x Earthquake/Magnitude/Bulldoze in Grassy Terrain
        if(terrain == Terrain.Misty && move.MoveBase.MoveType == PokemonType.Dragon)
        {
            modifier = RoundUp(modifier * 0.5f);
        }
        
        if(terrain == Terrain.Electric && move.MoveBase.MoveType == PokemonType.Electric)
        {
            modifier = RoundUp(modifier * 5325f/4096f);
        }
        if(terrain == Terrain.Grassy && move.MoveBase.MoveType == PokemonType.Grass)
        {   
            modifier = RoundUp(modifier * 5325f/4096f);
        }
        if(terrain == Terrain.Psychic && move.MoveBase.MoveType == PokemonType.Psychic)
        {
            modifier = RoundUp(modifier * 5325f/4096f);
        }

        //1.25x Rivalry
        //Supreme Overlord
        //4915/4096x Reckless (recoil move)
        //4915/4096x Iron Fist (punching move)
        //4915/4096x Normalize ability
        //4915/4096x Aerilate/Pixilate/Refrigerate/Galvanize (Base move is normal type)
        //5325/4096x Analytic (target already moved)
        //5325/4096x Sand Force (if sand & move is ground/rock/steel & no cloud nine/air lock)
        //5325/4096x Sheer Force (if move has additional effect)
        //5325/4096x Tough Claws (contact)
        //5325/4096x Battery (on ally & special move)
        //5325/4096x Power Spot (on ally)
        //5325/4096x Punk Rock (sound based move)
        //5448/4096x Dark Aura/Fairy Aura (dark/fairy type move & no mold breaker)
        //0.7x "" && Aura Break
        //1.5x Strong Jaw (Biting Move)
        //1.5x Mega Launcher (Aura/Pulse Move)
        //1.5x Technician (base power <= 60)
        //1.5x Toxic Boost
        //1.5x Flare Boost
        //1.5x Steely Spirit
        //0.5x Heatproof
        //1.25x Dry Skin
        //1.5x Sharpness (Slashing Move)
        //4505/4096x Muscle Band/Wise Glasses
        //4915/4096x Type-Boost Item/Incense/Plate
        //4915/4096x Adamant Orb/Lustrous Orb/Griseous Orb/Soul Dew
        //5325/4096x Normal Gem
        //4506/4096x Punching Glove (Punching Move)

        Debug.Log($"Modifier pre-round {modifier}");

        modifier = RoundUp(modifier/4096f);

        Debug.Log($"Base Power: {basePower}; Power: {basePower * modifier}; Modifier: {modifier}");
        return Round(((float)basePower) * modifier);

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
            if(type == PokemonType.Fire || move.MoveBase.MoveName == "Hydro Steam")
            {
                return 1.5f;
            }
            if(type == PokemonType.Water)
            {
                return 0f;
            }
        }
        if(weather == Weather.Rain)
        {
            if(type == PokemonType.Water)
            {
                return 1.5f;
            }
            if(type == PokemonType.Fire)
            {
                return 0.5f;
            }
        }
        if(weather == Weather.HeavyRain)
        {
            if(type == PokemonType.Water)
            {
                return 1.5f;
            }
            if(type == PokemonType.Fire)
            {
                return 0f;
            }
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
        PokemonType t1 = defender.Type1;
        PokemonType t2 = defender.Type2;      
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

//--------------------Custom Math Functions-----------------------

    int Round(float f)  //Round to nearest with .5 rounding down
    {
        if(f < 0)
        {
            return (int) Math.Floor(f + 0.5f);
        }
        return (int) Math.Ceiling(f - 0.5f);
    }

    int RoundUp(float f)  //Round to nearest with .5 rounding up
    {
        if(f < 0)
        {
            return (int) Math.Ceiling(f - 0.5f);
        }
        return (int) Math.Floor(f + 0.5f);
    }

    float RandomPercent()
    {
        float r;

        do
        {
            r = UnityEngine.Random.value;
        }while(r == 0f);    //if random is exactly 0, reroll

        return r;
    }

//------------------------Battle Functions--------------------------------------

    BattleChoice getEnemyChoice()
    {
        if(isTrainerBattle)
        {
            //Logic to decide if you want to use an item or switch

            return BattleChoice.Move;
        }
        else
        {
            return BattleChoice.Move;
        }
    }

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

    void HandleChoice()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) ||
           Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
           Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) ||
           Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            choice = !choice;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            choice = false;
        }
        mainBox.UpdateChoiceSelection(choice);
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            state = BattleState.Busy;
            mainBox.EnableChoiceBox(false);       
        }
    }

    void HandleForgetMove()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || 
           Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            moveChoice++;
            if(moveChoice > 4)
            {
                moveChoice = 0;
            }
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            moveChoice--;
            if(moveChoice < 0)
            {
                moveChoice = 4;
            }
        }

        Debug.Log($"New Choice = {moveChoice}");
        moveSelection.UpdateMoveSelection(moveChoice);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            state = BattleState.Busy;
            moveSelection.gameObject.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            state = BattleState.Busy;
            moveSelection.gameObject.SetActive(false);
        }
    }

    void HandleInput()
    {
        if(state == BattleState.Pokemon)
        {
            partyScreen.HandleInput(switchReason == SwitchReason.Fainted);
            return;
        }
        if(state == BattleState.Choice)
        {
            HandleChoice();
            return;
        }
        if(state == BattleState.ForgetMove)
        {
            HandleForgetMove();
            return;
        }
        if(state == BattleState.Busy)
        {
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
                if(curMoveOption < playerPokemon.Moves.Count && playerPokemon.Moves[curMoveOption] != null && playerPokemon.Moves[curMoveOption].CurPP > 0)
                {
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
            mainBox.UpdateMoveSelection(curMoveOption);
            if(curMoveOption < playerPokemon.Moves.Count)
            {
                sideBox.updateMoveDetails(playerPokemon.Moves[curMoveOption]);
            }
            else
            {
                sideBox.updateMoveDetails(null);
            }
        }
    }
}

//Implement option to run after pokemon fainted
//Implement fading into trainer battle (requires minor refactoring somewhere to allow for waiting on the coroutine)