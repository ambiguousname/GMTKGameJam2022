using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 10.0f;
    public float mouseSensitivity = 1.0f;
    // Player is slow to look, but the gun turns fast (it's the real source of bullets anyways)
    public float bodyLookSpeed = 1.0f;

    private Vector2 _accelIntent;
    private Rigidbody2D _rigidbody;

    [Header("Firing")]
    public float fireDelay = 0.1f;
    public float fireForce = 500.0f;
    public float bulletDrag = 0;
    public GameObject bullet;

    private GameObject _weapon;
    private GameObject _firePoint;

    private bool _intendsToFire = false;
    private float _fireTimer = 0.0f;


    [Header("Cursor")]
    // TODO: Replace with something else (custom function calls?)
    public Texture2D cursorToUse;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        // TODO: Replace?
        Cursor.SetCursor(cursorToUse, Vector2.zero, CursorMode.Auto);
        Application.targetFrameRate = 60;
        _rigidbody = GetComponent<Rigidbody2D>();
        _weapon = this.gameObject.FindChildWithName("Weapon");
        _firePoint = _weapon.FindChildWithName("FiringPoint");
    }

    #region PhysicsUpdates

    private void FixedUpdate()
    {
        Cursor.SetCursor(cursorToUse, Vector2.zero, CursorMode.Auto);
        _rigidbody.AddForce(_accelIntent * acceleration);
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, this.transform.position.y));
        UpdateRotations(mousePos);
        AttemptToFireWeapon(mousePos);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet") {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider, false);
        }
    }

    private void AttemptToFireWeapon(Vector3 mousePos) {
        if (_intendsToFire && _fireTimer <= 0.0f) {
            _fireTimer = fireDelay;
            var dir = mousePos - _firePoint.transform.position;
            var rotation = Quaternion.LookRotation(Vector3.forward, dir);
            var fired = Instantiate(bullet, _firePoint.transform.position, rotation);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fired.GetComponent<Collider2D>(), true);
            var rb = fired.GetComponent<Rigidbody2D>();
            rb.drag = bulletDrag;

            // If we want the bullet to be impacted by current speed:
            //rb.velocity = _rigidbody.velocity;
            //rb.angularVelocity = _rigidbody.angularVelocity;
            // So it's not based on how far away the mouse is:
            var unbiased = new Vector3(dir.x, dir.y);
            rb.AddForce(unbiased.normalized * fireForce);

            _intendsToFire = false;
        }

        if (_fireTimer > 0.0f)
        {
            _fireTimer -= Time.fixedDeltaTime;
        }
    }

    private void UpdateRotations(Vector3 mousePos) {
        Vector3 dir = mousePos - this.transform.position;
        Quaternion lookDir = Quaternion.LookRotation(Vector3.forward, dir);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookDir, bodyLookSpeed * Time.fixedDeltaTime);

        Vector3 weaponDir = mousePos - _weapon.transform.position;
        Quaternion weaponLookDir = Quaternion.LookRotation(Vector3.forward, weaponDir);
        _weapon.transform.rotation = weaponLookDir;
    }

    #endregion

    // All the events given from inputs. Set by Player Input component. Uses Unity Events.
    #region InputEvents
    public void OnMove(InputValue value) {
        _accelIntent = value.Get<Vector2>();
    }

    public void OnFire(InputValue value) {
        _intendsToFire = (value.Get<float>() >= 0.5f) ? true : false;
    }

    #endregion
}
