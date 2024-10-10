using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{


    private Vector3 targetLocation = new Vector3(0, 0, -16.5f);    
    public GameObject boxObject;
    public bool direction = true;
    // Start is called before the first frame update
    void Start()
    {        
     
    }

    // Update is called once per frame
    void Update()
    {
        moveBox();
    }

    public void moveBox()
    {
        float speed = 2.5f;
        var step = speed * Time.deltaTime;        
        transform.position = Vector3.MoveTowards(boxObject.transform.position, targetLocation, step);
        
        if(boxObject.transform.position == targetLocation)
        {
            Debug.Log("Hit wall.");
            targetLocation.z = -targetLocation.z;
        }        
    }
   
}
