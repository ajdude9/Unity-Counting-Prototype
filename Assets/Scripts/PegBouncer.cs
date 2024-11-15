using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegBouncer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if(collision.gameObject.CompareTag("Coin"))
        {
            Debug.Log("Pushing Away.");
            Vector3 awayFromPeg = collision.gameObject.transform.position - transform.position;
            Rigidbody coinRb = collision.gameObject.GetComponent<Rigidbody>();
            coinRb.AddForce(awayFromPeg * 0.005f, ForceMode.Impulse);
            forcePush(awayFromPeg, coinRb);
        }
    }
    public void OnCollisionExit(UnityEngine.Collision collision)
    {
        StopAllCoroutines();
    }
    private void forcePush(Vector3 direction, Rigidbody rb)
    {
        rb.AddForce(direction * 0.005f, ForceMode.Impulse);
    }
}
