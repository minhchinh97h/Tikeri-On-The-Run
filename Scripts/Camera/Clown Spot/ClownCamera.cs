using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownCamera : MonoBehaviour
{

    BoxCollider2D clownCollider;
    Camera camera;
    Camera parentCamera;
    CameraFollow cameraFollow;
    public LayerMask playerMask;

    GameObject blocks;

    // Use this for initialization
    void Start()
    {
        clownCollider = transform.GetComponent<BoxCollider2D>();
        camera = transform.Find("Camera").GetComponent<Camera>();
        parentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        blocks = GameObject.Find("Obstacle").transform.Find("Blocks 06").gameObject;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckIfPlayerIn();
    }


    void CheckIfPlayerIn()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, clownCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            blocks.SetActive(true);
            camera.enabled = true;
            parentCamera.enabled = false;
            cameraFollow.enabled = false;
        }
    }
}
