using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{


    private Vector3 targetLocationA;//The leftmost location the box should travel to
    private Vector3 targetLocationB;//The rightmost location the box should travel to
    private Vector3 targetLocationHold;//Temporary variable to allow swapping locations
    private GameObject boxObject;//The box
    private float speed = 2.5f;//How fast the box moves
    [SerializeField] private bool pause = false;//Whether the box is stopped or not
    // Start is called before the first frame update
    void Start()
    {        
        targetLocationA = new Vector3(0, 0, -16.5f);
        targetLocationB = new Vector3(0, 0, 16.5f);
        boxObject = GameObject.Find("Box");
        int randomSwitch = Random.Range(0, 1);//Randomly decide to move left or right to start with
        if(Random.Range(0, 2) == 1)
        {
            switchPos();//Switch the left and right positions
        }
    }

  

    // Update is called once per frame
    void Update()
    {
        moveBox();//Move the box
        if(Input.GetKeyDown(KeyCode.P))//If P is pressed, pause the box's movement
        {
            pause = !pause;
        }        
    }

    private void moveBox()//Move the box between two locations
    {                
        var step = speed * Time.deltaTime;//A step is defined as the speed of the box, regulated by deltaTime
        if(!pause)
        {
            transform.position = Vector3.MoveTowards(boxObject.transform.position, targetLocationA, step);//Move towards target location at a regulated speed
        }
        
        if(boxObject.transform.position == targetLocationA)//If target location has been reached, swap to the other target location
        {        
            switchPos();
        }        
    }

    private void switchPos()//Swap the positions of the target locations
    {
        targetLocationHold = new Vector3(targetLocationA.x, targetLocationA.y, targetLocationA.z);//Save the current target location
        targetLocationA = targetLocationB;//Set targetLocationA to be the same value as targetLocationB
        targetLocationB = targetLocationHold;//Set targetLocationB to targetLocationA's old value
    }
    
    public void setPos(Vector3 position)
    {
        transform.position = position;
    }

    public void setDestinations(Vector3 locA, Vector3 locB)
    {
        targetLocationA = locA;
        targetLocationB = locB;
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public Vector3[] getDestinations()
    {
        Vector3[] destinations = {targetLocationA, targetLocationB};
        return destinations;
    }
}
