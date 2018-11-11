using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HealthBar.PlayerHealthBar
{
    public class PlayerHealth : MonoBehaviour
    {

        PlayerMovement player;

        public Slider playerHealthBar;

        [HideInInspector]
        public float currentHealth;

        [HideInInspector]
        public float playerInitialHealth;

        // Use this for initialization
        void Start()
        {
            player = GameObject.Find("Player").GetComponent<PlayerMovement>();
            playerInitialHealth = 100;

            currentHealth = playerInitialHealth;
            playerHealthBar.value = 1;
        }

        // Update is called once per frame
        void Update()
        {

            //playerHealthBar.value = returnCurrentHealth();

            if(currentHealth <= 0 && playerHealthBar.value == 0)
            {
                player.isDead = true;
            }
        }

        public void ReceiveDamage(float value)
        {
            currentHealth -= value;
            playerHealthBar.value = returnCurrentHealth();
        }

        float returnCurrentHealth()
        {
            return currentHealth / playerInitialHealth;
        }

    }
}
