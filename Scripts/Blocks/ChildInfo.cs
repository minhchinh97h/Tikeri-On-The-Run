using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks.Blocks07;
namespace Blocks.Blocks07.Child
{

    public class ChildInfo : Blocks07
    {

        protected bool isGameObjectActivated { get; set; }

        SpriteRenderer spriteRenderer;
        BoxCollider2D boxCollider;


        private void Awake()
        {
            isGameObjectActivated = true;
        }

        public override void Start()
        {
            base.Start();
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
            boxCollider = transform.GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            

            if (!isGameObjectActivated)
            {
                if (currentTimeAppear > 0)
                {
                    currentTimeAppear -= Time.deltaTime;
                }
                else
                {
                    currentTimeAppear = timeToAppear;
                    spriteRenderer.enabled = true;
                    boxCollider.enabled = true;
                    isGameObjectActivated = true;
                }
            }

            else
            {
                if (currentTimeDisappear > 0)
                {
                    currentTimeDisappear -= Time.deltaTime;
                }
                else
                {
                    currentTimeDisappear = timeToDisappear;
                    spriteRenderer.enabled = false;
                    boxCollider.enabled = false;
                    isGameObjectActivated = false;
                }
            }

        }
    }
}
