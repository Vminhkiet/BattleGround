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
        SpawnPlayer(playerInfo.getCharacterType());
    }
    public SpawnController getInstance()
    {
        if(instance == null) instance = this;
        return instance;
    }

    public void SpawnPlayer(CharacterType type)
    {
        GameObject characterPrefab = Resources.Load<GameObject>($"Character/{type}");

        if (playerSpawn != null)
        {
            Destroy(playerSpawn);
            playerSpawn = Instantiate(characterPrefab, posSpawn);
        }
        else playerSpawn = Instantiate(characterPrefab, posSpawn);
    }

    public void AddCharacter(CharacterType type)
    {
        items.AddCharacter(type);
    }
}
