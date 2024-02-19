using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTrigger
{
    IEnumerator OnPlayerTriggered(PlayerController player);
}
