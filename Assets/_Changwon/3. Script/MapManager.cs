using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform Spawn;
    
    public GameObject GhostPrefab;

    public GameObject Ghost;

    public GameObject GhostOrb;
    Ghost ghost;


    private void Start()
    {
        GhostSpawn();
        ghost=GetComponent<Ghost>();

    }

    public void GhostSpawn()
    {
        
        Instantiate(GhostPrefab, Spawn.position, Quaternion.identity);        //��Ʈ ����
        if (ghost.ghostType==GhostType.NIGHTMARE||ghost.ghostType==GhostType.BANSHEE)
        {
            Instantiate(GhostOrb, Spawn.position, Quaternion.identity);        //��Ʈ ����
        }

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
