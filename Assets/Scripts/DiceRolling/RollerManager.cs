using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerManager : MonoBehaviour
{
    private Action<int, string> _endCallback;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EnableRolling(Action<int, string> callback) {
        transform.GetChild(0).gameObject.SetActive(true);
        _endCallback = callback;
    }

    public void EndRolling(int outcome, string attribute)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _endCallback(outcome, attribute);
    }
}
