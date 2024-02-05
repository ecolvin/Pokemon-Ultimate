using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] TextMeshProUGUI hpText;

    public void SetHP(int curHP, int maxHP)
    {
        float newHP = (float) curHP/(float) maxHP;
        if(newHP > 1)
        {
            newHP = 1;
        }
        if(newHP < 0)
        {
            newHP = 0;
        }

        health.transform.localScale = new Vector3(newHP, 1f, 1f);            
        
        if(hpText != null)
        {
            hpText.text = $"{curHP} / {maxHP}";
        }

        if(newHP < .5 )
        {
            if(newHP < .2)
            {
                health.GetComponent<Image>().color = Color.red;
            }
            else
            {                
                health.GetComponent<Image>().color = Color.yellow;
            }
        }
        else
        {
            health.GetComponent<Image>().color = Color.green;
        }
    }

    public void SetHP(float hpPercentage)
    {
        float newHP = hpPercentage;
        if(newHP > 1)
        {
            newHP = 1;
        }
        if(newHP < 0)
        {
            newHP = 0;
        }

        health.transform.localScale = new Vector3(newHP, 1f, 1f);

        if(newHP < .5 )
        {
            if(newHP < .2)
            {
                health.GetComponent<Image>().color = Color.red;
            }
            else
            {                
                health.GetComponent<Image>().color = Color.yellow;
            }
        }
        else
        {
            health.GetComponent<Image>().color = Color.green;
        }
    }

    public IEnumerator SetHPSmoothly(int curHP, int maxHP)
    {
        float curHPScale = health.transform.localScale.x;
        float newHP = (float) curHP / (float) maxHP;
        if(newHP > 1)
        {
            newHP = 1;
        }
        if(newHP < 0)
        {
            newHP = 0;
        }
        
        float diff = newHP - curHPScale;
        
        while(curHPScale - newHP > Mathf.Epsilon)
        {
            curHPScale += diff * Time.deltaTime;
            health.transform.localScale = new Vector3(curHPScale, 1f, 1f);
            
            if(hpText != null)
            {
                hpText.text = $"{Mathf.FloorToInt(curHPScale*maxHP)} / {maxHP}";
            }

            if(curHPScale < .5 )
            {
                if(curHPScale < .2)
                {
                    health.GetComponent<Image>().color = Color.red;
                }
                else
                {                
                    health.GetComponent<Image>().color = Color.yellow;
                }
            }
            else
            {
                health.GetComponent<Image>().color = Color.green;
            }
            yield return null;
        }

        health.transform.localScale = new Vector3(newHP, 1f, 1f);            
            
        if(hpText != null)
        {
            hpText.text = $"{curHP} / {maxHP}";
        }

        if(newHP < .5 )
        {
            if(newHP < .2)
            {
                health.GetComponent<Image>().color = Color.red;
            }
            else
            {                
                health.GetComponent<Image>().color = Color.yellow;
            }
        }
        else
        {
            health.GetComponent<Image>().color = Color.green;
        }
    }

    public IEnumerator SetHPSmoothly(float hpPercentage)
    {
        float curHP = health.transform.localScale.x;
        float newHP = hpPercentage;
        if(newHP > 1)
        {
            newHP = 1;
        }
        if(newHP < 0)
        {
            newHP = 0;
        }
        
        float diff = newHP - curHP;
        
        while(curHP - newHP > Mathf.Epsilon)
        {
            curHP += diff * Time.deltaTime;
            health.transform.localScale = new Vector3(curHP, 1f, 1f);
    
            if(curHP < .5 )
            {
                if(curHP < .2)
                {
                    health.GetComponent<Image>().color = Color.red;
                }
                else
                {                
                    health.GetComponent<Image>().color = Color.yellow;
                }
            }
            else
            {
                health.GetComponent<Image>().color = Color.green;
            }
            yield return null;
        }

        health.transform.localScale = new Vector3(newHP, 1f, 1f);

        if(newHP < .5 )
        {
            if(newHP < .2)
            {
                health.GetComponent<Image>().color = Color.red;
            }
            else
            {                
                health.GetComponent<Image>().color = Color.yellow;
            }
        }
        else
        {
            health.GetComponent<Image>().color = Color.green;
        }
    }
}
