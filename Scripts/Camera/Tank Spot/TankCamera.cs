using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCamera : MonoBehaviour {

    BoxCollider2D tankCollider;
    Camera camera;
    Camera parentCamera;
    CameraFollow cameraFollow;
    public LayerMask playerMask;

    // Use this for initialization
    void Start () {
        tankCollider = transform.GetComponent<BoxCollider2D>();
        camera = transform.Find("Camera").GetComponent<Camera>();
        parentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        CheckIfPlayerIn();
    }

    void CheckIfPlayerIn()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, tankCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            GameObject blocks = GameObject.Find("Obstacle").transform.Find("Blocks 08").gameObject;
            blocks.SetActive(true);

            camera.enabled = true;
            parentCamera.enabled = false;
            cameraFollow.enabled = false;
        }
    }
}
