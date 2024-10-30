using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioLibrary;
    private AudioSource audioPlayer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playAudio(int audioLocation, float pitch, float vol)
    {
        audioPlayer.pitch = pitch;
        audioPlayer.PlayOneShot(audioLibrary[audioLocation], vol);
        audioPlayer.pitch = 1;
    }
}
