using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Raycasts
{

    public LayerMask passengerMask;
    Vector3[] globalWaypoints;

    //public Vector3 move;
    public float speed;
    public bool cyclic;

    public float waitTime; //time to wait after the platform reaches the waypoint then continue moving
    float nextMoveTime;

    [Range(0, 2)]
    public float easeAmount; //the value of the power a to ease the movement of the platform smoothly.

    public Vector3[] localWaypoints;
    List<PassengerMovementInfo> passengerInfos; //to store info of each passenger
    Dictionary<Transform, Collider2D> passengerDictionary = new Dictionary<Transform, Collider2D>(); // to store each passenger's transform and collider 2D

    int fromWaypointIndex;

    float percentBetweenWaypoints;

    [HideInInspector]
    public Vector3 velocity;

    PlayerMovement playerMovement;

    public override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }


        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        velocity = CalculatePlatformMovement();
        if (!playerMovement.leftCtrlPressedWhileHangingOnWall)
        {
            CalculatePassengerMovement(velocity);
        }

        MovePassenger(true); //move the passenger first when its the upward moving platform with a passenger on top.

        transform.Translate(velocity);

        MovePassenger(false); //move the passenger later when its the downward moving platform with a passenger on top.
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }


    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime) // Time.time is the real time that the application is running.
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distance = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]); //distance between 2 waypoints

        percentBetweenWaypoints += speed * Time.deltaTime / distance;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints); //to make sure the value does not go over 1 so that easing will behave wrong.

        float easePercentBetweenWaypoints = Ease(percentBetweenWaypoints); //the eased value

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easePercentBetweenWaypoints); //to find the new position of the platform according to
                                                                                                                                          //the position of platfrom from fromWaypointIndex to
                                                                                                                                          //to toWaypointIndex and percentBetweenWaypoints

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }

            nextMoveTime = Time.time + waitTime; //calculate the final wait time for the next move when the platform reaches a waypoint.
        }

        return newPos - transform.position;
    }


    void MovePassenger(bool moveBeforePlaform)
    {
        foreach (PassengerMovementInfo passengerInfo in passengerInfos)
        {
            if (!passengerDictionary.ContainsKey(passengerInfo.passengerTransform))
            {
                passengerDictionary.Add(passengerInfo.passengerTransform, passengerInfo.passengerTransform.GetComponent<Collider2D>());
            }

            if (passengerInfo.moveBeforePlatform == moveBeforePlaform)
            {
                passengerDictionary[passengerInfo.passengerTransform].Move(passengerInfo.passengerVelocity, passengerInfo.standingOnPlatform, false);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity) //passengers refer to any objects inherit from the class Collider
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>(); //to ensure that for each passenger that will be moved once per loop.
        passengerInfos = new List<PassengerMovementInfo>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertically moving upward
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += (verticalRaySpacing * i) * Vector2.right;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit)
                {
                    ////To help platform go through player without unnecessary movements afterward when going downward
                    //if (hit.transform.tag.Equals("Player"))
                    //{
                    //    if (directionY == -1 || hit.distance == 0)
                    //        continue;
                    //}

                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY; // to eliminate little gap between passenger and platform.
                        float pushX = (directionY == 1) ? velocity.x : 0; // or pushX = velocity.x if we want the velocity.x affects on both up and down.

                        passengerInfos.Add(new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        //// Horizontally moving
        //if (velocity.x != 0)
        //{
        //    float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        //    for (int i = 0; i < horizontalRayCount; i++)
        //    {
        //        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        //        rayOrigin += (horizontalRaySpacing * i) * Vector2.up;

        //        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

        //        if (hit)
        //        {
        //            if (!movedPassengers.Contains(hit.transform))
        //            {
        //                movedPassengers.Add(hit.transform);

        //                float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
        //                float pushY = -skinWidth; //because the PlayerMovement script will get called first, then the MovingPlatform script will get called later and the desicion
        //                                          // to be able to jump will be decided in the next frame (because the first frame collisionInfo.grounded = null => cant jump)

        //                passengerInfos.Add(new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), false, true));
        //            }
        //        }
        //    }
        //}

        //when the passenger is on top of a horizontally moving platform or a downwardly moving platform
        if (directionY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft;
                rayOrigin += (verticalRaySpacing * i) * Vector2.right;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerInfos.Add(new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }

        
        
    }

    struct PassengerMovementInfo
    {
        public Transform passengerTransform;
        public Vector3 passengerVelocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovementInfo(Transform passengerTransform, Vector3 passengerVelocity, bool standingOnPlatform, bool moveBeforePlatform)
        {
            this.passengerTransform = passengerTransform;
            this.passengerVelocity = passengerVelocity;
            this.standingOnPlatform = standingOnPlatform;
            this.moveBeforePlatform = moveBeforePlatform;
        }
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Vector3 globalWaypoint = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                float size = .5f;
                Gizmos.DrawLine(globalWaypoint - Vector3.right * size, globalWaypoint + Vector3.right * size);
                Gizmos.DrawLine(globalWaypoint - Vector3.up * size, globalWaypoint + Vector3.up * size);
            }
        }
    }

}
