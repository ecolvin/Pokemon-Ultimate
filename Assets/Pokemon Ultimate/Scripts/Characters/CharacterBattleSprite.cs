using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBattleSprite : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] bool isPlayer = false;
    [SerializeField] float entranceSpeed;

    Vector3 battlePosition;
    Vector3 offScreenPosition;

    void Awake()
    {
        battlePosition = image.transform.localPosition;
    }

    public void Set(Sprite sprite)  //float size)
    {
        image.sprite = sprite;    
        if(isPlayer)
        {
            offScreenPosition = new Vector3(battlePosition.x - 800, battlePosition.y);
            image.transform.localPosition = new Vector3(battlePosition.x + 1800, battlePosition.y);
        }
        else
        {
            offScreenPosition = new Vector3(battlePosition.x + 800, battlePosition.y);
            image.transform.localPosition = new Vector3(battlePosition.x - 1800, battlePosition.y);
        }
    }

    public IEnumerator Entrance()
    {
        float curX = image.transform.localPosition.x;
        float diff = battlePosition.x - curX;
        
        while(Mathf.Abs(curX - battlePosition.x) > Mathf.Epsilon)
        {
            curX += diff * entranceSpeed * Time.deltaTime;
            if((battlePosition.x - curX) * diff <= 0)
            {
                curX = battlePosition.x;
            }
            image.transform.localPosition = new Vector3(curX, battlePosition.y);
            yield return null;
        }
    }

    public IEnumerator Exit()
    {
        float curX = image.transform.localPosition.x;
        float diff = offScreenPosition.x - curX;
        
        while(Mathf.Abs(curX - offScreenPosition.x) > Mathf.Epsilon)
        {
            curX += diff * entranceSpeed * Time.deltaTime;
            if((offScreenPosition.x - curX) * diff <= 0)
            {
                curX = offScreenPosition.x;
            }
            image.transform.localPosition = new Vector3(curX, offScreenPosition.y);
            yield return null;
        }
    }
}
