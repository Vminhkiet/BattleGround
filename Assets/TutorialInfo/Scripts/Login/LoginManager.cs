using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth ready.");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + status);
            }
        });
    }

    public void Register(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Registration Failed: " + task.Exception);
                return;
            }

            user = task.Result.User;
            Debug.Log("User registered: " + user.Email);
        });
    }

    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Login Failed: " + task.Exception);
                return;
            }

            user = task.Result.User;
            Debug.Log("User logged in: " + user.Email);
        });
    }
}
