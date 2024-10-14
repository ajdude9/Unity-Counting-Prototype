using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class CounterController : MonoBehaviour
{

    private int counterTotal;//The total number of projectiles
    public Text counterText;//Text object to display the number of projectiles
    public Text loadedText;//Text object to display the number of projectiles ready
    private int loadedTotal;//The number of projectiles ready to fire
    private int reloadMax;//The total number of projectiles that can be loaded at once.
    private float reloadSpeed;//How long it takes to reload one projectile
    public float fireRate;//How fast the player can autofire projectiles
    public String projType;//The name of the projectiles being fired
    public bool reloadingStatus = false;//Whether or not the player is currently 'reloading'
    public bool enableCheats = false;
    private AudioSource gameAudio;
    public AudioClip reloadSound;
    public AudioClip reloadSingle;
    private Camera throwCamera;
    private Camera coinCamera;
    public String viewType;
    // Start is called before the first frame update
    void Start()
    {
        gameAudio = gameObject.GetComponent<AudioSource>();
        counterTotal = 44;
        loadedTotal = 6;
        reloadMax = 6;
        reloadSpeed = 0.15f;
        fireRate = 0.35f;
        counterText.text = "Available " + projType + ": " + counterTotal;
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
             
        throwCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        coinCamera = GameObject.Find("Machine Watcher").GetComponent<Camera>();
        viewType = "throw";
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            infiniteAmmoCheat();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            throwCamera.enabled = !throwCamera.enabled;
            coinCamera.enabled = !coinCamera.enabled;
            switch(viewType)
            {
                case "throw":
                throwCamera.enabled = false;
                coinCamera.enabled = true;
                viewType = "coin";
                break;
                case "coin":
                throwCamera.enabled = true;
                coinCamera.enabled = false;
                viewType = "throw";
                break;
            }
        }
    }
    public void refreshCounter()
    {
        counterText.text = "Available " + projType + ": " + counterTotal;
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
    }

    public int getCounter(String type)
    {
        int returnAmount = 0;
        switch (type)
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

    public void addCounter(int amount, String type)
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

    public void minusCounter(int amount, String type)
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

    public void reload()
    {

        if (!reloadingStatus)
        {
            reloadingStatus = true;
            int amountToReload = reloadMax - loadedTotal;//The amount of projectiles to reload is the maximum allowed, minus however many are already loaded
            if (amountToReload > getCounter("total"))//If there isn't enough stored projectiles to reload fully
            {
                amountToReload = getCounter("total");//Instead reload however many are left
            }

            StartCoroutine(reloadTimer(amountToReload));
        }
    }


    private IEnumerator reloadTimer(int reloadAmount)
    {

        for (int i = 0; i < reloadAmount; i++)
        {

            yield return new WaitForSeconds(reloadSpeed);
            addCounter(1, "loaded");
            minusCounter(1, "total");
            gameAudio.PlayOneShot(reloadSingle, 0.8f);

        }
        reloadingStatus = false;
        StartCoroutine(delayedReload());
        yield return null;
    }

    private IEnumerator delayedReload()
    {
        yield return new WaitForSeconds(0.1f);
        gameAudio.PlayOneShot(reloadSound, 0.8f);
    }

    private void infiniteAmmoCheat()
    {
        enableCheats = true;
        counterTotal = 1000000;
        loadedTotal = 1000000;
        reloadMax = 1000000;
        reloadSpeed = 0.01f;
        fireRate = 0.05f;
        refreshCounter();
    }

   
}
