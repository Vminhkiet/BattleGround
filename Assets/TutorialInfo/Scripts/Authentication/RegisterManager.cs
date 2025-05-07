using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;

public class RegisterManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TMP_Text feedbackText;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase initialized.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies.");
            }
        });
    }

    public void OnRegisterButtonClicked()
    {

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string username = usernameInput.text.Trim().ToLower();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "Please fill all fields.";
            return;
        }

        CheckUsernameAvailability(email, password, username);
    }

    private void CheckUsernameAvailability(string email, string password, string username)
    {
        DocumentReference usernameRef = db.Collection("usernames").Document(username);
        usernameRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.Result.Exists)
            {
                feedbackText.text = "Username is already taken.";
            }
            else
            {
                RegisterNewUser(email, password, username);
            }
        });
    }

    private void RegisterNewUser(string email, string password, string username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                feedbackText.text = "Registration failed: " + task.Exception?.Message;
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("User created: " + newUser.UserId);

            newUser.SendEmailVerificationAsync().ContinueWithOnMainThread(verifyTask => {
                if (verifyTask.IsCompleted)
                {
                    Debug.Log("Verification email sent.");
                    feedbackText.text = "Verification email sent! Check your inbox.";
                }
            });

            StoreUserData(newUser.UserId, email, username);
        });
    }

    private void StoreUserData(string uid, string email, string username)
    {
        // Save user info in "users"
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", email },
            { "username", username },
            { "createdAt", Timestamp.GetCurrentTimestamp() }
        };

        db.Collection("users").Document(uid).SetAsync(userData);

        // Save username mapping
        db.Collection("usernames").Document(username).SetAsync(new Dictionary<string, object>
        {
            { "uid", uid }
        });

        Debug.Log("User data saved to Firestore.");
    }
}
