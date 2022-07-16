using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public GameObject doors;
    public bool inCombat = false;
    public List<GameObject> waves;
    private GameObject _wave;
    private int _index = 0;

    private void Update()
    {
        if (inCombat && _wave != null && _wave.transform.childCount <= 0)
        {
            if (_index < waves.Count - 1)
            {
                _index++;
                _wave.SetActive(false);
                _wave = waves[_index];
                _wave.SetActive(true);
            }
            else
            {
                inCombat = false;
                _index = 0;
                doors.SetActive(false);
                FindObjectOfType<PlayerController>().fireEnabled = false;
            }
        }
        else if (inCombat) {
            doors.SetActive(true);
            _wave = waves[_index];
            _wave.SetActive(true);
        }
    }
}
