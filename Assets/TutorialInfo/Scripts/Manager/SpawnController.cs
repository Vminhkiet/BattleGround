using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;
    [SerializeField]
    private Transform posSpawn;
    private GameObject playerSpawn;
    public CharacterHavedDatabase items;
    public InfoPlayerDatabase playerInfo;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        UserSession.Instance.OnUserDataLoaded += LoadSelectedPlayer;
    }

    void LoadSelectedPlayer()
    {
        SpawnPlayer(UserSession.Instance.userData.characterSelected);
    }    

    public SpawnController getInstance()
    {
        if(instance == null) instance = this;
        return instance;
    }

    public void SpawnPlayer(string characterName)
    {
        GameObject characterPrefab = Resources.Load<GameObject>($"Character/{characterName}");

        if (characterPrefab == null)
        {
            Debug.LogError($"Character prefab not found for name: {characterName}");
            return;
        }

        if (playerSpawn != null)
        {
            Destroy(playerSpawn);
        }

        playerSpawn = Instantiate(characterPrefab, posSpawn);
    }

    public void AddCharacter(CharacterType type)
    {
        items.AddCharacter(type);
    }
}
