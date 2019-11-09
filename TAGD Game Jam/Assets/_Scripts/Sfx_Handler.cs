using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sfx_Handler : MonoBehaviour
{
    public AudioClip sfx_gun;
    public AudioClip sfx_hurt;
    public AudioClip sfx_bulletmove;
    AudioSource audioSource_SFX;

    void Start()
    {
        audioSource_SFX = GetComponent<AudioSource>();
        GameManager.Instance.PatternUsed.AddListener(play_sfx_gun);
        GameManager.Instance.UpdateTimeDuration.AddListener(play_sfx_bulletmove);
        GameManager.Instance.PlayerDamaged.AddListener(play_sfx_hurt);
    }

    void play_sfx_gun()
    {
        audioSource_SFX.PlayOneShot(sfx_gun);
    }

    void play_sfx_bulletmove()
    {
        audioSource_SFX.PlayOneShot(sfx_bulletmove);
    }

    void play_sfx_hurt()
    {
        audioSource_SFX.PlayOneShot(sfx_hurt);
    }

}
