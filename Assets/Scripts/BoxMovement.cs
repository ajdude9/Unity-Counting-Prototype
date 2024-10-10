using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{


    private Vector3 leftLocation = new Vector3(0, 0, -16.5f);
    private Vector3 rightLocation = new Vector3(0, 0, -16.5f);
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
        int speed = 5;
        var step = speed * Time.deltaTime;
        if(direction)
        {
            transform.position = Vector3.MoveTowards(boxObject.transform.position, leftLocation, step);
        }
        else
        {
            transform.position = Vector3.MoveTowards(boxObject.transform.position, rightLocation, step);
        }

        if(boxObject.transform.position.z <= -16.5)
        {
            direction = false;
        }
        if(boxObject.transform.position.z >= 16.5)
        {
            direction = true;
        }
    }
   
}
