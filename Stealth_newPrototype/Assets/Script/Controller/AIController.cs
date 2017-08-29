using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public Transform pathHolder;
   // private Vector3[] m_waypoint;

    private IEnumerator currentCoroutine;



    public LayerMask viewMask;
    public Light spotLight;
    private Color OriSpotLightColor;

    public Vector3 OriWapPoint;


    //[SerializeField]
    private float viewDistance = 10;
    private float viewAngle;




    public bool make_follow;
    private bool m_Detected;

    [SerializeField]
    private GuardManager GM;
    [SerializeField]
    private Transform player;



    public bool m_CheckSee;

    private enum AiModes
    {
        Patrol,
        Found,
        Detected
    }

    private AiModes modes;


    private float timer;


    void Start()
    {
        make_follow = true;
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
        if (m_CanSee)
        {
            spotLight.color = Color.red;
            GM.make_follow = false;
            OriWapPoint = GM.m_waypoint[0];
            m_Detected = true;
            GM.enabled = false;
        }
        else
        {
            spotLight.color = OriSpotLightColor;
            GM.make_follow = true;
            if (GM.enabled == false)
            {
                StartCoroutine(Restart());
            }

        }

        
    }

   

    IEnumerator Restart()
    {
      while (m_Detected == true)
        {
            transform.position = Vector3.Lerp(transform.position, OriWapPoint, GM.speed * Time.deltaTime);
         //   yield return new WaitForSeconds(1f);
            GM.enabled = true;
            GM.FollowingPath();
            yield return new WaitForSeconds(2f);
            yield return m_Detected = false;
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





