using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, Interactable
{
    [SerializeField] Dialog signText;

    public IEnumerator Interact(Vector3 playerPos)
    {
        yield return DialogManager.Instance.ShowDialog(signText);
    }
}
