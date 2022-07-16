using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    public Transform player;
    public Camera cam;
    private float height;
    private float width;

    // Start is called before the first frame update
    void Start()
    {
        height = cam.orthographicSize * 2;
        width = height * cam.aspect;
    }

    // Update is called once per frame
    void Update()
    { 
        if (cam.WorldToScreenPoint(player.position).x > Screen.width)
        {
            transform.position += new Vector3(width, 0, 0);
        }
        else if (cam.WorldToScreenPoint(player.position).x < 0)
        {
            transform.position -= new Vector3(width, 0, 0);
        }

        if (cam.WorldToScreenPoint(player.position).y > Screen.height)
        {
            transform.position += new Vector3(0, height, 0);
        }
        else if (cam.WorldToScreenPoint(player.position).y < 0)
        {
            transform.position -= new Vector3(0, height, 0);
        }

    }
}
