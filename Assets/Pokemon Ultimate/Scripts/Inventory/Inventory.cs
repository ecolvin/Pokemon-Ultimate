using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum InventoryTab {Medicines, Balls, Battle, Berries, Other, TMs, Treasures, KeyItems}

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> medicines;
    [SerializeField] List<ItemSlot> balls;
    [SerializeField] List<ItemSlot> battleItems;
    [SerializeField] List<ItemSlot> berries;
    [SerializeField] List<ItemSlot> otherItems;
    [SerializeField] List<ItemSlot> tms;
    [SerializeField] List<ItemSlot> treasures;
    [SerializeField] List<ItemSlot> keyItems;

    Dictionary<InventoryTab, List<ItemSlot>> inventory = new Dictionary<InventoryTab, List<ItemSlot>>();
    public Dictionary<InventoryTab, List<ItemSlot>> Inv {get => inventory;}

    public event System.Action OnUpdated;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    void Awake()
    {
        SetInventory(null);
    }

    public void SetInventory(List<Tab> newInv)
    {
        inventory[InventoryTab.Medicines] = medicines != null ? medicines : new List<ItemSlot>();
        inventory[InventoryTab.Balls] = balls != null ? balls : new List<ItemSlot>();
        inventory[InventoryTab.Battle] = battleItems != null ? battleItems : new List<ItemSlot>();
        inventory[InventoryTab.Berries] = berries != null ? berries : new List<ItemSlot>();
        inventory[InventoryTab.Other] = otherItems != null ? otherItems : new List<ItemSlot>();
        inventory[InventoryTab.TMs] = tms != null ? tms : new List<ItemSlot>();
        inventory[InventoryTab.Treasures] = treasures != null ? treasures : new List<ItemSlot>();
        inventory[InventoryTab.KeyItems] = keyItems != null ? keyItems : new List<ItemSlot>();      

        if(newInv == null)
        {
            return;
        }

        foreach(Tab tab in newInv)
        {
            if(tab.items.Count > 0)
            {
                inventory[tab.tab] = tab.items;
            }
        }
    }

    public bool HasItem(ItemBase item, int quantity)
    {
        foreach(KeyValuePair<InventoryTab, List<ItemSlot>> tab in inventory)
        {
            ItemSlot slot = tab.Value.FirstOrDefault(slot => slot.Item == item);
            if(slot != null && slot.Quantity >= quantity)
            {
                return true;
            }
        }
        return false;
    }

    public bool AddItem(ItemBase item, int qty)
    {
        if(item == null)
        {
            return false;
        }
        InventoryTab tab = item.Tab;

        ItemSlot selectedSlot = inventory[tab].FirstOrDefault(i => i.Item == item);
        if(selectedSlot == null)
        {
            inventory[tab].Add(new ItemSlot(item, qty));
            OnUpdated?.Invoke();
            return true;
        }
        else
        {
            if(selectedSlot.Quantity + qty > GlobalSettings.Instance.MaxItemQuantity)
            {
                return false;
            }
            else
            {
                selectedSlot.Quantity += qty;
                OnUpdated?.Invoke();
                return true;
            }
        }
    }

    public ItemBase UseItem(int selectedItem, Pokemon selectedPokemon, InventoryTab selectedTab)
    {
        ItemBase item = inventory[selectedTab][selectedItem].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if(itemUsed)
        {
            bool itemRemoved = RemoveItem(inventory[selectedTab][selectedItem].Item, 1);
            if(!itemRemoved)
            {
                Debug.LogError("Tried to remove an item that didn't exist. Not sure how that's possible.");
            }
            return item;
        }
        else
        {
            return null;
        }
    }

    public ItemBase UseItem(int selectedItem, Pokemon selectedPokemon, InventoryTab selectedTab, int quantity)
    {
        /* Refactor to actually use the item multiple times additively*/

        ItemBase item = inventory[selectedTab][selectedItem].Item;
        //int itemUsed = item.Use(selectedPokemon, quantity); //Only implemented for medicine and false for everything else
        bool itemUsed = item.Use(selectedPokemon); 
        if(itemUsed)
        {
            bool itemRemoved = RemoveItem(inventory[selectedTab][selectedItem].Item, quantity);
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
        InventoryTab tab = item.Tab;
        ItemSlot itemSlot = inventory[tab].First(slot => slot.Item == item);
        if(itemSlot == null)
        {
            return false;
        }
        else
        {
            itemSlot.Quantity -= qty;
            if(itemSlot.Quantity <= 0)
            {
                inventory[tab].Remove(itemSlot);
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

[System.Serializable]
public class Tab
{
    public InventoryTab tab;
    public List<ItemSlot> items;
}