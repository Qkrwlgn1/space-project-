using UnityEngine;
using UnityEngine.Audio;
public class SettingMenu : MonoBehaviour
{
    public GameObject[] _menu;

    public AudioMixer audioMixer;

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
                    _menu[0].SetActive(true);
                    break;
                case "Setting":
                    _menu[1].SetActive(true);
                    break;
            }
    }
    public void DisActiveMenu(string name)
    {
        switch (name)
            {
                case "Desc":
                    _menu[0].SetActive(false);
                    break;
                case "Setting":
                    _menu[1].SetActive(false);
                    break;
            }
    }

}
