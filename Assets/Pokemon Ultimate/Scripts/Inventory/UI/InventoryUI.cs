using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public enum UIState {Items, Menu, Qty, Pokemon, ForgetMove, Choice, Busy}

public class InventoryUI : MonoBehaviour
{
    const int ITEMS_IN_VIEWPORT = 10;
    const int LIST_SPACING = 5; //Get from Vertical Layout Group spacing field

    [SerializeField] GameObject itemListUI;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] GameObject descriptionBox;
    [SerializeField] TextMeshProUGUI descriptionName;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject twoOptionMenu;
    [SerializeField] GameObject threeOptionMenu;
    [SerializeField] InvParty partySprites;
    [SerializeField] GameObject emptyText;
    [SerializeField] TextMeshProUGUI tabName;
    [SerializeField] ForgetMoveScreen forgetMoveScreen;
    [SerializeField] TMDescriptionBox tmDescription;

    [SerializeField] GameObject choiceBox;
    [SerializeField] Image yesBox;
    [SerializeField] Image noBox;

    List<ItemSlotUI> items = new List<ItemSlotUI>();

    RectTransform itemListRect;

    GameObject menu;

    Party party;
    Inventory inventory;

    int selectedItem = 0;
    int menuSelection = 0;
    int selectedPokemon = 0;
    int selectedQty = 0;
    InventoryTab selectedTab = InventoryTab.Medicines;
    bool choice = true;

    int listSize = 0;
    int menuSize = 3;
    int numTabs = 8;
    int numPokemon = 6;
    int maxQuantity = 1;

    float slotHeight = 0;
    float defaultMenuY = 0;

    bool useItem = false;
    bool battle = false;

    Action<ItemBase, Pokemon> onClose;

    UIState state;

    /* Initialization Function */
    void Awake()
    {
        itemListRect = itemListUI.GetComponent<RectTransform>();
        menu = threeOptionMenu;
        defaultMenuY = menu.transform.localPosition.y;
    }

    void Start()
    {
        if(inventory == null)
        {
            inventory = Inventory.GetInventory();
        }
        party = Party.GetParty();
        numPokemon = party.Pokemon.Count;
        UpdateItemList(selectedTab);

        inventory.OnUpdated += RefreshItems;
    }

    void RefreshItems()
    {
        UpdateItemList(selectedTab);
    }

    void UpdateItemList(InventoryTab tab)
    {
        foreach(Transform child in itemListUI.transform)
        {
            Destroy(child.gameObject);
        }

        items.Clear();
        if(inventory == null)
        {
            inventory = Inventory.GetInventory();
        }
        foreach(ItemSlot itemSlot in inventory.Inv[tab])
        {
            ItemSlotUI slotUI = Instantiate(itemSlotUI, itemListUI.transform);
            slotUI.SetData(itemSlot);
            items.Add(slotUI);
        }
        listSize = inventory.Inv[tab].Count;
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
        UpdateTabName();     
    }

    public void OpenInventory(bool inBattle, Action<ItemBase, Pokemon> onClose)
    {
        tmDescription.gameObject.SetActive(false);
        descriptionBox.gameObject.SetActive(true);
        if(inBattle)
        {
            SetBattle();
        }
        else
        {
            SetOverworld();
        }
        this.onClose = onClose;
        gameObject.SetActive(true);
        RefreshItems();
    }

    void SetBattle()
    {
        battle = true;
        numTabs = 4;
        menuSize = 2;
    }

    void SetOverworld()
    {
        battle = false;
        numTabs = 8;
        menuSize = 3;
    }

    /* Input Handling */
    public void HandleUpdate()
    {
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
        else if(state == UIState.ForgetMove)
        {
            forgetMoveScreen.HandleUpdate();
        }
        else if(state == UIState.Choice)
        {
            HandleInputChoice();
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
            int tab = (int) selectedTab;
            tab--;
            while(tab < 0)
            {
                tab += numTabs;
            }
            selectedTab = (InventoryTab) tab;
            UpdateTab();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            int tab = (int) selectedTab;
            tab++;
            while(tab >= numTabs)
            {
                tab -= numTabs;
            }
            selectedTab = (InventoryTab) tab;
            UpdateTab();
        }

        if(listSize <= 0)
        {
            descriptionBox.SetActive(false);
            tmDescription.gameObject.SetActive(false);
            emptyText.SetActive(true);
            return;
        }
        else
        {
            if(selectedTab == InventoryTab.TMs)
            {
                tmDescription.gameObject.SetActive(true);
                descriptionBox.SetActive(false);
                UpdateTMDescription();
            }
            else
            {
                tmDescription.gameObject.SetActive(false);
                descriptionBox.SetActive(true);
                UpdateDescription();
            }
            
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
            maxQuantity = inventory.Inv[selectedTab][selectedItem].Quantity;
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
            state = UIState.Busy;
            StartCoroutine(HandlePokemonSelection());
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

    void HandleInputChoice()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)
        || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)
        || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) 
        || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            choice = !choice;
            UpdateChoiceSelection();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            choice = false;
            UpdateChoiceSelection();            
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HandleChoiceSelection());
        }
    }

    /* Perform the chosen selection */
    IEnumerator HandleMenuSelection()
    {
        if(menuSelection == 0)
        {
            useItem = true;
            if(selectedTab == InventoryTab.Balls || selectedTab == InventoryTab.Battle || 
               selectedTab == InventoryTab.Treasures || selectedTab == InventoryTab.KeyItems)
            {
                state = UIState.Busy;
                if(battle)
                {
                    CloseMenu();
                    inventory.RemoveItem(inventory.Inv[selectedTab][selectedItem].Item, 1);
                    onClose?.Invoke(inventory.Inv[selectedTab][selectedItem].Item, null);
                    state = UIState.Items;
                }
                else
                {
                    CloseMenu();
                    yield return UseItem();
                }
            }
            else
            {
                state = UIState.Busy;
                yield return DialogManager.Instance.DisplayText("Which Pokemon will you use it on?");
                OpenPokemonSelection();
            }
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
                state = UIState.Busy;
                useItem = false;
                OpenPokemonSelection();
                yield return DialogManager.Instance.DisplayText("Which Pokemon do you want to give this item to?");
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
        ItemBase itemSelection = inventory.Inv[selectedTab][selectedItem].Item;

        if(useItem)
        {
            if(selectedTab == InventoryTab.TMs)
            {
                yield return UseTM();
            }
            else if(maxQuantity > 1 && !battle && false)    //Check if the current item allows quantity
            {
                OpenQty();
            }
            else if(battle)
            {
                ClosePokemonSelection();
                inventory.RemoveItem(itemSelection, 1);
                onClose?.Invoke(itemSelection, party.Pokemon[selectedPokemon]);
                state = UIState.Items;
            }
            else
            {
                ClosePokemonSelection();
                yield return UseItem();
            }
        }
        else //Give Item
        {
            if(party.Pokemon[selectedPokemon].HeldItem == null)
            {
                inventory.RemoveItem(itemSelection, 1);
                ItemBase prevItem = party.Pokemon[selectedPokemon].GiveItem(itemSelection);
                yield return DialogManager.Instance.ShowDialog($"Your Pokemon is now holding the {itemSelection.ItemName}");
                if(prevItem != null)
                {
                    Debug.Log($"Gave an item to {party.Pokemon[selectedPokemon].Nickname} and received {prevItem.ItemName} back despite the pokemon holding no item previously...");
                }
                state = UIState.Items;
            }
            else
            {
                yield return DialogManager.Instance.DisplayText($"Swap in the {itemSelection.ItemName} for the {party.Pokemon[selectedPokemon].HeldItem.ItemName} it's holding now?");
                choice = true;
                UpdateChoiceSelection();
                choiceBox.SetActive(true);
                state = UIState.Choice;
            }
        }
    }

    IEnumerator UseItem()
    {
        state = UIState.Busy;
       
        ItemBase usedItem = inventory.UseItem(selectedItem, party.Pokemon[selectedPokemon], selectedTab);
        if(usedItem != null)
        {
            //UPDATE TO DETERMINE DIALOG DYNAMICALLY!!!!
            yield return DialogManager.Instance.ShowDialog($"{party.Pokemon[selectedPokemon]}'s HP was restored.");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog("It would have no effect.");
        }
        state = UIState.Items;
    }

    IEnumerator UseTM()
    {
        state = UIState.Busy;

        ItemBase item = inventory.Inv[selectedTab][selectedItem].Item;
        Pokemon poke = party.Pokemon[selectedPokemon];
        if(!(item is TM))
        {
            Debug.LogError($"Non-TM item ({item.ItemName}) stored in the TM tab of the bag.");
            ClosePokemonSelection();
            state = UIState.Items;
            yield break;
        }

        TM tm = (TM) item;

        if(poke.Moves.FirstOrDefault(m => m.MoveBase == tm.Move) != null)
        {
            yield return DialogManager.Instance.ShowDialog($"{poke.Nickname} already knows {tm.Move.MoveName}.");
            ClosePokemonSelection();
            state = UIState.Items;
            yield break;
        }

        if(!poke.Species.TMLearnset.Contains(tm.Move))
        {
            yield return DialogManager.Instance.ShowDialog($"{poke.Nickname} can't learn {tm.Move.MoveName}! The two are incompatible!");            
            ClosePokemonSelection();
            state = UIState.Items;
            yield break;
        }

        if(poke.AvailableMoveSlot())
        {
            poke.LearnMove(new PokemonMove(tm.Move));
            if(!inventory.RemoveItem(item, 1))
            {
                Debug.LogError($"Failed to remove a copy of {item.ItemName} from the inventory.");
            }
            yield return DialogManager.Instance.ShowDialog($"{poke.Nickname} learned {tm.Move.MoveName}.");
            ClosePokemonSelection();
            state = UIState.Items;
        }
        else
        {            
            yield return DialogManager.Instance.ShowDialog($"Please choose a move that will be replaced with {tm.Move.MoveName}.");

            state = UIState.ForgetMove;

            PokemonMove move = new PokemonMove(tm.Move);
            Action<int> forgetMoveCloseAction = (int chosenSlot) => 
            {
                StartCoroutine(HandleMoveToForget(chosenSlot));
            };

            forgetMoveScreen.OpenScreen(poke, move, forgetMoveCloseAction);
        }

        yield return null;
    }

    IEnumerator HandleMoveToForget(int chosenSlot)
    {
        state = UIState.Busy;
        TM tm = (TM) inventory.Inv[selectedTab][selectedItem].Item;
        Pokemon poke = party.Pokemon[selectedPokemon];

        if(chosenSlot < poke.Moves.Count)
        {
            PokemonMove oldMove = poke.ReplaceMove(new PokemonMove(tm.Move), chosenSlot);
            if(!inventory.RemoveItem(tm, 1))
            {
                Debug.LogError($"Failed to remove a copy of {tm.ItemName} from the inventory.");
            }
            yield return DialogManager.Instance.ShowDialog("One...two...and...ta-da!"); 
            yield return DialogManager.Instance.ShowDialog($"{poke.Nickname} forgot {oldMove.MoveBase.MoveName}...\n and it learned {tm.Move.MoveName} instead!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog($"{poke.Nickname} did not learn {tm.Move.MoveName}.");
        }
        ClosePokemonSelection();
        state = UIState.Items;
    }

    IEnumerator HandleQtySelection()
    {
        ItemBase usedItem = inventory.UseItem(selectedItem, party.Pokemon[selectedPokemon], selectedTab, selectedQty);           
        CloseQty(); 
        state = UIState.Items;
        yield return null;
    }

    IEnumerator HandleChoiceSelection()
    {
        state = UIState.Busy;
        choiceBox.SetActive(false);
        if(choice)
        {                
            inventory.RemoveItem(inventory.Inv[selectedTab][selectedItem].Item, 1);
            ItemBase prevItem = party.Pokemon[selectedPokemon].GiveItem(inventory.Inv[selectedTab][selectedItem].Item);
            ClosePokemonSelection();
            yield return DialogManager.Instance.ShowDialog($"You took your Pokemon's {prevItem.ItemName} and gave it the {inventory.Inv[selectedTab][selectedItem].Item.ItemName}.");
            if(prevItem != null)
            {
                inventory.AddItem(prevItem, 1);
            }
            state = UIState.Items;
        }
        else
        {                
            yield return DialogManager.Instance.DisplayText("Which Pokemon do you want to give this item to?");
            state = UIState.Pokemon;
        }
    }

    /* UI Updates */
    void UpdateItemSelection() 
    {
        if(items.Count == 0)
        {
            partySprites.SetState(InvUIState.Default);
            return;
        }

        ItemBase item = inventory.Inv[selectedTab][selectedItem].Item;

        if(selectedItem >= items.Count)
        {
            selectedItem = items.Count - 1;
        }

        InvUIState newState = item.UIState;
        partySprites.SetState(newState);
        if(newState == InvUIState.Learnable)
        {
            if(item is TM)
            {
                partySprites.UpdateLearnable((TM) item);
            }
            else
            {
                Debug.LogError($"Why does this non-TM ({item.ItemName})set the state to learnable?");
            }
        }
        else if(newState == InvUIState.Useable)
        {
            partySprites.UpdateUseable(item);
        }
        

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

        UpdateTMDescription();
        UpdateDescription();
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
                partySprites.SetState(inventory.Inv[selectedTab][selectedItem].Item.UIState);
            }
        }
    }

    void UpdateSelectedQty(bool increase)
    {
        StartCoroutine(items[selectedItem].UpdateQtySelector(increase, selectedQty));
    }

    void UpdateTab()
    {
        if(battle || selectedTab == InventoryTab.KeyItems)
        {
            menu = twoOptionMenu;
            menuSize = 2;
        }
        else
        {
            menu = threeOptionMenu;
            menuSize = 3;
        }

        selectedItem = 0; //Reset selection to the top of the new tab
        UpdateItemList(selectedTab);
        UpdateTabName();
        //Update Icon at the top
    }

    void UpdateChoiceSelection()
    {
        if(choice)
        {
            yesBox.color = GlobalSettings.Instance.SelectedBarColor;
            noBox.color = Color.clear;
        }
        else
        {
            yesBox.color = Color.clear;
            noBox.color = GlobalSettings.Instance.SelectedBarColor;
        }
    }

    void UpdateTabName()
    {
        string name;
        switch(selectedTab)
        {
            case InventoryTab.Medicines:
                name = "Medicines";
                break;
            case InventoryTab.Balls:
                name = "Poke Balls";
                break;
            case InventoryTab.Battle:
                name = "Battle Items";
                break;
            case InventoryTab.Berries:
                name = "Berries";
                break;
            case InventoryTab.Other:
                name = "Other Items";
                break;
            case InventoryTab.TMs:
                name = "TMs";
                break;
            case InventoryTab.Treasures:
                name = "Treasures";
                break;
            case InventoryTab.KeyItems:
                name = "Key Items";
                break;
            default:
                name = "Items";
                Debug.LogError($"Non-existent InventoryTab selected: {selectedTab}");
                break;
        }
        tabName.text = name;
    }

    void UpdateDescription()
    {
        if(inventory.Inv[selectedTab] == null || inventory.Inv[selectedTab].Count == 0)
        {
            return;
        }
        ItemBase item = inventory.Inv[selectedTab][selectedItem].Item;
        if(item == null)
        {
            return;
        }
        descriptionName.text = item.ItemName;
        descriptionText.text = item.Description;
    }

    void UpdateTMDescription()
    {
        if(selectedTab != InventoryTab.TMs)
        {
            return;
        }
        ItemBase item = inventory.Inv[selectedTab][selectedItem].Item;
        if(item is TM)
        {
            tmDescription.Set((TM) item);
        }
        else
        {
            tmDescription.Set(null);
        }
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
        partySprites.SetState(inventory.Inv[selectedTab][i].Item.UIState);
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
        else if(selectedItem >= listSize - ((ITEMS_IN_VIEWPORT-1)/2))
        {
            //int diff = Mathf.Min((((ITEMS_IN_VIEWPORT-1)/2)-(listSize - selectedItem)) + 1, 3);
            float targetY = defaultMenuY - slotHeight; //- (slotHeight*diff);
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
        if(listSize == 0 || selectedItem >= inventory.Inv[selectedTab].Count)
        {
            return;
        }
        int curQty = inventory.Inv[selectedTab][selectedItem].Quantity;
        items[selectedItem].CloseQtySelector(curQty);
        SelectItem(selectedItem);
    }
}


//Inventory TO-DOs:
//------------------
//Fix Dialog Box being behind forget a move screen in the main world (not in the prefab)
//---------
//Update Description for TMs
//Move Selection Screen for Ether
//Add sorting options to the UI

