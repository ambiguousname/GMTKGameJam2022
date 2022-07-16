using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public bool inCombat = false;
    public List<GameObject> waves;
    private GameObject _wave;
    private int _index = 0;

    private void Update()
    {
        if (inCombat && _wave != null && _wave.transform.childCount <= 0)
        {
            inCombat = false;
            _index = 0;
            FindObjectOfType<PlayerController>().fireEnabled = false;
        }
        else if (inCombat) {
            _wave = waves[_index];
            _wave.SetActive(true);
        }
    }
}
