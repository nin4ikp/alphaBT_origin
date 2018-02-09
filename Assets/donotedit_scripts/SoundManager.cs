using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public AudioSource backgroundMusicSource;
    public AudioSource singleSoundSource;
    public Slider vol;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    public static SoundManager instance = null;

    /// <summary>
    /// Makes a Singleton of this class. This way the backgorundmusic
    /// does start with every level from teh beginning.
    /// </summary>
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays a single clip. Used for right/wrong sounds.
    /// </summary>
    /// <param name="clip"></param> the clip
    public void PlaySingle (AudioClip clip)
    {
        singleSoundSource.clip = clip;
        singleSoundSource.Play();
    }

    /// <summary>
    /// Sounds more natural, if the pitchrange is randomnized. Used for backgroundmusic.
    /// </summary>
    /// <param name="clips"></param>
    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        singleSoundSource.pitch = randomPitch;
        singleSoundSource.clip = clips[randomIndex];
        singleSoundSource.Play();
    }
}
