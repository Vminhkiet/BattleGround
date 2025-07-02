using Firebase.Firestore;
using System;
using UnityEngine;
using System.Threading.Tasks;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }
    public UserData userData;
    public event Action OnUserDataLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        string uid = PlayerPrefs.GetString("currentUID", null);

        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("No UID found. User probably bypassed login.");
            return;
        }

        try
        {
            await LoadUserDataAsync(uid);
            Debug.Log("User data loaded. Welcome, " + userData.username);           
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load user data: " + ex.Message);
        }
    }

    public async Task LoadUserDataAsync(string uid)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentSnapshot snapshot = await db.Collection("users").Document(uid).GetSnapshotAsync();

        if (!snapshot.Exists)
            throw new Exception("User document not found.");

        userData = UserData.FromDictionary(snapshot.ToDictionary());
        OnUserDataLoaded?.Invoke();
        return;
    }
}
