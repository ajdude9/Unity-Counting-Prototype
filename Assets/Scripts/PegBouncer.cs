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
            StartCoroutine(forcePush(awayFromPeg, coinRb));
        }
    }
    public void OnCollisionExit(UnityEngine.Collision collision)
    {
        StopAllCoroutines();
    }
    private IEnumerator forcePush(Vector3 direction, Rigidbody rb)
    {
        yield return new WaitForSeconds(1);
        rb.AddForce(direction * 0.05f, ForceMode.Impulse);
    }
}
