using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapManager : MonoBehaviourPun
{
    public Transform[] Spawn;
    Ghost ghost;
    public Vector3 returnRandom;

    private void Awake()
    {
        GhostSpawn();
    }

    public void GhostSpawn()
    {
        // Master Client�� �ͽ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            int randomIndex = Random.Range(0, Spawn.Length);
            print(randomIndex);

            // GhostPrefab�� ��Ʈ��ũ���� ��� �÷��̾ ������ �� �ֵ��� Master Client�� ����
            GameObject ghostInstance = PhotonNetwork.Instantiate("Ghost", Spawn[randomIndex].position, Quaternion.identity);
            ghost = ghostInstance.GetComponent<Ghost>();

            // ghost ��ü�� null�� �ƴ��� Ȯ���� �� GhostOrb�� ����
            if (ghost != null && (ghost.ghostType == GhostType.NIGHTMARE || ghost.ghostType == GhostType.BANSHEE))
            {
                // GhostOrb�� ��Ʈ��ũ���� ����
                PhotonNetwork.Instantiate("GhostOrbs", Spawn[randomIndex].position, Quaternion.identity);
            }
            else if (ghost == null)
            {
                Debug.LogError("Ghost component is missing on the instantiated GhostPrefab.");
            }

            returnRandom = new Vector3(Spawn[randomIndex].position.x, Spawn[randomIndex].position.y, Spawn[randomIndex].position.z);
        }
        else
        {
            // Master Client�� �ƴϸ� �̹� ������ �ͽ��� ã��
            GameObject ghostInstance = GameObject.Find("Ghost");
            if (ghostInstance != null)
            {
                ghost = ghostInstance.GetComponent<Ghost>();
            }
            else
            {
                Debug.Log("No Ghost found in the scene.");
            }
        }
    }
}
