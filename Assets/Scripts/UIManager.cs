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
    public GameObject TeacherObject;

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
            if (child.name == "WarningText")
                child.GetComponent<TMP_Text>().text = "";
        }
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void Back()
    {
        if (RegisterPanel.activeSelf == true)
        {
            for (int i = 0; i < RegisterPanel.transform.childCount; i++)
            {
                GameObject child = RegisterPanel.transform.GetChild(i).gameObject;
                if (child.GetComponent<TMP_InputField>() != null)
                    child.GetComponent<TMP_InputField>().text = "";
                if (child.name == "WarningText")
                    child.GetComponent<TMP_Text>().text = "";
            }
            RegisterPanel.SetActive(false);
        }
        else if(LoggedInPanel.activeSelf == true)
        {
            for (int i = 0; i < LoggedInPanel.transform.childCount; i++)
            {
                GameObject child = LoggedInPanel.transform.GetChild(i).gameObject;
                if (child.name == "UserInfo")
                    child.GetComponent<TMP_Text>().text = "";
            }
            if(TeacherObject.activeSelf == true)
            {
                for (int i = 0; i < TeacherObject.transform.childCount; i++)
                {
                    GameObject child = TeacherObject.transform.GetChild(i).gameObject;
                    if (child.GetComponent<TMP_InputField>() != null)
                        child.GetComponent<TMP_InputField>().text = "";
                    if (child.name == "WarningText")
                        child.GetComponent<TMP_Text>().text = "";
                }
            }
            LoggedInPanel.SetActive(false);
        }
        LoginPanel.SetActive(true);
    }

    public void LoggedInScreen()
    {
        for (int i = 0; i < LoginPanel.transform.childCount; i++)
        {
            GameObject child = LoginPanel.transform.GetChild(i).gameObject;
            if (child.GetComponent<TMP_InputField>() != null)
                child.GetComponent<TMP_InputField>().text = "";
            if (child.name == "WarningText")
            {
                child.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            }

        }
        LoggedInPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    public void SetTeacherObject(bool active)
    {
        TeacherObject.SetActive(active);
    }
}
