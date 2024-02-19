using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GroundTM : MonoBehaviour, Interactable, ISaveable
{
    [SerializeField] string id;

    [SerializeField] TM tm;
    [SerializeField] int quantity;

    bool obtained = false;

    public IEnumerator Interact(PlayerController player)
    {
        Inventory inventory = player.GetComponent<Inventory>();
        bool success = inventory.AddItem(tm, quantity);
        if(success)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            obtained = true;            
            yield return DialogManager.Instance.ObtainItem(tm, quantity);
        }
        else
        {
            if(quantity == 1)
            {
                yield return DialogManager.Instance.ShowDialog($"Could not pick up the TM{tm.Number:000}! Not enough room in Bag!");
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog($"Could not pick up the TM{tm.Number:000}s! Not enough room in Bag!");
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
        if(data.groundTMsObtained.TryGetValue(id, out itemObtained))
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
        if(data.groundTMsObtained.ContainsKey(id))
        {
            data.groundTMsObtained.Remove(id);
        }

        if(obtained)
        {
            data.groundTMsObtained.Add(id, obtained);
        }
    }
}
