using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthBar.PlayerHealthBar;

namespace Blocks.WaterZone
{
    public class WaterZone : MonoBehaviour
    {

        BoxCollider2D boxCollider;
        public LayerMask playerMask;
        PlayerMovement playerMovement;
        PlayerHealth playerHealth;

        float currentToxicTime;
        float toxicTime;

        bool isTocinated;
        // Use this for initialization
        void Start()
        {
            boxCollider = transform.GetComponent<BoxCollider2D>();
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

            toxicTime = 1f;
            currentToxicTime = toxicTime;

        }

        // Update is called once per frame
        void Update()
        {
            if (!isTocinated)
            {
                IfPlayerStepOn();
            }

            else
            {
                if(currentToxicTime > 0)
                {
                    currentToxicTime -= Time.deltaTime;
                }
                else
                {
                    currentToxicTime = toxicTime;
                    isTocinated = false;
                }
            }
        }

        void IfPlayerStepOn()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

            if (hit)
            {
                isTocinated = true;
                playerHealth.ReceiveDamage(5);
            }
        }
    }
}
