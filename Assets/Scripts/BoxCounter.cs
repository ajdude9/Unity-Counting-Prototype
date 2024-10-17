using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private CounterController gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.addCounter(3, "coins");
    }
}
