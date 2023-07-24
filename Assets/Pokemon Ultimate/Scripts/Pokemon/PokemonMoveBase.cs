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
    [SerializeField] bool isReflected;
    [SerializeField] bool isProtected;
    [SerializeField] bool copyable;
    [SerializeField] bool hitsFlyBounceSkyDrop;
    [SerializeField] bool hitsDig;
    [SerializeField] bool hitsDive;
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