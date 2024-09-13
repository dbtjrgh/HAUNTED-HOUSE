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
            asd = false; // �ѹ� ���� �� �ٽ� ������� �ʵ��� asd�� false�� ����
        }
        
    }

    private void OnEnable()
    {
        // ������Ʈ�� Ȱ��ȭ�� ���� ���̵� �� ����
        if (ResultUIGroup != null && asd)
        {
            ResultUIGroup.alpha = 0; // ó���� �����ϰ� ����
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
