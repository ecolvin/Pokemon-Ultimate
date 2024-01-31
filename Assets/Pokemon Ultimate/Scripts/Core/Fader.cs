using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 5;
    [SerializeField] float flashAlpha = 1f;
    [SerializeField] int numFlashes = 3;

    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator Fade(float targetA)
    {
        float curA = image.color.a;
        float diffA = targetA - curA;

        while(Mathf.Abs(curA - targetA) > Mathf.Epsilon)
        {
            curA += diffA * fadeSpeed * Time.deltaTime;
            if((targetA - curA) * diffA <= 0)
            {
                curA = targetA;
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, curA);
            yield return null;
        }
    }

    public IEnumerator BattleStart()
    {        
        for(int i = 0; i < numFlashes; i++)
        {
            yield return Fade(flashAlpha);
            yield return Fade(0);
        }
    }
}
