using UnityEngine;
using Firebase;
using Firebase.Firestore;

[FirestoreData]

public struct UserData
{
    [FirestoreProperty]
    public string userID {  get; set; }

    [FirestoreProperty]
    public string userName { get; set; }
}

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    [SerializeField] private string _filePath = "";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SetData()
    {
        var userData = new UserData
        {
            userID = AuthManager.instance.User.UserId,
            userName = AuthManager.instance.User.DisplayName
        };

        var firestore = FirebaseFirestore.DefaultInstance;
        firestore.Document(_filePath).SetAsync(userData);
    }
}
