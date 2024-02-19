using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pokemon
{
    string nickname;
    public string Nickname{get{return nickname;}}
    PokemonSpecies species;
    public PokemonSpecies Species{get{return species;}}
    int level;
    public int Level{get{return level;}}
    PokemonGender gender;
    public PokemonGender Gender{get{return gender;}}
    bool isShiny;    
    bool isWild = true;
    public bool IsWild{get{return isWild;}}
    public bool IsPlayer{get;set;}
    string ability;
    public string Ability{get{return ability;}}
    PokemonType type1;
    public PokemonType Type1{get{return type1;}}
    PokemonType type2;
    public PokemonType Type2{get{return type2;}}
    PokemonType teraType;
    public PokemonType TeraType{get{return teraType;}}
    ItemBase heldItem = null;
    public ItemBase HeldItem{get{return heldItem;}}

    PokemonNature nature;
    PokemonNature natureMint = PokemonNature.None;

    public PokemonNature Nature {get
    {
        if(natureMint != PokemonNature.None)
        {
            return natureMint;
        }
        else
        {
            return nature;
        }
    }}

    StatBlock stats = new StatBlock(0,0,0,0,0,0);
    public StatBlock Stats{get{return stats;}}
    StatBlock evs = new StatBlock(0,0,0,0,0,0);
    StatBlock ivs = new StatBlock(0,0,0,0,0,0);
    public StatBlock Ivs{get{return ivs;}}
    StatBlock hyperTrained = new StatBlock(0,0,0,0,0,0);
    
    List<PokemonMove> moves = new List<PokemonMove>();
    public List<PokemonMove> Moves{get{return moves;}}

//-------------------In-battle Effects------------------
//Lasts outside of battle
    NonVolatileStatus nvStatus;
    public NonVolatileStatus Status{get{return nvStatus;}}
    public int SleepCounter{get; set;}
    int poisonCounter = 0;
    public int PoisonCounter{
        get{return poisonCounter;} 
        set{
            poisonCounter = value; 
            if(poisonCounter > 15)
            {
                poisonCounter = 15;
            } 
        }
    }

    int experience = 0;

    int curHP;
    public int CurHP{get{return curHP;}}
    bool fainted = false;
    public bool Fainted{get{return fainted;}}
    public bool IsActive{get;set;}

//Lasts when switched out but not outside of battle
    bool isTera = false;
    public bool IsTera{get{return isTera;}}

//Wears off when switched out
    StatBlock statChanges = new StatBlock(0,0,0,0,0,0);
    public StatBlock StatChanges{get{return statChanges;}}
    int accuracy = 0;
    public int Accuracy{get{return accuracy;}}
    int evasion = 0;
    public int Evasion{get{return evasion;}}  
    bool nGas = false;     //Whether or not its ability has been neutralized
    public bool NGas{get{return nGas;}}
    bool tarShot = false;
    public bool TarShot{get{return tarShot;}}
    bool forestsCurse = false;
    public bool ForestsCurse{get{return forestsCurse;}}
    bool trickOrTreat = false;
    public bool TrickOrTreat{get{return trickOrTreat;}}
    bool soak = false;
    public bool Soak{get{return soak;}}
    bool laserFocus = false;
    public bool LaserFocus{get{return laserFocus;}}
    Dictionary<VolatileStatus,int> volatileStatus;
    public Dictionary<VolatileStatus,int> VolatileStatus{get{return volatileStatus;}}

    public event System.Action OnDataChange;

//-------------Art-----------------
    GameObject model;
    Sprite sprite;
    public Sprite Sprite{get{return sprite;}}

//-------------------------Constructors---------------------

    public Pokemon(PokemonSpecies species, int level, bool isShiny, bool isHiddenAbility, bool isWild, bool isPlayer)
    {
        this.species = species;
        nickname = species.SpeciesName;
        this.level = level;
        this.isShiny = isShiny;
        this.isWild = isWild;
        this.IsPlayer = isPlayer;
        
        experience = GetExpAtLevel(this.level);

        SetTypes();
        RandomizeIVs(0);
        RandomizeNature();
        RandomizeTeraType(true);        
        DetermineGender();
        DetermineAbility(isHiddenAbility);
        DetermineWildHeldItem();
        DetermineMoves();
        CalculateStats();
        curHP = stats.HP;
        SetModelAndSprite();
        ClearVolatileStatuses();
    }

    public Pokemon(PokemonSaveData saveData)
    {
        this.species = saveData.species;
        this.nickname = saveData.name;
        this.level = saveData.level;
        this.gender = saveData.gender;
        this.isShiny = saveData.shiny;
        this.ability = saveData.ability;
        this.teraType = saveData.tt;
        this.heldItem = saveData.item;
        this.nature = saveData.nat;
        this.natureMint = saveData.mint;
        this.evs = saveData.Evs;
        this.ivs = saveData.ivs;
        this.hyperTrained = saveData.ht;
        this.moves = saveData.moveList.Select(m => new PokemonMove(m)).ToList();
        this.nvStatus = saveData.status;
        this.experience = saveData.exp;
        this.curHP = saveData.hp;
        this.fainted = saveData.fainted;
    
        this.isWild = false;
        this.IsPlayer = true;

        SetTypes();
        CalculateStats();
        SetModelAndSprite();
    }

    public Pokemon(PokemonSpecies species, int level, int numPerfectIVs = 0, StatBlock evs = null, 
                   PokemonNature nature = PokemonNature.None, PokemonType teraType = PokemonType.None,
                   PokemonGender gender = PokemonGender.None, string ability = "", ItemBase heldItem = null,
                   List<PokemonMove> moves = null, bool isShiny=false, bool isWild=false, bool isPlayer=false)
    {

        this.species = species;
        nickname = species.SpeciesName;
        this.level = level;
        this.isShiny = isShiny;
        this.isWild = isWild;
        this.IsPlayer = isPlayer;

        experience = GetExpAtLevel(this.level);

        SetTypes();

        RandomizeIVs(numPerfectIVs);

        if(evs == null)
        {
            this.evs = new StatBlock(0,0,0,0,0,0);
        }
        else
        {
            this.evs = evs;
        }

        if(nature == PokemonNature.None)
        {
            RandomizeNature();
        }
        else
        {
            this.nature = nature;
        }

        if(gender == PokemonGender.None)
        {
            DetermineGender();
        }
        else
        {
            this.gender = gender;
        }

        if(ability == "")
        {
            DetermineAbility(false);
        }
        else
        {
            this.ability = ability;
        }

        this.heldItem = heldItem;

        if(moves == null || moves.Count == 0)
        {
            DetermineMoves();
        }
        else
        {
            this.moves = moves;
        }

        CalculateStats();
        curHP = stats.HP;
        SetModelAndSprite();
        ClearVolatileStatuses();
    }

//-------------------Initialization Functions------------------

    void SetTypes()
    {
        type1 = species.Type1;
        type2 = species.Type2;
    }

    void RandomizeIVs(int numRandom)
    {
        int [] perfectStats = new int[6];

        for(int i = 0; i < numRandom; i++)
        {
            int index = Random.Range(0,6);
            if(perfectStats[index] == 31)
            {
                i--;
            }
            else
            {
                perfectStats[index] = 31;
            }
        }

        int hp = perfectStats[0] == 31 ? 31 : Random.Range(0, 32);
        int atk = perfectStats[1] == 31 ? 31 : Random.Range(0, 32);
        int def = perfectStats[2] == 31 ? 31 : Random.Range(0, 32);
        int spa = perfectStats[3] == 31 ? 31 : Random.Range(0, 32);
        int spd = perfectStats[4] == 31 ? 31 : Random.Range(0, 32);
        int spe = perfectStats[5] == 31 ? 31 : Random.Range(0, 32);

        ivs = new StatBlock(hp, atk, def, spa, spd, spe);
    }

    void RandomizeNature()
    {
        nature = (PokemonNature) Random.Range(0,25); //25 not included = None is not possible
    }

    void RandomizeTeraType(bool limitTeraType)
    {
        if(limitTeraType) //Choose only one of the pokemon's types
        {
            
            PokemonType t2 = type2;
            if(t2 == PokemonType.None || Random.Range(0,2) == 0)
            {
                teraType = type1;
            }
            else
            {
                teraType = t2;
            }
        }
        else //Choose any type at random
        {
            teraType = (PokemonType) Random.Range(1,19); //0 not included = None is not possible
        }
    }

    void DetermineGender()
    {
        float ratio = species.GenderRatio;
        float rand = Random.value;
        if(ratio < 0)
        {
            gender = PokemonGender.Genderless;
        }
        else if(rand < ratio)
        {
            gender = PokemonGender.Male;
        }
        else
        {
            gender = PokemonGender.Female;
        }
    }

    void DetermineAbility(bool isHiddenAbility)
    {
        if(isHiddenAbility)
        {
            ability = species.HiddenAbility;
        }
        else
        {
            string a2 = species.Ability2;
            if(a2 == "" || Random.Range(0,2) == 0)
            {
                ability = species.Ability1;
            }
            else
            {
                ability = a2;
            }
        }
    }

    void DetermineWildHeldItem()
    {
        List<(ItemBase,int)> items = species.WildHeldItems;
        if(items != null && items.Count > 0)
        {
            ItemBase[] itemTable = new ItemBase[100];
            int i = 0;
            foreach((ItemBase,int) item in items)
            {
                for(int j = 0; j < item.Item2; ++i)
                {
                    itemTable[i] = item.Item1;
                    i++;
                }
            }
            heldItem = itemTable[Random.Range(0,100)];
        }
    }

    void DetermineMoves()
    {
        Stack<LearnableMove> moveOptions = new Stack<LearnableMove>();
        foreach(LearnableMove move in species.Learnset)
        {
            if(move.LevelLearned <= level)
            {
                moveOptions.Push(move);
            }
        }

        for(int i = 0; i < 4 && moveOptions.Count > 0; i++)
        {
            LearnableMove m = moveOptions.Pop();
            moves.Add(new PokemonMove(m.MoveBase));
        }
    }

    void CalculateStats()
    {
        stats.HP = HPFormula();        
        if(species.SpeciesName == "Shedinja") 
        {
            stats.HP = 1;
        }

        stats.Atk = StatFormula(species.BaseStats.Atk, ivs.Atk, evs.Atk, hyperTrained.Atk, 1);
        stats.Def = StatFormula(species.BaseStats.Def, ivs.Def, evs.Def, hyperTrained.Def, 2);
        stats.SpA = StatFormula(species.BaseStats.SpA, ivs.SpA, evs.SpA, hyperTrained.SpA, 3);
        stats.SpD = StatFormula(species.BaseStats.SpD, ivs.SpD, evs.SpD, hyperTrained.SpD, 4);
        stats.Spe = StatFormula(species.BaseStats.Spe, ivs.Spe, evs.Spe, hyperTrained.Spe, 5);
    }

    int HPFormula()
    {
        int hp = ((((2*species.BaseStats.HP) + ivs.HP + (evs.HP/4)) * level)/100) + level + 10;
        return hp;
    }

    int StatFormula(int baseStat, int iv, int ev, int hyperTrained, int index)
    {
        if(hyperTrained > 0)
        {
            iv = 31;
        }

        int natBonus = GetNatBonus(index);

        int stat = ((((((2*baseStat) + iv + (ev/4)) * level) / 100) + 5) * natBonus) / 100;
        return stat;
    }

    int GetNatBonus(int index)
    {
        PokemonNature nat = nature;
        if(natureMint != PokemonNature.None)
        {
            nat = natureMint;
        }

        int dec = (int) nat % 10; //Ones place of enum
        int inc = (int) nat / 10; //Tens place of enum

        if(inc == dec)
        {
            return 100;
        }

        if(index == inc)
        {
            return 110;
        }
        else if(index == dec)
        {
            return 90;
        }
        else
        {
            return 100;
        }
    }

    void SetModelAndSprite()
    {
        if(isShiny)
        {
            if(gender == PokemonGender.Female)
            {
                model = species.FemaleModelShiny;
                sprite = species.FemaleSpriteShiny;
            }
            else
            {
                model = species.MaleModelShiny;
                sprite = species.MaleSpriteShiny;
            }
        }
        else
        {
            if(gender == PokemonGender.Female)
            {
                model = species.FemaleModel;
                sprite = species.FemaleSprite;
            }
            else
            {
                model = species.MaleModel;
                sprite = species.MaleSprite;                
            }
        }
    }

    
//--------------------------Battle Functions------------------------------
    public void PC()
    {
        curHP = stats.HP;
        nvStatus = NonVolatileStatus.None;
        foreach(PokemonMove move in moves)
        {
            move.CurPP = move.MaxPP;
        }
    }

    public void SwitchOut()
    {
        nGas = false;
        tarShot = false;
        forestsCurse = false;
        trickOrTreat = false;
        soak = false;
        laserFocus = false;
        ClearVolatileStatuses();
        ClearStatChanges();
    }

    public void EndBattle()
    {
        isTera = false;
        SetTypes();
        SwitchOut();
        CalculateStats();
    }
    
    public void TakeDamage(int damage)
    {
        curHP -= damage;
        if(curHP <= 0)
        {
            curHP = 0;
            Faint();
        }
        else
        {
            fainted = false;
        }

        if(curHP > stats.HP)
        {
            curHP = stats.HP;
        }
        if(damage != 0)
        {
            OnDataChange?.Invoke();
        }
    }

    int GetExpAtLevel(int lvl)
    {
        if(lvl == 1)
        {
            return 0;
        }
        if (species.LevelingRate == LevelingRate.Erratic)
        {
            if(lvl < 50)
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * (100f - lvl))/50f);
            }
            else if(lvl < 68)
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * (150f - lvl))/100f);
            }
            else if(lvl < 98)
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * Mathf.FloorToInt((1911f - (10f*lvl))/3f))/500f);
            }
            else
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * (160f - lvl))/100f);
            }
        }
        else if(species.LevelingRate == LevelingRate.Fast)
        {
            return Mathf.RoundToInt(4f * Mathf.Pow(lvl, 3f)/5f);
        }
        else if(species.LevelingRate == LevelingRate.Fluctuating)
        {
            if(lvl < 15)
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * (Mathf.FloorToInt((lvl + 1f)/3f) + 24f))/50f);
            }
            else if(lvl < 36)
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * (lvl + 14f))/50f);
            }
            else
            {
                return Mathf.RoundToInt((Mathf.Pow(lvl, 3f) * (Mathf.FloorToInt(lvl/2f) + 32f))/50f);
            }
        }
        else if(species.LevelingRate == LevelingRate.MediumFast)
        {
            return Mathf.RoundToInt(Mathf.Pow(lvl, 3f));
        }
        else if(species.LevelingRate == LevelingRate.MediumSlow)
        {
            return Mathf.RoundToInt(((6f/5f) * Mathf.Pow(lvl, 3f)) - (15f * Mathf.Pow(lvl, 2f)) + (100f * lvl) - 140f);
        }
        else if(species.LevelingRate == LevelingRate.Slow)
        {
            return Mathf.RoundToInt(5f * Mathf.Pow(lvl, 3f)/4f);
        }
        else
        {
            Debug.Log($"Error! Invalid leveling rate: {species.LevelingRate}");
            return -1;
        }
    }

    public float GetExpPercent()
    {
        float expAtCurLevel = GetExpAtLevel(level);
        float totalExpDiff = GetExpAtLevel(level + 1) - expAtCurLevel;
        float expDiff = experience - expAtCurLevel;

        return Mathf.Clamp01(expDiff/totalExpDiff);
    }

    public void GainExp(int exp)
    {
        experience += exp;
    }

    public bool LevelUp()
    {
        if(experience < GetExpAtLevel(level+1))
        {
            return false;
        }
        level++;      
        int prevHP = stats.HP;  
        CalculateStats();        
        curHP += stats.HP - prevHP;
        OnDataChange?.Invoke();
        return true;
    }

    public bool PPFull()
    {
        for(int i = 0; i < moves.Count; i++)
        {
            if(moves[i] != null && moves[i].CurPP < moves[i].MaxPP)
            {
                return false;
            }
        }
        return true;
    }

    //Negative number for full heal
    public void Elixir(int ppHealing)
    {
        for(int i = 0; i < moves.Count; i++)
        {
            PokemonMove m = moves[i];
            if(m == null)
            {
                continue;
            }
            if(ppHealing < 0)
            {
                m.CurPP = m.MaxPP;
            }
            else
            {
                m.CurPP += ppHealing;
                m.CurPP = Mathf.Min(m.CurPP, m.MaxPP);
            }
        }
    }

    public void Ether(int ppHealing, int moveSelection)
    {
        PokemonMove m = moves[moveSelection];
        if(m == null)
        {
            return;
        }
        if(ppHealing < 0)
        {
            m.CurPP = m.MaxPP;
        }
        else
        {
            m.CurPP += ppHealing;
            m.CurPP = Mathf.Min(m.CurPP, m.MaxPP);
        }
    }

    public List<PokemonMove> NewMoveAtCurLevel()
    {
        List<PokemonMove> moves = new List<PokemonMove>();
        foreach(LearnableMove lm in species.Learnset)
        {
            if(lm.LevelLearned == level)
            {
                moves.Add(new PokemonMove(lm.MoveBase));
            }
        }
        return moves;
    }

    public bool AvailableMoveSlot()
    {
        return moves.Count < 4;
    }

    public PokemonMove ReplaceMove(PokemonMove move, int slot)
    {
        if(slot >= moves.Count)
        {
            if(moves.Count < 4)
            {
                LearnMove(move);
            }
            else
            {
                Debug.Log($"Invalid move slot: {slot}; Could not learn move {move.MoveBase.MoveName}");
            }
            return null;
        }
        PokemonMove oldMove = moves[slot];
        moves[slot] = move;
        return oldMove;
    }

    public void LearnMove(PokemonMove move)
    {
        moves.Add(move);
    }

    public void ClearStatChanges()
    {
        statChanges = new StatBlock(0,0,0,0,0,0);
        accuracy = 0;
        evasion = 0;

        CalculateStats();
    }

    public void ChangeStats(StatBlock changes)
    {   
        statChanges.Atk = Mathf.Clamp(statChanges.Atk + changes.Atk, -6, 6);
        statChanges.Def = Mathf.Clamp(statChanges.Def + changes.Def, -6, 6); 
        statChanges.SpA = Mathf.Clamp(statChanges.SpA + changes.SpA, -6, 6);        
        statChanges.SpD = Mathf.Clamp(statChanges.SpD + changes.SpD, -6, 6);        
        statChanges.Spe = Mathf.Clamp(statChanges.Spe + changes.Spe, -6, 6);

        CalculateStats();
    }

    public void ChangeAccuracy(int acc)
    {
        accuracy += acc;
    }

    public void ChangeEvasion(int eva)
    {
        evasion += eva;
    }

    public int ApplyNVStatus(NonVolatileStatus status)
    {
        if(nvStatus != NonVolatileStatus.None)
        {
            return -1;
        }
        if(status == NonVolatileStatus.Burn && (type1 == PokemonType.Fire || type2 == PokemonType.Fire))
        {
            return -2;
        }        
        if(status == NonVolatileStatus.Paralysis && (type1 == PokemonType.Electric || type2 == PokemonType.Electric))
        {
            return -2;
        }
        if(status == NonVolatileStatus.Freeze && (type1 == PokemonType.Ice || type2 == PokemonType.Ice))
        {
            return -2;
        }
        if((status == NonVolatileStatus.Poison || status == NonVolatileStatus.BadlyPoisoned) && 
            (type1 == PokemonType.Poison || type2 == PokemonType.Poison || type1 == PokemonType.Steel || type2 == PokemonType.Steel))
        {
            return -2;
        }
        else if(status == NonVolatileStatus.BadlyPoisoned)
        {
            poisonCounter = 1;
        }

        if(status == NonVolatileStatus.Sleep)
        {
            SleepCounter = UnityEngine.Random.Range(1,4);
        }
        
        nvStatus = status;
        OnDataChange?.Invoke();
        return 0;
    }

    public int ApplyVolatileStatus(VolatileStatus status)
    {      
        Debug.Log("Applying Volatile Status - " + status.ToString());  
        if(status == global::VolatileStatus.Confused)
        {
            volatileStatus[status] = UnityEngine.Random.Range(2,6);
        }
        if(status == global::VolatileStatus.Flinch)
        {
            volatileStatus[status] = 1;
        }
        if(status == global::VolatileStatus.Grounded)
        {
            volatileStatus[status] = 1;
        }
        return 0;
    }

    public void ClearNVStatus()
    {
        nvStatus = NonVolatileStatus.None;
        OnDataChange?.Invoke();
    }

    public bool ClearVolatileStatuses()
    {
        return false;
        // foreach(KeyValuePair<global::VolatileStatus,int> volStatus in volatileStatus)
        // {
        //     volatileStatus[volStatus.Key] = -1;
        // }
    }

    int GetSumOfStats(StatBlock sb)
    {
        return sb.HP + sb.Atk + sb.Def + sb.SpA + sb.SpD + sb.Spe;
    }

    public void ModifyEVs(StatBlock evChanges)
    {
        int availableEVs = 510 - GetSumOfStats(evs);

        if(availableEVs <= 0)
        {
            return;
        }
        if(evChanges.HP > availableEVs)
        {
            evs.HP = Mathf.Min(255, evs.HP + availableEVs);
        }
        else
        {
            evs.HP = Mathf.Min(255, evs.HP + evChanges.HP);
            availableEVs -= evChanges.HP;
        }

        if(availableEVs <= 0)
        {
            return;
        }
        if(evChanges.Atk > availableEVs)
        {
            evs.Atk = Mathf.Min(255, evs.Atk + availableEVs);
        }
        else
        {
            evs.Atk = Mathf.Min(255, evs.Atk + evChanges.Atk);
            availableEVs -= evChanges.Atk;
        }

        if(availableEVs <= 0)
        {
            return;
        }
        if(evChanges.Def > availableEVs)
        {
            evs.Def = Mathf.Min(255, evs.Def + availableEVs);
        }
        else
        {
            evs.Def = Mathf.Min(255, evs.Def + evChanges.Def);
            availableEVs -= evChanges.Def;
        }

        if(availableEVs <= 0)
        {
            return;
        }
        if(evChanges.Spe > availableEVs)
        {
            evs.Spe = Mathf.Min(255, evs.Spe + availableEVs);
        }
        else
        {
            evs.Spe = Mathf.Min(255, evs.Spe + evChanges.Spe);
            availableEVs -= evChanges.Spe;
        }

        if(availableEVs <= 0)
        {
            return;
        }
        if(evChanges.SpA > availableEVs)
        {
            evs.SpA = Mathf.Min(255, evs.SpA + availableEVs);
        }
        else
        {
            evs.SpA = Mathf.Min(255, evs.SpA + evChanges.SpA);
            availableEVs -= evChanges.SpA;
        }

        if(availableEVs <= 0)
        {
            return;
        }
        if(evChanges.SpD > availableEVs)
        {
            evs.SpD = Mathf.Min(255, evs.SpD + availableEVs);
        }
        else
        {
            evs.SpD = Mathf.Min(255, evs.SpD + evChanges.SpD);
            availableEVs -= evChanges.SpD;
        }
    }

    public void Terastalize()
    {
        isTera = true;
        type1 = teraType;
        type2 = PokemonType.None;
    }

    public string GetBattleMessageName()
    {
        if(IsPlayer)
        {
            return nickname;
        }
        else
        {
            if(isWild)
            {
                return "The wild " + nickname;
            }
            else
            {
                return "The opposing " + nickname;
            }
        }
    }

    public void Captured()
    {
        IsPlayer = true;
        isWild = false;
        //Set OT, capture date/location, etc...
    }

    public PokemonSaveData GetSaveData()
    {
        //Debug.Log($"Creating save data for {nickname}");
        return new PokemonSaveData()
        {
            species = Species,
            name = nickname,
            level = Level,
            gender = Gender,
            shiny = isShiny,
            ability = Ability,
            tt = TeraType,
            item = HeldItem,
            nat = nature,
            mint = natureMint,
            Evs = evs,
            ivs = Ivs,
            ht = hyperTrained,
            moveList = moves.Select(m => m?.GetSaveData()).ToList(),
            status = nvStatus,
            exp = experience,
            hp = curHP,
            fainted = Fainted
        };
    }

    void Faint()
    {
        curHP = 0;
        fainted = true;
        ClearNVStatus();
    }

    public ItemBase GiveItem(ItemBase newItem)
    {
        ItemBase oldItem = heldItem;
        heldItem = newItem;
        OnDataChange?.Invoke();
        return oldItem;
    }

}

[System.Serializable]
public class PokemonSaveData
{
    public PokemonSpecies species;
    public string name;
    public int level;
    public PokemonGender gender;
    public bool shiny;
    public string ability;
    public PokemonType tt;
    public ItemBase item;
    public PokemonNature nat;
    public PokemonNature mint;
    public StatBlock Evs;
    public StatBlock ivs;
    public StatBlock ht;    
    public List<MoveSaveData> moveList;
    public NonVolatileStatus status;
    public int exp;
    public int hp;
    public bool fainted;
};

public enum PokemonGender
{
    Genderless,
    Male,
    Female,
    None
}

public enum PokemonNature   //## = Increase Decrease (e.g. 24 = increase Defense, decrease Sp. Def)
{
    Hardy = 11,   
    Lonely = 12,
    Adamant = 13,
    Naughty = 14,
    Brave = 15,

    Bold = 21,
    Docile = 22,
    Impish = 23,
    Lax = 24,
    Relaxed = 25,

    Modest = 31,
    Mild = 32,
    Bashful = 33,
    Rash = 34,
    Quiet = 35,

    Calm = 41,
    Gentle = 42,
    Careful = 43,
    Quirky = 44,
    Sassy = 45,

    Timid = 51,
    Hasty = 52,
    Jolly = 53,
    Naive = 54,
    Serious = 55,

    None = 66
}

public enum NonVolatileStatus
{
    None,
    Burn,
    Freeze,
    Paralysis,
    Poison,
    BadlyPoisoned,
    Sleep,
    Frostbite,
    Drowsy
}

public enum VolatileStatus
{
    Bound,
    Trapped,
    Confused,
    Curse,
    Drowsy,
    Embargo,
    Encore,
    Flinch,
    Grounded,
    HealBlock,
    Identified,
    Infatuation,
    Nightmare,
    PerishSong,
    Seeded,
    Taunt,
    Telekinesis,
    Tormented
}