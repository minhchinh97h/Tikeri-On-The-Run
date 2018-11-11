using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_bullet_space;
using Enemy_space;
using HealthBar.PlayerHealthBar;

public class CannonBullet : Bullet
{

    Transform vision;
    FieldOfView fieldOfView;
    Transform parent;
    public float velocityX;
    Transform lauchPointTransform;

    Vector2 initialVelocity;

    public float initialVel;
    
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;
    public Vector2 fallBackVelocity;
    
    
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        parent = transform.parent;
        vision = parent.Find("Vision").transform;

        fieldOfView = vision.GetComponent<FieldOfView>();

        lauchPointTransform = parent.Find("Launching Point").transform;

        CalculateTheLaunch(ref velocity);

        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        circleCollider = transform.GetComponent<CircleCollider2D>();

        
    }

    // Update is called once per frame
    void Update()
    {
        DestroyThisBullet();

        if (!playerMovement.damaged)
        {
            IfHitPlayer();
        }

        transform.Translate(velocity * Time.deltaTime);
        //velocity.y += gravity * Time.deltaTime;
    }

    void CalculateTheLaunch(ref Vector3 velocity)
    {
        //float angle = Vector2.Angle(fieldOfView.visionInfo.dirToPlayer, Vector2.left);

        //float timeOfFlight = Mathf.Abs(fieldOfView.visionInfo.distanceToPlayerX) / velocityX;
        //velocity.x = velocityX * Mathf.Sign(fieldOfView.visionInfo.distanceToPlayerX);
        //velocity.y = fieldOfView.visionInfo.distanceToPlayerY / timeOfFlight;


        float g = Mathf.Abs(gravity) * Time.deltaTime;
        float d = fieldOfView.visionInfo.distanceToPlayerX;
        float h = fieldOfView.visionInfo.distanceToPlayerY;

        float timeOfFlight = (Mathf.Sqrt(Mathf.Sqrt(g) * ((g * Mathf.Pow(d, 2) + g * Mathf.Pow(h, 2)) / (Mathf.Pow(initialVel, 2) - Mathf.Pow(g, 2))) + (Mathf.Pow(h, 2) / Mathf.Pow((Mathf.Pow(initialVel, 2) - Mathf.Pow(g, 2)), 2))) - (h / (Mathf.Pow(initialVel, 2) - Mathf.Pow(g, 2))))/ Mathf.Sqrt(g);

        velocity.x = d / timeOfFlight;
        velocity.y = h / timeOfFlight - g;
        
    }

    void IfHitPlayer()
    {
        RaycastHit2D hit = Physics2D.CircleCast(circleCollider.transform.position, circleCollider.radius,Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            playerHealth.ReceiveDamage(10);
            playerMovement.damaged = true;

            Destroy(gameObject);
        }
    }
}
