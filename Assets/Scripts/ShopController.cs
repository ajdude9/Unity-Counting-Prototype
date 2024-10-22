using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
    private AudioSource shopAudio;
    public AudioClip buySFX;
    private CounterController gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
        shopAudio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void purchaseItem(String itemType, int value, int quantity)
    {
        if (gameManager.getCounter("bank") > value)
        {
            switch (itemType)
            {
                case "redGem":
                    gameManager.addCounter(quantity, "total");
                    gameManager.minusCounter(value, "bank");
                    shopAudio.PlayOneShot(buySFX, 0.2f);
                    break;
            }
        }
        else
        {
            shopAudio.pitch = 0.5f;
            shopAudio.PlayOneShot(buySFX, 0.2f);
        }
    }

}
