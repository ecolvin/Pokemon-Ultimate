using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokeBallSprite : MonoBehaviour
{
    [SerializeField] int xOffset = 1000;
    [SerializeField] int yOffset = 100;
    [SerializeField] int ballHeight = 250;
    [SerializeField] int critShake = 5;
    [SerializeField] int shakeAngle = 30;

    [SerializeField] float pauseAfterThrow = .25f;
    [SerializeField] float pauseAfterFall = .5f;
    [SerializeField] float pauseAfterShake = .75f;
    [SerializeField] float throwSpeed = 1f;
    [SerializeField] float fallSpeed = 3f;
    [SerializeField] float shakeSpeed = 10f;
    [SerializeField] float critSpeed = 15f;

    //[SerializeField] float amplitude = 1;

    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 airPosition;

    void Awake()
    {
        endPosition = transform.localPosition;
        startPosition = endPosition - new Vector3(xOffset, yOffset);
        airPosition = endPosition + new Vector3(0f, ballHeight);
        
        GetComponent<Image>().color = Color.white;
    }

    public IEnumerator Thrown()
    {        
        GetComponent<Image>().color = Color.white;
        //gameObject.SetActive(true);
        transform.localPosition = startPosition;
                
        // float distance = Vector3.Distance(startPosition, airPosition);
        // float stepSize = throwSpeed/distance;

        // float progress = 0;

        // while(progress != 1.0f)
        // {
        //     progress = Mathf.Min(progress + (Time.deltaTime * stepSize), 1.0f);

        //     float parabola = 1.0f - (4.0f * (progress - .5f) * (progress - .5f));

        //     Vector3 nextPos = Vector3.Lerp(startPosition, airPosition, progress);
        //     nextPos.y += parabola * amplitude;

        //     transform.position = nextPos;

        //     yield return null;
        // }

        float curX = transform.localPosition.x;
        float curY = transform.localPosition.y;
        float diffX = airPosition.x - curX;
        float diffY = airPosition.y - curY;

        while(Mathf.Abs(curY - airPosition.y) > Mathf.Epsilon && Mathf.Abs(curX - airPosition.x) > Mathf.Epsilon)
        {      
            curX += diffX * throwSpeed * Time.deltaTime;
            curY += diffY * throwSpeed * Time.deltaTime;
            if((airPosition.x - curX) * diffX <= 0)
            {
                curX = airPosition.x;
            }
            if((airPosition.y - curY) * diffY <= 0)
            {
                curY = airPosition.y;
            }
            transform.localPosition = new Vector3(curX, curY);
            yield return null;
        }

        yield return new WaitForSeconds(pauseAfterThrow);
    }

    public IEnumerator Critical()
    {
        float curX = transform.localPosition.x;

        float startX = curX;
        float leftX = curX - critShake;
        float rightX = curX + critShake;

        float diffX = leftX - curX;

        while(Mathf.Abs(curX - leftX) > Mathf.Epsilon)
        {
            curX += diffX * critSpeed * Time.deltaTime;
            if((leftX - curX) * diffX <= 0)
            {
                curX = leftX;
            }
            transform.localPosition = new Vector3(curX, transform.localPosition.y);
            yield return null;
        }

        while(Mathf.Abs(curX - rightX) > Mathf.Epsilon)
        {
            curX += diffX * critSpeed * Time.deltaTime;
            if((rightX - curX) * diffX <= 0)
            {
                curX = rightX;
            }
            transform.localPosition = new Vector3(curX, transform.localPosition.y);
            yield return null;
        }

        while(Mathf.Abs(curX - leftX) > Mathf.Epsilon)
        {
            curX += diffX * critSpeed * Time.deltaTime;
            if((leftX - curX) * diffX <= 0)
            {
                curX = leftX;
            }
            transform.localPosition = new Vector3(curX, transform.localPosition.y);
            yield return null;
        }

        while(Mathf.Abs(curX - rightX) > Mathf.Epsilon)
        {
            curX += diffX * critSpeed * Time.deltaTime;
            if((rightX - curX) * diffX <= 0)
            {
                curX = rightX;
            }
            transform.localPosition = new Vector3(curX, transform.localPosition.y);
            yield return null;
        }

        while(Mathf.Abs(curX - startX) > Mathf.Epsilon)
        {
            curX += diffX * critSpeed * Time.deltaTime;
            if((startX - curX) * diffX <= 0)
            {
                curX = startX;
            }
            transform.localPosition = new Vector3(curX, transform.localPosition.y);
            yield return null;
        }

        yield return new WaitForSeconds(pauseAfterShake);

        yield return Fall();
    }

    public IEnumerator Fall()
    {
        yield return new WaitForSeconds(pauseAfterThrow);

        float curY = transform.localPosition.y;
        float diff = endPosition.y - curY;

        while(Mathf.Abs(curY - endPosition.y) > Mathf.Epsilon)
        {
            curY += diff * fallSpeed * Time.deltaTime;
            if((endPosition.y - curY) * diff <= 0)
            {
                curY = endPosition.y;
            }
            transform.localPosition = new Vector3(endPosition.x, curY);
            yield return null;
        }

        yield return new WaitForSeconds(pauseAfterFall);
    }

    public IEnumerator Shake()
    {
        float startRotation = transform.eulerAngles.z;
        float firstRotation = startRotation + shakeAngle;
        float secondRotation = startRotation - shakeAngle;

        float cur = transform.eulerAngles.z;
        float targetRotation = firstRotation;
        float diff = targetRotation - cur;

        while(Mathf.Abs(cur - targetRotation) > Mathf.Epsilon)
        {
            cur += diff * shakeSpeed * Time.deltaTime;
            if((targetRotation - cur) * diff <= 0)
            {
                cur = targetRotation;
            }
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, cur);
            yield return null;
        }

        cur = transform.eulerAngles.z;
        targetRotation = startRotation;
        diff = targetRotation - cur;

        while(Mathf.Abs(cur - targetRotation) > Mathf.Epsilon)
        {
            cur += diff * shakeSpeed * Time.deltaTime;
            if((targetRotation - cur) * diff <= 0)
            {
                cur = targetRotation;
            }
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, cur);
            yield return null;
        }

        cur = transform.eulerAngles.z;
        targetRotation = secondRotation;
        diff = targetRotation - cur;

        while(Mathf.Abs(cur - targetRotation) > Mathf.Epsilon)
        {
            cur += diff * shakeSpeed * Time.deltaTime;
            if((targetRotation - cur) * diff <= 0)
            {
                cur = targetRotation;
            }
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, cur);
            yield return null;
        }

        cur = transform.eulerAngles.z;
        targetRotation = startRotation;
        diff = (360-targetRotation) - cur;

        while(Mathf.Abs(cur - targetRotation) > Mathf.Epsilon)
        {
            cur += diff * shakeSpeed * Time.deltaTime;
            if((targetRotation - cur) * diff <= 0)
            {
                cur = targetRotation;
            }
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, cur);
            yield return null;
        }

        yield return new WaitForSeconds(pauseAfterShake);
    }

    public IEnumerator BreakOut()
    {
        gameObject.SetActive(false);
        yield return null;
    }

    public IEnumerator Catch()
    {
        GetComponent<Image>().color = Color.gray;
        yield return null;
    }
}


//TODO:
//Arc the throw
//Add some pizazz when the pokemon is going into the ball
//Catch animation:
//
//Recolor pokeball after catch