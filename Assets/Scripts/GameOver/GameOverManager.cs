using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameOverManager : MonoBehaviour
{
    public void ExitGame()
    {
        StartCoroutine(GoToExitGame(1f));
    }

    public void GoTitle()
    {
        StartCoroutine(GoToTitle(1f));
    }
    IEnumerator GoToExitGame(float time)
    {
        yield return new WaitForSeconds(time);

        Application.Quit();
    }
    
    IEnumerator GoToTitle(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene("MainScene");
    }
}
