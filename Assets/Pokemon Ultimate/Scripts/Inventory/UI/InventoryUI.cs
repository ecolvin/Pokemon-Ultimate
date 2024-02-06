using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public enum UIState {Items, Menu, Qty, Pokemon, Busy}

public class InventoryUI : MonoBehaviour
{
    const int ITEMS_IN_VIEWPORT = 12;
    const int LIST_SPACING = 5; //Get from Vertical Layout Group spacing field

    [SerializeField] GameObject itemListUI;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] GameObject descriptionBox;
    [SerializeField] TextMeshProUGUI descriptionName;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject menu;
    [SerializeField] InvParty partySprites;
    [SerializeField] GameObject emptyText;

    List<ItemSlotUI> items = new List<ItemSlotUI>();

    RectTransform itemListRect;

    Party party;
    Inventory inventory;
    int selectedItem = 0;
    int menuSelection = 0;
    float defaultMenuY = 0;
    int selectedPokemon = 0;
    int selectedQty = 0;
    int listSize = 0;
    int menuSize = 3;
    int selectedTab = 0;
    int numTabs = 1;
    int numPokemon = 6;
    int maxQuantity = 1;
    float slotHeight = 0;

    bool useItem = false;
    bool battle = false;

    UIState state;

    /* Initialization Function */
    void Awake()
    {
        itemListRect = itemListUI.GetComponent<RectTransform>();
        defaultMenuY = menu.transform.localPosition.y;
    }

    void Start()
    {
        inventory = Inventory.GetInventory();
        party = Party.GetParty();
        numPokemon = party.Pokemon.Count;
        UpdateItemList(selectedTab);
        partySprites.SetState(InvUIState.Healing);

        inventory.OnUpdated += RefreshItems;
    }

    void RefreshItems()
    {
        UpdateItemList(selectedTab);
    }

    void UpdateItemList(int tab)
    {
        foreach(Transform child in itemListUI.transform)
        {
            Destroy(child.gameObject);
        }

        items.Clear();
        foreach(ItemSlot itemSlot in inventory.Items)
        {
            ItemSlotUI slotUI = Instantiate(itemSlotUI, itemListUI.transform);
            slotUI.SetData(itemSlot);
            items.Add(slotUI);
        }
        listSize = inventory.Items.Count;
        if(listSize == 0)
        {
            return;
        }
        slotHeight = items[0].Height + LIST_SPACING;
        if(selectedItem > listSize - 1)
        {
            selectedItem = listSize - 1;
        }
        UpdateItemSelection();
    }

    void SetBattle()
    {
        battle = true;
        numTabs = 1;   //Update to 4 - Healing/Balls/Battle/Berries
        menuSize = 2;
    }

    void SetOverworld()
    {
        battle = false;
        numTabs = 1;    //Update to 8 - Healing/Balls/Battle/Berries/Comp/TMs/Treasures/Key Items
        menuSize = 3;
    }

    /* Input Handling */
    public void HandleUpdate(bool inBattle, Action<ItemBase, Pokemon> onClose)
    {
        if(inBattle)
        {
            if(battle != inBattle)
            {
                SetBattle();
            }
        }
        else
        {
            if(battle != inBattle)
            {
                SetOverworld();
            }
        }
        if(state == UIState.Items)
        {
            HandleInputItems(onClose);
        }
        else if(state == UIState.Menu)
        {
            HandleInputMenu();
        }
        else if(state == UIState.Pokemon)
        {
            HandleInputPokemon(onClose);
        }
        else if(state == UIState.Qty)
        {
            HandleInputQty();
        }
        else if(state == UIState.Busy)
        {
            //Do nothing
        }
    }

    void HandleInputItems(Action<ItemBase, Pokemon> onClose)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            onClose?.Invoke(null, null);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            selectedTab--;
            selectedTab %= numTabs;
            UpdateTab();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selectedTab++;
            selectedTab %= numTabs;
            UpdateTab();
        }
        if(listSize <= 0)
        {
            descriptionBox.SetActive(false);
            emptyText.SetActive(true);
            return;
        }
        else
        {
            descriptionBox.SetActive(true);
            emptyText.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedItem++;            
            selectedItem %= listSize;
            UpdateItemSelection();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedItem--;
            if(selectedItem < 0)
            {
                selectedItem += listSize;
            }
            else
            {
                selectedItem %= listSize;
            }
            UpdateItemSelection();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            maxQuantity = inventory.Items[selectedItem].Quantity;
            OpenMenu();
        }
    }

    void HandleInputMenu()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)
        || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            menuSelection++;            
            menuSelection %= menuSize;
            UpdateMenuSelection();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) 
        || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            menuSelection--;
            if(menuSelection < 0)
            {
                menuSelection += menuSize;
            }
            else
            {
                menuSelection %= menuSize;
            }
            UpdateMenuSelection();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {            
            state = UIState.Items;
            CloseMenu();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HandleMenuSelection());
        }
    }

    void HandleInputPokemon(Action<ItemBase, Pokemon> onClose)
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)
        || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selectedPokemon++;            
            selectedPokemon %= numPokemon;
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) 
        || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            selectedPokemon--;
            if(selectedPokemon < 0)
            {
                selectedPokemon += numPokemon;
            }
            else
            {
                selectedPokemon %= numPokemon;
            }
        }
        StartCoroutine(partySprites.UpdateSelection(selectedPokemon));
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(true);
            state = UIState.Menu;
            ClosePokemonSelection();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if(battle)
            {
                //Check if item is useable
                onClose?.Invoke(inventory.Items[selectedItem].Item, party.Pokemon[selectedPokemon]);
            }
            else
            {
                state = UIState.Busy;
                StartCoroutine(HandlePokemonSelection());
            }
        }
    }

    void HandleInputQty()
    {
        StartCoroutine(partySprites.UpdateSelection(selectedPokemon));
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedQty--;
            if(selectedQty <= 0)
            {
                selectedQty += maxQuantity;
            }        
            UpdateSelectedQty(false);
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedQty++;
            if(selectedQty > maxQuantity)
            {
                selectedQty -= maxQuantity;
            }
            UpdateSelectedQty(true);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if(selectedQty == 1)
            {
                selectedQty = maxQuantity;
            }
            else
            {
                selectedQty -= 10;
                if(selectedQty < 1)
                {
                    selectedQty = 1;
                }    
            }
            UpdateSelectedQty(false);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if(selectedQty == maxQuantity)
            {
                selectedQty = 1;
            }
            else
            {
                selectedQty += 10;
                if(selectedQty > maxQuantity)
                {
                    selectedQty = maxQuantity;
                }    
            }
            UpdateSelectedQty(true);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            state = UIState.Pokemon;
            CloseQty();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            state = UIState.Busy;
            StartCoroutine(HandleQtySelection());
        }
    }

    /* Perform the chosen selection */
    IEnumerator HandleMenuSelection()
    {
        if(menuSelection == 0)
        {
            useItem = true;
            OpenPokemonSelection();
                        
            //yield return DialogManager.Instance.ShowDialog("Which Pokemon will you use it on?");
            //Update to not block input to pokemon so the dialog box is visible while selecting a pokemon
            //Also update the Dialog Box UI with it to fit better with the inventory screen
        }
        else if(menuSelection == 1)
        {
            if(menuSize == 2)
            {
                state = UIState.Items;
                CloseMenu();
            }
            else
            {
                // Give the item to a pokemon *Not Implemented Yet*
                // useItem = false;
                // OpenPokemonSelection();
            }
        }
        else
        {
            state = UIState.Items;
            CloseMenu();
        }
        yield return null;
    }

    IEnumerator HandlePokemonSelection()
    {            
        if(useItem)
        {
            if(maxQuantity > 1 && !battle && false)    //Check if the current item allows quantity
            {
                OpenQty();
            }
            else if(battle)
            {

            }
            else
            {
                ItemBase usedItem = inventory.UseItem(selectedItem, party.Pokemon[selectedPokemon]);
                if(usedItem != null)
                {
                    yield return DialogManager.Instance.ShowDialog($"{party.Pokemon[selectedPokemon]}'s HP was restored.");
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog("It would have no effect.");
                }
                state = UIState.Items;
            }
        }
        else
        {
            //GiveItem(selectedPokemon);
        }
        yield return null;
    }

    IEnumerator HandleQtySelection()
    {
        ItemBase usedItem = inventory.UseItem(selectedItem, party.Pokemon[selectedPokemon], selectedQty);           
        CloseQty(); 
        state = UIState.Items;
        yield return null;
    }

    /* UI Updates */
    void UpdateItemSelection()
    {
        for(int i = 0; i < listSize; i++)
        {
            if(i == selectedItem)
            {
                SelectItem(i);
            }
            else
            {
                UnselectItem(i);
            }
        }
        UpdateDescription(inventory.Items[selectedItem].Item);
        UpdateScrolling();
    }

    void UpdateMenuSelection()
    {
        int i = 0;
        foreach(Transform child in menu.transform)
        {
            if(!child.gameObject.activeInHierarchy)
            {
                continue;
            }
            if(i == menuSelection)
            {
                child.GetComponent<Image>().color = GlobalSettings.Instance.SelectedBarColor;
            }
            else
            {
                child.GetComponent<Image>().color = Color.clear;
            }
            i++;
        }

        if(!battle)
        {
            if(menuSelection == 1) //Give Item -> Show what held item the pokemon has
            {
                partySprites.SetState(InvUIState.HeldItem);
            }
            else
            {                
                partySprites.SetState(InvUIState.Healing);
            }
        }
    }

    void UpdateSelectedQty(bool increase)
    {
        StartCoroutine(items[selectedItem].UpdateQtySelector(increase, selectedQty));
    }

    void UpdateTab()
    {
        selectedItem = 0; //Reset selection to the top of the new tab
        UpdateItemList(selectedTab);

        //Change the state of the InvParty to reflect necessary UI Icons
        partySprites.SetState(InvUIState.Healing);

    }

    void UpdateDescription(ItemBase item)
    {
        descriptionName.text = item.ItemName;
        descriptionText.text = item.Description;
    }

    void UpdateScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - ITEMS_IN_VIEWPORT/2, 0, selectedItem) * slotHeight;
        itemListRect.localPosition = new Vector3(itemListRect.localPosition.x, scrollPos);
    }

    void SelectItem(int i)
    {
        items[i].Background.color = GlobalSettings.Instance.SelectedBarColor;
        items[i].NameText.color = Color.black;
        items[i].QtyText.color = Color.black;
        items[i].XText.color = Color.black;
    }

    void UnselectItem(int i)
    {
        items[i].Background.color = Color.clear;
        items[i].NameText.color = Color.white;
        items[i].QtyText.color = Color.white;
        items[i].XText.color = Color.white;

    }

    /* State Changes */
    void OpenMenu()
    {
        state = UIState.Menu;

        if(selectedItem < ITEMS_IN_VIEWPORT/2)
        {
            int diff = ITEMS_IN_VIEWPORT/2 - selectedItem;
            float targetY = defaultMenuY + (diff * slotHeight);
            menu.transform.localPosition = new Vector3(menu.transform.localPosition.x, targetY, menu.transform.localPosition.z);
        }
        else if(selectedItem >= listSize - (ITEMS_IN_VIEWPORT/2))
        {
            int diff = Mathf.Min(((ITEMS_IN_VIEWPORT/2)-(listSize - selectedItem)) + 1, 3);
            float targetY = defaultMenuY - (diff * slotHeight);
            menu.transform.localPosition = new Vector3(menu.transform.localPosition.x, targetY, menu.transform.localPosition.z);
        }
        else
        {
            menu.transform.localPosition = new Vector3(menu.transform.localPosition.x, defaultMenuY, menu.transform.localPosition.z);
        }

        menu.SetActive(true);

        menuSelection = 0;
        UpdateMenuSelection();        
    }

    void CloseMenu()
    {
        menu.SetActive(false);
    }

    void OpenPokemonSelection() 
    {
        state = UIState.Pokemon;
        selectedPokemon = 0;
        CloseMenu();        
        UnselectItem(selectedItem);
    }

    void ClosePokemonSelection()
    {
        SelectItem(selectedItem);
    }

    void OpenQty()
    {
        state = UIState.Qty;
        selectedQty = 1;
        UnselectItem(selectedItem);
        items[selectedItem].OpenQtySelector();
    }

    void CloseQty()
    {
        if(listSize == 0 || selectedItem >= inventory.Items.Count)
        {
            return;
        }
        int curQty = inventory.Items[selectedItem].Quantity;
        items[selectedItem].CloseQtySelector(curQty);
        SelectItem(selectedItem);
    }
}
