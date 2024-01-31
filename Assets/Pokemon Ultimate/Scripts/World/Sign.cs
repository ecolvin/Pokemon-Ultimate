using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, Interactable
{
    [SerializeField] Dialog signText;

    public void Interact(Vector3 playerPos)
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(signText, () => {}));
    }
}
