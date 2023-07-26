using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSprite : MonoBehaviour
{
    [SerializeField] Image image;  //Image game object to add the sprite to
    [SerializeField] bool isPlayerSprite;
    [SerializeField] float entranceSpeed;
    [SerializeField] float sendOutSpeed;
    [SerializeField] float returnSpeed;
    [SerializeField] float transparentSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] float faintSpeed;
    [SerializeField] float faintFadeSpeed;
    [SerializeField] float hitSpeed;
    [SerializeField] Color hitColor = Color.grey;

    public Pokemon Pokemon {get;set;}

    Vector3 battlePosition;
    float battleScale;
    Color battleColor;

    void Awake()
    {
        battlePosition = image.transform.localPosition;
        battleScale = image.transform.localScale.x;
        battleColor = image.color;
    }

    public void Set(Pokemon p)  //float size)
    {
        Pokemon = p;
        image.sprite = Pokemon.Sprite;    
        image.color = battleColor;    
        if(isPlayerSprite)
        {
            image.transform.localPosition = new Vector3(battlePosition.x - 700, battlePosition.y);
            image.transform.localScale = new Vector3(0f, 0f);
        }
        else
        {
            image.transform.localPosition = new Vector3(battlePosition.x + 700, battlePosition.y);
        }
        //image.transform.localScale = new Vector3(size, size);
        //battleScale = image.transform.localScale;
    }

    public IEnumerator Entrance()
    {
        if(!Pokemon.IsWild)
        {
            yield return SendOut();
            yield break;
        }
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

    public IEnumerator Faint()
    {
        if(!Pokemon.IsWild)
        {
            yield return Return();
            yield break;
        }

        float curY = image.transform.localPosition.y;
        float targetY = curY - 500f;
        float diffY = curY - targetY;

        float curA = image.color.a;
        float targetA = 0f;
        float diffA = curA - targetA;
        
        while(curA - targetA > Mathf.Epsilon)
        {
            curY -= diffY * faintSpeed * Time.deltaTime;
            curA -= diffA * faintFadeSpeed * Time.deltaTime;
            if(curY - targetY < 0)
            {
                curY = targetY;
            }
            if(curA < 0f)
            {
                curA = 0f;
            }
            image.transform.localPosition = new Vector3(battlePosition.x, curY);
            image.color = new Color(battleColor.r, battleColor.g, battleColor.b, curA);
            yield return null;
        }
    }

    public IEnumerator SendOut()
    {
        image.transform.localPosition = battlePosition;
        image.transform.localScale = new Vector3(0f, 0f);
        float curScale = image.transform.localScale.x;
        float diff = battleScale - curScale;

        while(curScale < battleScale)
        {
            curScale += diff * sendOutSpeed * Time.deltaTime;
            if(curScale > battleScale)
            {
                curScale = battleScale;
            }
            image.transform.localScale = new Vector3(curScale, curScale);
            yield return null;
        }
    }

    public IEnumerator Return()
    {
        float curScale = image.transform.localScale.x;
        float diff = curScale;

        float curAlpha = 1f;

        image.color = Color.red;

        while(curScale > 0f)
        {
            curScale -= diff * returnSpeed * Time.deltaTime;
            if(curScale < 0)
            {
                curScale = 0;
            }
            image.transform.localScale = new Vector3(curScale, curScale);

            curAlpha -= 1f * transparentSpeed * Time.deltaTime;
            if(curAlpha < 0)
            {
                curAlpha = 0;
            }            
            image.color = new Color(image.color.r, image.color.g, image.color.b, curAlpha);

            yield return null;
        }

        image.color = Color.white;
    }

    public IEnumerator Attack()
    {
        float curX = battlePosition.x;
        float targetX = curX;
        if(isPlayerSprite)
        {
            targetX += 50f;
        }
        else
        {
            targetX -= 50f;
        }
        float diff = targetX - curX;

        while(Mathf.Abs(targetX - curX) > Mathf.Epsilon)
        {
            curX += diff * attackSpeed * Time.deltaTime;
            if((targetX - curX) * diff <= 0)
            {
                curX = targetX;
            }
            image.transform.localPosition = new Vector3(curX, battlePosition.y);
            yield return null;
        }

        curX = targetX;
        targetX = battlePosition.x;
        diff = targetX - curX;

        while(Mathf.Abs(targetX - curX) > Mathf.Epsilon)
        {
            curX += diff * attackSpeed * Time.deltaTime;
            if((targetX - curX) * diff <= 0)
            {
                curX = targetX;
            }
            image.transform.localPosition = new Vector3(curX, battlePosition.y);
            yield return null;
        }
    }

    public IEnumerator Hit()
    {
        float curR = battleColor.r;
        float curG = battleColor.g;
        float curB = battleColor.b;
        float curA = battleColor.a;
        float targetR = hitColor.r;
        float targetG = hitColor.g;
        float targetB = hitColor.b;
        float targetA = hitColor.a;

        float diffR = targetR - curR;
        float diffG = targetG - curG;
        float diffB = targetB - curB;
        float diffA = targetA - curA;

        while(Mathf.Abs(targetR - curR) > Mathf.Epsilon)
        {
            curR += diffR * hitSpeed * Time.deltaTime;
            curG += diffG * hitSpeed * Time.deltaTime;
            curB += diffB * hitSpeed * Time.deltaTime;
            curA += diffA * hitSpeed * Time.deltaTime;
            if((targetR - curR) * diffR <= 0)
            {
                curR = targetR;
                curG = targetG;
                curB = targetB;
                curA = targetA;
            }
            image.color = new Color(curR, curG, curB, curA);
            yield return null;
        }

        curR = hitColor.r;
        curG = hitColor.g;
        curB = hitColor.b;
        curA = hitColor.a;
        targetR = battleColor.r;
        targetG = battleColor.g;
        targetB = battleColor.b;
        targetA = battleColor.a;

        diffR = targetR - curR;
        diffG = targetG - curG;
        diffB = targetB - curB;
        diffA = targetA - curA;

        while(Mathf.Abs(targetR - curR) > Mathf.Epsilon)
        {
            curR += diffR * hitSpeed * Time.deltaTime;
            curG += diffG * hitSpeed * Time.deltaTime;
            curB += diffB * hitSpeed * Time.deltaTime;
            curA += diffA * hitSpeed * Time.deltaTime;
            if((targetR - curR) * diffR <= 0)
            {
                curR = targetR;
                curG = targetG;
                curB = targetB;
                curA = targetA;
            }
            image.color = new Color(curR, curG, curB, curA);
            yield return null;
        }
    }
}
