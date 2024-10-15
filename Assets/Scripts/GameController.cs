using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using TMPro;

public class CounterController : MonoBehaviour
{
    private CoinDropController coinDropController;
    private int counterTotal;//The total number of projectiles
    public Text counterText;//Text object to display the number of projectiles
    public Text loadedText;//Text object to display the number of projectiles ready
    public TextMeshProUGUI reloadNotify;//Text object to tell the player to reload
    private int loadedTotal;//The number of projectiles ready to fire
    private int reloadMax;//The total number of projectiles that can be loaded at once.
    private float reloadSpeed;//How long it takes to reload one projectile
    public float fireRate;//How fast the player can autofire projectiles
    public String projType;//The name of the projectiles being fired
    public bool reloadingStatus = false;//Whether the player is currently 'reloading'
    public bool enableCheats = false;//Whether cheats are enabled
    private AudioSource gameAudio;//Source for the universal game audio
    public AudioClip reloadSound;//Reloading sound (fully reloaded)
    public AudioClip reloadSingle;//Reloading sound (single projectile)
    private Camera throwCamera;//The camera for throwing projectiles into a box
    private Camera coinCamera;//The camera for putting coins in a machine
    public String viewType;//Which camera is currently being viewed
    public bool firstSwitch = false;//Whether the switch to the coin camera is the first one since the game started
    public int fadeValue;
    // Start is called before the first frame update
    void Start()
    {
        //get the game audio and set the default values for variabls
        gameAudio = gameObject.GetComponent<AudioSource>();
        coinDropController = GameObject.Find("Coin Dropper").GetComponent<CoinDropController>();
        counterTotal = 44;
        loadedTotal = 6;
        reloadMax = 6;
        reloadSpeed = 0.15f;
        fireRate = 0.35f;
        counterText.text = "Available " + projType + ": " + counterTotal;
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
        reloadNotify.color = new Color(reloadNotify.color.r, reloadNotify.color.g, reloadNotify.color.b, 0);
        throwCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        coinCamera = GameObject.Find("Machine Watcher").GetComponent<Camera>();
        viewType = "throw";
    }

    // Update is called once per frame

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            infiniteAmmoCheat();//Enable cheats
        }
        if(Input.GetKeyDown(KeyCode.Q))//Switch the camera view
        {
            /**
            throwCamera.enabled = !throwCamera.enabled;
            coinCamera.enabled = !coinCamera.enabled;
            */
            switch (viewType)
            {
                case "throw":
                    switchToCoin();
                    break;
                case "coin":
                    switchToThrow();
                    break;
                case "shop":

                    break;
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            fadeValue = 0;
            StartCoroutine(textFadeOut(1f, reloadNotify));
            reload();
        }
    }
    public void refreshCounter()//Update the counters to their current values
    {
        counterText.text = "Available " + projType + ": " + counterTotal;
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
    }

    public int getCounter(String type)//Get a specific counter
    {
        int returnAmount = 0;
        switch (type)
        {
            case "total"://Get the total amount of projectiles
                returnAmount = counterTotal;
                break;
            case "loaded"://Get the amount of projectiles able to be fired
                returnAmount = loadedTotal;
                break;
        }
        return returnAmount;
    }

    public void setCounter(int amount, String type)//Set a counter to a specific value
    {
        switch (type)
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

    public void addCounter(int amount, String type)//Add to a counter a specific value
    {
        switch (type)
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

    public void minusCounter(int amount, String type)//Remove a specific value from a specific counter
    {
        switch (type)
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

    public void reload()//Reload projectiles
    {

        if (!reloadingStatus)//If the player isn't already reloading
        {
            int amountToReload = reloadMax - loadedTotal;//The amount of projectiles to reload is the maximum allowed, minus however many are already loaded
            if(amountToReload > 0)
            {
                reloadingStatus = true;            
                if (amountToReload > getCounter("total"))//If there isn't enough stored projectiles to reload fully
                {
                    amountToReload = getCounter("total");//Instead reload however many are left
                }

                StartCoroutine(reloadTimer(amountToReload));
            }
        }
    }


    private IEnumerator reloadTimer(int reloadAmount)//Reload projectiles on a timer rather than all at once
    {

        for (int i = 0; i < reloadAmount; i++)
        {

            yield return new WaitForSeconds(reloadSpeed);//Delay based on reload speed
            addCounter(1, "loaded");
            minusCounter(1, "total");
            gameAudio.PlayOneShot(reloadSingle, 0.8f);

        }        
        yield return new WaitForSeconds(0.1f);
        if(reloadingStatus)
        {
            gameAudio.PlayOneShot(reloadSound, 0.8f);
            reloadingStatus = false;
        }
        yield return null;
    }

    private void infiniteAmmoCheat()//Enable cheats, setting status to immensely high values
    {
        enableCheats = true;
        counterTotal = 1000000;
        loadedTotal = 1000000;
        reloadMax = 1000000;
        reloadSpeed = 0.01f;
        fireRate = 0.05f;
        refreshCounter();
    }

    private void switchToCoin()
    {
        throwCamera.enabled = false;
        coinCamera.enabled = true;
        viewType = "coin";
        counterText.enabled = false;
        loadedText.enabled = false;
        if (!firstSwitch)
        {
            firstSwitch = true;
            coinDropController.generateCoins(coinDropController.coinSpawnAmount);
        }
    }

    private void switchToThrow()
    {
        throwCamera.enabled = true;
        coinCamera.enabled = false;
        counterText.enabled = true;
        loadedText.enabled = true;
        viewType = "throw";
    }

    public void callFadeIn(float time, TextMeshProUGUI text)
    {
        StartCoroutine(textFadeIn(time, text));
    }

    private IEnumerator textFadeIn(float time, TextMeshProUGUI text)
    {
        Debug.Log("Attempting to fade in text");
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a < fadeValue)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
            yield return null;
        }
    }

    public void callFadeOut(float time, TextMeshProUGUI text)
    {
        StartCoroutine(textFadeOut(time, text));
    }

    private IEnumerator textFadeOut(float time, TextMeshProUGUI text)
    {
        Debug.Log("Attempting to fade out text");

        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a > fadeValue)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }
    }
    
}
