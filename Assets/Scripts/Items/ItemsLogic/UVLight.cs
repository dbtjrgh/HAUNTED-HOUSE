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
                _curPos = transform.position;                           // ���� ��ġ ����
                _distance = (_lastPos - _curPos).magnitude;             // ���� ��ġ�� ���� ��ġ�� �Ÿ� ���

                if (_distance > _epsilon)                               // ���� ��ġ�� ���� ��ġ�� �Ÿ��� ���� �� �̻��̸�
                {                   
                    _lastPos = _curPos;                                 // ���� ��ġ�� ���� ��ġ�� ����
                    ChangeMaterialParameters();
                }
            }
        }
    }

    public void DisableUVLight()
    {
        _revealableMaterial.SetFloat("lightAngle", 0f);                 // ����Ʈ ���� 0���� ����
        ChangeMaterialParameters();                                     // ��Ƽ���� �Ķ���� ����
        _isEnabled = false;                                             // UV ����Ʈ ��Ȱ��ȭ
        StopCoroutine(CheckPosition());
    }
    public void EnableUVLight()
    {
        _revealableMaterial.SetFloat("lightAngle", _lightAngle);        // ����Ʈ ���� ����
        ChangeMaterialParameters();                                     // ��Ƽ���� �Ķ���� ����
        _isEnabled = true;                                              // UV ����Ʈ Ȱ��ȭ
        StartCoroutine(CheckPosition());                                // ��ġ ���� üũ
    }
    private void ChangeMaterialParameters()                 // ��Ƽ���� �Ķ���� ����
    {
        _revealableMaterial.SetVector("lightPosition", _light.transform.position);              // ����Ʈ�� ��ġ
        _revealableMaterial.SetVector("lightDirection", -_light.transform.forward);             // ����Ʈ�� ����
    }
}
