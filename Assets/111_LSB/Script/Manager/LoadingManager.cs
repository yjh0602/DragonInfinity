using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =================== LOADING MANAGER CLASS ==========================================
// �� ��ȯ ��û�� �������� �ε� ���൵�� �����ִ� Ŭ����
// =====================================================================================

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;  // �ε� ȭ�� ���൵ �����̴�
    static string nextSceneName;            // ��ȯ ��û�� ���� ���� �̸�

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
