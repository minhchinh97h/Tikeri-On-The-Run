using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy_space
{
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {

        protected Vector3 velocity;
        protected float gravity;
        protected float jumpHeight = 4f;

        protected float timeToJumpApex = .5f;

        protected float timeOfFlight;

        public LayerMask playerMask;
        public LayerMask obstacleMask;
        protected Collider2D enemyCollider;
        
        protected float timeToWaitForReActivate { get; set; }
        protected float currentTime;
        protected float timeShouldWait;
        
        protected Transform vision;
        protected CircleCollider2D visionCollider;
        protected FieldOfView fieldOfView;

        protected bool isDamaged;
        protected float currentDamagedTime;
        protected float DamagedTime;

        public virtual void Start()
        {
            enemyCollider = GetComponent<Collider2D>();

            vision = transform.Find("Vision").transform;
            visionCollider = vision.GetComponent<CircleCollider2D>();
            fieldOfView = vision.GetComponent<FieldOfView>();
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);

            currentTime = 0;

            DamagedTime = 0.02f;
            currentDamagedTime = DamagedTime;
        }
    }
}