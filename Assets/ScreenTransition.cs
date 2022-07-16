using Cinemachine;
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
        print(height);
        print(width);
    }

    void GetClosestAndActivate() {
        var cameras = GameObject.FindGameObjectsWithTag("CineCamera");
        var player = GameObject.FindGameObjectWithTag("Player");
        var dist = Vector3.Distance(cameras[0].transform.position, player.transform.position);
        var closest = cameras[0];
        for (int i = 1; i < cameras.Length; i++) {
            var newDist = Vector3.Distance(cameras[i].transform.position, player.transform.position);
            if (newDist < dist) {
                dist = newDist;
                closest = cameras[i];
            }
        }
        closest.GetComponent<ICinemachineCamera>().Priority = 11;
    }

    // Update is called once per frame
    void Update()
    { 
        if (cam.WorldToScreenPoint(player.position).x > Screen.width || cam.WorldToScreenPoint(player.position).x < 0 || cam.WorldToScreenPoint(player.position).y > Screen.height || cam.WorldToScreenPoint(player.position).y < 0)
        {
            GetComponent<CinemachineBrain>().ActiveVirtualCamera.Priority = 10;
            GetClosestAndActivate();
        }

    }
}
