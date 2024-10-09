using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    private CounterController counterController;

    private void Start()
    {
        counterController = GameObject.Find("Counter Observer").GetComponent<CounterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        counterController.addCounter(1);
    }
}
