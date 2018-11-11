using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{

    Vector3 velocity;

    public float jumpHeight = 4f;
    public float timeToJumpApex = .4f;

    float accelerationTimeAirbone = 0.03f;
    float accelerationTimeGrounded = .05f;
    float accelerationTimeSprint = 0.007f;
    public float moveSpeed;
    public float sprintSpeed;

    [HideInInspector]
    public static float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    float sprintXSmoothing;
    bool wallSliding;
    float wallSlidingVel = 1f;

    public Vector2 wallJumpOff;
    public Vector2 wallDropOff;
    public Vector2 wallLeap;

    [HideInInspector]
    public Collider2D playerCollider;

    [HideInInspector]
    public Vector2 playerInput;

    public float wallStickTime = 0.25f;
    float wallToUnstickTime;
    float totalWaitTime;
    [HideInInspector]
    public bool leftCtrlPressedWhileHangingOnWall;
    bool isDashing;


    int numberOfTimesSpacePressed;
    int numberOfTimesLeftCtrlPressed;
    bool ableToJumpAgain;

    float lastDirX;
    bool disableLeftControl;

    [HideInInspector]
    public bool isDead;

    [HideInInspector]
    public bool damaged;

    Animator animator;
    public SpriteRenderer sprite;

    float currentTimeAttack;
    float timeAttack;

    bool attack;

    PolygonCollider2D attack01Collider;
    Transform attacksTransform;

    int rotateTimeA;
    int rotateTimeD;


    float currentInvincibleTime;
    public float invincibleTimeShouldWait;

    float currentTimeDie;
    float timeDie;

    private void Awake()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        playerCollider = GetComponent<Collider2D>();
    }
    // Use this for initialization
    void Start()
    {
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        numberOfTimesSpacePressed = 0;
        leftCtrlPressedWhileHangingOnWall = false;
        ableToJumpAgain = true;
        animator = GetComponent<Animator>();

        wallToUnstickTime = wallStickTime;

        timeAttack = 0.04f;
        currentTimeAttack = timeAttack;
        attacksTransform = transform.Find("Attacks").transform;
        attack01Collider = attacksTransform.Find("Attack 01").GetComponent<PolygonCollider2D>();
        currentInvincibleTime = invincibleTimeShouldWait;

        timeDie = 3.5f;
        currentTimeDie = timeDie;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            sprite.flipX = false;
            animator.SetBool("isDead", true);

            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isDashing", false);
            animator.SetBool("isHanging", false);
            animator.SetBool("isAttack", false);
            animator.SetBool("isSliding", false);
            animator.SetBool("isHit", false);

            if (currentTimeDie > 0)
            {
                currentTimeDie -= Time.deltaTime;

            }

            else
            {
                currentTimeDie = timeDie;
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            wallSliding = false;
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            animator.SetBool("isAttack", false);
            float targetVelocityX = input.x * moveSpeed;
            float wallDir = (playerCollider.collisionInfo.left) ? -1 : 1;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerCollider.collisionInfo.grounded) ? accelerationTimeGrounded : accelerationTimeAirbone);


            if (playerCollider.collisionInfo.grounded || playerCollider.collisionInfo.above)
            {
                velocity.y = 0;
                if (playerCollider.collisionInfo.grounded)
                {
                    numberOfTimesSpacePressed = 0;
                    numberOfTimesLeftCtrlPressed = 0;
                    animator.SetBool("isGrounded", true);
                    wallSliding = false;
                    leftCtrlPressedWhileHangingOnWall = false;
                    animator.SetBool("isHanging", false);
                }
            }

            else
            {
                animator.SetBool("isGrounded", false);
            }

            velocity.y += gravity * Time.deltaTime;


            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
                && (playerCollider.collisionInfo.left || playerCollider.collisionInfo.right)
                && (!playerCollider.collisionInfo.grounded)) // only Input.GetKeyDown(KeyCode.LeftControl) will need to press down 2 times at the start of playing mode.
            {
                leftCtrlPressedWhileHangingOnWall = true;
            }

            if ((playerCollider.collisionInfo.left || playerCollider.collisionInfo.right) && !playerCollider.collisionInfo.grounded)
            {
                if (input.x == playerCollider.collisionInfo.faceDir && !leftCtrlPressedWhileHangingOnWall)
                {
                    wallSliding = true;
                    velocity.y = -wallSlidingVel;
                    numberOfTimesSpacePressed = 0;
                    numberOfTimesLeftCtrlPressed = 0;
                    animator.SetBool("isDashing", false);
                }

            }

            if (leftCtrlPressedWhileHangingOnWall)
            {
                if (wallToUnstickTime > 0)
                {
                    if (playerCollider.collisionInfo.left || playerCollider.collisionInfo.right)
                    {

                        numberOfTimesSpacePressed = 0;

                        velocity = Vector2.zero;
                        wallToUnstickTime -= Time.deltaTime;
                    }

                    else if (!playerCollider.collisionInfo.left && !playerCollider.collisionInfo.right)
                    {
                        wallToUnstickTime = wallStickTime;
                        leftCtrlPressedWhileHangingOnWall = false;
                    }

                }
                else
                {
                    wallToUnstickTime = wallStickTime;
                    leftCtrlPressedWhileHangingOnWall = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {

                if (playerCollider.collisionInfo.grounded)
                {
                    targetVelocityX = input.x * sprintSpeed;
                    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref sprintXSmoothing, accelerationTimeSprint);
                    //velocity.x = targetVelocityX;
                }
                else if (!playerCollider.collisionInfo.grounded && !wallSliding && !leftCtrlPressedWhileHangingOnWall)
                {
                    if (numberOfTimesLeftCtrlPressed == 0)
                    {
                        targetVelocityX = input.x * sprintSpeed;
                        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref sprintXSmoothing, accelerationTimeSprint);
                        numberOfTimesLeftCtrlPressed++;
                    }
                }

                if (leftCtrlPressedWhileHangingOnWall)
                {
                    leftCtrlPressedWhileHangingOnWall = false;
                }
            }

            else if (Input.GetKeyUp(KeyCode.LeftControl) && !leftCtrlPressedWhileHangingOnWall)
            {
                targetVelocityX = input.x * moveSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerCollider.collisionInfo.grounded) ? accelerationTimeGrounded : accelerationTimeAirbone);

            }

            if (numberOfTimesSpacePressed < 2)
            {
                ableToJumpAgain = true;
            }
            else
            {
                ableToJumpAgain = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && ableToJumpAgain)
            {

                velocity.y = jumpVelocity;

                if (wallSliding || leftCtrlPressedWhileHangingOnWall)
                {
                    if (input.x == wallDir)
                    {
                        velocity.x = -wallJumpOff.x * wallDir;
                        velocity.y = wallJumpOff.y;
                        numberOfTimesSpacePressed = 0;
                    }

                    else if (input.x == 0)
                    {
                        velocity.x = -wallDropOff.x * wallDir;
                        velocity.y = gravity * Time.deltaTime;
                    }

                    else
                    {
                        velocity.x = -wallLeap.x * wallDir;
                        velocity.y = wallLeap.y;
                    }
                }

                leftCtrlPressedWhileHangingOnWall = false;

                numberOfTimesSpacePressed++;
            }

            playerInput.x = Mathf.Sign(velocity.x);

            if (velocity.x != 0)
                lastDirX = Mathf.Sign(velocity.x);
            else
                lastDirX = 0;


            if (damaged)
            {
                velocity = Vector3.zero;
                numberOfTimesSpacePressed = 0;
                animator.SetBool("isHit", true);

                if (currentInvincibleTime > 0)
                {
                    currentInvincibleTime -= Time.deltaTime;
                }

                else
                {
                    damaged = false;
                    currentInvincibleTime = invincibleTimeShouldWait;
                }
            }

            else
            {
                animator.SetBool("isHit", false);
            }

            if ((Input.GetMouseButtonDown(0)) && playerCollider.collisionInfo.grounded && !attack && !damaged)
            {
                attack = true;

            }


            if (attack)
            {
                velocity.x = 0;
                if (currentTimeAttack > 0)
                {
                    currentTimeAttack -= Time.deltaTime;
                    animator.SetBool("isAttack", true);

                    attack01Collider.enabled = true;
                }

                else
                {
                    currentTimeAttack = timeAttack;
                    animator.SetBool("isAttack", false);
                    attack = false;

                    attack01Collider.enabled = false;
                }
            }

            playerCollider.Move(velocity * Time.deltaTime, false, leftCtrlPressedWhileHangingOnWall);



            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isDashing", false);
            animator.SetBool("isHanging", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isSliding", false);

            if (!playerCollider.collisionInfo.grounded && velocity.y > 0)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isDashing", false);
                animator.SetBool("isHanging", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isSliding", false);
            }

            if (!playerCollider.collisionInfo.grounded && velocity.y < 0)
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isDashing", false);
                animator.SetBool("isHanging", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isSliding", false);
            }


            if (playerCollider.collisionInfo.grounded && input.x != 0 && Mathf.Abs(velocity.x) <= 14f && !attack)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isDashing", false);
                animator.SetBool("isHanging", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isSliding", false);
            }



            if (wallSliding)
            {
                animator.SetBool("isSliding", true);
                animator.SetBool("isDashing", false);
                animator.SetBool("isHanging", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isFalling", false);
            }

            if (leftCtrlPressedWhileHangingOnWall)
            {
                animator.SetBool("isHanging", true);

                sprite.flipX = wallDir == -1;

                animator.SetBool("isCrouching", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isDashing", false);
                animator.SetBool("isSliding", false);
            }

            else
            {

                if (Input.GetKeyDown(KeyCode.A) && rotateTimeA < 1)
                {
                    Vector3 localScale = attacksTransform.localScale;
                    localScale.x = -1;
                    attacksTransform.localScale = localScale;

                    rotateTimeA++;
                    rotateTimeD = 0;
                    sprite.flipX = true;
                }
                else if (Input.GetKeyDown(KeyCode.D) && rotateTimeD < 1)
                {
                    Vector3 localScale = attacksTransform.localScale;
                    localScale.x = 1;
                    attacksTransform.localScale = localScale;

                    rotateTimeA = 0;
                    rotateTimeD++;
                    sprite.flipX = false;
                }
            }

            if (Mathf.Abs(velocity.x) > 39f)
            {
                animator.SetBool("isDashing", true);

                animator.SetBool("isHanging", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isSliding", false);
            }


        }

    }


}
