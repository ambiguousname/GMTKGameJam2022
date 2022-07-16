using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject spread;
    public GameObject bullet;
    public float bulletDrag = 0;
    public float fireForce = 500f;
    public float recoil = 0.1f;
    public float fireDelay = 2;
    public float rotateSpeed = 1;

    private GameObject _firePoint;
    private GameObject _player;
    private Rigidbody2D _rigidbody;
    private float _fireTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _firePoint = this.gameObject.FindChildWithName("FiringPoint");
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void EvaluateSpread()
    {
        // For each bullet in the spread:
        foreach (Transform child in spread.transform)
        {
            // Get the offset of where the bullet should be based on rotation of the weapon.
            // We only need the rotation of the weapon since it should be automatically pointing to the mouse at all times.
            var newPos = Helper.RotateAroundPivot(child.localPosition, -_firePoint.transform.localPosition, this.transform.eulerAngles);
            var rotation = Quaternion.LookRotation(Vector3.forward, newPos);
            // To avoid close collisions, we add the 
            var fired = Instantiate(bullet, _firePoint.transform.position + newPos.normalized * 0.1f, rotation);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fired.GetComponent<Collider2D>(), true);

            var rb = fired.GetComponent<Rigidbody2D>();
            rb.drag = bulletDrag;

            // If we want the bullet to be impacted by current speed:
            //rb.velocity = _rigidbody.velocity;
            //rb.angularVelocity = _rigidbody.angularVelocity;
            // So it's not based on how far away the mouse is:
            var unbiased = new Vector3(newPos.x, newPos.y);
            rb.AddForce(unbiased.normalized * fireForce);
            _rigidbody.AddForce(-unbiased.normalized * recoil);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var playerDir = _player.transform.position - this.transform.position;
        var hit = Physics2D.Raycast(this.transform.position, playerDir);
        Debug.DrawLine(this.transform.position, hit.point);
        if (hit.transform.tag == "Player") {
            if (_fireTimer <= 0 && Vector3.Dot(this.transform.up, _player.transform.position) > 0)
            {
                _fireTimer = fireDelay;
                EvaluateSpread();
            }

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(Vector3.forward, playerDir), rotateSpeed * Time.fixedDeltaTime);
        }

        if (_fireTimer > 0) {
            _fireTimer -= Time.fixedDeltaTime;
        }
    }
}
