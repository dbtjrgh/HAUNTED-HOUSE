using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

namespace Michsky.UI.Dark
{
    public class MainPanelManager : MonoBehaviour
    {
        // List
        public List<PanelItem> panels = new List<PanelItem>();

        // Settings
        public bool settingsHelper;
        public bool editMode;
        public bool instantInOnEnable;
        public int currentPanelIndex = 0;
        private int currentButtonIndex = 0;
        private int newPanelIndex;
        [Range(0.75f, 4)] public float disablePanelAfter = 1;
        [Range(0, 1)] public float animationSmoothness = 0.25f;
        [Range(0.75f, 4)] public float animationSpeed = 1;

        // Hidden vars
        private GameObject currentPanel;
        private GameObject nextPanel;
        private GameObject currentButton;
        private GameObject nextButton;

        private Animator currentPanelAnimator;
        private Animator nextPanelAnimator;
        private Animator currentButtonAnimator;
        private Animator nextButtonAnimator;

        // Animator state vars
        public string panelFadeIn = "Panel In";
        public string panelFadeOut = "Panel Out";
        public string panelFadeOutHelper = "Panel Out Helper";
        public string panelInstantIn = "Instant In";
        public string buttonFadeIn = "Hover to Pressed";
        public string buttonFadeOut = "Pressed to Normal";
        public string buttonFadeNormal = "Pressed to Normal";

        bool firstTime = true;
        [HideInInspector] public bool gamepadEnabled = false;

        [Serializable]
        public class PanelItem
        {
            public string panelName = "My Panel";
            public GameObject panelObject;
            public GameObject panelButton;
            public GameObject defaultSelected;
        }

        void OnEnable()
        {
            if (panels[currentPanelIndex].panelButton != null)
            {
                currentButton = panels[currentPanelIndex].panelButton;
                currentButtonAnimator = currentButton.GetComponent<Animator>();
                currentButtonAnimator.Play(buttonFadeIn);
            }

            currentPanel = panels[currentPanelIndex].panelObject;
            currentPanel.SetActive(true);
            currentPanelAnimator = currentPanel.GetComponent<Animator>();

            if (instantInOnEnable == true && currentPanelAnimator.gameObject.activeInHierarchy == true)
                currentPanelAnimator.Play(panelInstantIn);
            else if (instantInOnEnable == false && currentPanelAnimator.gameObject.activeInHierarchy == true)
                currentPanelAnimator.Play(panelFadeIn);

            firstTime = false;

            for (int i = 0; i < panels.Count; i++)
            {
                if (i != currentPanelIndex)
                    panels[i].panelObject.SetActive(false);
            }
        }

        public void EnableFirstPanel()
        {
            try
            {
                panels[currentPanelIndex].panelObject.GetComponent<Animator>().Play("Instant In");
                panels[currentPanelIndex].panelButton.GetComponent<Animator>().Play("Instant In");
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(panels[currentPanelIndex].panelObject.GetComponent<RectTransform>());
            }

            catch { }
        }


        public void GameStart()
        {
            SceneManager.LoadScene("Boot");
        }

    }
}