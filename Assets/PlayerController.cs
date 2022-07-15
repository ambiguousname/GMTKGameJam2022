using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 10.0f;
    public float mouseSensitivity = 1.0f;

    // TODO: Replace with something else (custom function calls?)
    public Texture2D cursorToUse;

    private Vector2 _accelIntent;
    private Rigidbody2D _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        // TODO: Replace?
        Cursor.SetCursor(cursorToUse, Vector2.zero, CursorMode.Auto);
        Application.targetFrameRate = 60;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(_accelIntent * acceleration);
    }

    // All the events given from inputs. Set by Player Input component. Uses Unity Events.
    #region InputEvents
    public void OnMove(InputValue value) {
        _accelIntent = value.Get<Vector2>();
    }

    #endregion
}
