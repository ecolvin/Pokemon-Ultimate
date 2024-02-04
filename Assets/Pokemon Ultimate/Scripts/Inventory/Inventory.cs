using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> items;
    public List<ItemSlot> Items {get => items;}

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

    public bool RemoveItem(ItemBase item, int qty)
    {
        foreach(ItemSlot i in items)
        {
            if(i.Item == item)
            {
                i.Quantity -= qty;
                if(i.Quantity <= 0)
                {
                    items.Remove(i);
                }
                return true;
            }
        }
        return false;
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