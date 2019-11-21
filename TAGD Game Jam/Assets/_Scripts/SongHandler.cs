using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongHandler : MonoBehaviour
{
    private int count;


    public AudioClip song_1;
    public AudioClip song_2;
    AudioSource audioSource_Song;

    void Start()
    {
        count = 0;
        GameManager.Instance.NextEnemy.AddListener(update_count);
        audioSource_Song = GetComponent<AudioSource>();
        audioSource_Song.volume = .8F;
    }

    void Update()
    {
        if (!audioSource_Song.isPlaying)
        {
            if(count < 3)
            {
                audioSource_Song.clip = song_1;
                audioSource_Song.Play();
            }
            else
            {
                audioSource_Song.clip = song_2;
                audioSource_Song.Play();
            }

        }
    }

    void update_count()
    {
        count++;
    }

}
