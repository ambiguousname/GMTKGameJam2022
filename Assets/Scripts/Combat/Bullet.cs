using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 0;
    public float range = 0;
    public bool isLaser = false;
    public string attribute = "";
    private Vector3 _startDist;

    private void Start()
    {
        _startDist = this.transform.position;
    }

    private void Update()
    {
        if (attribute == "fire") {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (attribute == "ice") {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        if (!isLaser && Vector3.Distance(_startDist, this.transform.position) > range) {
            Destroy(this.gameObject);
        }
    }
}
