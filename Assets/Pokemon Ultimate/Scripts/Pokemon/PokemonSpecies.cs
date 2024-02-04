using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonSpecies : ScriptableObject
{
    [Header("Metadata")]
    [SerializeField] string speciesName;
    public string SpeciesName{get{return speciesName;}}
    [TextArea]
    [SerializeField] string description;
    [SerializeField] string classification;
    [SerializeField] int dexNo;

    [Header("Battle Info")]
    [SerializeField] PokemonType type1;
    public PokemonType Type1{get{return type1;}}
    [SerializeField] PokemonType type2;
    public PokemonType Type2{get{return type2;}}
    [SerializeField] string ability1;
    public string Ability1{get{return ability1;}}
    [SerializeField] string ability2;
    public string Ability2{get{return ability2;}}
    [SerializeField] string hiddenAbility;
    public string HiddenAbility{get{return hiddenAbility;}}
    [SerializeField] StatBlock baseStats;
    public StatBlock BaseStats{get{return baseStats;}}

    //[SerializeField] int[] baseStats = new int[6];
    //public int[] BaseStats{get{return baseStats;}}

    [Header("Breeding Info")]
    [Tooltip("Percent chance of being male; Use a negative number if genderless")]
    [SerializeField] [Range(-.01f, 1f)]float genderRatio;
    public float GenderRatio{get{return genderRatio;}}
    [SerializeField] EggGroup eggGroup1;
    [SerializeField] EggGroup eggGroup2;
    [SerializeField] int hatchTime;

    [Header("Size Info")]
    [Tooltip("Use metric (easier to program)")]
    [SerializeField] float height;
    [SerializeField] float weight;
    
    [Header("Other Info")]
    [SerializeField] int baseHappiness;
    [SerializeField] int catchRate;
    public int CatchRate {get => catchRate;}
    [SerializeField] int baseXPYield;
    public int BaseXPYield {get => baseXPYield;}
    [SerializeField] LevelingRate levelingRate;
    public LevelingRate LevelingRate {get => levelingRate;}
    [SerializeField] StatBlock evYield;
    public StatBlock EvYield {get => evYield;}

    //TODO: Rework wildHeldItems to be serializable
    [SerializeField] List<(ItemBase, int)> wildHeldItems;
    public List<(ItemBase, int)> WildHeldItems{get{return wildHeldItems;}}

    [Header("Pokemon Models")]
    [SerializeField] GameObject maleModel;
    public GameObject MaleModel {get{return maleModel;}}
    [SerializeField] GameObject femaleModel;
    public GameObject FemaleModel {get{return femaleModel;}}
    [SerializeField] GameObject maleModelShiny;
    public GameObject MaleModelShiny {get{return maleModelShiny;}}
    [SerializeField] GameObject femaleModelShiny;
    public GameObject FemaleModelShiny {get{return femaleModelShiny;}}
    [SerializeField] Sprite maleSprite;
    public Sprite MaleSprite {get{return maleSprite;}}
    [SerializeField] Sprite femaleSprite;
    public Sprite FemaleSprite {get{return femaleSprite;}}
    [SerializeField] Sprite maleSpriteShiny;
    public Sprite MaleSpriteShiny {get{return maleSpriteShiny;}}
    [SerializeField] Sprite femaleSpriteShiny;
    public Sprite FemaleSpriteShiny {get{return femaleSpriteShiny;}}

    [SerializeField] List<LearnableMove> learnset;
    public List<LearnableMove> Learnset{get{return learnset;}}
    [SerializeField] List<PokemonMoveBase> tmLearnset;


    //[Header("Movepool")]
    //Evolutions
    //TMs
    //Egg Moves
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] PokemonMoveBase moveBase;
    public PokemonMoveBase MoveBase{get{return moveBase;}}
    [Tooltip("Use -1 if learned when it evolves and 0 if it's a starting move")]
    [SerializeField] [Range(-1,100)] int levelLearned;
    public int LevelLearned{get{return levelLearned;}}
}

[System.Serializable]
public class StatBlock
{
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] int def;
    [SerializeField] int spa;
    [SerializeField] int spd;
    [SerializeField] int spe;

    public int HP{get{return hp;}set{hp=value;}}  //HP
    public int Atk{get{return atk;}set{atk=value;}} //Attack
    public int Def{get{return def;}set{def=value;}} //Defense
    public int SpA{get{return spa;}set{spa=value;}} //Sepcial Attack
    public int SpD{get{return spd;}set{spd=value;}} //Special Defense
    public int Spe{get{return spe;}set{spe=value;}} //Speed

    public StatBlock(int hp, int atk, int def, int spa, int spd, int spe)
    {
        HP = hp;
        Atk = atk;
        Def = def;
        SpA = spa;
        SpD = spd;
        Spe = spe;
    }

    public void print()
    {
        Debug.Log($"HP: {HP}; Atk: {Atk}; Def: {Def}; SpA: {SpA}; SpD: {SpD}; Spe: {Spe}");
    }
}

public class TypeChart
{
    static float [][] chart =
    {
        new float[18]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,0.0f,1.0f,1.0f,0.5f,1.0f},
        new float[18]{1.0f,0.5f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,1.0f,2.0f,1.0f},
        new float[18]{1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f,1.0f},
        new float[18]{1.0f,0.5f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,0.5f,1.0f,0.5f,1.0f},
        new float[18]{1.0f,1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f},
        new float[18]{1.0f,0.5f,0.5f,2.0f,1.0f,0.5f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f},
        new float[18]{2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f,0.5f,0.5f,2.0f,0.0f,1.0f,2.0f,2.0f,0.5f},
        new float[18]{1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,0.0f,2.0f},
        new float[18]{1.0f,2.0f,1.0f,0.5f,2.0f,1.0f,1.0f,2.0f,1.0f,0.0f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f},
        new float[18]{1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,1.0f},
        new float[18]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,0.0f,0.5f,1.0f},
        new float[18]{1.0f,0.5f,1.0f,2.0f,1.0f,1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,0.5f,1.0f,2.0f,0.5f,0.5f},
        new float[18]{1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f},
        new float[18]{0.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f},
        new float[18]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,0.0f},
        new float[18]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f},
        new float[18]{1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,2.0f},
        new float[18]{1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,0.5f,1.0f}
    };

    public static float GetTypeEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if(attackType == PokemonType.None || defenseType == PokemonType.None)
        {
            return 1f;
        }

        int row = (int) attackType - 1;
        int col = (int) defenseType - 1;

        return chart[row][col];
    }
}

public enum PokemonType     //DO NOT CHANGE THIS AGAIN OR IT WILL MESS UP THE TYPE CHART
{   
    None,
    Normal,
    Fire,
    Water,
    Grass,
    Electric,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum EggGroup
{
    None,
    Monster,
    Water1,
    Bug,
    Flying,
    Field,
    Fairy,
    Grass,
    HumanLike,
    Water3,
    Mineral,
    Amorphous,
    Water2,
    Dragon
}

public enum LevelingRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating
}

    // public float[][] TYPE_CHART = 
    // {
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f},
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,0.0f,1.0f,1.0f,0.5f,1.0f},
    //     new float[19]{1.0f,1.0f,0.5f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,1.0f,2.0f,1.0f},
    //     new float[19]{1.0f,1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f,1.0f},
    //     new float[19]{1.0f,1.0f,0.5f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,0.5f,1.0f,0.5f,1.0f},
    //     new float[19]{1.0f,1.0f,1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f},
    //     new float[19]{1.0f,1.0f,0.5f,0.5f,2.0f,1.0f,0.5f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f},
    //     new float[19]{1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f,0.5f,0.5f,2.0f,0.0f,1.0f,2.0f,2.0f,0.5f},
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,0.0f,2.0f},
    //     new float[19]{1.0f,1.0f,2.0f,1.0f,0.5f,2.0f,1.0f,1.0f,2.0f,1.0f,0.0f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f},
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,1.0f},
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,0.0f,0.5f,1.0f},
    //     new float[19]{1.0f,1.0f,0.5f,1.0f,2.0f,1.0f,1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,0.5f,1.0f,2.0f,0.5f,0.5f},
    //     new float[19]{1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f},
    //     new float[19]{1.0f,0.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f},
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,0.0f},
    //     new float[19]{1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f},
    //     new float[19]{1.0f,1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,2.0f},
    //     new float[19]{1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,0.5f,1.0f}
    // };