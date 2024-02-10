using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalSpriteDictionary : MonoBehaviour
{
    [SerializeField] List<TypeSprite> types;
    [SerializeField] List<MoveCatSprite> moveCategorys;

    public Dictionary<PokemonType, Sprite> TypeIcons {get; private set;}
    public Dictionary<PokemonType, Sprite> TypeBars {get; private set;} 
    public Dictionary<MoveCategory, Sprite> MoveCategorySprites {get; private set;}

    public static GlobalSpriteDictionary Instance {get; private set;}

    void Awake()
    {
        Instance = this;
        TypeIcons = types.ToDictionary(t => t.Type, t => t.Icon);   //new Dictionary<PokemonType, Sprite>();
        TypeBars = types.ToDictionary(t => t.Type, t => t.Bar);
        MoveCategorySprites = moveCategorys.ToDictionary(mc => mc.MoveCategory, mc => mc.Icon);
    }
}

[System.Serializable]
public class TypeSprite
{
    [SerializeField] PokemonType type;
    [SerializeField] Sprite icon;
    [SerializeField] Sprite bar;

    public PokemonType Type {get => type;}
    public Sprite Icon {get => icon;}
    public Sprite Bar {get => bar;}
}

[System.Serializable]
public class MoveCatSprite
{
    [SerializeField] MoveCategory moveCategory;
    [SerializeField] Sprite icon;

    public MoveCategory MoveCategory {get => moveCategory;}
    public Sprite Icon {get => icon;}
}
