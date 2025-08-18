using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoading : MonoBehaviour
{
    public static int sceneCount = 0;
    private AsyncOperation loadOperation;

    void Start()
    {
        if (sceneCount == 1)
        {
            sceneCount = 0;
        }

        loadOperation = SceneManager.LoadSceneAsync("MainScene");
        loadOperation.allowSceneActivation = false;
        sceneCount++;

        StartCoroutine(GameLoading());
    }

    IEnumerator GameLoading()
    {
        yield return new WaitForSeconds(1f);

        GetProgress();

        yield return new WaitForSeconds(1f);

        ActivateScene();
    }

    public void ActivateScene()
    {
        loadOperation.allowSceneActivation = true;
    }

     public float GetProgress()
    {
        return Mathf.Clamp01(loadOperation.progress / 0.9f);
    }
}
