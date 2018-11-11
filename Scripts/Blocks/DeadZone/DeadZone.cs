using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthBar.PlayerHealthBar;

namespace Blocks.DeadZone
{
    public class DeadZone : MonoBehaviour
    {
        BoxCollider2D boxCollider;
        public LayerMask playerMask;
        PlayerMovement playerMovement;
        PlayerHealth playerHealth;

        // Use this for initialization
        void Start()
        {
            boxCollider = transform.GetComponent<BoxCollider2D>();
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        }

        // Update is called once per frame
        void Update()
        {
            IfPlayerLose();
        }

        void IfPlayerLose()
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

