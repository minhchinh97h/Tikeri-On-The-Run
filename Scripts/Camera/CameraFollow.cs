using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Vector2 focusAreaSize;
    public Vector2 centreSize;
    Collider2D playerCollider;
    Transform playerTransform;
    PlayerMovement playerMovement;
    FocusArea focusArea;

    public float lookAheadDstX;
    //public float lookAheadDstY;

    public float transitionTimeX;
    //public float transitionTimeY;

    float targetLookAheadX;
    float targetLookAheadY;
    float currentLookAheadX;
    float currentLookAheadY;
    float smoothVelX;
    float smoothVelY;
    float lookDirX;
    float lookDirY;
    //It seems like in Unity, scripts will be executed one by one from top to bottom for every game objects which are enabled 
    //so the Main Camera object is called first then to be able to access other components in this script, we need to decalre
    //them in the Awake() function, which is called before the Start() function. Otherwise, it will return null.
    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        playerCollider = playerTransform.GetComponent<Collider2D>();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        focusArea = new FocusArea(playerCollider.boxCollider.bounds, focusAreaSize);
    }

    //LateUpdate() is usually used for cameras because it will be invoked after all the player inputs are executed. 
    private void LateUpdate()
    {
        focusArea.UpdateFocusArea(playerCollider.boxCollider.bounds, playerTransform, centreSize);
        currentLookAheadX = 0;
        if (focusArea.velocityFromLastFrame != Vector2.zero)
        {
            lookDirX = Mathf.Sign(focusArea.velocityFromLastFrame.x);
            lookDirY = Mathf.Sign(focusArea.velocityFromLastFrame.y);

            if(Mathf.Sign(playerMovement.playerInput.x) == lookDirX && playerMovement.playerInput.x != 0)
            {
                targetLookAheadX = lookDirX * lookAheadDstX;
            }

            else
            {
                targetLookAheadX = lookDirX * lookAheadDstX / 5f;
            }
            
        }
        if (focusArea.betweenCentreX)
        {
            currentLookAheadX = 0;
        }
        else
        {
            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothVelX, transitionTimeX);
        }
        
        focusArea.centre += Vector2.right * currentLookAheadX;

        focusArea.left += currentLookAheadX;
        focusArea.right += currentLookAheadX;
        transform.position = (Vector3) focusArea.centre + Vector3.forward*-10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);

        //Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }


    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocityFromLastFrame;
        public float left, right, bottom, top;
        bool shouldCentreUpdateX, shouldCentreUpdateY;
        public bool betweenCentreX, betweenCentreY;

        public FocusArea(Bounds targetBounds, Vector2 focusSize)
        {
            shouldCentreUpdateX = false;
            shouldCentreUpdateY = false;
            betweenCentreX = false;
            betweenCentreY = false;
            left = targetBounds.center.x - focusSize.x / 2;
            right = targetBounds.center.x + focusSize.x / 2;
            bottom = targetBounds.center.y - focusSize.y / 2;
            top = targetBounds.center.y + focusSize.y / 2;
            
            velocityFromLastFrame = Vector2.zero;
            centre = new Vector2((left + right) / 2, (bottom + top) / 2);
        }

        public void UpdateFocusArea(Bounds targetBounds, Transform playerTransform, Vector2 centreSize)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
                shouldCentreUpdateX = true;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
                shouldCentreUpdateX = true;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
                shouldCentreUpdateY = true;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
                shouldCentreUpdateY = true;
            }
            top += shiftY;
            bottom += shiftY;

            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocityFromLastFrame = new Vector2(shiftX, shiftY);


            if (shouldCentreUpdateX)
            {
                if (playerTransform.position.x < centre.x - centreSize.x / 2 || playerTransform.position.x > centre.x + centreSize.x / 2)
                {
                    shouldCentreUpdateX = true;
                    betweenCentreX = false;
                    float currentLookAheadX = 0;
                    float dstX = playerTransform.position.x - centre.x;
                    float smoothVelX = 0;
                    float transitionTimeX = 0;

                    if (Input.GetKeyDown(KeyCode.LeftControl))
                    {
                        transitionTimeX = 0.17f;
                    }
                    else
                    {
                        transitionTimeX = 0.17f;
                    }
                    
                    currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, dstX, ref smoothVelX, transitionTimeX);
                    
                    centre.x += currentLookAheadX;
                    

                    left += currentLookAheadX;
                    right += currentLookAheadX;
                    
                }
                else
                {
                    betweenCentreX = true;
                    shouldCentreUpdateX = false;
                }


            }

            if (shouldCentreUpdateY)
            {
                if (playerTransform.position.y < centre.y - centreSize.y / 2 || playerTransform.position.y > centre.y + centreSize.y / 2)
                {
                    shouldCentreUpdateY = true;
                    betweenCentreY = false;

                    float currentLookAheadY = 0;
                    float dstY = playerTransform.position.y - centre.y;
                    float smoothVelY = 0;
                    float transitionTimeY = 0.19f;

                    currentLookAheadY = Mathf.SmoothDamp(currentLookAheadY, dstY, ref smoothVelY, transitionTimeY);

                    centre.y += currentLookAheadY;
                    bottom += currentLookAheadY;
                    top += currentLookAheadY;
                }
                else
                {
                    betweenCentreY = true;
                    shouldCentreUpdateY = false;
                }
            }
        }

    }
}
