using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private static int musicState = 1, effectState = 1; 
    [SerializeField] AudioSource musicSource, effectSource;


    [SerializeField] AudioClip[] musicClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            musicSource = transform.GetChild(0).GetComponent<AudioSource>();
            effectSource = transform.GetChild(1).GetComponent<AudioSource>();
            musicSource.clip = musicClips[Random.Range(0, 3)];

            if (musicState != 1)
                musicSource.mute = true;
            if(effectState != 1)
                effectSource.mute = true;

            musicSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip audioClip)
    {
        if (musicSource.mute == false)
        {
            musicSource.clip = audioClip;
            musicSource.Play();
        }
    }

    public void PlayEffect(AudioClip audioClip)
    {
        if(effectSource.mute == false)
        {
            effectSource.PlayOneShot(audioClip, Random.Range(0.32f, 0.45f));
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public bool MuteMusicSource()
    {
        musicSource.mute = !musicSource.mute;
        if (musicSource.mute == false)
        {
            musicState = 1;
            return true;
        }
        else
        {
            musicState = 0;
            return false;
        }
    }

    public bool MuteEffectSource()
    {
        effectSource.mute = !effectSource.mute;
        if (effectSource.mute == false)
        {
            effectState = 1;
            return true;
        }
        else
        {
            effectState = 0;
            return false;
        }
    }
}
