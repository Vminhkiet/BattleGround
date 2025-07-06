
using Firebase;
using Firebase.Extensions;
using UnityEngine;
using System;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance;
    public bool IsReady { get; private set; } = false;

    public event Action OnFirebaseReady;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase ready.");
                IsReady = true;
                OnFirebaseReady?.Invoke();
            }
            else
            {
                Debug.LogError("Firebase init failed: " + task.Result);
            }
        });
    }
}
