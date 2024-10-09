using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterController : MonoBehaviour
{

    private int counterTotal;
    public Text counterText;


    // Start is called before the first frame update
    void Start()
    {
        counterTotal = 50;
        counterText.text = "Count: " + counterTotal;
    }

    // Update is called once per frame
    /**
    void Update()
    {
        
    }
    */
    public int getCounter()
    {
        return counterTotal;
    }

    public void setCounter(int counterAmount)
    {
        counterTotal = counterAmount;
        counterText.text = "Count: " + counterTotal;
    }

    public void addCounter(int counterAmount)
    {
        counterTotal += counterAmount;
        counterText.text = "Count: " + counterTotal;
    }

    public void minusCounter(int counterAmount)
    {
        counterTotal -= counterAmount;
        counterText.text = "Count: " + counterTotal;
    }
}
