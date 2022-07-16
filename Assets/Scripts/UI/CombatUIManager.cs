using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    public void Show(bool enable) {
        this.transform.GetChild(0).gameObject.SetActive(enable);
    }
}
