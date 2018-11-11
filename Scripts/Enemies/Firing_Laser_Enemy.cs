using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_space;
using HealthBar.PlayerHealthBar;
using HealthBar.EnemyHealthBar;

public class Firing_Laser_Enemy : Enemy
{
    public Vector2 fallOffVelocity;
    Transform launchingPoint;

    float currentTimeForLaserShown;
    float timeForLaserShown;

    float currentTimeToFire;
    float timeToFire;

    public Vector2 rollingVelocity;

    float dirX;
    Tank_FieldOfView tank_fieldOfView;

    SpriteRenderer spriteRenderer;
    SpriteRenderer laserLeftSprite;
    SpriteRenderer laserRightSprite;

    Animator animator;
    Animator fireIndicatorAnimator;
    Animator laserLeftAnimator;
    Animator laserRightAnimator;


    BoxCollider2D movingRangeCollider;
    bool end;

    BoxCollider2D laserLeftCollider;
    BoxCollider2D laserRightCollider;

    PlayerHealth playerHealth;
    PlayerMovement playerMovement;
    
    LaserLeft laserLeft;
    LaserRight laserRight;

    EnemyHealthBar enemyHealth;
    public LayerMask playerAttack;

    GameObject tankSpot;
    Camera tankCamera;
    Camera mainCamera;
    CameraFollow cameraFollow;
    GameObject blocks10;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        tank_fieldOfView = transform.Find("Vision").GetComponent<Tank_FieldOfView>();
        launchingPoint = transform.Find("Launching Point").transform;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        animator = transform.GetComponent<Animator>();
        fireIndicatorAnimator = transform.Find("Firing Indicator").GetComponent<Animator>();

        movingRangeCollider = transform.parent.Find("Moving Range").GetComponent<BoxCollider2D>();

        laserLeftCollider = transform.Find("Laser Left").GetComponent<BoxCollider2D>();
        laserRightCollider = transform.Find("Laser Right").GetComponent<BoxCollider2D>();

        laserLeftSprite = transform.Find("Laser Left").GetComponent<SpriteRenderer>();
        laserRightSprite = transform.Find("Laser Right").GetComponent<SpriteRenderer>();

        laserLeftAnimator = transform.Find("Laser Left").GetComponent<Animator>();
        laserRightAnimator = transform.Find("Laser Right").GetComponent<Animator>();

        laserLeft = transform.Find("Laser Left").GetComponent<LaserLeft>();
        laserRight = transform.Find("Laser Right").GetComponent<LaserRight>();

        timeToWaitForReActivate = 3f;

        timeForLaserShown = 2.5f;
        currentTimeForLaserShown = timeForLaserShown;
        dirX = 1;

        timeToFire = 1.3f;
        currentTimeToFire = timeToFire;

        end = true;

        laserLeftCollider.enabled = false;
        laserLeftSprite.enabled = false;
        laserLeft.enabled = false;

        laserRightCollider.enabled = false;
        laserRightSprite.enabled = false;
        laserRight.enabled = false;
        

        enemyHealth = transform.GetComponent<EnemyHealthBar>();
        tankSpot = GameObject.Find("Tank Spot").gameObject;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        tankCamera = tankSpot.transform.Find("Camera").GetComponent<Camera>();
        blocks10 = GameObject.Find("Blocks 10").gameObject;
        //StartCoroutine("WaitThenRoll", 3f);
    }

    //IEnumerator WaitThenRoll(float time)
    //{
    //    while (true)
    //    {
    //        CalculateTheRollVelocity(ref velocity);
    //        yield return new WaitForSeconds(time);

    //    }
    //}
    

    // Update is called once per frame
    void Update()
    {
        if (end)
        {
            tank_fieldOfView.visionInfo.ableToDetect = true;
            laserLeftCollider.enabled = false;
            laserRightCollider.enabled = false;
        }

        velocity.y += gravity * Time.deltaTime;

        
        if (enemyCollider.boxCollider.bounds.min.x > movingRangeCollider.bounds.min.x || enemyCollider.boxCollider.bounds.max.x < movingRangeCollider.bounds.max.x)
        {

            velocity.x = dirX * rollingVelocity.x;

            
        }

        if(enemyCollider.boxCollider.bounds.min.x <= movingRangeCollider.bounds.min.x || enemyCollider.boxCollider.bounds.max.x >= movingRangeCollider.bounds.max.x)
        {
            dirX = dirX * -1;
            velocity.x = dirX * rollingVelocity.x;
        }

        spriteRenderer.flipX = dirX == 1;

        if (tank_fieldOfView.visionInfo.playerDetected)
        {
            tank_fieldOfView.visionInfo.playerWasDetected = true;
        }

        if (tank_fieldOfView.visionInfo.playerWasDetected)
        {
            end = false;

            tank_fieldOfView.visionInfo.ableToDetect = false;

            velocity.x = 0;

            spriteRenderer.flipX = Mathf.Sign(tank_fieldOfView.visionInfo.distanceToPlayerX) == 1;
            
            if (currentTimeToFire > 0)
            {
                currentTimeToFire -= Time.deltaTime;
            }

            else
            {
                animator.SetBool("isDuringFire", true);
                fireIndicatorAnimator.SetBool("isDuringFire", true);

                laserLeftAnimator.SetBool("isDuringFire", true);
                laserRightAnimator.SetBool("isDuringFire", true);

                if(Mathf.Sign(tank_fieldOfView.visionInfo.distanceToPlayerX) == -1)
                {
                    laserLeftCollider.enabled = true;
                    laserLeftSprite.enabled = true;
                    laserLeft.enabled = true;
                    
                    laserRightCollider.enabled = false;
                    laserRightSprite.enabled = false;
                    laserRight.enabled = false;
                }

                else
                {
                    laserLeftCollider.enabled = false;
                    laserLeftSprite.enabled = false;
                    laserLeft.enabled = false;

                    laserRightCollider.enabled = true;
                    laserRightSprite.enabled = true;
                    laserRight.enabled = true;
                }

                if(currentTimeForLaserShown > 0)
                {
                    currentTimeForLaserShown -= Time.deltaTime;
                }

                else
                {
                    currentTimeForLaserShown = timeForLaserShown;
                    currentTimeToFire = timeToFire;
                    tank_fieldOfView.visionInfo.ableToDetect = false;
                    tank_fieldOfView.visionInfo.playerWasDetected = false;

                    animator.SetBool("isRolling", false);
                    animator.SetBool("isFiring", false);
                    animator.SetBool("isDuringFire", false);

                    fireIndicatorAnimator.SetBool("isFiring", false);
                    fireIndicatorAnimator.SetBool("isDuringFire", false);
                    end = true;

                    laserLeftCollider.enabled = false;
                    laserLeftSprite.enabled = false;
                    laserLeft.enabled = false;

                    laserRightCollider.enabled = false;
                    laserRightSprite.enabled = false;
                    laserRight.enabled = false;

                    laserLeftAnimator.SetBool("isDuringFire", false);
                    laserRightAnimator.SetBool("isDuringFire", false);
                }
            }
        }


        enemyCollider.Move(velocity * Time.deltaTime, true);

        if (enemyCollider.collisionInfo.grounded || enemyCollider.collisionInfo.above)
        {
            velocity.y = 0;
        }

        if(velocity.x != 0)
        {
            animator.SetBool("isFiring", false);
            fireIndicatorAnimator.SetBool("isFiring", false);
            animator.SetBool("isRolling", true);
        }

        else
        {
            animator.SetBool("isFiring", true);
            fireIndicatorAnimator.SetBool("isFiring", true);
            animator.SetBool("isRolling", false);
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
            if (currentDamagedTime > 0)
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

            mainCamera.enabled = true;
            cameraFollow.enabled = true;
            tankCamera.enabled = false;
            tankSpot.SetActive(false);
            blocks10.SetActive(false);
        }
    }

    //void CalculateTheRollVelocity(ref Vector3 velocity)
    //{
    //    velocity = dirX * rollingVelocity;

    //    dirX = dirX * -1;

    //}

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
