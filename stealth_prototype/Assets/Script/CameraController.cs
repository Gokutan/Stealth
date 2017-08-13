using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public const float rotforce = 10f;
    public GameObject Player;
    public GameObject fake_Player;
    private Quaternion pos;
    private Vector3 temp_pos;
	void Start () {
        Player =  GameObject.FindWithTag("Player") ;
        pos = transform.rotation;
    
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.timeScale != 0)
        {
            float MouseX = Input.GetAxis("Mouse X") * rotforce;
            float MouseY = Input.GetAxis("Mouse Y") * rotforce;
            transform.Rotate(-MouseY, 0, 0);
            Player.transform.Rotate(0, MouseX, 0);
            temp_pos = new Vector3(9.68f, Player.transform.position.y, Player.transform.position.z);
            // temp_pos = transform.rotation ;
        }
        else { get_defualt(); }

    }
   
    void get_defualt( )
    {
       this.gameObject.transform.LookAt(fake_Player.transform);
       
        //Vector3 relativePos = temp_pos - transform.position;
        //transform.rotation = Quaternion.LookRotation(relativePos);
    }


}
