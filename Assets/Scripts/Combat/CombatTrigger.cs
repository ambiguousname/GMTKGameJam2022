using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    bool _triggered = false;
    public List<GameObject> waves;
    private void OnBecameVisible()
    {
        FindObjectOfType<CombatController>().inCombat = false;
        if (!_triggered) {
            FindObjectOfType<CombatController>().waves = waves;
            _triggered = true;
            FindObjectOfType<LoadoutController>().ShowLoadout();
        }
    }
}
