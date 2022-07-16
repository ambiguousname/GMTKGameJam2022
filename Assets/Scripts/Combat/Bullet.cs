using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 0;
    public float range = 0;
    private Vector3 _startDist;

    private void Start()
    {
        _startDist = this.transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(_startDist, this.transform.position) > range) {
            Destroy(this.gameObject);
        }
    }
}
