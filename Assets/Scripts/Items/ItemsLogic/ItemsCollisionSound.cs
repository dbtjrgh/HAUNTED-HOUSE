using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class ItemsCollisionSound : MonoBehaviourPun
{
    [SerializeField]
    private AudioClip _hitSound;
    [SerializeField]
    private float _hitVolume = 0.1f;

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_hitSound == null)
        {
            Debug.LogWarning("오디오 클립이 설정되지 않았습니다.");
        }

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = 1f;
        _audioSource.maxDistance = 8f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_audioSource == null)
        {
            Debug.Log("AudioSource가 초기화되지 않았습니다.");
            return;
        }

        if (_hitSound == null)
        {
            Debug.Log("히트 사운드가 할당되지 않았습니다.");
            return;
        }

        float speed = _rigidbody.velocity.magnitude;
        if (speed > 1f)
        {
            _audioSource.PlayOneShot(_hitSound, _hitVolume * speed);
        }
    }
}
