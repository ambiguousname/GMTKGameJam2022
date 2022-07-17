using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    bool _triggered = false;
    public List<GameObject> waves;
    public GameObject teleportTo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<CombatController>().teleportTo = teleportTo;
            if (!_triggered)
            {
                FindObjectOfType<CombatController>().inCombat = false;
                FindObjectOfType<CombatController>().waves = waves;
                _triggered = true;
                FindObjectOfType<LoadoutController>().ShowLoadout();
            }
        }
    }
}
