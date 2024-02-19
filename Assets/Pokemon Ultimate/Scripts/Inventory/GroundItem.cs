using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour, Interactable, ISaveable
{
    [SerializeField] string id;

    [SerializeField] ItemSlot item;

    bool obtained = false;

    public IEnumerator Interact(PlayerController player)
    {
        if(obtained)
        {
            yield break;
        }
        if(item == null)
        {
            yield break;
        }
        Inventory inventory = player.GetComponent<Inventory>();
        bool success = inventory.AddItem(item.Item, item.Quantity);
        if(success)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            obtained = true;
            yield return DialogManager.Instance.ObtainItem(item.Item, item.Quantity);
        }
        else
        {
            if(item.Quantity == 1)
            {
                yield return DialogManager.Instance.ShowDialog($"Could not pick up the {item.Item.ItemName}! Not enough room in Bag!");
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog($"Could not pick up the {item.Item.ItemName}s! Not enough room in Bag!");
            }
        }
    }
    
    [ContextMenu("Generate guid for id")]
    void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data)
    {
        bool itemObtained;
        if(data.groundItemsObtained.TryGetValue(id, out itemObtained))
        {
            obtained = itemObtained;
        }
        
        if(obtained)
        {            
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.groundItemsObtained.ContainsKey(id))
        {
            data.groundItemsObtained.Remove(id);
        }

        if(obtained)
        {
            data.groundItemsObtained.Add(id, obtained);
        }
    }
}
