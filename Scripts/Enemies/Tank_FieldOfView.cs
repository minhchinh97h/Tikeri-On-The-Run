using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy_space
{
    public class Tank_FieldOfView : MonoBehaviour {

        BoxCollider2D boxCollider;
        public VisionInfo visionInfo;

        public LayerMask playerMask;
        public LayerMask obstacleMask;

        List<Transform> visiblePlayers = new List<Transform>();

        void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        void Update()
        {
            visionInfo.Reset();

            if (visionInfo.ableToDetect)
            {
                DetectingPlayer();
            }

        }

        void DetectingPlayer()
        {
            visiblePlayers.Clear();

            UnityEngine.Collider2D[] targetsSeen = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0, playerMask);

            for (int i = 0; i < targetsSeen.Length; i++)
            {
                Transform target = targetsSeen[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visiblePlayers.Add(target);
                    visionInfo.playerDetected = true;
                    visionInfo.distanceToPlayerX = target.position.x - transform.position.x;
                    visionInfo.distanceToPlayerY = Mathf.Abs(target.position.y - transform.position.y);
                    visionInfo.playerPosition = target.position;
                    visionInfo.dirToPlayer = dirToTarget;
                }
            }
        }

        public struct VisionInfo
        {
            public bool playerDetected;
            public Vector3 dirToPlayer;
            public bool playerWasDetected;
            public float distanceToPlayerX;
            public float distanceToPlayerY;
            public Vector3 playerPosition;
            public bool ableToDetect;

            public void Reset()
            {
                playerDetected = false;
            }
        }
    }
}
