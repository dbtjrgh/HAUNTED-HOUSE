using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class CMenuScreen : MonoBehaviour
{
    [Header("Main Menu")]
    public RectTransform mainMenuScreen;
    public TextMeshProUGUI playerName;
    public TMP_InputField playerNameInput;
    public Button changeNickNameButton;
    public Button createRoomButton;
    public Button findRoomButton;
    public Button joinRandomRoomButton;
    public Button quitButton;


    [Header("Create Room Menu")]
    public RectTransform createRoom;
    public TMP_InputField roomNameInput;
    public TMP_InputField playerNumInput;
    public Button createButton;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
    }



}
