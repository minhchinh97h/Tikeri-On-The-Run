using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HealthBar.EnemyHealthBar
{
    public class EnemyHealthBar : MonoBehaviour
    {
        BoxCollider2D enemyCollider;
        BoxCollider2D playerCollider;

        public LayerMask playerMask;

        public Slider enemyHealthBar;

        float currentHealth;
        float initialHealth;

        [HideInInspector]
        public bool isDead;

        private void Start()
        {
            enemyCollider = transform.GetComponent<Collider2D>().boxCollider;
            //playerCollider = GameObject.Find("Player").GetComponent<Collider2D>().boxCollider;

            initialHealth = 100;
            currentHealth = initialHealth;
            enemyHealthBar.value = 1;
        }

        private void Update()
        {
            if(currentHealth <= 0)
            {
                isDead = true;
            }
        }

        public void ReceiveDamage(float value)
        {
            currentHealth -= value;
            enemyHealthBar.value = returnCurrentHealth();
        }

        float returnCurrentHealth()
        {
            return currentHealth / initialHealth;
        }
    }
}


