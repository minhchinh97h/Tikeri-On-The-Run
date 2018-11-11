using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_space;
using HealthBar.PlayerHealthBar;
using HealthBar.EnemyHealthBar;

public class JumpingEnemy : Enemy {

    bool end;

    Animator animator;
    Animator indicatorAnimator;
    PlayerHealth playerHealth;
    PlayerMovement playerMovement;

    public Vector2 fallOffVelocity;
    float oldSignVelX;

    public LayerMask playerAttack;
    EnemyHealthBar enemyHealth;

    GameObject blocks07;
    GameObject blocks06;

    Camera clownCamera;
    Camera mainCamera;
    CameraFollow cameraFollow;

    GameObject clownSpot;
    // Use this for initialization
    public override void Start () {
        base.Start();

        timeShouldWait = 2.5f;
        currentTime = timeShouldWait;
        animator = transform.GetComponent<Animator>();
        indicatorAnimator = transform.Find("Indicator").GetComponent<Animator>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        
        enemyHealth = transform.GetComponent<EnemyHealthBar>();

        blocks07 = GameObject.Find("Obstacle").transform.Find("Blocks 07").gameObject;
        blocks06 = GameObject.Find("Obstacle").transform.Find("Blocks 06").gameObject;

        clownCamera = GameObject.Find("Clown Spot").transform.Find("Camera").GetComponent<Camera>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();

        clownSpot = GameObject.Find("Clown Spot").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (enemyCollider.collisionInfo.grounded)
        {
            if (fieldOfView.visionInfo.playerDetected)
            {
                fieldOfView.visionInfo.playerWasDetected = true;
                fieldOfView.visionInfo.ableToDetect = false;
            }

            else
            {
                fieldOfView.visionInfo.ableToDetect = true;

                animator.SetBool("isOnAir", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isFall", false);
                indicatorAnimator.SetBool("isJumping", false);
            }
        }

        if(fieldOfView.visionInfo.playerDetected || fieldOfView.visionInfo.playerWasDetected)
        {
            
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                animator.SetBool("isJumping", true);
                indicatorAnimator.SetBool("isJumping", true);
            }

            else
            {
                currentTime = timeShouldWait;
                CalculateTheJump(ref velocity);
                fieldOfView.visionInfo.playerDetected = false;
                fieldOfView.visionInfo.playerWasDetected = false;
                animator.SetBool("isJumping", false);
                indicatorAnimator.SetBool("isJumping", false);
            }
        }


        velocity.y += gravity * Time.deltaTime;

        enemyCollider.Move(velocity * Time.deltaTime, true);


        if (enemyCollider.collisionInfo.grounded || enemyCollider.collisionInfo.above)
        {
            velocity.y = 0;
            velocity.x = 0;
        }

        if(!enemyCollider.collisionInfo.grounded && velocity.y > 0)
        {
            animator.SetBool("isOnAir", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFall", false);
            indicatorAnimator.SetBool("isJumping", false);
        }

        else if(!enemyCollider.collisionInfo.grounded && velocity.y < 0)
        {
            animator.SetBool("isFall", true);
            animator.SetBool("isOnAir", false);
            animator.SetBool("isJumping", false);
            indicatorAnimator.SetBool("isJumping", false);
        }

        if (!playerMovement.damaged)
        {
            IfHitPlayer();
        }

        if (!isDamaged)
        {
            IfHitByPlayer();
        }

        else
        {
            if(currentDamagedTime > 0)
            {
                currentDamagedTime -= Time.deltaTime;

            }

            else
            {
                currentDamagedTime = DamagedTime;
                isDamaged = false;
            }
        }

        if (enemyHealth.isDead)
        {
            Destroy(gameObject);

            blocks07.SetActive(true);
            blocks06.SetActive(false);
            mainCamera.enabled = true;
            cameraFollow.enabled = true;
            clownCamera.enabled = false;
            clownSpot.SetActive(false);
        }
    }
    
    void CalculateTheJump(ref Vector3 velocity)
    {
        float maxHeight = Mathf.Abs(fieldOfView.visionInfo.distanceToPlayerY) + jumpHeight;

        velocity.y = Mathf.Sqrt(maxHeight * 2 * Mathf.Abs(gravity));

        timeOfFlight = 2 * velocity.y / Mathf.Abs(gravity);

        velocity.x = fieldOfView.visionInfo.distanceToPlayerX / timeOfFlight;
    }


    void IfHitPlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(enemyCollider.boxCollider.transform.position, enemyCollider.boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

        if (hit)
        {
            playerHealth.ReceiveDamage(30);
            playerMovement.damaged = true;
            Vector2 velocity = new Vector2(fallOffVelocity.x * -Mathf.Sign(hit.normal.x), fallOffVelocity.y);

            playerMovement.playerCollider.Move(velocity);
        }
    }

    void IfHitByPlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(enemyCollider.boxCollider.transform.position, enemyCollider.boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerAttack);

        if (hit)
        {
            isDamaged = true;
            enemyHealth.ReceiveDamage(2);
        }
    }
}
