using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
    private AudioSource shopAudio;//The audio controller for the shop's audio
    public AudioClip buySFX;//The audio for buying an item
    public AudioClip poorSFX;//The audio for when the player doesn't have enough money
    private CounterController gameManager;//The game manager object
    private Dictionary<string, int> shopItems;//A key:value list of the shop's inventory and the item's cost
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
        shopAudio = gameObject.GetComponent<AudioSource>();
        takeInventory();

    }

    void takeInventory()//Set up the shopItems dictionary
    {
        shopItems = new Dictionary<string, int>()
        {
            {"ruby", 1},
            {"emerald", 2},
            {"amethyst", 5},
            {"diamond", 20}
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void purchaseItem(String itemType)//Purchase a specific item for a certain price
    {

        shopAudio.pitch = 1f;
        int quantity = 0;//The amount to buy
        int value = 0;//The amount the item costs
        string gemType = "";
        switch (itemType)
        {
            case "RubyLabel":
                value = shopItems["ruby"];
                quantity = 5;
                gemType = "ruby";
            break;
            case "EmeraldLabel":
                value = shopItems["emerald"];
                quantity = 5;
                gemType = "emerald";
            break;
            case "AmethystLabel":
                value = shopItems["amethyst"];
                quantity = 3;
                gemType = "amethyst";
            break;
            case "DiamondLabel":
                value = shopItems["diamond"];
                quantity = 2;
                gemType = "diamond";
            break;
        }
        if(gameManager.getCounter("bank", "") >= value)//If the player has more money than the purchase costs
        {
            gameManager.addCounter(quantity, "total", gemType);
            gameManager.minusCounter(value, "bank", "");
            shopAudio.PlayOneShot(buySFX);
        }
        else
        {
            shopAudio.pitch = 0.8f;
            shopAudio.PlayOneShot(poorSFX);
        }

    }

}
