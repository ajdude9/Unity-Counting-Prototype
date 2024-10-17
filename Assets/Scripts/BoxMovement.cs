using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{


    public Vector3 targetLocationA; 
    public Vector3 targetLocationB;   
    private Vector3 targetLocationHold; 
    public GameObject boxObject;
    public float speed;
    public bool pause = false;
    // Start is called before the first frame update
    void Start()
    {        
        int randomSwitch = Random.Range(0, 1);
        if(Random.Range(0, 2) == 1)
        {
            switchPos();
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveBox();
        if(Input.GetKeyDown(KeyCode.P))
        {
            pause = !pause;
        }
    }

    public void moveBox()
    {                
        var step = speed * Time.deltaTime;        
        if(!pause)
        {
            transform.position = Vector3.MoveTowards(boxObject.transform.position, targetLocationA, step);
        }
        
        if(boxObject.transform.position == targetLocationA)
        {        
            switchPos();
        }        
    }

    public void switchPos()
    {
        targetLocationHold = new Vector3(targetLocationA.x, targetLocationA.y, targetLocationA.z);
        targetLocationA = targetLocationB;
        targetLocationB = targetLocationHold;
    }
   
}
