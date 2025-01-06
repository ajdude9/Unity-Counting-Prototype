using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchController : MonoBehaviour
{

    public GameObject projectilePrefab;//The prefab that holds the projectile
    private CounterController gameManager;//The game controller



    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();//Find the game controller
    }

    // Update is called once per frame
    void Update()
    {

        launchHandler();//Handle launch input
    }

    void launchHandler()
    {

        if (gameManager.viewType == "throw")//If the player is looking at the box throw view
        {
            if (Input.GetKeyDown(KeyCode.Space))//If the player presses the space bar
            {
                InvokeRepeating("fire", 0f, gameManager.fireRate);//Begin automatically firing projectiles, the first with no delay, then the second with the firerate delay
                //Being able to spam shots by rapidly pressing the spacebar is intentional; the autofire is an upgradable QOL to prevent RSI
            }
            else if (Input.GetKeyUp(KeyCode.Space))//If the player lets go of the spacebar
            {
                CancelInvoke("fire");//Stop firing
            }            
        }
        if(gameManager.viewType != "throw")//If the player stops looking at the box throw view
        {
            CancelInvoke("fire");//Stop firing, if the player was already firing.
        }

    }

    void callFade()//Call to fade in the reload text when the player is out of ammo
    {
        //Debug.Log("Making call to fade in text");
        gameManager.fadeValue = 1;
        gameManager.callFadeIn(0.25f, gameManager.reloadNotify);
    }

    void fire()//Fire a projectile
    {
        if (gameManager.getCounter("loaded", "") > 0 && !gameManager.reloadingStatus || gameManager.getCheatStatus())//If the player has enough ammo loaded and isn't currently reloading, OR is cheating.
        {
            Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);//Create a projectile
            if(!gameManager.getCheatStatus())//If the player isn't cheating
            {
                gameManager.minusCounter(1, "loaded", "");//Remove a projectile from the projectiles loaded
            }
        }
        else
        {
            if(!gameManager.reloadingStatus)//If the player attempts to fire, but has no ammo loaded and isn't currently reloading
            {
                callFade();//Fade in the reload text
            }
        }

    }

}
