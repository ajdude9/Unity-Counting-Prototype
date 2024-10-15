using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchController : MonoBehaviour
{

    public GameObject projectilePrefab;
    private CounterController gameManager;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
    }

    // Update is called once per frame
    void Update()
    {

        launchHandler();
    }

    void launchHandler()
    {

        if (gameManager.viewType == "throw")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InvokeRepeating("fire", 0f, gameManager.fireRate);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                CancelInvoke("fire");
            }            
        }

    }

    void callFade()
    {
        Debug.Log("Making call to fade in text");
        gameManager.fadeValue = 1;
        gameManager.callFadeIn(0.25f, gameManager.reloadNotify);
    }

    void fire()
    {
        if (gameManager.getCounter("loaded") > 0 && !gameManager.reloadingStatus)
        {
            Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
            gameManager.minusCounter(1, "loaded");
        }
        else
        {
            callFade();
        }

    }

}
