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
    [TextArea]
    [SerializeField] string description;

    [Header("Battle Info")]
    [SerializeField] PokemonType moveType;
    [SerializeField] MoveCategory category;
    [SerializeField] int basePower;
    [SerializeField] int pp;
    public int PP{get{return pp;}}
    [SerializeField] int accuracy;
    [SerializeField] int priority;
    [SerializeField] float critRate;
    [SerializeField] MoveRange range;
    [SerializeField] float recoilPercent;
    [SerializeField] string SecondaryEffect;   //Will need to update this eventually
    [Tooltip("-1 for a guaranteed effect")]
    [SerializeField] [Range(-1,100)] int effectChance;

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
    Status,
    Physical,
    Special
}

public enum MoveRange
{
    Target,
    Self,
    Ally,
    SelfOrAlly,
    Adjacent,
    AdjacentFoes,
    All,
    AllFoes,
    AllAllies,
    Team
}

public enum NonVolatileStatus
{
    None,
    Burn,
    Freeze,
    Paralysis,
    Poison,
    BadlyPoisoned,
    Sleep
}
