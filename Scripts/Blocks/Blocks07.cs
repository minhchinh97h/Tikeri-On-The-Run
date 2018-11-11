using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks.Blocks07
{
    public class Blocks07 : MonoBehaviour
    {
        public LayerMask playerMask;

        public float timeToAppear;
        public float timeToDisappear;
        protected float currentTimeDisappear { get; set; }
        protected float currentTimeAppear { get; set; }
        protected bool wasPlayerHere { get; set; }
        

        public virtual void Start()
        {
            currentTimeAppear = timeToAppear;
            currentTimeDisappear = timeToDisappear;
        }
        

    }
}