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
    public AudioClip poorSFX;
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
        if (gameManager.getCounter("bank", "") >= value)
        {
            shopAudio.pitch = 1f;
            switch (itemType)
            {                
                case "ruby":
                    gameManager.addCounter(quantity, "total", itemType);
                    gameManager.minusCounter(value, "bank", "");
                    shopAudio.PlayOneShot(buySFX, 0.2f);
                    break;
            }
        }
        else
        {
            shopAudio.pitch = 0.85f;
            shopAudio.PlayOneShot(poorSFX, 0.2f);
        }
    }

}
