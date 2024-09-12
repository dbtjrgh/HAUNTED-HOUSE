using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    public float CharPerSeconds;
    public string targetMsg;
    public GameObject Retry;
    public Text Correctanswer;
    int index;
    float interval;
    

    
    private void Awake()
    {
        setMsg($"유령의 정체는...?  {Ghost.instance.ghostType}");
    }

    public void setMsg(string msg)
    {
        targetMsg = msg;
        EffectStart();
        
    }
    void EffectStart()
    {
        Correctanswer.text = "";
        index = 0;

        interval=1.0f/CharPerSeconds;

        Invoke("Effecting", interval);
    }

    void Effecting()
    {
        if (Correctanswer.text == targetMsg)
        {
            EffectEnd();
            return;
        }

        Correctanswer.text += targetMsg[index];
        index++;

        Invoke("Effecting", interval);
    }

    void EffectEnd()
    {
        Retry.SetActive(true);
    }
}
