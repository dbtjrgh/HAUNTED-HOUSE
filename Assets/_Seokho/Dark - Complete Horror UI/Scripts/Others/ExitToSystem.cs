using UnityEngine;

namespace Michsky.UI.Dark
{
    public class ExitToSystem : MonoBehaviour
    {
        public void ExitGame()
        {
            Application.Quit();
            Debug.Log("Exit method is working in builds.");
        }
    }
}