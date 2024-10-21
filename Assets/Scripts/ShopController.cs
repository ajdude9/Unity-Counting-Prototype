using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    private AudioSource shopAudio;
    public AudioClip buySFX;
    // Start is called before the first frame update
    void Start()
    {
        shopAudio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        shopAudio.PlayOneShot(buySFX, 0.5f);
    }
}
