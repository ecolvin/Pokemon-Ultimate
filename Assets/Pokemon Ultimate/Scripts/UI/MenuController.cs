using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;

    public event Action<int> OnSelection;
    public event Action OnClose;

    List<TextMeshProUGUI> menuItems;
    
    int selectedItem = 0;

    void Start()
    {
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        UpdateItemSelection();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedItem--;
            if(selectedItem < 0)
            {
                selectedItem = menuItems.Count - 1;
            }
            UpdateItemSelection();
        }
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedItem++;
            if(selectedItem >= menuItems.Count)
            {
                selectedItem = 0;
            }
            UpdateItemSelection();
        }


        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
            OnClose?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnSelection?.Invoke(selectedItem);
        }
    }

    void UpdateItemSelection()
    {
        Debug.Log($"Current Selection = {selectedItem}");
        for(int i = 0; i < menuItems.Count; i++)
        {
            if(i == selectedItem)
            {
                Debug.Log($"Selected Text: Item #{i} - {menuItems[i].text}");
                menuItems[i].color = GlobalSettings.Instance.SelectedColor;
            }
            else
            {
                Debug.Log($"Default Text: Item #{i} - {menuItems[i].text}");
                menuItems[i].color = GlobalSettings.Instance.DefaultColor;
            }
        }
    }
}
