using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotate : MonoBehaviour
{
    //The speed at which the object rotates on each axis
    private float[] spinSpeeds = { 0, 0, 0 };
    //The upper boundary of how fast the object can rotate
    private float spinSpeedBoundary = 100;
    //Which direction the speed change is going; true = up, false = down.
    private bool[] spinDirections = { false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        calculateSpeeds();//Calculate the speed at which the object should spin and the direction its speed change should move
        InvokeRepeating("adjustSpeeds", 0, 1);//Repeatedly adjust the speed every second
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinSpeeds[0] * Time.deltaTime / 2, spinSpeeds[1] * Time.deltaTime / 2, spinSpeeds[2] * Time.deltaTime / 2);//Spin the object on each defined axis, multiplied by halved deltatime
    }

    public void calculateSpeeds()
    {
        //Generate a random speed for the X, Y and Z axis in the array
        spinSpeeds[0] = Random.Range(-spinSpeedBoundary, spinSpeedBoundary);
        spinSpeeds[1] = Random.Range(-spinSpeedBoundary, spinSpeedBoundary);
        spinSpeeds[2] = Random.Range(-spinSpeedBoundary, spinSpeedBoundary);
        for (int i = 0; i < 3; i++)//Iterate through each axis in the array
        {
            if (spinSpeeds[i] > 50)//If the spin speed is above 50
            {
                spinDirections[i] = true;//Have the speed gradually increase
            }
            else
            {
                spinDirections[i] = false;//Otherwise, have the speed gradually decrease
            }
        }
    }

    void adjustSpeeds()//Adjust the speed of the spin, either making it faster or slower, and flipping direction at the lowest and highest values
    {

        for (int i = 0; i < 3; i++)//For each axis
        {

            if (spinSpeeds[i] >= spinSpeedBoundary)//If the current speed is greater than or equal to the upper boundary
            {
                spinDirections[i] = false;//Switch the direction so it starts decreasing
            }
            else//Otherwise
            {
                if (spinSpeeds[i] <= -spinSpeedBoundary)//If the current speed is less than or equal to the lower boundary
                {
                    spinDirections[i] = true;//Switch the direction so it starts increasing
                }

            }//If the speeds are at neither boundary
            if (spinDirections[i])//If spinDirection is true, increase the value by a random amount between 1 and 5 (upper bound is exclusive)
            {
                spinSpeeds[i] = spinSpeeds[i] + Random.Range(1, 6);
                
            }
            else//Otherwise, the spinDirection must be false, so decrease the value by a random amount between 1 and 5 instead
            {
                spinSpeeds[i] = spinSpeeds[i] - Random.Range(1, 6);
                
            }
            
        }
    }
}
