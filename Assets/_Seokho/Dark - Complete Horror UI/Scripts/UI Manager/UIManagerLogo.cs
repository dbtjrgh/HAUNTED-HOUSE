#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
    [ExecuteInEditMode]
    [AddComponentMenu("Dark UI/UI Manager/UI Game Logo")]
    public class UIManagerLogo : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;
        public TextMeshProUGUI logoObject;

        [Header("Settings")]
        public bool keepAlphaValue = false;
        public bool useCustomColor = false;
        public LogoType logoType;

        public enum LogoType
        {
            BRAND,
            GAME
        }

        void Awake()
        {
            if (UIManagerAsset == null)
            {
                try
                {
                    UIManagerAsset = Resources.Load<UIManager>("Dark UI Manager");

                    this.enabled = true;

                    if (UIManagerAsset.enableDynamicUpdate == false)
                    {
                        this.enabled = false;
                    }
                }

                catch { Debug.LogWarning("No <b>UI Manager</b> variable found. Please assign it manually.", this); }
            }

            if (logoObject == null)
            {
                try { logoObject = gameObject.GetComponent<TextMeshProUGUI>(); }
                catch { }
            }
        }

        void LateUpdate()
        {
            if (UIManagerAsset == null || logoObject == null)
                return;

        }
    }
}
#endif