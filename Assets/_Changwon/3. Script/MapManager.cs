using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform Spawn;
    
    public GameObject GhostPrefab;

    public GameObject Ghost;

    private void Start()
    {
        GhostSpawn();

    }

    public void GhostSpawn()
    {
        
        Instantiate(GhostPrefab, Spawn.position, Quaternion.identity);        //��Ʈ ����

    }

    public void ghostblink()
    {
        //��Ʈ ������
        //��Ʈ�� �÷��̾ ���� �̵�
        //��Ʈ�� �÷��̾�� ����
        //��Ʈ�� �÷��̾�� �����ϸ� �÷��̾ ����
        GhostPrefab.SetActive(false);
        
    }
}
