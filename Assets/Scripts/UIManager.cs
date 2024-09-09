using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    // Panel UI Variables
    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject LoggedInPanel;

    private void Awake()
    {
        if(instance == null )
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Register()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void Back()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void LoggedInScreen()
    {
        LoggedInPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }
}
