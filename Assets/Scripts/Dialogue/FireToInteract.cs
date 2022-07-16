using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireToInteract : MonoBehaviour
{
    private Action _callback;
    public void Enable(Action callback) {
        _callback = callback;
        FindObjectOfType<PlayerController>().onFire.AddListener(Fire);
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    void Fire() {
        _callback();
        Disable();
    }

    public void Disable() {
        FindObjectOfType<PlayerController>().onFire.RemoveListener(Fire);
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
