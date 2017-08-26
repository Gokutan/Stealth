using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class GuardManager : MonoBehaviour
    {

        public static event System.Action OnguardHadSpottedPlayer;

        public Transform pathHolder;
        public Vector3[] m_waypoint;
        public float turnSpeed = 90; //90degree per sec
        IEnumerator currentCoroutine;

        public float speed = 5;
        public float waitTiem = .3f;

        public Light spotLight;
        public float viewDistance;
        float viewAngle;

        public LayerMask viewMask;

        Transform player;
        Color OriSpotLightColor;

        ThirdPersonUserControl tp;
        ThirdPersonCharacter tpc;

        void Start()
        {

            player = GameObject.FindGameObjectWithTag("Player").transform;
            tp = player.GetComponent<ThirdPersonUserControl>();
            tpc = player.GetComponent<ThirdPersonCharacter>();
            viewAngle = spotLight.spotAngle;
            OriSpotLightColor = spotLight.color;

            m_waypoint = new Vector3[pathHolder.childCount];
            for (int i = 0; i < m_waypoint.Length; i++)
            {
                m_waypoint[i] = pathHolder.GetChild(i).position;

                //set the path node to be that same y as guard
                m_waypoint[i] = new Vector3(m_waypoint[i].x, transform.position.y, m_waypoint[i].z);

            }
            StartCoroutine(FollowPath(m_waypoint));
            // StartCoroutine(Patrolling(m_waypoint));


        }

        void Update()
        {
            //if (currentCoroutine != null)
            //{
            //    StopCoroutine(currentCoroutine);
            //}else
            //{
            //    currentCoroutine = Patrolling(m_waypoint);
            //    StartCoroutine(currentCoroutine);
            //}

            if (CanseePlayer())
            {
                spotLight.color = Color.red;
                if (OnguardHadSpottedPlayer != null)
                {
                    OnguardHadSpottedPlayer();
                }


                Time.timeScale = 0;
              

            }
            else
            {
                //If want the color to slowy turn red 
                //spotLight.color = Color.Lerp(OriSpotLightColor,Color.red,float which you want it to turen);
                spotLight.color = OriSpotLightColor;
                Time.timeScale = 1;
            }

        }



        void OnDrawGizmos()
        {
            Vector3 startPosition = pathHolder.GetChild(0).position;
            Vector3 previousPosition = startPosition;
            foreach (Transform waypoint in pathHolder)
            {
                Gizmos.DrawSphere(waypoint.position, .3f);
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }
            //   Gizmos.DrawLine(previousPosition, startPosition); // to draw close loop

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        }

        bool CanseePlayer()
        {
            if (Vector3.Distance(transform.position, player.position) < viewDistance)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

                if (angleBetweenGuardAndPlayer < viewAngle / 2f)
                {
                    if (!Physics.Linecast(transform.position, player.position, viewMask))
                    {
                        return true;
                    }
                }
            }
            return false;

        }


        IEnumerator TurnToFace(Vector3 lookTarget)
        {
            Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }

        IEnumerator FollowPath(Vector3[] waypoints)
        {
            transform.position = waypoints[0];
            int targetWaypointIndex = 1;
            Vector3 targetWaypoint = waypoints[targetWaypointIndex];
            transform.LookAt(targetWaypoint);
            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
                //    transform.rotation = Quaternion.LookRotation(transform.rotation,targetWaypoint.rotation)
                if (transform.position == targetWaypoint)
                {
                    //when targetWaypointIndex == waypoints.Length so it mod into 0
                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                    targetWaypoint = waypoints[targetWaypointIndex];
                    yield return new WaitForSeconds(waitTiem);
                    yield return StartCoroutine(TurnToFace(targetWaypoint));
                }
                yield return null;
            }

        }

        //IEnumerator Patrolling(Vector3[] destination )
        //{

        //    foreach(Vector3 dest in destination)
        //    {

        //        yield return StartCoroutine(moving(dest, 8));
        //    }

        //}

        //IEnumerator moving(Vector3 destination , float speed)
        //{
        //    while (transform.position != destination)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        //        yield return null;
        //    }
        //}
    }
}
