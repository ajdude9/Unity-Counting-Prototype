using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterController : MonoBehaviour
{

    private int counterTotal;
    public Text counterText;
    public Text loadedText;
    private int loadedTotal;
    public String projType;

    // Start is called before the first frame update
    void Start()
    {        
        counterTotal = 44;
        loadedTotal = 6;
        counterText.text = "Available " + projType + ": " + counterTotal;
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
    }

    // Update is called once per frame
    /**
    void Update()
    {
        
    }
    */
    public int getCounter(String type)
    {
        int returnAmount = 0;
        switch(type)
        {
            case "total":
                returnAmount = counterTotal;
            break;
            case "loaded":
                returnAmount = loadedTotal;
            break;
        }
        return returnAmount;
    }

    public void setCounter(int amount, String type)
    {
        switch(type)
        {
            case "total":
                counterTotal = amount;
                counterText.text = "Available " + projType + ": " + counterTotal;
            break;
            case "loaded":
                loadedTotal = amount;
                loadedText.text = "Loaded " + projType + ": " + loadedTotal;
            break;
        }
    }

    public void addCounter(int amount, String type)
    {
        switch(type)
        {
            case "total":
                counterTotal += amount;
                counterText.text = "Available " + projType + ": " + counterTotal;
            break;
            case "loaded":
                loadedTotal += amount;
                loadedText.text = "Loaded " + projType + ": " + loadedTotal;
            break;
        }
    }

    public void minusCounter(int amount, String type)
    {
        switch(type)
        {
            case "total":
                counterTotal -= amount;
                counterText.text = "Available " + projType + ": " + counterTotal;
            break;
            case "loaded":
                loadedTotal -= amount;
                loadedText.text = "Loaded " + projType + ": " + loadedTotal;
            break;
        }
    }
}
