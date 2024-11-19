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
        shop = GameObject.Find("Shop").GetComponent<ShopController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        shop.purchaseItem(gameObject.name);
    }
}
