using System.Collections;
using UnityEngine;
using DG.Tweening;
using Infrastructure;
using Infrastructure.States.GameStates;
using Utilities.Constants;
using Infrastructure.Services;
using UnityEngine.SceneManagement;

public class CLoadingScreen : MonoBehaviour
{
    #region 변수
    [Space(10f)]
    [SerializeField]
    private CanvasGroup[] _panelsToShow;

    private const float MinValue = 0f;
    private const float MaxValue = 1f;
    private const float TimeToFade = 0.8f;
    private const float TimeBetweenPanels = 3f;
    #endregion

    private void Start()
    {
        StartCoroutine(nameof(ShowPanels));
    }
    /// <summary>
    /// 로딩 화면 보여주는 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowPanels()
    {
        for (int i = 0; i < _panelsToShow.Length; i++)
        {
            if (i > 0)
            {
                _panelsToShow[i].DOFade(MaxValue, TimeToFade);
            }

            if (i == _panelsToShow.Length - 1)
            {
                yield return new WaitForSeconds(TimeToFade);

                SceneLoader sceneLoader = AllServices.Container.Single<SceneLoader>();
                if (sceneLoader != null)
                {
                    sceneLoader.Load(SceneNames.LobbyScene, GameBootstrapper.Instance.StateMachine.Enter<LobbyState>);
                }

                yield return null;
            }

            yield return new WaitForSeconds(TimeBetweenPanels);


            _panelsToShow[i].DOFade(MinValue, TimeToFade);
            yield return new WaitForSeconds(TimeToFade);
        }
        SceneManager.LoadScene("SingleLobby");
    }
}