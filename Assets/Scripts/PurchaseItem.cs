using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseItem : MonoBehaviour
{

    private ShopController shop;


    // Start is called before the first frame update
    void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<ShopController>();//Find the shop controller
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()//When the object this script is attached to is clicked
    {
        shop.purchaseItem(gameObject.name);//Run the shop's purchaseItem script, and send it this object's name
    }
}
