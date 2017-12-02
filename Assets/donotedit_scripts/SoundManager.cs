using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public AudioSource backgroundMusicSource;
    public AudioSource singleSoundSource;
    public Slider vol;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    public static SoundManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle (AudioClip clip)
    {
        singleSoundSource.clip = clip;
        singleSoundSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        singleSoundSource.pitch = randomPitch;
        singleSoundSource.clip = clips[randomIndex];
        singleSoundSource.Play();
    }
}
