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
        for (int i = 0; i < LoginPanel.transform.childCount; i++)
        {
            GameObject child = LoginPanel.transform.GetChild(i).gameObject;
            if (child.GetComponent<TMP_InputField>() != null)
                child.GetComponent<TMP_InputField>().text = "";
        }
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void Back()
    {
        for (int i = 0; i < RegisterPanel.transform.childCount; i++)
        {
            GameObject child = RegisterPanel.transform.GetChild(i).gameObject;
            if (child.GetComponent<TMP_InputField>() != null)
                child.GetComponent<TMP_InputField>().text = "";
        }
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }

    public void LoggedInScreen()
    {
        LoggedInPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }
}
