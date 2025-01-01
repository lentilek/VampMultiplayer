using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip pickUpAudio, clickAudio, endgameAudio, hitAudio, pointAudio, throwAudio;

    public float pickUpVolume, clickVolume, endgameVolume, hitVolume, pointVolume, throwVolume;

    [HideInInspector] public AudioSource audioSrc;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlaySound(string clip)
    {
        switch(clip)
        {
            case "pickUp":
                audioSrc.PlayOneShot(pickUpAudio, pickUpVolume);
                break;
            case "click":
                audioSrc.PlayOneShot(clickAudio, clickVolume);
                break;
            case "endgame":
                audioSrc.PlayOneShot(endgameAudio, endgameVolume);
                break;
            case "hit":
                audioSrc.PlayOneShot(hitAudio, hitVolume);
                break;
            case "point":
                audioSrc.PlayOneShot(pointAudio, pointVolume);
                break;
            case "throw":
                audioSrc.PlayOneShot(throwAudio, throwVolume);
                break;
            default: break;
        }
    }
}
