using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeResult : MonoBehaviour
{
    public CanvasGroup ResultUIGroup;
    public float FadeTime = 1.0f;
    
    bool asd = true;


    public void FadeIn()
    {
        if (ResultUIGroup != null)
        {
            StartCoroutine(fadeCanvasGroup(ResultUIGroup, ResultUIGroup.alpha, 1, FadeTime));
            asd = false; // 한번 실행 후 다시 실행되지 않도록 asd를 false로 설정
        }
        
    }

    private void OnEnable()
    {
        // 오브젝트가 활성화될 때만 페이드 인 실행
        if (ResultUIGroup != null && asd)
        {
            ResultUIGroup.alpha = 0; // 처음에 투명하게 설정
            FadeIn();
        }
    }



    IEnumerator fadeCanvasGroup(CanvasGroup cg,float start,float end,float duration)
    {
        float elastpedTime = 1.0f;
        while(elastpedTime<duration)
        {
            elastpedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start,end,elastpedTime/duration);
            yield return null;
        }
        cg.alpha = end;
    }

    
}
