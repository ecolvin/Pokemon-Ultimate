using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{    
    [SerializeField] string id = "";

    [SerializeField] Transform body;
  
    [SerializeField] float moveSpeed = 50f;

    bool isPlayer = false;

    void Awake()
    {
        SnapToPos(transform.position);
        isPlayer = GetComponent<PlayerController>() != null;
    }

    public void SnapToPos(Vector3 pos)
    {
        pos.x = RoundToNearestTen(pos.x);
        pos.y = 0;
        pos.z = RoundToNearestTen(pos.z);

        transform.position = pos;
    }

    int RoundToNearestTen(float f)
    {
        float r = Mathf.Abs(f % 10);
        if(r >= 5)
        {
            return Mathf.RoundToInt(f+(10-r));
        }
        else
        {
            return Mathf.RoundToInt(f-r);
        }
    }

    public IEnumerator SmoothGridMovement(Vector3 newPos)
    {       
        Vector3 curPos = transform.position;

        GetComponent<Rigidbody>().freezeRotation = false;
        body.transform.LookAt(newPos);
        GetComponent<Rigidbody>().freezeRotation = true;
        Collider[] obstacles = Physics.OverlapBox(newPos, new Vector3(GlobalSettings.Instance.GridSize/2, 5f, GlobalSettings.Instance.GridSize/2), Quaternion.identity, GameLayers.Instance.BlockingLayers);
        if(obstacles.Length != 0)
        {
            yield break;
        }        

        do
        {
            transform.position = Vector3.MoveTowards(curPos, newPos, moveSpeed * Time.deltaTime);
            curPos = transform.position;
            yield return null;
        }while(curPos != newPos);
    }

    [ContextMenu("Generate guid for id")]
    void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }
        
    // public void LoadData(GameData data)
    // {
    //     if(isPlayer)
    //     {
    //         return;
    //     }
    //     Vector3 position;
    //     if(data.characterPositions.TryGetValue(id, out position))
    //     {
    //         transform.position = position;
    //     }

    //     Quaternion rotation = Quaternion.identity;
    //     if(data.characterRotations.TryGetValue(id, out rotation))
    //     {
    //         body.transform.rotation = rotation;
    //     }
    // }

    // public void SaveData(ref GameData data)
    // {
    //     if(isPlayer)
    //     {
    //         return;
    //     }
    //     if(data.characterPositions.ContainsKey(id))
    //     {
    //         data.characterPositions.Remove(id);
    //     }
    //     if(data.characterRotations.ContainsKey(id))
    //     {
    //         data.characterRotations.Remove(id);
    //     }

    //     data.characterPositions.Add(id, transform.position);
    //     data.characterRotations.Add(id, body.transform.rotation);
    // }
}
