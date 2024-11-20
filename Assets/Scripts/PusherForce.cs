using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherForce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {        
        if(collision.gameObject.CompareTag("Coin"))
        {
            Vector3 awayFromPeg = collision.gameObject.transform.position - transform.position;
            Rigidbody coinRb = collision.gameObject.GetComponent<Rigidbody>();
            coinRb.AddForce(awayFromPeg * 0.005f, ForceMode.Impulse);
        }
    }

     void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {
            Vector3 awayFromPeg = other.gameObject.transform.position - transform.position;
            Rigidbody coinRb = other.gameObject.GetComponent<Rigidbody>();
            coinRb.AddForce(awayFromPeg * 0.005f, ForceMode.Impulse);
        }
    }
}
