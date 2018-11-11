using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2D : Raycasts
{
    public CollisionInfo collisionInfo;

    public LayerMask collisionMask;

    float maxSlope = 75f;
    float oldInputXSign;

    PlayerMovement playerMovement;

    public override void Start()
    {
        base.Start();

        collisionInfo.faceDir = 1;
        collisionInfo.faceDirOld = collisionInfo.faceDir;
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void Move(Vector3 velocity, bool isEnemy)
    {
        Move(velocity);
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false, bool hangOnWall = false)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();

        collisionInfo.oldVelocity = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            collisionInfo.faceDir = Mathf.Sign(velocity.x);
            collisionInfo.faceDirOld = collisionInfo.faceDir;
        }

        else
        {
            collisionInfo.faceDir = collisionInfo.faceDirOld;
        }
        HorizontalCollision(ref velocity, hangOnWall);
        

        VerticalCollision(ref velocity, standingOnPlatform);


        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            collisionInfo.grounded = true;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle <= maxSlope)
            {
                if (directionX != hit.normal.x)
                {
                    if (hit.distance - skinWidth < Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.downSlope = true;
                        collisionInfo.grounded = true;
                    }
                }
            }
        }
    }


    void HorizontalCollision(ref Vector3 velocity, bool hangOnWall)
    {
        float directionX = collisionInfo.faceDir;
        
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (hangOnWall)
        {
            rayLength = 2 * skinWidth + .5f;
        }
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += (horizontalRaySpacing * i) * Vector2.up;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            //Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if (hit.collider.tag.Equals("Moving Platform"))
                {
                    rayLength = 2 * skinWidth + .5f;

                    velocity.x = hit.transform.GetComponent<MovingPlatform>().velocity.x;

                    if (hangOnWall)
                    {
                        velocity.y = hit.transform.GetComponent<MovingPlatform>().velocity.y;
                    }
                    
                }

                else
                {
                    collisionInfo.tag = hit.collider.tag;
                    if (hit.distance == 0)
                    {
                        continue;
                    }
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);


                    if (collisionInfo.downSlope)
                    {
                        collisionInfo.downSlope = false;

                        velocity = collisionInfo.oldVelocity;
                    }

                    if (slopeAngle <= maxSlope && i == 0)
                    {
                        float distanceToSlopeStart = 0;

                        if (slopeAngle != collisionInfo.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            velocity.x -= distanceToSlopeStart * directionX;
                        }

                        ClimbingSlope(ref velocity, slopeAngle);
                        velocity.x += distanceToSlopeStart * directionX;
                    }

                    if (slopeAngle > maxSlope || !collisionInfo.upSlope)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisionInfo.upSlope)
                        {
                            velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        }
                    }


                    
                }

                collisionInfo.left = directionX == -1;
                collisionInfo.right = directionX == 1;
            }
        }
    }


    void ClimbingSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            velocity.y = climbVelocityY;

            collisionInfo.upSlope = true;
            collisionInfo.grounded = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
    }


    void VerticalCollision(ref Vector3 velocity, bool standingOnPlatform)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += (verticalRaySpacing * i + velocity.x) * Vector2.right;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);


            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
                
                if (collisionInfo.upSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisionInfo.grounded = directionY == -1;
                collisionInfo.above = directionY == 1;
            }
        }

        if (collisionInfo.upSlope)
        {
            float directionX = collisionInfo.faceDir;
            rayLength = Mathf.Abs(velocity.x) + skinWidth;

            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != collisionInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;

                    collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }

    public struct CollisionInfo
    {
        public string tag;
        public Vector3 tagVel;
        public bool grounded, above;
        public bool left, right;
        public bool upSlope, downSlope;

        public Vector3 oldVelocity;
        public float slopeAngle, slopeAngleOld;

        public float faceDir;

        public float faceDirOld;

        public void Reset()
        {
            grounded = above = false;
            left = right = false;
            upSlope = downSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
