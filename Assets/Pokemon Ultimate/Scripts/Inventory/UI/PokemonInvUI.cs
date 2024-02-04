using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonInvUI : MonoBehaviour
{
    [SerializeField] Image sprite;
    public Image Sprite {get => sprite;}

    public bool IsMoving {get; set;}
}
