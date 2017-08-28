using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class GuardManager : MonoBehaviour
    {

        public static event System.Action OnguardHadSpottedPlayer;


        public Transform pathHolder;
        public Vector3[] m_waypoint;
      
        private IEnumerator currentCoroutine;

      

        public LayerMask viewMask;
        public Light spotLight;
        private Transform player;
        private Color OriSpotLightColor;


        private enum AiModes
        {
            Patrol,
            Found,
            Detected
        }

        private AiModes modes;

        private float speed = 5;
     
        //[SerializeField]
        public float viewDistance;
        public float viewAngle;

    //  [SerializeField]
    private float turnSpeed = 90; //90degree per sec

       
        private int targetWaypointIndex;

        private bool make_follow;
        private bool m_start;

      
        public float waitTime = 0.3f;

        private float timer;

        void Start()
        {
            make_follow = true;

           // waitTime = 2f;
          //  player = GameObject.FindGameObjectWithTag("Player").transform;
            //viewAngle = spotLight.spotAngle;
            OriSpotLightColor = spotLight.color;

            m_waypoint = new Vector3[pathHolder.childCount];
            for (int i = 0; i < m_waypoint.Length; i++)
            {
                m_waypoint[i] = pathHolder.GetChild(i).position;

                //set the path node to be that same y as guard
                m_waypoint[i] = new Vector3(m_waypoint[i].x, transform.position.y, m_waypoint[i].z);

            }
           

          StartCoroutine(FollowPath());
   

        }

        void Update()
        {

            // CheckMode();

//            timer += Time.deltaTime;

//            if (CanseePlayer())
//            {
//                spotLight.color = Color.red;
//                //if (OnguardHadSpottedPlayer != null)
//                //{
//                //    OnguardHadSpottedPlayer();
//                //}
//                modes = AiModes.Found;  
            
//              make_follow = false;
////                StartCoroutine(returnStart());
//               // StopCoroutine(FollowPath());
//            }
//            else
//            {
//                //If want the color to slowy turn red 
//                //spotLight.color = Color.Lerp(OriSpotLightColor,Color.red,float which you want it to turen);

               
//                spotLight.color = OriSpotLightColor;
//                make_follow = true;

             
                   
//                    FollowPathCopyCat();
                
        
              
//               // StartCoroutine(FollowPath());


//            }

        }

        void CheckMode()
        {
            switch (modes)
            {
                case AiModes.Patrol:
                    {
                        StartCoroutine(FollowPath());
                    }
                    break;

                case AiModes.Detected:
                    {
                        StartCoroutine(FollowPath());
                    }
                    break;
                case AiModes.Found:
                    {
                        StartCoroutine(returnStart());
                    }
                    break;
            }
               
        }

    bool CanseePlayer()
    {
        //if (Vector3.Distance(transform.position, player.position) < viewDistance)
        //{
        //    Vector3 dirToPlayer = (player.position - transform.position).normalized;
        //    float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        //    if (angleBetweenGuardAndPlayer < viewAngle / 2f)
        //    {
        //        if (!Physics.Linecast(transform.position, player.position, viewMask))
        //        {
        //            return true;
        //        }
        //    }
        //}
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
              //  yield return new WaitForSeconds(waitTime);
                yield return null;
            }

          
        }



        IEnumerator returnStart()
        {
            Vector3 targetWaypoint = m_waypoint[0];
         
                yield return StartCoroutine(TurnToFace(targetWaypoint));
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
       
            yield return null;
            
        }
        
        void FollowPathCopyCat()
        {
            Vector3 targetWaypoint = m_waypoint[targetWaypointIndex];
            transform.LookAt(targetWaypoint);

            do
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
                if (transform.position == targetWaypoint)
                {
                    //when targetWaypointIndex == waypoints.Length so it mod into 0
                    targetWaypointIndex = (targetWaypointIndex + 1) % m_waypoint.Length;
                    targetWaypoint = m_waypoint[targetWaypointIndex];




                    StartCoroutine(TurnToFace(targetWaypoint));
                }

                break;
            } while (make_follow == true && timer < waitTime);

            //if (make_follow == true)
            //{
            //    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            //    if (transform.position == targetWaypoint)
            //    {
            //        //when targetWaypointIndex == waypoints.Length so it mod into 0
            //        targetWaypointIndex = (targetWaypointIndex + 1) % m_waypoint.Length;
            //        targetWaypoint = m_waypoint[targetWaypointIndex];

                   


            //         StartCoroutine(TurnToFace(targetWaypoint));
            //    }
            //}
        }

        IEnumerator FollowPath()
        {


        transform.position = Vector3.Lerp(transform.position, m_waypoint[0], speed * Time.deltaTime);
        //transform.position = m_waypoint[0];
        targetWaypointIndex = 1;
        Vector3 targetWaypoint = m_waypoint[targetWaypointIndex];
            transform.LookAt(targetWaypoint);

      

           while(make_follow == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
                if (transform.position == targetWaypoint)
                {
                    //when targetWaypointIndex == waypoints.Length so it mod into 0
                    targetWaypointIndex = (targetWaypointIndex + 1) % m_waypoint.Length;
                    targetWaypoint = m_waypoint[targetWaypointIndex];

                    yield return new WaitForSeconds(waitTime);
                    
                    yield return StartCoroutine(TurnToFace(targetWaypoint));
                }
                yield return null;
            }
         

        }



    //void OnDrawGizmos()
    //{
    //    Vector3 startPosition = pathHolder.GetChild(0).position;
    //    Vector3 previousPosition = startPosition;
    //    foreach (Transform waypoint in pathHolder)
    //    {
    //        Gizmos.DrawSphere(waypoint.position, .3f);
    //        Gizmos.DrawLine(previousPosition, waypoint.position);
    //        previousPosition = waypoint.position;
    //    }
    //    //   Gizmos.DrawLine(previousPosition, startPosition); // to draw close loop

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    //}

}

