using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCamera : MonoBehaviour {

    BoxCollider2D boatCollider;
    Camera camera;
    Camera parentCamera;
    CameraFollow cameraFollow;
    public LayerMask playerMask;
    GameObject unableToReturnBlock;
    // Use this for initialization
    void Start()
    {
        boatCollider = transform.GetComponent<BoxCollider2D>();
        camera = transform.Find("Camera").GetComponent<Camera>();
        parentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        unableToReturnBlock = transform.Find("Unable To Return Block").gameObject;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckIfPlayerIn();
    }

    void CheckIfPlayerIn()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boatCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            camera.enabled = true;
            parentCamera.enabled = false;
            cameraFollow.enabled = false;
            unableToReturnBlock.SetActive(true);
        }
    }
}
