using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> items;
    public List<ItemSlot> Items {get => items;}

    public event System.Action OnUpdated;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public void SetInventory(List<ItemSlot> newItems)
    {
        if(newItems.Count > 0)
        {
            items = newItems;
        }
    }

    public bool AddItem(ItemBase item, int qty)
    {
        foreach(ItemSlot i in items)
        {
            if(i.Item == item)
            {
                i.Quantity += qty;
                if(i.Quantity > GlobalSettings.Instance.MaxItemQuantity)
                {
                    i.Quantity = GlobalSettings.Instance.MaxItemQuantity;
                    return false;
                }
                return true;
            }
        }
        Items.Add(new ItemSlot(item, qty));
        return true;
    }

    public ItemBase UseItem(int selectedItem, Pokemon selectedPokemon)
    {
        ItemBase item = Items[selectedItem].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if(itemUsed)
        {
            bool itemRemoved = RemoveItem(Items[selectedItem].Item, 1);
            if(!itemRemoved)
            {
                Debug.LogError("Tried to remove an item that didn't exist. Not sure how that's possible.");
            }
            return item;
        }
        else
        {
            Debug.Log("It has no effect!");
            return null;
        }
    }

    public ItemBase UseItem(int selectedItem, Pokemon selectedPokemon, int quantity)
    {
        /* Refactor to actually use the item multiple times additively*/

        ItemBase item = Items[selectedItem].Item;
        //int itemUsed = item.Use(selectedPokemon, quantity); //Only implemented for medicine and false for everything else
        bool itemUsed = item.Use(selectedPokemon); 
        if(itemUsed)
        {
            bool itemRemoved = RemoveItem(Items[selectedItem].Item, quantity);
            if(!itemRemoved)
            {
                Debug.LogError("Tried to remove an item that didn't exist. Not sure how that's possible.");
            }
            return item;
        }
        else
        {
            Debug.Log("It has no effect!");
            return null;
        }
    }

    public bool RemoveItem(ItemBase item, int qty)
    {
        ItemSlot itemSlot = items.First(slot => slot.Item == item);
        if(itemSlot == null)
        {
            return false;
        }
        else
        {
            itemSlot.Quantity -= qty;
            if(itemSlot.Quantity <= 0)
            {
                items.Remove(itemSlot);
            }
            OnUpdated?.Invoke();
            return true;
        }
    }
}

[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int quantity;
    public ItemBase Item {get => item;}
    public int Quantity {get => quantity; set => quantity = value;}

    public ItemSlot(ItemBase i, int qty)
    {
        item = i;
        quantity = qty;
    }
}