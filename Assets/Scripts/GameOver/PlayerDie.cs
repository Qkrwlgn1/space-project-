using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerDie : MonoBehaviour
{
    public PlayerController playerCon;
    public SettingMenu menuScript;

    void Update()
    {
        if (playerCon.playerCurrentHealth <= 0)
        {
            StartCoroutine(GameOver("GameOver"));
        }
    }

    public IEnumerator GameOver(string sceneName)
    {
        yield return new WaitForSeconds(1f);

        menuScript.GameOverMotion();

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(sceneName);
    }
}
