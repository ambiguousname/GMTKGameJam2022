using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullOffscreen : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
