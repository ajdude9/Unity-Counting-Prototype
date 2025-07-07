using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherController : MonoBehaviour
{


    private Vector3 targetLocationA;//The most forward the pusher should go
    private Vector3 targetLocationB;//The furthest back the pusher should go
    private Vector3 targetLocationHold;//A temporary value for allowing the target locations to be switched
    private GameObject boxObject;//The machine pusher itself
    private float speed = 3;//The speed at which the machine pusher moves
    private bool pause = false;//Whether or not the machine pusher is stopped
    // Start is called before the first frame update
    void Start()
    {
        boxObject = GameObject.Find("MachinePusherParent");//Find the machine pusher's parent, as its what actually controls the pusher
        targetLocationA = new Vector3(66.4f, 16.47f, 41.1f);
        targetLocationB = new Vector3(66.4f, 16.47f, 47.1f);
    }

    // Update is called once per frame
    void Update()
    {
        movePusher();//Move the pusher
        if(Input.GetKeyDown(KeyCode.P))//If the player presses P
        {
            pause = !pause;//Invert the pause value - from false to true or vice versa
        }
    }

    private void movePusher()//Move the pusher back and forth
    {        
        var step = speed * Time.deltaTime;//Step is speed constrained by time in seconds   
        if(!pause)//f the pusher isn't paused
        {
            transform.position = Vector3.MoveTowards(boxObject.transform.position, targetLocationA, step);//Move toward the target location
        }
        
        if(boxObject.transform.position == targetLocationA)//If the pusher is at the target location, swap it with the second target location
        {
            
            targetLocationHold = new Vector3(targetLocationA.x, targetLocationA.y, targetLocationA.z);
            targetLocationA = targetLocationB;
            targetLocationB = targetLocationHold;

        }        
    }

    
   
}
