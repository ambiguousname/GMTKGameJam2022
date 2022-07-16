using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    bool _triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !_triggered) {
            _triggered = true;
            FindObjectOfType<LoadoutController>().ShowLoadout();
        }
    }
}
