using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }
    public UserData userData;
    private event Action OnUserDataLoaded;

    private FirebaseFirestore db;
    private string uid;

    private List<Action> subscribers = new List<Action>();

    public void Subscribe(Action action)
    {
        OnUserDataLoaded += action;
        subscribers.Add(action);
    }
    public void UnsubscribeAll()
    {
        foreach (var s in subscribers)
            OnUserDataLoaded -= s;

        subscribers.Clear();
    }
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

    private void Start()
    {
        uid = PlayerPrefs.GetString("currentUID", null);

        if (string.IsNullOrEmpty(uid))
        {
            uid = "fake_uid_001"; // Thiết lập UID giả nếu cần
            PlayerPrefs.SetString("currentUID", uid);
        }

        // Tạo dữ liệu giả cho user
        Dictionary<string, object> fakeData = new Dictionary<string, object>
     {
         { "username", "vo_kiet_fake" },
         { "email", "kiet.fake@gmail.com" },
         { "money", 9999 },
         { "score", 999 },
         { "charactersOwned", new List<object> { "KAVENT", "ALIA" } },
         { "spellsOwned", new List<object> { "fireball", "heal", "dash" } },
         { "characterSelected", "ALIA" },
         { "spellSelected", "dash" },
         { "createdAt", Timestamp.GetCurrentTimestamp() }
     };

        userData = UserData.FromDictionary(fakeData);
        Debug.Log("🧪 Đang sử dụng dữ liệu giả: " + userData.username);
        StartCoroutineFetchData();
    }
    
    public void StartCoroutineFetchData()
    {
        StartCoroutine(fetchdt());
    }
    IEnumerator fetchdt()
    {
        yield return new WaitForSeconds(1f);

        OnUserDataLoaded?.Invoke();
    }

    public async Task LoadUserDataAsync(string uid)
    {
        DocumentSnapshot snapshot = await db.Collection("users").Document(uid).GetSnapshotAsync();

        if (!snapshot.Exists)
            throw new Exception("User document not found.");

        userData = UserData.FromDictionary(snapshot.ToDictionary());
        OnUserDataLoaded?.Invoke();
    }

    public async Task UpdateUserDataAsync()
    {
        if (userData == null || string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("No user data to update.");
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "username", userData.username },
            { "email", userData.email },
            { "money", userData.money },
            { "score", userData.score },
            { "charactersOwned", userData.charactersOwned },
            { "spellsOwned", userData.spellsOwned },
            { "characterSelected", userData.characterSelected },
            { "spellSelected", userData.spellSelected },
            { "createdAt", userData.createdAt }
        };

        await db.Collection("users").Document(uid).SetAsync(data);
        Debug.Log("User data updated.");
    }

    public async Task UpdateFieldsAsync(Dictionary<string, object> fieldsToUpdate)
    {
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("UID missing. Cannot update.");
            return;
        }

        await db.Collection("users").Document(uid).UpdateAsync(fieldsToUpdate);
        Debug.Log("User fields updated: " + string.Join(", ", fieldsToUpdate.Keys));
    }

    public async Task<bool> PurchaseCharacterAsync(string characterId, int price)
    {
        // 1. Kiểm tra phía client trước để có phản hồi ngay lập tức
        if (userData.money < price)
        {
            Debug.Log("Not enough money to buy character " + characterId);
            return false;
        }

        try
        {
            // 2. Tạo một dictionary để cập nhật các trường trên Firestore
          /*  var updates = new Dictionary<string, object>
            {
                // Dùng FieldValue.Increment để trừ tiền một cách an toàn trên server
                { "money", FieldValue.Increment(-price) },
                // Dùng FieldValue.ArrayUnion để thêm nhân vật vào danh sách, tránh trùng lặp
                { "charactersOwned", FieldValue.ArrayUnion(characterId) }
            };

        //    await UpdateFieldsAsync(updates);

            // 3. Cập nhật lại dữ liệu trên local sau khi server xác nhận thành công
       
            if (!userData.charactersOwned.Contains(characterId))
            {
                userData.charactersOwned.Add(characterId);
            }

            Debug.Log("Successfully purchased character: " + characterId);*/
            OnUserDataLoaded?.Invoke();
            userData.money -= price;
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to purchase character: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Xử lý việc mua một phép thuật mới.
    /// </summary>
    public async Task<bool> PurchaseSpellAsync(string spellId, int price)
    {
        if (userData.money < price)
        {
            Debug.Log("Not enough money to buy spell " + spellId);
            return false;
        }

        try
        {
            var updates = new Dictionary<string, object>
            {
                { "money", FieldValue.Increment(-price) },
                { "spellsOwned", FieldValue.ArrayUnion(spellId) }
            };

            await UpdateFieldsAsync(updates);

            userData.money -= price;
            if (!userData.spellsOwned.Contains(spellId))
            {
                userData.spellsOwned.Add(spellId);
            }

            Debug.Log("Successfully purchased spell: " + spellId);
            OnUserDataLoaded?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to purchase spell: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Cập nhật nhân vật đang được chọn.
    /// </summary>
    public async Task SelectCharacterAsync(string characterId)
    {
        // Kiểm tra xem người dùng có thực sự sở hữu nhân vật này không
        if (!userData.charactersOwned.Contains(characterId))
        {
            Debug.LogError($"Attempted to select character '{characterId}' which is not owned.");
            return;
        }

        // Nếu đã chọn rồi thì không cần làm gì cả
        if (userData.characterSelected == characterId) return;

        try
        {
            await UpdateFieldsAsync(new Dictionary<string, object> { { "characterSelected", characterId } });

            // Cập nhật local data
            userData.characterSelected = characterId;
            Debug.Log("Selected character: " + characterId);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to select character: " + ex.Message);
        }
    }

    /// <summary>
    /// Cập nhật phép thuật đang được chọn.
    /// </summary>
    public async Task SelectSpellAsync(string spellId)
    {
        if (!userData.spellsOwned.Contains(spellId))
        {
            Debug.LogError($"Attempted to select spell '{spellId}' which is not owned.");
            return;
        }

        if (userData.spellSelected == spellId) return;

        try
        {
            await UpdateFieldsAsync(new Dictionary<string, object> { { "spellSelected", spellId } });
            userData.spellSelected = spellId;
            Debug.Log("Selected spell: " + spellId);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to select spell: " + ex.Message);
        }
    }

}

