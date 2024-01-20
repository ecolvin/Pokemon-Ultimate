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
    [SerializeField] float hitSpeed = 15;
    [SerializeField] Color hitColor = Color.grey;
    [SerializeField] float poisonSpeed = 10;
    [SerializeField] Color poisonColor = new Color(100f, 50f, 150f, 255f);
    [SerializeField] float burnSpeed = 10;
    [SerializeField] Color burnColor = Color.red;
    [SerializeField] float paraSpeed = 10;
    [SerializeField] Color paraColor = Color.yellow;
    [SerializeField] int paraPause = 150;
    [SerializeField] float sleepSpeed = 3;
    [SerializeField] Color sleepColor = new Color(0f, 0f, 100f, 255f);
    [SerializeField] int sleepPause = 30;
    [SerializeField] float freezeSpeed = 10;
    [SerializeField] Color freezeColor = new Color(0f, 255f, 255f, 255f);
    [SerializeField] int freezePause = 150;


    public Pokemon Pokemon {get;set;}

    Vector3 battlePosition;
    float battleScale = 1;
    Color battleColor;

    void Awake()
    {
        battlePosition = image.transform.localPosition;
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
            //image.transform.localScale = new Vector3(0f, 0f);
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

        image.transform.localScale = new Vector3(battleScale, battleScale, battleScale);
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
        image.transform.localScale = new Vector3(0f, 0f);
        image.transform.localPosition = battlePosition;
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

    public IEnumerator Poison()
    {
        float curR = battleColor.r;
        float curG = battleColor.g;
        float curB = battleColor.b;
        float curA = battleColor.a;
        float targetR = poisonColor.r;
        float targetG = poisonColor.g;
        float targetB = poisonColor.b;
        float targetA = poisonColor.a;

        float diffR = targetR - curR;
        float diffG = targetG - curG;
        float diffB = targetB - curB;
        float diffA = targetA - curA;
        while(Mathf.Abs(targetR - curR) > Mathf.Epsilon)
        {
            curR += diffR * poisonSpeed * Time.deltaTime;
            curG += diffG * poisonSpeed * Time.deltaTime;
            curB += diffB * poisonSpeed * Time.deltaTime;
            curA += diffA * poisonSpeed * Time.deltaTime;
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

        curR = poisonColor.r;
        curG = poisonColor.g;
        curB = poisonColor.b;
        curA = poisonColor.a;
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
            curR += diffR * poisonSpeed * Time.deltaTime;
            curG += diffG * poisonSpeed * Time.deltaTime;
            curB += diffB * poisonSpeed * Time.deltaTime;
            curA += diffA * poisonSpeed * Time.deltaTime;
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

    public IEnumerator Burn()
    {
        float curR = battleColor.r;
        float curG = battleColor.g;
        float curB = battleColor.b;
        float curA = battleColor.a;
        float targetR = burnColor.r;
        float targetG = burnColor.g;
        float targetB = burnColor.b;
        float targetA = burnColor.a;

        float diffR = targetR - curR;
        float diffG = targetG - curG;
        float diffB = targetB - curB;
        float diffA = targetA - curA;

        while(Mathf.Abs(targetG - curG) > Mathf.Epsilon)
        {
            curR += diffR * burnSpeed * Time.deltaTime;
            curG += diffG * burnSpeed * Time.deltaTime;
            curB += diffB * burnSpeed * Time.deltaTime;
            curA += diffA * burnSpeed * Time.deltaTime;
            if((targetG - curG) * diffG <= 0)
            {
                curR = targetR;
                curG = targetG;
                curB = targetB;
                curA = targetA;
            }
            image.color = new Color(curR, curG, curB, curA);
            yield return null;
        }

        curR = burnColor.r;
        curG = burnColor.g;
        curB = burnColor.b;
        curA = burnColor.a;
        targetR = battleColor.r;
        targetG = battleColor.g;
        targetB = battleColor.b;
        targetA = battleColor.a;

        diffR = targetR - curR;
        diffG = targetG - curG;
        diffB = targetB - curB;
        diffA = targetA - curA;

        while(Mathf.Abs(targetG - curG) > Mathf.Epsilon)
        {
            curR += diffR * burnSpeed * Time.deltaTime;
            curG += diffG * burnSpeed * Time.deltaTime;
            curB += diffB * burnSpeed * Time.deltaTime;
            curA += diffA * burnSpeed * Time.deltaTime;
            if((targetG - curG) * diffG <= 0)
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
    
    public IEnumerator Paralyzed()
    {
        float curR = battleColor.r;
        float curG = battleColor.g;
        float curB = battleColor.b;
        float curA = battleColor.a;
        float targetR = paraColor.r;
        float targetG = paraColor.g;
        float targetB = paraColor.b;
        float targetA = paraColor.a;

        float diffR = targetR - curR;
        float diffG = targetG - curG;
        float diffB = targetB - curB;
        float diffA = targetA - curA;

        while(Mathf.Abs(targetB - curB) > Mathf.Epsilon)
        {
            curR += diffR * paraSpeed * Time.deltaTime;
            curG += diffG * paraSpeed * Time.deltaTime;
            curB += diffB * paraSpeed * Time.deltaTime;
            curA += diffA * paraSpeed * Time.deltaTime;
            if((targetB - curB) * diffB <= 0)
            {
                curR = targetR;
                curG = targetG;
                curB = targetB;
                curA = targetA;
            }
            image.color = new Color(curR, curG, curB, curA);
            yield return null;
        }

        //Pause for a number of frames equal to paraPause
        // for(int i = 0; i < paraPause; i++)
        // {
        //     yield return null;
        // }
        yield return Attack();
        yield return Attack();

        curR = paraColor.r;
        curG = paraColor.g;
        curB = paraColor.b;
        curA = paraColor.a;
        targetR = battleColor.r;
        targetG = battleColor.g;
        targetB = battleColor.b;
        targetA = battleColor.a;

        diffR = targetR - curR;
        diffG = targetG - curG;
        diffB = targetB - curB;
        diffA = targetA - curA;

        while(Mathf.Abs(targetB - curB) > Mathf.Epsilon)
        {
            curR += diffR * paraSpeed * Time.deltaTime;
            curG += diffG * paraSpeed * Time.deltaTime;
            curB += diffB * paraSpeed * Time.deltaTime;
            curA += diffA * paraSpeed * Time.deltaTime;
            if((targetB - curB) * diffB <= 0)
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
    
    public IEnumerator Sleep()
    {
        float curR = battleColor.r;
        float curG = battleColor.g;
        float curB = battleColor.b;
        float curA = battleColor.a;
        float targetR = sleepColor.r;
        float targetG = sleepColor.g;
        float targetB = sleepColor.b;
        float targetA = sleepColor.a;

        float diffR = targetR - curR;
        float diffG = targetG - curG;
        float diffB = targetB - curB;
        float diffA = targetA - curA;

        while(Mathf.Abs(targetR - curR) > Mathf.Epsilon)
        {
            curR += diffR * sleepSpeed * Time.deltaTime;
            curG += diffG * sleepSpeed * Time.deltaTime;
            curB += diffB * sleepSpeed * Time.deltaTime;
            curA += diffA * sleepSpeed * Time.deltaTime;
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

        //Pause for a number of frames equal to sleepPause
        for(int i = 0; i < sleepPause; i++)
        {
            yield return null;
        }

        curR = sleepColor.r;
        curG = sleepColor.g;
        curB = sleepColor.b;
        curA = sleepColor.a;
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
            curR += diffR * sleepSpeed * Time.deltaTime;
            curG += diffG * sleepSpeed * Time.deltaTime;
            curB += diffB * sleepSpeed * Time.deltaTime;
            curA += diffA * sleepSpeed * Time.deltaTime;
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

    public IEnumerator Freeze()
    {
        float curR = battleColor.r;
        float curG = battleColor.g;
        float curB = battleColor.b;
        float curA = battleColor.a;
        float targetR = freezeColor.r;
        float targetG = freezeColor.g;
        float targetB = freezeColor.b;
        float targetA = freezeColor.a;

        float diffR = targetR - curR;
        float diffG = targetG - curG;
        float diffB = targetB - curB;
        float diffA = targetA - curA;
            
        while(Mathf.Abs(targetR - curR) > Mathf.Epsilon)
        {
            curR += diffR * freezeSpeed * Time.deltaTime;
            curG += diffG * freezeSpeed * Time.deltaTime;
            curB += diffB * freezeSpeed * Time.deltaTime;
            curA += diffA * freezeSpeed * Time.deltaTime;

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

        //Pause for a number of frames equal to freezePause
        for(int i = 0; i < freezePause; i++)
        {
            yield return null;
        }

        curR = freezeColor.r;
        curG = freezeColor.g;
        curB = freezeColor.b;
        curA = freezeColor.a;
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
            curR += diffR * freezeSpeed * Time.deltaTime;
            curG += diffG * freezeSpeed * Time.deltaTime;
            curB += diffB * freezeSpeed * Time.deltaTime;
            curA += diffA * freezeSpeed * Time.deltaTime;
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
