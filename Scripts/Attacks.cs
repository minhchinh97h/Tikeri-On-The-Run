using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    PolygonCollider2D attackCollider;

    public LayerMask enemyMask;

    Collider2D[] colliderBuffer;

	// Use this for initialization
	void Start () {
        attackCollider = transform.Find("Attack 01").GetComponent<PolygonCollider2D>();
        colliderBuffer = new Collider2D[16];
    }
	
	// Update is called once per frame
	void Update () {
        IfHitEnemy();

    }


    void IfHitEnemy()
    {
        Debug.Log(attackCollider.IsTouchingLayers());
    }
}
