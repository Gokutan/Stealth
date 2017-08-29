using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

    public class GuardManager : MonoBehaviour
    {

        public static event System.Action OnguardHadSpottedPlayer;


        NavMeshAgent agent;

        public Transform pathHolder;
        public Vector3[] m_waypoint;

        public float speed = 5;

        private float turnSpeed = 90; //90degree per sec


        private int targetWaypointIndex;

        public bool make_follow;


        private float waitTime = 0.3f;
    private bool m_start = true;


    private AIController AIC;

        void Start()
        {
            make_follow = true;
            AIC = this.gameObject.GetComponent<AIController>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        agent.enabled = false;
            SetUpPath();

          StartCoroutine(FollowPath());
        }

    void Update ()
    {
      
    }

    public void GoChese(Transform target)
    {
        agent.enabled = true;
        agent.SetDestination(target.transform.position);
    }

    public void StopChase()
    {
       // agent.Stop();
        agent.enabled = false;
    }

    void SetUpPath()
    {
        m_waypoint = new Vector3[pathHolder.childCount];
        for (int i = 0; i < m_waypoint.Length; i++)
        {
            m_waypoint[i] = pathHolder.GetChild(i).position;

            //set the path node to be that same y as guard
            m_waypoint[i] = new Vector3(m_waypoint[i].x, transform.position.y, m_waypoint[i].z);

        }
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

        if (m_start)
        {
          
            m_start = false;
        }
        transform.position = Vector3.Lerp(transform.position, m_waypoint[0], speed * Time.deltaTime);
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



  

}

