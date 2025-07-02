using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;

    private FirebaseAuth auth;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth Initialized.");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void OnLoginButtonPressed()
    {
        // B? qua Firebase, s? d?ng d? li?u gi?
        string fakeUID = "fake_uid_001";
        PlayerPrefs.SetString("currentUID", fakeUID);

        Debug.Log("?? ??ng nh?p gi? thành công v?i UID: " + fakeUID);
        feedbackText.text = "Login (fake) successful!";
        SceneManager.LoadScene("Main");
        //if (emailInput == null  passwordInput == null  feedbackText == null)
        //{
        //    Debug.LogError("? M?t trong các UI component ch?a ???c gán!");
        //    return;
        //}
        //string email = emailInput.text.Trim();
        //string password = passwordInput.text;

        //if (string.IsNullOrEmpty(email)  string.IsNullOrEmpty(password))
        //{
        //    feedbackText.gameObject.SetActive(true);
        //    feedbackText.text = "Email and password must not be empty.";
        //    return;
        //}

        //auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
        //    if (task.IsCanceled  task.IsFaulted)
        //    {
        //        Debug.LogError("Login failed: " + task.Exception);
        //        feedbackText.gameObject.SetActive(true);
        //        feedbackText.text = "Login failed. Check your email and password.";
        //        return;
        //    }

        //    FirebaseUser user = task.Result.User;
        //    Debug.Log("User logged in: " + user.UserId);

        //    PlayerPrefs.SetString("currentUID", user.UserId);

        //    feedbackText.text = "Login successful!";
        //    SceneManager.LoadScene("Main");
        //});
    }
}
