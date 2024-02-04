using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI qtyText;
    [SerializeField] TextMeshProUGUI xText;
    [SerializeField] Image background;
    [SerializeField] Image sprite;
    [SerializeField] Image qtyBox;
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    [SerializeField] float arrowSpeed = 15;
    [SerializeField] float arrowMoveDistance = 5;

    public TextMeshProUGUI NameText {get => nameText;}
    public TextMeshProUGUI QtyText {get => qtyText;}
    public TextMeshProUGUI XText {get => xText;}
    public Image Background {get => background;}

    RectTransform rectTransform;
    public float Height => rectTransform.rect.height;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.ItemName;
        qtyText.text = $"{itemSlot.Quantity}";
        sprite.sprite = itemSlot.Item.Icon;
    }

    public void OpenQtySelector()
    {
        upArrow.gameObject.SetActive(true);
        downArrow.gameObject.SetActive(true);
        qtyBox.color = GlobalSettings.Instance.SelectedBarColor;
        qtyText.text = "1";
        qtyText.color = Color.black;
        xText.color = Color.black;
    }

    public void CloseQtySelector(int slotQty)
    {
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
        qtyBox.color = Color.clear;
        qtyText.text = $"{slotQty}";
        qtyText.color = Color.white;
        xText.color = Color.white;
    }

    public IEnumerator UpdateQtySelector(bool increase, int newQty)
    {
        Image arrow;
        if(increase)
        {
            arrow = upArrow;
        }
        else
        {
            arrow = downArrow;
        }

        qtyText.text = $"{newQty}";

        Vector3 initPos = arrow.transform.localPosition;
        float targetY = initPos.y;
        if(increase)
        { 
            targetY += arrowMoveDistance;
        }
        else
        {
            targetY -= arrowMoveDistance;
        }
        float curY = arrow.transform.localPosition.x;
        float diff = initPos.y - curY;
        
        while(Mathf.Abs(curY - targetY) > Mathf.Epsilon)
        {
            curY += diff * arrowSpeed * Time.deltaTime;
            if((targetY - curY) * diff <= 0)
            {
                curY = targetY;
            }
            arrow.transform.localPosition = new Vector3(initPos.x, curY);
            yield return null;
        }

        arrow.transform.localPosition = initPos;
        yield return null;
    }
}
