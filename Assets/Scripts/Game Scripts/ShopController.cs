using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
    private AudioSource shopAudio;//The audio controller for the shop's audio
    [SerializeField] private AudioClip buySFX;//The audio for buying an item
    [SerializeField] private AudioClip poorSFX;//The audio for when the player doesn't have enough money
    private CounterController gameManager;//The game manager object
    private Dictionary<string, int> shopItems;//A key:value list of the shop's inventory and the item's cost
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();//Find the game manager
        shopAudio = gameObject.GetComponent<AudioSource>();//Get the shop audio's audiosource
        takeInventory();//Call to set up the shopItem's dictionary

    }

    void takeInventory()//Set up the shopItems dictionary
    {
        shopItems = new Dictionary<string, int>()//Each item has the name of the item and how much it costs to buy
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

        shopAudio.pitch = 1f;//Set the shop audio pitch to normal
        int quantity = 0;//The amount to buy
        int value = 0;//The amount the item costs
        string gemType = "";//Define a gemtype string variable
        switch (itemType)//Based on the item being purchased
        {
            case "RubyLabel"://If the ruby label was clicked
                value = shopItems["ruby"];//Get the value of a ruby
                quantity = 5;//Set the quantity to buy to 5
                gemType = "ruby";//Set the gemtype to ruby
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
        if(gameManager.getCounter("bank", "") >= value || gameManager.getCheatStatus())//If the player has more money than the purchase costs, or if they're cheating
        {
            if(!gameManager.getCheatStatus())//If the player isn't cheating
            {
                gameManager.addCounter(quantity, "total", gemType);//Add the quantity purchased to the total number of gems carried
                gameManager.minusCounter(value, "bank", "");//Remove the cost of the item from the player's bank
            }
            shopAudio.PlayOneShot(buySFX);//Play the buy sound effect (even if the player is cheating, to give feedback)
        }
        else//If the player doesn't have enough money to buy the item
        {
            shopAudio.pitch = 0.8f;
            shopAudio.PlayOneShot(poorSFX);//Play an error sound effect instead of the purchase sound effect to notify them
        }

    }

}
