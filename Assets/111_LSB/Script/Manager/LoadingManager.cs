using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =================== LOADING MANAGER CLASS ==========================================
// 씬 전환 요청이 들어왔을때 로딩 진행도를 보여주는 클래스
// =====================================================================================

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;  // 로딩 화면 진행도 슬라이더
    static string nextSceneName;            // 전환 요청이 들어온 씬의 이름

    void Start()
    {
        UIManager.Instance.OffBackgroundUI();
        StartCoroutine(LoadSceneProgress());
    }

    public static void LoadScene(string _sceneName)
    {
        UIManager.Instance.OnBackgroundUI();
        nextSceneName = _sceneName;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation loadingProgress = SceneManager.LoadSceneAsync(nextSceneName);
        loadingProgress.allowSceneActivation = false;

        float timer = 0.0f;
        while (loadingProgress.isDone == false)
        {
            yield return null;

            if (loadingProgress.progress < 0.9f)
            {
                loadingSlider.value = loadingProgress.progress;
            }

            else
            {
                timer += Time.unscaledDeltaTime;
                loadingSlider.value = Mathf.Lerp(0.9f, 1f, timer);

                if (loadingSlider.value >= 1.0f)
                {
                    loadingProgress.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
