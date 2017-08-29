using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIController : MonoBehaviour {

    
   // private Vector3[] m_waypoint;

    private IEnumerator currentCoroutine;
    

    [SerializeField]
    private GuardManager GM;
    [SerializeField]
    private Transform player;
    public Transform pathHolder;

    public LayerMask viewMask;
    public Light spotLight;
    private Color OriSpotLightColor;

    public Vector3 OriWayPoint;


    //[SerializeField]
    private float viewDistance = 10;
    private float viewAngle;
    private float chase_Timer;
    private float returnPatrolTimer;
    
    public bool m_CheckSee;
    public bool m_Found;
    public bool m_Detected;





    private enum AiModes
    {
        Patrol,
        Found,
        Detected
    }

    private AiModes modes;


  


    void Start()
    {
        GM = this.gameObject.GetComponent<GuardManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotLight.spotAngle;
        OriSpotLightColor = spotLight.color;
       


    }

    void Update()
    {
        ChangeSpotLight(CanseePlayer());
        
    }


    public void ChangeSpotLight(bool m_CanSee)
    {
        OriWayPoint = GM.m_waypoint[0];
        if (m_CanSee)
        {
            spotLight.color = Color.yellow;
           
            GM.make_follow = false;
           
            chase_Timer += Time.deltaTime;
            m_Found = true;

            if (chase_Timer >= 1f && chase_Timer <= 2.8f)
            {
                spotLight.color = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(Time.time, 0.3f));
            }

            else if (chase_Timer >= 3f)
            {
                m_Detected = true;
                spotLight.color = Color.red;
                m_Found = false;
                if (Vector3.Distance(transform.position, player.position) < viewDistance)
                {
                    GM.GoChese(player);
                }
            }
           


        }
        else
        {
            spotLight.color = OriSpotLightColor;
            GM.make_follow = true;
            chase_Timer = 0f;
            if (m_Detected )
            { GM.StopChase(); GM.enabled = false; print("Detect & Lost"); }
            if (m_Found) { GM.enabled = false; print("found & Lost"); }

            if (GM.enabled == false)
            {
                if (m_Detected)
                {
                    print("Need guard from detect");
                    StartCoroutine(Restart());
                }
               else if(m_Found) { print("Need guard from found"); StartCoroutine(Continue()); }
              
                
               
            }

        }

       

       
       


    }

    void TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, 90 * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            // yield return new WaitForSeconds(0.05f);
            break;
        }
    }

    IEnumerator Continue()
    {
        while (m_Found == true)
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 targetturn = new Vector3(OriWayPoint.x, OriWayPoint.y, OriWayPoint.z);

            TurnToFace(targetturn);
            transform.position = Vector3.Lerp(transform.position, OriWayPoint, 5.0f * Time.deltaTime);

            if (transform.position == OriWayPoint)
            {
                GM.enabled = true;
                GM.FollowingPath();
                yield return m_Found = false;
            }

           

        }
    }

  
   

    IEnumerator Restart()
    {
        while (m_Detected == true)
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 targetturn = new Vector3(OriWayPoint.x, OriWayPoint.y, OriWayPoint.z);
            transform.LookAt(OriWayPoint);
            //   StartCoroutine(TurnToFace(targetturn));
            transform.position = Vector3.Lerp(transform.position, OriWayPoint, 5.0f * Time.deltaTime);
            if (transform.position == OriWayPoint)
            {
                GM.enabled = true;
                GM.FollowingPath();
                yield return m_Detected = false;
            }

        }
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



}





