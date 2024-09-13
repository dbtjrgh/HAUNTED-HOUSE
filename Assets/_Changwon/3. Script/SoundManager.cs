using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    public static SoundManager instance;
    public AudioSource musicSource;
    private AudioSource sfxSource;
    public AudioClip DoorOpen;
    public AudioClip DoorClose;
    public AudioClip LIGHT;
    public AudioClip EMF13;
    public AudioClip EMF5;
    public AudioClip WALKING;
    public AudioClip TRUCKBUTTON;
    public AudioClip GHOSTWALK;
    public AudioClip GHOSTLAUGH;
    public AudioClip GameScene;
    public AudioClip ClearScene;
    public AudioClip FailScene;



    private void Awake()
    {


        if (instance != null)
        {
            Destroy(gameObject);
            return;

        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        sfxSource = gameObject.AddComponent<AudioSource>();


    }

    public void PlayGameSceneMusic()
    {
        musicSource.clip = GameScene;
        musicSource.loop = true;
        musicSource.Play();
        musicSource.volume = 1f;
    }

    public void StopGameSceneMusic()
    {
        if (musicSource.clip == GameScene)
        {
            musicSource.Stop();
            musicSource.loop = false;
            musicSource.clip = null;
        }
    }

    public void PlayClearSceneMusic()
    {
        musicSource.clip = ClearScene;
        musicSource.volume = 0.1f;
        musicSource.PlayOneShot(musicSource.clip);
        
    }

    public void stopSceneMusic()
    {
        if ((musicSource.isPlaying))
        {
            musicSource.Stop();

            musicSource.clip = null;
        }
    }

    public void PlayFailSceneMusic()
    {
        
        musicSource.clip = FailScene;
        musicSource.volume = 0.1f;
        musicSource.PlayOneShot(musicSource.clip);
        
    }
    public void StopFailSceneMusic()
    {
        if (musicSource.clip == FailScene)
        {
            musicSource.Stop();
            musicSource.loop = false;
            musicSource.clip = null;
        }
    }



    public void DoorOpenSound()
    {
        sfxSource.PlayOneShot(DoorOpen);
    }

    public void DoorCloseSound()
    {
        sfxSource.PlayOneShot(DoorClose);
    }

    public void FlashLightSound()
    {
        sfxSource.PlayOneShot(LIGHT);
    }

    

    

    public void TruckButtonSound()
    {
        sfxSource.PlayOneShot(TRUCKBUTTON);
    }

    public void GhostWalkSound()
    {
        sfxSource.PlayOneShot(GHOSTWALK);
    }

    public void GhostLaughSound()
    {
        sfxSource.PlayOneShot(GHOSTLAUGH);
    }



    public void PlayWalkingSound()
    {
        if (!sfxSource.isPlaying)
        {
            
            sfxSource.clip = WALKING;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopWalkingSound()
    {
        if (sfxSource.clip == WALKING)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }


    public void EMFNormalSound()
    {
        if(!sfxSource.isPlaying)
        {
            sfxSource.clip = EMF13;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }
    public void NormalEMFStop()
    {
        if (sfxSource.clip == EMF13)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }

    public void EMFHighSound()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.clip = EMF5;
            sfxSource.loop = true;
            sfxSource.Play();
        }

    }

    public void StopEMFHighSound()
    {
        if (sfxSource.clip == EMF5)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }


}
