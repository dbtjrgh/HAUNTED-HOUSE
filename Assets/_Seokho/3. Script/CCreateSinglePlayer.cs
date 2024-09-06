using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCreateSinglePlayer : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "SingleLobby")
        {
            CreateSinglePlayer();
        }
    }

    public override void OnLeftRoom()
    {
        CreateSinglePlayer();
    }

    private void CreateSinglePlayer()
    {
        Transform startPositions;
        GameObject tmp = Resources.Load("SinglePlayer") as GameObject;
        GameObject startPositionsObject = GameObject.Find("PlayerStartPositions");

        if (startPositionsObject != null)
        {
            startPositions = startPositionsObject.GetComponent<Transform>();
            if (startPositions != null)
            {
                GameObject singlePlayer = Instantiate(tmp);
                singlePlayer.transform.SetParent(startPositions.transform);
            }
        }
    }
}
