using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_bullet_space;
using Enemy_space;
using HealthBar.PlayerHealthBar;

public class FollowingBullet : Bullet
{
    PlayerHealth playerHealth;
    FieldOfView fieldOfView;
    Firing_Follow_Enemy followEnemy;
    Transform launchingPoint;
    CircleCollider2D collider;
    Transform playerTransform;
    PlayerMovement playerMovement;

    // Use this for initialization
    new void Start()
    {
        fieldOfView = transform.parent.Find("Vision").transform.GetComponent<FieldOfView>();
        followEnemy = transform.parent.GetComponent<Firing_Follow_Enemy>();
        collider = GetComponent<CircleCollider2D>();
        launchingPoint = transform.parent.Find("Launching Point").transform;
        playerTransform = GameObject.Find("Player").transform;
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followEnemy.firing)
        {
            //if(fieldOfView.visionInfo.playerPosition != Vector3.zero)
            //{
            //transform.Translate(fieldOfView.visionInfo.playerPosition * 5f * Time.deltaTime);
            transform.Translate((playerTransform.position - transform.position).normalized * 5f * Time.deltaTime);
            //transform.Translate((fieldOfView.visionInfo.playerPosition - launchingPoint.position).normalized * 2f * Time.deltaTime);
            IfHitPlayer();
            //}
        }

    }

    void IfHitPlayer()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, collider.radius, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            followEnemy.endProgress = true;

            playerHealth.ReceiveDamage(30);
            playerMovement.damaged = true;
        }
    }

}
