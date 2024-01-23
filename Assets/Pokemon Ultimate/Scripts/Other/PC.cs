using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour
{
    [SerializeField] int boxSize = 30;
    [SerializeField] int numBoxes = 32;

    List<PCBox> boxes;
    public List<PCBox> Boxes {get => boxes;}
    int curBox = 0; //Update on close of PC Screen
    public int CurBox {get => curBox;}

    public static PC Instance {get; private set;}

    void Awake()
    {
        Instance = this;
        CreateBoxes();
    }

    void CreateBoxes()
    {
        for(int i = 0; i < numBoxes; i++)
        {
            boxes.Add(new PCBox(boxSize));
        }
    }

    public bool PCFull()
    {
        foreach(PCBox box in boxes)
        {
            if(!box.IsFull())
            {
                return false;
            }
        }
        return true;
    }

    public bool AddPokemon(Pokemon p)
    {
        if(PCFull())
        {
            Debug.Log("PC is full.");
            return false;
        }

        for(int i = curBox; i < numBoxes; i++)  //curBox -> end
        {
            if(!boxes[i].IsFull())
            {
                boxes[i].AddPokemon(p);
                return true;
            }
        }
        for(int i = 0; i < curBox; i++)    //start -> curBox
        {
            if(!boxes[i].IsFull())
            {
                boxes[i].AddPokemon(p);
                return true;
            }
        }

        Debug.Log("PC is not full, but couldn't find a box that wasn't full (Something went wrong)!");
        return false;
    }    
}
