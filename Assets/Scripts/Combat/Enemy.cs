using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public AudioSource enemyHit_sfx;
    public AudioSource enemyShoot_sfx;
    public AudioSource enemyDead_sfx;


    [Header("Hits")]
    public float health = 100;
    public Sprite damageSprite;
    public float hitTimer = 0.05f;

    private Sprite _defaultSprite;
    private Color _defaultTint;

    [Header("Firing")]
    public GameObject spread;
    public GameObject bullet;
    public float damage = 10.0f;
    public float bulletDrag = 0;
    public float fireForce = 500f;
    public float recoil = 0.1f;
    public float fireDelay = 2;
    public float rotateSpeed = 1;
    public float minDistFromPlayer = 10;
    public float range = 50f;


    [Header("Animation")]
    public SpriteAnimator animator;

    private GameObject _firePoint;
    private GameObject _player;
    private Rigidbody2D _rigidbody;
    private float _fireTimer = 0.0f;
    private float _frozenTimer = 0.0f;
    private float _onFireTimer = 0.0f;
    private int _onFireTimes = 0;

    private IAstarAI _ai;

    // Start is called before the first frame update
    void OnEnable()
    {
        _firePoint = this.gameObject.FindChildWithName("FiringPoint");
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody2D>();
        _ai = GetComponent<IAstarAI>();
        _ai.onSearchPath += FixedUpdate;

        _defaultSprite = GetComponent<SpriteRenderer>().sprite;
        _defaultTint = GetComponent<SpriteRenderer>().color;
    }

    private void OnDisable()
    {
        _ai.onSearchPath -= FixedUpdate;
    }

    void EvaluateSpread()
    {
        // For each bullet in the spread:
        enemyShoot_sfx.Play();
        foreach (Transform child in spread.transform)
        {
            // Get the offset of where the bullet should be based on rotation of the weapon.
            // We only need the rotation of the weapon since it should be automatically pointing to the mouse at all times.
            var newPos = Helper.RotateAroundPivot(child.localPosition, -_firePoint.transform.localPosition, transform.GetChild(0).eulerAngles);
            var rotation = Quaternion.LookRotation(Vector3.forward, newPos);
            // To avoid close collisions, we add the 
            var fired = Instantiate(bullet, _firePoint.transform.position + newPos.normalized * 0.1f, rotation);
            fired.GetComponent<Bullet>().damage = damage;
            fired.GetComponent<Bullet>().range = range;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBullet") {
            enemyHit_sfx.Play();
            health -= collision.GetComponent<Bullet>().damage;
            if (collision.GetComponent<Bullet>().attribute == "fire") {
                _onFireTimer = 1;
                _onFireTimes = 3;
                _frozenTimer = 0;
            }
            if (collision.GetComponent<Bullet>().attribute == "ice") {
                _frozenTimer = 1;
                _onFireTimer = 0;
                _onFireTimes = 0;
            }
            if (!collision.name.Contains("GiantLaser"))
            {
                Destroy(collision.gameObject);
            }
            StartCoroutine(HitPause());
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Contains("GiantLaser")) {
            health -= collision.GetComponent<Bullet>().damage;
            StartCoroutine(HitPause());
        }
    }

    IEnumerator HitPause() {
        GetComponent<SpriteRenderer>().sprite = damageSprite;
        GetComponent<SpriteRenderer>().color = Color.white;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitTimer);
        Time.timeScale = 1;
        GetComponent<SpriteRenderer>().sprite = _defaultSprite;
        GetComponent<SpriteRenderer>().color = _defaultTint;
        if (health <= 0) {
            enemyDead_sfx.Play();
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        var playerDir = _player.transform.position - this.transform.position;
        var hit = Physics2D.Raycast(this.transform.position, playerDir);
        if (hit.transform.tag == "Player")
        {
            if (_frozenTimer <= 0 && _fireTimer <= 0)
            {
                _fireTimer = fireDelay;
                EvaluateSpread();
            }
        }


        if (_frozenTimer <= 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.transform.GetChild(0).rotation = Quaternion.Slerp(this.transform.GetChild(0).rotation, Quaternion.LookRotation(Vector3.forward, playerDir), rotateSpeed * Time.fixedDeltaTime);
        }

        if (_fireTimer > 0)
        {
            _fireTimer -= Time.fixedDeltaTime;
        }

        var distFromPlayer = -(playerDir.normalized * minDistFromPlayer) + _player.transform.position;

        if (_frozenTimer > 0)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
            _ai.destination = this.transform.position;
            _frozenTimer -= Time.fixedDeltaTime;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = _defaultTint;
            _ai.destination = distFromPlayer;
        }
    }

    private void Update()
    {

        if(_rigidbody.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            animator.playAnimation(animator.runRight, .4f);
        } else if (_rigidbody.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            animator.playAnimation(animator.runRight, .4f);
        } else
        {
            animator.playAnimation(animator.idle, .4f);
        }

        if (_onFireTimer > 0)
        {
            _onFireTimer -= Time.deltaTime;
            if (_onFireTimer <= 0)
            {
                if (_onFireTimes > 0)
                {
                    _onFireTimes -= 1;
                    _onFireTimer = 1;
                    health -= 10;
                    StartCoroutine(HitPause());
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = _defaultTint;
                }
            }
        }
    }
}
