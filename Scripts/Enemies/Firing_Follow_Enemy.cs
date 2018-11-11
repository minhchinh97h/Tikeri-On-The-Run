using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy_space;

public class Firing_Follow_Enemy : Enemy
{

    GameObject bullet;
    GameObject launchingPoint;
    GameObject initialPoint;
    Vector3 bulletInitialVel;
    Vector3 bulletVel;

    bool fireBullet;
    bool bulletToLaunchPoint;
    bool reload;
    bool startProgress;
    bool inProgress;

    [HideInInspector]
    public bool firing;
    [HideInInspector]
    public bool endProgress;

    float fireSpeed = 3f;

    float currentTimeForBulletWait;
    float totalTimeForBulletWait;

    float currentTimeForBulletDetect;
    float totalTimeForBulletDetect;

    List<Vector3> playerPositions = new List<Vector3>(3);


    // Use this for initialization
    public override void Start()
    {
        base.Start();

        timeToWaitForReActivate = 3f;
        launchingPoint = transform.Find("Launching Point").gameObject;
        bullet = transform.Find("Bullet").gameObject;
        initialPoint = transform.Find("Initial Point").gameObject;
        bulletInitialVel = ToFindBulletInitialVel();
        fireBullet = false;
        bulletToLaunchPoint = false;

        totalTimeForBulletWait = 10f;
        totalTimeForBulletDetect = 3f;

        bullet.transform.position = initialPoint.transform.position;
        reload = false;
        startProgress = false;
        endProgress = false;
        inProgress = false;
        firing = false;

        timeShouldWait = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (firing) //for type 2
        {
            if(currentTime < timeShouldWait)
            {
                currentTime += Time.deltaTime;
            }

            else
            {
                currentTime = 0;
                endProgress = true;
            }
        }


        if (endProgress)
        {
            firing = false;
            inProgress = false;
            startProgress = false;

            Destroy(transform.Find("Bullet(Clone)").gameObject);
        }

        if (!startProgress)
        {
            endProgress = false;
            fieldOfView.visionInfo.ableToDetect = true;

            if (fieldOfView.visionInfo.playerDetected)
            {
                GameObject bulletClone;
                startProgress = true;
                inProgress = true;
                //reload
                bulletClone = Instantiate(bullet, transform) as GameObject;
                bulletClone.SetActive(true);
                bulletClone.transform.position = initialPoint.transform.position;
                bulletToLaunchPoint = true;
            }
        }

        if (inProgress)
        {
            if (bulletToLaunchPoint)
            {
                if (transform.Find("Bullet(Clone)").transform.position.y < launchingPoint.transform.position.y)
                {
                    transform.Find("Bullet(Clone)").transform.Translate(bulletInitialVel * Time.deltaTime);
                    fieldOfView.visionInfo.ableToDetect = false;
                }

                else
                {
                    if (currentTimeForBulletDetect < totalTimeForBulletDetect)
                    {
                        currentTimeForBulletDetect += Time.deltaTime;

                        fieldOfView.visionInfo.ableToDetect = true;
                        
                        if (fieldOfView.visionInfo.playerDetected)
                        {
                            //fireBullet = true; //for type 1

                            fieldOfView.visionInfo.playerWasDetected = true; //for type 2

                        }
                    }

                    else
                    {
                        currentTimeForBulletDetect = 0;

                        //if (!fireBullet) //for type 1
                        //{
                        //    bulletToLaunchPoint = false;
                        //}

                        //type 2
                        if(fieldOfView.visionInfo.playerDetected || fieldOfView.visionInfo.playerWasDetected)
                        {
                            firing = true;
                        }

                        else
                        {
                            bulletToLaunchPoint = false;
                        }

                    }
                }
            }
            
            else
            {
                if (transform.Find("Bullet(Clone)").transform.position.y > initialPoint.transform.position.y)
                {
                    transform.Find("Bullet(Clone)").transform.Translate(-bulletInitialVel * Time.deltaTime);
                }

                else
                {
                    endProgress = true;
                }
            }

            //if (fireBullet) //type 1
            //{
            //    if (currentTimeForBulletWait < totalTimeForBulletWait)
            //    {
            //        currentTimeForBulletWait += Time.deltaTime;

            //        //fire code goes here

            //        bulletVel = BulletVelocity(GameObject.Find("Player").transform.position, transform.Find("Bullet(Clone)").transform.position);

            //        transform.Find("Bullet(Clone)").transform.Translate(bulletVel * Time.deltaTime);
            //    }
            //    else
            //    {
            //        currentTimeForBulletWait = 0;

            //        fieldOfView.visionInfo.ableToDetect = false;
            //        bulletToLaunchPoint = false;

            //        fireBullet = false;
            //    }
            //}
        }

        velocity.y += gravity * Time.deltaTime;

        enemyCollider.Move(velocity * Time.deltaTime, true);

        if (enemyCollider.collisionInfo.grounded || enemyCollider.collisionInfo.above)
        {
            velocity.y = 0;
            velocity.x = 0;
        }
    }

    Vector3 ToFindBulletInitialVel()
    {
        float distanceY = Mathf.Abs(launchingPoint.transform.position.y - bullet.transform.position.y);
        float time = 1f;

        return new Vector3(0, distanceY / time, 0);
    }

    Vector3 BulletVelocity(Vector3 playerPosition, Vector3 bulletPosition)
    {
        Vector3 dirFromBulletToPlayer = (playerPosition - bulletPosition).normalized;

        return dirFromBulletToPlayer * fireSpeed;
    }
}
