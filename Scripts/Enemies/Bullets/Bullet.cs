using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_space;
namespace Enemy_bullet_space
{
    public class Bullet : MonoBehaviour
    {
        protected Vector3 velocity;
        protected float gravity;

        protected CircleCollider2D circleCollider;
        public LayerMask obstacleMask;
        public LayerMask playerMask;

        float currentTimeAlive;
        float timeAlive;

        // Use this for initialization
        public virtual void Start()
        {
            gravity = PlayerMovement.gravity;
            circleCollider = GetComponent<CircleCollider2D>();
            timeAlive = 3f;
            currentTimeAlive = timeAlive; 
        }

        protected void DestroyThisBullet()
        {
            UnityEngine.Collider2D targetsSeen = Physics2D.OverlapCircle(transform.position, circleCollider.radius, obstacleMask);

            if (targetsSeen)
            {
                Destroy(gameObject);
            }

            else
            {
                if(currentTimeAlive > 0)
                {
                    currentTimeAlive -= Time.deltaTime;
                }

                else
                {
                    currentTimeAlive = timeAlive;
                    Destroy(gameObject);
                }
            }
        }
    }
}
