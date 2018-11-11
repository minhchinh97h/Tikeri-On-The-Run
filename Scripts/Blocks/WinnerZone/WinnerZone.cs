using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blocks.WinnerZone
{
    public class WinnerZone : MonoBehaviour
    {
        BoxCollider2D boxCollider;
        public LayerMask playerMask;

        // Use this for initialization
        void Start()
        {
            boxCollider = transform.GetComponent<BoxCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            IfPlayerWin();
        }

        void IfPlayerWin()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.zero, Mathf.Infinity, playerMask);

            if (hit)
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}

