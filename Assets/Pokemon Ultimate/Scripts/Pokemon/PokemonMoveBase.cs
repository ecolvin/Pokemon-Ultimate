using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Unadded move functionality

-Secondary Effects
-Electro Ball Power
-Thunder Accuracy
-Solar Beam Charge/Weather

*/

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new move")]
public class PokemonMoveBase : ScriptableObject
{
    [Header("Metadata")]
    [SerializeField] string moveName;
    public string MoveName{get{return moveName;}}
    [TextArea]
    [SerializeField] string description;
    public string Description {get => description;}

    [Header("Battle Info")]
    [SerializeField] PokemonType moveType;
    public PokemonType MoveType{get{return moveType;}}
    [SerializeField] MoveCategory category;
    public MoveCategory Category{get{return category;}}
    [SerializeField] [Range(-1, 250)] int basePower;
    public int BasePower{get{return basePower;}}
    [SerializeField] int pp;
    public int PP{get{return pp;}}
    [SerializeField] [Range(-1,100)] int accuracy;
    public int Accuracy{get{return accuracy;}}
    [SerializeField] int priority;
    public int Priority{get{return priority;}}
    [Tooltip("-1: Can't crit (status or direct damage move); 0: Regular crit rate (1/24); 1: (1/8); 2: (1/2); 3: (1)")]
    [SerializeField] [Range(-1,3)] int critRate;
    public int CritRate{get{return critRate;}}
    [SerializeField] MoveRange range;

    [Header("Flags")]
    [SerializeField] bool contact;
    [SerializeField] bool isSound;
    [SerializeField] bool isPunch;
    [SerializeField] bool isBite;
    [SerializeField] bool isSnatchable;
    [SerializeField] bool isSlice;
    [SerializeField] bool isBullet;
    [SerializeField] bool isWind;
    [SerializeField] bool isPowder;
    [SerializeField] bool metronome;
    [SerializeField] bool gravity;
    [SerializeField] bool defrosts;   
    public bool Defrosts{get{return defrosts;}} 
    [SerializeField] bool isReflected;
    [SerializeField] bool isProtected;
    [SerializeField] bool copyable;
    [SerializeField] bool hitsFlyBounceSkyDrop;
    [SerializeField] bool hitsDig;
    [SerializeField] bool hitsDive;


    [Header("Secondary Effects")]
    [Header("User Stat Changes")]
    [SerializeField] [Range(0,1)] float selfStatChangeChance;
    public float SelfStatChangeChance{get{return selfStatChangeChance;}}
    [SerializeField] StatBlock selfStatChanges;
    public StatBlock SelfStatChanges{get{return selfStatChanges;}}
    [SerializeField] int selfAccuracyChange;
    public int SelfAccuracyChange{get{return selfAccuracyChange;}}
    [SerializeField] int selfEvasionChange;
    public int SelfEvasionChange{get{return selfEvasionChange;}}

    [Header("Enemy Stat Changes")]
    [SerializeField] [Range(0,1)] float enemyStatChangeChance;
    public float EnemyStatChangeChance{get{return enemyStatChangeChance;}}
    [SerializeField] StatBlock enemyStatChanges;
    public StatBlock EnemyStatChanges{get{return enemyStatChanges;}}
    [SerializeField] int enemyAccuracyChange;
    public int EnemyAccuracyChange{get{return enemyAccuracyChange;}}
    [SerializeField] int enemyEvasionChange;
    public int EnemyEvasionChange{get{return enemyEvasionChange;}}

    [Header("User Non-Volatile Status Effects")]
    [SerializeField] [Range(0,1)] float userNvStatusChance;
    public float UserNVStatusChance{get{return userNvStatusChance;}}
    [SerializeField] List<NonVolatileStatus> userNonVolatileStatuses;
    public List<NonVolatileStatus> UserNonVolatileStatuses{get{return userNonVolatileStatuses;}}

    [Header("User Volatile Status Effects")]
    [SerializeField] [Range(0,1)] float userVolatileStatusChance;
    public float UserVolatileStatusChance{get{return userVolatileStatusChance;}}
    [SerializeField] List<VolatileStatus> userVolatileStatuses;
    public List<VolatileStatus> UserVolatileStatuses{get{return userVolatileStatuses;}}

    [Header("Enemy Non-Volatile Status Effects")]
    [SerializeField] [Range(0,1)] float enemyNvStatusChance;
    public float EnemyNVStatusChance{get{return enemyNvStatusChance;}}
    [SerializeField] List<NonVolatileStatus> enemyNonVolatileStatuses;
    public List<NonVolatileStatus> EnemyNonVolatileStatuses{get{return enemyNonVolatileStatuses;}}

    [Header("Enemy Volatile Status Effects")]
    [SerializeField] [Range(0,1)] float enemyVolatileStatusChance;
    public float EnemyVolatileStatusChance{get{return enemyVolatileStatusChance;}}
    [SerializeField] List<VolatileStatus> enemyVolatileStatuses;
    public List<VolatileStatus> EnemyVolatileStatuses{get{return enemyVolatileStatuses;}}

    [Header("Field Effects")]
    [SerializeField] [Range(0,1)] float weatherChance;
    public float WeatherChance{get{return weatherChance;}}
    [SerializeField] Weather weather;
    public Weather Weather{get{return weather;}}

    [SerializeField] [Range(0,1)] float terrainChance;
    public float TerrainChance{get{return terrainChance;}}
    [SerializeField] Terrain terrain;
    public Terrain Terrain{get{return terrain;}}

    [SerializeField] bool lightScreen;
    public bool LightScreen{get{return lightScreen;}}
    [SerializeField] bool reflect;
    public bool Reflect{get{return reflect;}}

    [Header("Other Effects")]
    [SerializeField] [Range(0,1)] float recoil;
    public float Recoil{get{return recoil;}}
    //Other recoil (Explosion/Mind Blown/Curse)  
    [SerializeField] [Range(0,1)] float damageBasedHealing;
    public float DamageBasedHealing{get{return damageBasedHealing;}}
    [SerializeField] [Range(0,1)] float healthBasedHealing;
    public float HealthBasedHealing{get{return healthBasedHealing;}}

}


public enum MoveCategory
{
    None,
    Status,
    Physical,
    Special
}

public enum MoveRange
{
    Adjacent,
    User,
    UserOrAdjacentAlly,
    AdjacentAlly,
    AdjacentFoe,
    AnyOther,
    UserAndAllies,
    AllAllies,
    AllFoes,
    AllAdjacent,
    AllAdjacentFoes,
    AllPokemon
}