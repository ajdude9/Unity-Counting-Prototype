using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallForward : MonoBehaviour
{

    private Rigidbody ballRb;
    private Vector3 mousePos;
    public Vector3 worldPos;
    public float torque = 50;
    // Start is called before the first frame update
    void Start()
    {                
        ballRb = gameObject.GetComponent<Rigidbody>();
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 1;
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.position = new Vector3(15.6f, 3.2f, -0.22f);
        ballRb.transform.LookAt(worldPos);
        ballRb.AddRelativeForce(ballRb.transform.forward * 12, ForceMode.Impulse);
        ballRb.AddRelativeForce(Vector3.up * (worldPos.y / 2) , ForceMode.Impulse);
        ballRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        StartCoroutine(tilDeath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator tilDeath()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    float RandomTorque()
    {
        return Random.Range(-torque, torque);
    }
    
}
