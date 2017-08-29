using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class GuardManager : MonoBehaviour
    {

        public static event System.Action OnguardHadSpottedPlayer;


        public Transform pathHolder;
        public Vector3[] m_waypoint;

    //    private IEnumerator currentCoroutine;



    //    public LayerMask viewMask;
    //    public Light spotLight;
    //    private Color OriSpotLightColor;
    //    private Transform player;



        public float speed = 5;

    //    //[SerializeField]
    //    private float viewDistance = 10;
    //    private float viewAngle;

    ////  [SerializeField]
        private float turnSpeed = 90; //90degree per sec


        private int targetWaypointIndex;

        public bool make_follow;
    //    private bool m_start;


        private float waitTime = 0.3f;

    //    private float timer;

    private AIController AIC;

        void Start()
        {
            make_follow = true;
        AIC = this.gameObject.GetComponent<AIController>();
         //   player = GameObject.FindGameObjectWithTag("Player").transform;
          //  viewAngle = spotLight.spotAngle;
         //   OriSpotLightColor = spotLight.color;

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



      

    }

   


    //bool CanseePlayer()
    //{
    //    if (Vector3.Distance(transform.position, player.position) < viewDistance)
    //    {
    //        Vector3 dirToPlayer = (player.position - transform.position).normalized;
    //        float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

    //        if (angleBetweenGuardAndPlayer < viewAngle / 2f)
    //        {
    //            if (!Physics.Linecast(transform.position, player.position, viewMask))
    //            {
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}


 

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
        
    public void FollowingPath()
    {
        StartCoroutine(FollowPath());
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

