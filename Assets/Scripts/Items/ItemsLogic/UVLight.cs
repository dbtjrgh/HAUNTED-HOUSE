using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVLight : MonoBehaviour
{
    [SerializeField]
    private Light _light;

    [SerializeField]
    public Material _revealableMaterial;

    [SerializeField]
    private float _lightAngle = 360f;

    private Vector3 _curPos;
    private Vector3 _lastPos = new Vector3(0f, 0f, 0f);
    private float _epsilon = 0.001f;                    
    private float _distance;

    private bool _isEnabled = false;

    private void Start()
    {
        _lastPos = transform.position;
        DisableUVLight();
    }

    IEnumerator CheckPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            if (_isEnabled)
            {
                _curPos = transform.position;                           // 현재 위치 저장
                _distance = (_lastPos - _curPos).magnitude;             // 이전 위치와 현재 위치의 거리 계산

                if (_distance > _epsilon)                               // 이전 위치와 현재 위치의 거리가 일정 값 이상이면
                {                   
                    _lastPos = _curPos;                                 // 이전 위치를 현재 위치로 갱신
                    ChangeMaterialParameters();
                }
            }
        }
    }

    public void DisableUVLight()
    {
        _revealableMaterial.SetFloat("lightAngle", 0f);                 // 라이트 각도 0으로 설정
        ChangeMaterialParameters();                                     // 머티리얼 파라미터 변경
        _isEnabled = false;                                             // UV 라이트 비활성화
        StopCoroutine(CheckPosition());
    }
    public void EnableUVLight()
    {
        _revealableMaterial.SetFloat("lightAngle", _lightAngle);        // 라이트 각도 설정
        ChangeMaterialParameters();                                     // 머티리얼 파라미터 변경
        _isEnabled = true;                                              // UV 라이트 활성화
        StartCoroutine(CheckPosition());                                // 위치 변경 체크
    }
    private void ChangeMaterialParameters()                 // 머티리얼 파라미터 변경
    {
        _revealableMaterial.SetVector("lightPosition", _light.transform.position);              // 라이트의 위치
        _revealableMaterial.SetVector("lightDirection", -_light.transform.forward);             // 라이트의 방향
    }
}
