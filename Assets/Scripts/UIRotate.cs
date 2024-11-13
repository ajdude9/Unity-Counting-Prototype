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
        

        spinSpeeds[0] = Random.Range(-spinSpeedBoundary, spinSpeedBoundary);
        spinSpeeds[1] = Random.Range(-spinSpeedBoundary, spinSpeedBoundary);
        spinSpeeds[2] = Random.Range(-spinSpeedBoundary, spinSpeedBoundary);
        for (int i = 0; i < 3; i++)
        {
            if (spinSpeeds[i] > 50)
            {
                spinDirections[i] = true;
            }
            else
            {
                spinDirections[i] = false;
            }
        }
        InvokeRepeating("adjustSpeeds", 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinSpeeds[0] * Time.deltaTime / 2, spinSpeeds[1] * Time.deltaTime / 2, spinSpeeds[2] * Time.deltaTime / 2);
    }

    void adjustSpeeds()
    {

        for (int i = 0; i < 3; i++)
        {

            if (spinSpeeds[i] >= spinSpeedBoundary)
            {
                spinDirections[i] = false;
            }
            else
            {
                if (spinSpeeds[i] <= -spinSpeedBoundary)
                {
                    spinDirections[i] = true;
                }

            }
            if (spinDirections[i])
            {
                spinSpeeds[i] = spinSpeeds[i] + Random.Range(1, 6);
                
            }
            else
            {
                spinSpeeds[i] = spinSpeeds[i] - Random.Range(1, 6);
                
            }
            
        }
    }
}
