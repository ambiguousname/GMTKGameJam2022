using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public bool inCombat = false;
    public List<Enemy> enemies;

    private void Update()
    {
        if (inCombat && enemies.Count <= 0)
        {
            inCombat = false;
            FindObjectOfType<PlayerController>().fireEnabled = true;
        }
        else if (!inCombat && enemies.Count > 0) {
            enemies.Clear();
        }
    }
}
