using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish_block : MonoBehaviour {
    BoxCollider2D boxCollider;
    public LayerMask playerMask;

    GameObject finishIndicator;
    
	// Use this for initialization
	void Start () {
        boxCollider = transform.GetComponent<BoxCollider2D>();
        finishIndicator = GameObject.Find("Finish Indicator").gameObject;
        finishIndicator.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            finishIndicator.SetActive(true);
        }
    }
}
