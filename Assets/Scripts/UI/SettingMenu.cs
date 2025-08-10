using UnityEngine;
using UnityEngine.Audio;
public class SettingMenu : MonoBehaviour
{
    public GameObject[] _menu;

    public AudioMixer audioMixer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetActiveMenu("EscMenu");
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void SetActiveMenu(string name)
    {
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
}
