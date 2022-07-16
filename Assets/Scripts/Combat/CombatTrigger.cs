using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    bool _triggered = false;
    private void OnBecameVisible()
    {
        FindObjectOfType<CombatController>().inCombat = false;
        if (!_triggered) {
            _triggered = true;
            FindObjectOfType<LoadoutController>().ShowLoadout();
        }
    }
}
