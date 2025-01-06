using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegBouncer : MonoBehaviour
{

    //Pegbouncer is designed to push coins away from pegs with a small amount of force, making it harder for them to perfectly balance or get stuck

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(UnityEngine.Collision collision)//Upon having something collide with it
    {
        if(collision.gameObject.CompareTag("Coin"))//If the collision is a coin
        {
            //Debug.Log("Pushing Away.");
            Vector3 awayFromPeg = collision.gameObject.transform.position - transform.position;//Calculate the opposite direction from the point of impact
            Rigidbody coinRb = collision.gameObject.GetComponent<Rigidbody>();//Find the coin's rigidbody
            forcePush(awayFromPeg, coinRb);//Run the function with the direction and rigidbody to push it away
        }
    }
    
    private void forcePush(Vector3 direction, Rigidbody rb)//Push a rigidbody away
    {
        rb.AddForce(direction * 0.0025f, ForceMode.Impulse);//Add a small amount of force to the supplied rigidbody in the supplied direction
    }
}
