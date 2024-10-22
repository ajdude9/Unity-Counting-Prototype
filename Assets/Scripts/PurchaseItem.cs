using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseItem : MonoBehaviour
{

    private ShopController shop;
    [SerializeField] private String itemName;
    [SerializeField] private int itemValue;
    [SerializeField] private int itemQuantity;

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
        shop.purchaseItem(itemName, itemValue, itemQuantity);
    }
}
