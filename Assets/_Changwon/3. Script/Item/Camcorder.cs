using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camcorder : MonoBehaviour
{
    public Camera camcorder;
    public Camera greenScreen;
    public MeshRenderer screenMesh;

    public RenderTexture renderTexture;
    public Material renderTextureMat;

    private bool normalMode = true;

    private void Start()
    {
        CamcorderSetup();
    }

    /*public void OnMainUse()               // 사용함수.
    {
        if (normalMode)
        {
            SetGhostOrbMode();
        }
        else
        {
            SetNormalMode();
        }
    }*/


    private void SetNormalMode()
    {
        renderTextureMat.color = Color.white;
        greenScreen.gameObject.SetActive(false);
        camcorder.gameObject.SetActive(true);
        normalMode = true;
    }

    private void SetGhostOrbMode()
    {
        renderTextureMat.color = Color.green;
        camcorder.gameObject.SetActive(false);
        greenScreen.gameObject.SetActive(true);
        normalMode = false;
    }

    private void CamcorderSetup()
    {
        camcorder.targetTexture = renderTexture;
        greenScreen.targetTexture = renderTexture;
        screenMesh.material = renderTextureMat;
    }
}
