using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongHandler : MonoBehaviour
{

    public AudioClip song_1;
    AudioSource audioSource_Song;

    void Start()
    {
        audioSource_Song = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audioSource_Song.isPlaying)
        {
            audioSource_Song.clip = song_1;
            audioSource_Song.Play();
        }
    }
}
