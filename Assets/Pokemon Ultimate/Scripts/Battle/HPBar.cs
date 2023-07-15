using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void setHP(float hpPercentage)
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
}
