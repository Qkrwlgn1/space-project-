using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public GameObject _desc;

    public void SetActiveDesc()
    {
        _desc.SetActive(true);
    }
    public void DisActiveDesc()
    {
        _desc.SetActive(false);
    }

}
