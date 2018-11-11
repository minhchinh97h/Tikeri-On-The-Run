using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthBar.PlayerHealthBar;

namespace Blocks.Stakes
{
    public class Stakes : MonoBehaviour
    {
        public LayerMask playerMask;
        PlayerMovement playerMovement;
        PlayerHealth playerHealth;

        BoxCollider2D boxCollider;
        GameObject playerGO;
        // Use this for initialization
        void Start()
        {
            boxCollider = transform.GetComponent<BoxCollider2D>();
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
            playerGO = GameObject.Find("Player").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (!playerMovement.damaged)
            {
                IfHitPlayer();
            }
        }


        void IfHitPlayer()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

            if (hit)
            {
                playerHealth.ReceiveDamage(100);
                playerMovement.isDead = true;
            }
        }
    }
}