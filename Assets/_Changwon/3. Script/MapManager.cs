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
        // Master Client만 귀신을 생성
        if (PhotonNetwork.IsMasterClient)
        {
            int randomIndex = Random.Range(0, Spawn.Length);
            print(randomIndex);

            // GhostPrefab을 네트워크에서 모든 플레이어가 공유할 수 있도록 Master Client가 생성
            GameObject ghostInstance = PhotonNetwork.Instantiate("Ghost", Spawn[randomIndex].position, Quaternion.identity);
            ghost = ghostInstance.GetComponent<Ghost>();

            // ghost 객체가 null이 아닌지 확인한 후 GhostOrb를 생성
            if (ghost != null && (ghost.ghostType == GhostType.NIGHTMARE || ghost.ghostType == GhostType.BANSHEE))
            {
                // GhostOrb도 네트워크에서 생성
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
            // Master Client가 아니면 이미 생성된 귀신을 찾음
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
