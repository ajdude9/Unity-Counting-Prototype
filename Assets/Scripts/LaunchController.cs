using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchController : MonoBehaviour
{

    public GameObject projectilePrefab;
    private CounterController counterController;
    

    // Start is called before the first frame update
    void Start()
    {
        counterController = GameObject.Find("Counter Observer").GetComponent<CounterController>();
    }

    // Update is called once per frame
    void Update()
    {

        launchHandler();
    }

    void launchHandler()
    {

        if(Input.GetKeyDown(KeyCode.Space) && counterController.getCounter("loaded") > 0 && !counterController.reloadingStatus)
        {            
            Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);            
            counterController.minusCounter(1, "loaded");
        }

        if(Input.GetKeyDown(KeyCode.Space) && counterController.enableCheats)
        {
            InvokeRepeating("autoFire", 0.1f, 0.1f);
        }
        else if (Input.GetKeyUp (KeyCode.Space)) {
            CancelInvoke ("autoFire");
        }

    }

    void autoFire()
    {
        Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);            
        counterController.minusCounter(1, "loaded");
    }
   
}
