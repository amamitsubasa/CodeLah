using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using TMPro.EditorUtilities;
using Firebase.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

[FirestoreData]
public struct UserData
{
    [FirestoreProperty]
    public string UserName { get; set; }

    [FirestoreProperty]
    public string Email { get; set; }

    [FirestoreProperty]
    public bool IsTeacher { get; set; }
}

[FirestoreData]
public struct ClassroomData
{
    [FirestoreProperty]
    public string ClassName { get; set; }

    [FirestoreProperty]
    public List<string> Students { get; set; }
}

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;
    private static bool validCode;
    private static UserData currUser;

    // Firebase Variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseFirestore fireStore;
    public FirebaseUser User;
    public string collection = "userData";

    // Login Variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    // Register Variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_InputField classroomCodeField;
    public TMP_Text warningRegisterText;

    // Logged In Variables
    [Header("Logged In")]
    public TMP_Text userInfoText;

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

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if(dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        fireStore = FirebaseFirestore.DefaultInstance;
    }

    public void SetData()
    {
        var userData = new UserData
        {
            UserName = User.DisplayName,
            Email = User.Email,
            IsTeacher = false
        };

        fireStore.Document(collection + "/" + User.UserId).SetAsync(userData);
    }

    public void SetClassData(string classroomCode)
    {
        List<string> students;
        DocumentReference docRef = fireStore.Collection("classroomCodes").Document(classroomCode);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var classData = task.Result.ConvertTo<ClassroomData>();
            students = classData.Students;
            students.Add(User.UserId);

            var newClassData = new ClassroomData
            {
                ClassName = classData.ClassName,
                Students = students
            };
            docRef.SetAsync(newClassData);
        });
    }

    public IEnumerator GetData()
    {
        var task = fireStore.Document(collection + "/" + User.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

            currUser = task.Result.ConvertTo<UserData>();
        });

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if(task.Exception == null)
        {
            if(currUser.IsTeacher)
            {
                userInfoText.text = "Hi Teacher, " + User.DisplayName;
            }
            else
            {
                userInfoText.text = "Hi, " + User.DisplayName;
            }
        }
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        if (classroomCodeField.text != "")
        {
            StartCoroutine(CheckClass(classroomCodeField.text));
        }
        else
        {
            StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        }

    }

    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError) firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            if(confirmLoginText.text != "")
            {
                confirmLoginText.text = "";
            }
            warningLoginText.text = message;
        }
        else
        {
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            UIManager.instance.LoggedInScreen();
            StartCoroutine(GetData());
        }
    }

    public IEnumerator CheckClass(string classCode)
    {
        DocumentReference docRef = fireStore.Collection("classroomCodes").Document(classCode);
        var task = docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                validCode = true;
                Debug.Log("true");
            }
            else
            {
                validCode = false;
                Debug.Log("false");
            }
        });

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password does not match";
        }
        else if (validCode == false && classroomCodeField.text != "")
        {
            warningRegisterText.text = "Invalid Classroom Code";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    var ProfileTask = User.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null) 
                    { 
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username set failed!";
                    }
                    else
                    {
                        SetData();
                        if (classroomCodeField.text != "")
                            SetClassData(classroomCodeField.text);
                        UIManager.instance.Back();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }
}
