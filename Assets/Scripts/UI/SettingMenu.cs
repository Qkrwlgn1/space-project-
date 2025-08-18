using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
public class SettingMenu : MonoBehaviour
{
    [Header("Score")]
    public static float finalScore;
    public float scorePerSecond = 1;
    public TextMeshProUGUI scoreText;

    [Header("Menu")]
    public GameObject[] _menu;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetActiveMenu("EscMenu");
        }
    }

    void Start()
    {
        scoreText.gameObject.SetActive(false);
        AudioManagerScript.currentScore = 0f;
    }


    void FixedUpdate()
    {
        if (GameManager.instance.isGameStarted)
        {
            AudioManagerScript.Instance.AddScore(scorePerSecond);
        }
    }
    

    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void SetActiveMenu(string name)
    {
        AudioManagerScript.Instance.PlayUISource();
        switch (name)
        {
            case "Desc":
                if (GameManager.instance.isGameStarted && _menu[2].activeInHierarchy)
                {
                    GameManager.instance.Stop();
                    _menu[2].SetActive(false);
                    _menu[0].SetActive(true);
                }
                else
                {
                    _menu[0].SetActive(true);
                }
                break;
            case "Setting":
                if (GameManager.instance.isGameStarted && _menu[2].activeInHierarchy)
                {
                    GameManager.instance.Stop();
                    _menu[2].SetActive(false);
                    _menu[1].SetActive(true);
                }
                else
                {
                    _menu[1].SetActive(true);
                }
                break;
            case "EscMenu":
                if (GameManager.instance.isGameStarted)
                {
                    GameManager.instance.Stop();
                    _menu[2].SetActive(true);
                }
                break;
            case "ExitGame":
                Application.Quit();
                break;
            case "Title":
                _menu[4].SetActive(true);
                StartCoroutine(GoTitle());
                break;
        }

    }
    public void DisActiveMenu(string name)
    {
        switch (name)
        {
            case "Desc":
                if (GameManager.instance.isGameStarted && _menu[0].activeInHierarchy)
                {
                    _menu[0].SetActive(false);
                    _menu[2].SetActive(true);
                }
                else
                {
                    _menu[0].SetActive(false);
                }
                break;
            case "Setting":
                if (GameManager.instance.isGameStarted && _menu[1].activeInHierarchy)
                {
                    _menu[1].SetActive(false);
                    _menu[2].SetActive(true);
                }
                else
                {
                    _menu[1].SetActive(false);
                }
                break;
            case "EscMenu":
                _menu[2].SetActive(false);
                GameManager.instance.Resume();
                break;
        }
    }

    public void GameOverMotion()
    {
        _menu[3].SetActive(true);
    }

    public IEnumerator GoTitle()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        SceneManager.LoadScene("Loading");
        GameManager.instance.Resume();
    }
}
