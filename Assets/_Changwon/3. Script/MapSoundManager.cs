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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(BGM bgm)
    {
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
