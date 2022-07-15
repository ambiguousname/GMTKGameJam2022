using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 10.0f;
    public float mouseSensitivity = 1.0f;
    // Player is slow to look, but the gun turns fast (it's the real source of bullets anyways)
    public float bodyLookSpeed = 1.0f;

    // TODO: Replace with something else (custom function calls?)
    public Texture2D cursorToUse;

    private Vector2 _accelIntent;
    private Rigidbody2D _rigidbody;
    private GameObject _weapon;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        // TODO: Replace?
        Cursor.SetCursor(cursorToUse, Vector2.zero, CursorMode.Auto);
        Application.targetFrameRate = 60;
        _rigidbody = GetComponent<Rigidbody2D>();
        _weapon = this.gameObject.FindChildWithName("Weapon");
    }

    private void FixedUpdate()
    {
        Cursor.SetCursor(cursorToUse, Vector2.zero, CursorMode.Auto);
        _rigidbody.AddForce(_accelIntent * acceleration);
        UpdateRotations();
    }

    private void UpdateRotations() {

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, this.transform.position.y));
        Vector3 dir = mousePos - this.transform.position;
        Quaternion lookDir = Quaternion.LookRotation(Vector3.forward, dir);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookDir, bodyLookSpeed * Time.fixedDeltaTime);

        Vector3 weaponDir = mousePos - _weapon.transform.position;
        Quaternion weaponLookDir = Quaternion.LookRotation(Vector3.forward, weaponDir);
        _weapon.transform.rotation = weaponLookDir;
    }

    // All the events given from inputs. Set by Player Input component. Uses Unity Events.
    #region InputEvents
    public void OnMove(InputValue value) {
        _accelIntent = value.Get<Vector2>();
    }

    #endregion
}
