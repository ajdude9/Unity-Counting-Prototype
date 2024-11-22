using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherForce : MonoBehaviour
{

    //PusherForce is code to ensure the coins are properly pushed away from the pusher

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {        
        if(collision.gameObject.CompareTag("Coin"))//If it's a coin
        {
            //Push it away from the object
            Vector3 awayFromPeg = collision.gameObject.transform.position - transform.position;
            Rigidbody coinRb = collision.gameObject.GetComponent<Rigidbody>();
            coinRb.AddForce(awayFromPeg * 0.005f, ForceMode.Impulse);
        }
    }

     void OnTriggerEnter(Collider other)//Same as above, but upon touching a trigger
    {
        if(other.CompareTag("Coin"))
        {
            Vector3 awayFromPeg = other.gameObject.transform.position - transform.position;
            Rigidbody coinRb = other.gameObject.GetComponent<Rigidbody>();
            coinRb.AddForce(awayFromPeg * 0.005f, ForceMode.Impulse);
        }
    }
}
