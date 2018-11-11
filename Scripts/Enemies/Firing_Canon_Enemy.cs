using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_space;
using HealthBar.PlayerHealthBar;

public class Firing_Canon_Enemy : Enemy
{
    Vector3 bulletVelocity;

    GameObject bullet;
    GameObject launchingPoint;

    public FiringCannonEnemyInfo info;

    float dirX;
    public Vector2 sailVelocity;

    SpriteRenderer spriteRenderer;

    Animator animator;

    float currentFireTime;
    float TimeToFire;

    bool end;
    
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;

    BoxCollider2D boxCollider;
    public Vector2 fallBackVelocity;
    public override void Start()
    {
        base.Start();
        bullet = transform.Find("Bullet").gameObject;
        launchingPoint = transform.Find("Launching Point").gameObject;
        timeToWaitForReActivate = 2f;
        timeShouldWait = timeToWaitForReActivate;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        animator = transform.GetComponent<Animator>();
        dirX = 1;

        TimeToFire = 0.5f;
        currentFireTime = TimeToFire;
        end = true;

        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        boxCollider = transform.GetComponent<BoxCollider2D>();
    }

    IEnumerator TimeForErasingBullet(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //EraseBullet();
    }

    // Update is called once per frame
    void Update()
    {
        if (end)
        {
            fieldOfView.visionInfo.ableToDetect = true;
        }

        velocity.y += gravity * Time.deltaTime;
        
        

        if (fieldOfView.visionInfo.playerDetected)
        {
            if(currentFireTime > 0)
            {
                currentFireTime -= Time.deltaTime;
            }

            else
            {
                currentFireTime = TimeToFire;
                GameObject bulletClone = Instantiate(bullet, transform) as GameObject;
                bulletClone.transform.position = launchingPoint.transform.position;
                bulletClone.SetActive(true);
            }
        }
        

        enemyCollider.Move(velocity * Time.deltaTime, true);


        if (enemyCollider.collisionInfo.grounded || enemyCollider.collisionInfo.above)
        {
            velocity.y = 0;
        }

        if(Mathf.Abs(velocity.x) > 0.01)
        {
            animator.SetBool("isSailing", true);
            animator.SetBool("isFire", false);
        }

        if (!playerMovement.damaged)
        {
            IfHitPlayer();
        }

    }

    public struct FiringCannonEnemyInfo
    {
        public bool fire;

        public void Reset()
        {
            fire = false;
        }
    }

    void IfHitPlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.transform.position, boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            playerHealth.ReceiveDamage(30);
            playerMovement.damaged = true;
            Vector2 velocity = new Vector2(fallBackVelocity.x * -1, fallBackVelocity.y);

            playerMovement.playerCollider.Move(velocity);

            Destroy(gameObject);
        }
    }
}
