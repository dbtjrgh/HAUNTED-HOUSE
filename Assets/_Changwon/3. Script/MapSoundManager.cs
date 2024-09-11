using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MapSoundManager : MonoBehaviour
{
    public enum BGM
    {
        GAME,
        ENDING,
        CLEAR
    }

    public enum Interaction
    {
        DOOROPEN,
        DOORCLOSE,
        LIGHT,
        EMF13,
        EMF5,
        WALKING,                
        TRUCKBUTTON,
        GHOSTWALK,
        GHOSTLAUGH
    }


    public static MapSoundManager instance;
    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] interactions;
    

    [SerializeField] AudioSource background;
    [SerializeField] AudioSource interaction;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SoundManager instance 설정됨");

        }
        else
        {
            Debug.Log("SoundManager 인스턴스 중복, 파괴됨");
            Destroy(gameObject);
        }
        
    }

    private void DisableOtherAudioSources()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            // SoundManager의 AudioSource가 아니라면 비활성화
            if (source != background && source != interaction)
            {
                source.enabled = false;
            }
        }
    }

    public void PlayBGM(BGM bgm)
    {
        Debug.Log("PlayBGM");
        background.clip = bgms[(int)bgm];
        background.Play();
    }

    public void StopBGM()
    {
        background.Stop();
    }
    
    public void PlayInteraction(Interaction inter)
    {
        
        interaction.PlayOneShot(interactions[(int)inter]);
    }


}
