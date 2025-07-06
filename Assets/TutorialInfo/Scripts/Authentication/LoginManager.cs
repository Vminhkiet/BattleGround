using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;

    private FirebaseAuth auth;

    void Start()
    {
        if (FirebaseInitializer.Instance.IsReady)
        {
            InitFirebase();
        }
        else
        {
            FirebaseInitializer.Instance.OnFirebaseReady += InitFirebase;
        }
    }

    void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("RegisterManager: Firebase is ready");
    }


    public void OnLoginButtonPressed()
    {
        // B? qua Firebase, s? d?ng d? li?u gi?
      /*  string fakeUID = "fake_uid_001";
        PlayerPrefs.SetString("currentUID", fakeUID);

        Debug.Log("?? ??ng nh?p gi? thành công v?i UID: " + fakeUID);
        feedbackText.text = "Login (fake) successful!";
        SceneManager.LoadScene("Main");*/

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "Email and password must not be empty.";
            return;
        }

        if (auth == null)
        {
            Debug.Log("null>>>>>>>>");
        }
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Login failed: " + task.Exception);
                feedbackText.gameObject.SetActive(true);
                feedbackText.text = "Login failed. Check your email and password.";
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("User logged in: " + user.UserId);

            PlayerPrefs.SetString("currentUID", user.UserId);

            feedbackText.text = "Login successful!";
            SceneManager.LoadScene("Main");
        });
    }
}
