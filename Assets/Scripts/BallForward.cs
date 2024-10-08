using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallForward : MonoBehaviour
{

    private Rigidbody ballRb;
    private Vector3 mousePos;
    public Vector3 worldPos;
    // Start is called before the first frame update
    void Start()
    {                
        ballRb = gameObject.GetComponent<Rigidbody>();
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 1;
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.position = worldPos;
        ballRb.AddRelativeForce(Vector3.left * 15, ForceMode.Impulse);
        ballRb.AddRelativeForce(Vector3.up * worldPos.y, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        moveTest();
    }

    void moveTest()
    {
        
    }
}
