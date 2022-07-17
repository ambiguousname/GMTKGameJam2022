using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandVFX : MonoBehaviour
{
    public void Landed() {
        GetComponentInParent<DiceDragAndDrop>().LandEnd();
    }
}
