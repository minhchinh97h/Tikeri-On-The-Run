using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthBar.PlayerHealthBar;

namespace Enemy_space
{
    public class LaserRight : MonoBehaviour
    {

        public LayerMask playerMask;
        public Vector2 fallBackVelocity;

        PlayerHealth playerHealth;
        PlayerMovement playerMovement;

        BoxCollider2D boxCollider;

        float currentInvincibleTime;
        public float invincibleTimeShouldWait;

        // Use this for initialization
        void Start()
        {
            playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            boxCollider = transform.GetComponent<BoxCollider2D>();

            currentInvincibleTime = invincibleTimeShouldWait;
        }

        // Update is called once per frame
        void Update()
        {
            if (!playerMovement.damaged)
            {
                IfHitPlayer();
            }

            else
            {
                if (currentInvincibleTime > 0)
                {
                    currentInvincibleTime -= Time.deltaTime;
                }

                else
                {
                    playerMovement.damaged = false;
                    currentInvincibleTime = invincibleTimeShouldWait;
                }
            }
        }


        void IfHitPlayer()
        {
            RaycastHit2D hit = Physics2D.BoxCast(boxCollider.transform.position, boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

            if (hit)
            {
                playerHealth.ReceiveDamage(50);
                playerMovement.damaged = true;
                Vector2 velocity = new Vector2(fallBackVelocity.x, fallBackVelocity.y);

                playerMovement.playerCollider.Move(velocity);
            }
        }
    }
}
