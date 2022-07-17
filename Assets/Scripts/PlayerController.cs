using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool moveEnabled = true;
    [HideInInspector]
    public UnityEvent onFire;

    public AudioSource playerHit_sfx;
    public AudioSource playerFire_sfx;

    [Header("Movement")]
    public float acceleration = 10.0f;
    // Player is slow to look, but the gun turns fast (it's the real source of bullets anyways)
    public float bodyLookSpeed = 1.0f;

    private Vector2 _accelIntent;
    private Rigidbody2D _rigidbody;

    [Header("Firing")]
    public float fireDelay = 0.1f;
    public float fireForce = 500.0f;
    public float bulletDrag = 0;
    public float recoil = 0.1f;
    public float damage = 50f;
    public int ammo = 10;
    public int maxAmmo = 10;
    public GameObject bullet;
    public GameObject giantLaser;
    public GameObject spread;
    public bool fireFastAsTrigger = false;
    public bool isGiantLaser = false;
    public float range = 1000f;

    private GameObject _giantLaser;
    private GameObject _weapon;
    private GameObject _firePoint;

    private bool _intendsToFire = false;
    private float _fireTimer = 0.0f;
    private bool _isReloading = false;

    [Header("Damage")]
    public float health = 100;
    public Sprite damageSprite;
    public float damagePauseTimer = 0.1f;
    public bool fireEnabled = false;

    private Sprite _defaultSprite;
    private Color _defaultTint;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Application.targetFrameRate = 60;
        _rigidbody = GetComponent<Rigidbody2D>();
        _weapon = this.gameObject.FindChildWithName("Weapon");
        _firePoint = _weapon.FindChildWithName("FiringPoint");

        _defaultSprite = GetComponent<SpriteRenderer>().sprite;
        _defaultTint = GetComponent<SpriteRenderer>().color;
        _giantLaser = Instantiate(giantLaser, _firePoint.transform);
        _giantLaser.SetActive(false);
    }

    #region PhysicsUpdates

    private void FixedUpdate()
    {
        if (moveEnabled)
        {
            _rigidbody.AddForce(_accelIntent * acceleration);
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, this.transform.position.y));
            UpdateRotations(mousePos);
            AttemptToFireWeapon();
        }
        else {
            _rigidbody.velocity = Vector2.zero;
        }
    }
    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        //float angle360 =  (signed_angle + 180) % 360;

        return signed_angle;
    }

    void EvaluateSpread()
    {
        // For each bullet in the spread:
        foreach (Transform child in spread.transform) {
            // Get the offset of where the bullet should be based on rotation of the weapon.
            // We only need the rotation of the weapon since it should be automatically pointing to the mouse at all times.
            var newPos = Helper.RotateAroundPivot(child.localPosition, -_firePoint.transform.localPosition, _weapon.transform.eulerAngles);
            var rotation = Quaternion.LookRotation(Vector3.forward, newPos);
            var fired = Instantiate(bullet, _firePoint.transform.position, rotation);
            fired.GetComponent<Bullet>().damage = damage;
            fired.GetComponent<Bullet>().range = range;
            fired.GetComponent<Bullet>().attribute = FindObjectOfType<LoadoutController>().activeAttr;

            var rb = fired.GetComponent<Rigidbody2D>();
            rb.drag = bulletDrag;

            // If we want the bullet to be impacted by current speed:
            //rb.velocity = _rigidbody.velocity;
            //rb.angularVelocity = _rigidbody.angularVelocity;
            // So it's not based on how far away the mouse is:
            var angle = SignedAngleBetween(child.localPosition, Vector3.zero, Vector3.forward);
            angle -= (3 * Mathf.PI/2) + Mathf.Deg2Rad * (_weapon.transform.eulerAngles.z);
            var unbiased = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
            rb.AddForce(unbiased.normalized * fireForce);
            _rigidbody.AddForce(-unbiased.normalized * recoil);
        }
        FindObjectOfType<Shake>().shake = 0.03f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet") {
            health -= collision.GetComponent<Bullet>().damage;
            playerHit_sfx.Play();
            Destroy(collision.gameObject);
            StartCoroutine(HitPause());

            if (health <= 0)
            {
                GameOver();
            }
        }
    }
    IEnumerator HitPause() {
        Time.timeScale = 0;
        GetComponent<SpriteRenderer>().sprite = damageSprite;
        GetComponent<SpriteRenderer>().color = Color.white;
        FindObjectOfType<Shake>().shakeAmount = damagePauseTimer;
        FindObjectOfType<Shake>().shake = 0.2f;
        yield return new WaitForSecondsRealtime(damagePauseTimer);
        GetComponent<SpriteRenderer>().sprite = _defaultSprite;
        GetComponent<SpriteRenderer>().color = _defaultTint;
        Time.timeScale = 1;
    }

    IEnumerator Reload() {
        _isReloading = true;
        yield return new WaitForSeconds(1);
        _isReloading = false;
        FindObjectOfType<LoadoutController>().Reload();
    }

    private void GameOver()
    {
        SceneManager.LoadScene(3);
    }

    private void AttemptToFireWeapon() {
        if (fireEnabled && _intendsToFire && _fireTimer <= 0.0f && ammo > 0) {

            playerFire_sfx.Play();

            ammo--;

            if (ammo == 0)
            {
                StartCoroutine(Reload());
            }

            _fireTimer = fireDelay;
            if (isGiantLaser)
            {
                _giantLaser.SetActive(true);
            }
            else
            {
                EvaluateSpread();
            }

            if (fireFastAsTrigger) {
                _intendsToFire = false;
            }
        }

        if (_fireTimer > 0.0f)
        {
            _fireTimer -= Time.fixedDeltaTime;
            if (_fireTimer <= 0 && _giantLaser.activeInHierarchy) {
                _giantLaser.SetActive(false);
            }
        }
    }

    private void UpdateRotations(Vector3 mousePos) {
        Vector3 dir = mousePos - this.transform.position;
        Quaternion lookDir = Quaternion.LookRotation(Vector3.forward, dir);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookDir, bodyLookSpeed * Time.fixedDeltaTime);

        Vector3 weaponDir = mousePos - _weapon.transform.position;
        Quaternion weaponLookDir = Quaternion.LookRotation(Vector3.forward, weaponDir);
        _weapon.transform.LookAt(mousePos, Vector3.forward);
    }

    #endregion

    // All the events given from inputs. Set by Player Input component. Uses Unity Events.
    #region InputEvents
    public void OnMove(InputValue value) {
        _accelIntent = value.Get<Vector2>();
    }

    public void OnFire(InputValue value) {
        if (!fireFastAsTrigger)
        {
            _intendsToFire = (value.Get<float>() >= 0.5f) ? true : false;
        }
    }

    public void OnFireFastAsTrigger(InputValue value) {
        if (fireFastAsTrigger) {
            _intendsToFire = true;
        }
        onFire.Invoke();
    }

    public void OnReload(InputValue value) {
        if (value.Get<float>() >= 0.5f) {
            if (fireEnabled && !_isReloading)
            {
                ammo = 0;
                StartCoroutine(Reload());
            }
        }
    }

    #endregion
}
