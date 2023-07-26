using System.Collections;
using System.Collections.Generic;
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
    string ability;
    public string Ability{get{return ability;}}
    PokemonType teraType;
    public PokemonType TeraType{get{return teraType;}}
    string heldItem = "";
    public string HeldItem{get{return heldItem;}}

    PokemonNature nature;
    PokemonNature natureMint = PokemonNature.None;
    StatBlock stats = new StatBlock(0,0,0,0,0,0);
    public StatBlock Stats{get{return stats;}}
    StatBlock evs = new StatBlock(0,0,0,0,0,0);
    StatBlock ivs = new StatBlock(0,0,0,0,0,0);
    public StatBlock Ivs{get{return ivs;}}
    StatBlock hyperTrained = new StatBlock(0,0,0,0,0,0);
    
    PokemonMove[] moves = new PokemonMove[4];
    public PokemonMove[] Moves{get{return moves;}}

//-------------------In-battle Effects------------------
//Lasts outside of battle
    NonVolatileStatus status;
    public NonVolatileStatus Status{get{return status;}}    
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

//-------------Art-----------------
    GameObject model;
    Sprite sprite;
    public Sprite Sprite{get{return sprite;}}

//-------------------------Constructors---------------------

    public Pokemon(PokemonSpecies species, int level, bool isShiny, bool isHiddenAbility, bool isWild)
    {
        this.species = species;
        nickname = species.SpeciesName;
        this.level = level;
        this.isShiny = isShiny;
        this.isWild = isWild;
        
        RandomizeIVs();
        RandomizeNature();
        RandomizeTeraType(true);        
        DetermineGender();
        DetermineAbility(isHiddenAbility);
        DetermineWildHeldItem();
        DetermineMoves();
        CalculateStats();
        SetModelAndSprite();
    }

    //public Pokemon(PokemonSpecies species, int level, bool isShiny, bool isHiddenAbility){}
    //public Pokemon(PokemonSpecies species, int level, bool isShiny, bool isHiddenAbility){}
    //public Pokemon(PokemonSpecies species, int level, bool isShiny, bool isHiddenAbility){}
    //public Pokemon(PokemonSpecies species, int level, bool isShiny, bool isHiddenAbility){}

//-------------------Initialization Functions------------------

    void RandomizeIVs()
    {
        int hp = Random.Range(0, 32);
        int atk = Random.Range(0, 32);
        int def = Random.Range(0, 32);
        int spa = Random.Range(0, 32);
        int spd = Random.Range(0, 32);
        int spe = Random.Range(0, 32);

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
            
            PokemonType t2 = species.Type2;
            if(t2 == PokemonType.None || Random.Range(0,2) == 0)
            {
                teraType = species.Type1;
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
        List<(string,int)> items = species.WildHeldItems;
        if(items != null && items.Count > 0)
        {
            string[] itemTable = new string[100];
            int i = 0;
            foreach((string,int) item in items)
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
            moves[i] = new PokemonMove(m.MoveBase);
        }
    }

    void CalculateStats()
    {
        stats.HP = HPFormula();        
        if(species.SpeciesName == "Shedinja") 
        {
            stats.HP = 1;
        }
        curHP = stats.HP;

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
        status = NonVolatileStatus.None;
        foreach(PokemonMove move in moves)
        {
            move.CurPP = move.MaxPP;
        }
    }

    public void SwitchOut()
    {
        statChanges = new StatBlock(0,0,0,0,0,0);
        accuracy = 0;
        evasion = 0;
        nGas = false;
        tarShot = false;
        forestsCurse = false;
        trickOrTreat = false;
        soak = false;
        laserFocus = false;
    }

    public void EndBattle()
    {
        isTera = false;
        SwitchOut();
    }
    
    public void TakeDamage(int damage)
    {
        curHP -= damage;
        if(curHP <= 0)
        {
            curHP = 0;
            fainted = true; //fainted
        }
    }

    public void Terastalize()
    {
        isTera = true;
    }


}

public enum PokemonGender
{
    Genderless,
    Male,
    Female
}

public enum PokemonNature
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