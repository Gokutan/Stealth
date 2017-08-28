using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    [SerializeField]
    private GuardManager GM;

    private Transform player;

   

    void Start () {
        GM = this.gameObject.GetComponent<GuardManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
      
    }
	
	// Update is called once per frame
	void Update () {
        if (CanseePlayer())
        {
            print("fount");
        }
        else
        {
          
        }



    }

    bool CanseePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < GM.viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleBetweenGuardAndPlayer < GM.viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, GM.viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }



    void OnDrawGizmos()
    {
        Vector3 startPosition = GM.pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in GM.pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        //   Gizmos.DrawLine(previousPosition, startPosition); // to draw close loop

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * GM.viewDistance);
    }


}
