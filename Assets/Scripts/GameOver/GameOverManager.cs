using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOverManager : MonoBehaviour
{
    public GameObject[] _gameOverScene;
    private float[] deLay = {1f, 1f, 1f, 0.05f, 1f};
    public TextMeshProUGUI scoreText;
    private float score;
    private int index;
    void Start()
    {
        StartCoroutine(GameOverAction());
        score = AudioManagerScript.currentScore;
        AudioManagerScript.Instance.PlayBgm(5);
        FinalScore();
    }

    private void FinalScore()
    {
        scoreText.text = "Final Score : " + Mathf.FloorToInt(score);
    }

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
        SceneManager.LoadScene("Loading");
    }
    
    public IEnumerator GameOverAction()
    {
        for (index = 0; index < 5; index++)
        {
            yield return new WaitForSeconds(deLay[index]);
            _gameOverScene[index].SetActive(true);
        }
    }
}
