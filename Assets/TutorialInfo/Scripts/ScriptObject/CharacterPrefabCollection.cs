using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacterPrefabCollection", menuName = "Game Data/Character Prefab Collection")]
public class CharacterPrefabCollection : ScriptableObject
{
    [Tooltip("Danh sách các loại nhân vật và đường dẫn Prefab tương ứng.")]
    public List<CharacterPrefabEntry> characterPrefabEntries = new List<CharacterPrefabEntry>();

    private Dictionary<CharacterType, string> _prefabPathsDictionary;

    public Dictionary<CharacterType, string> GetPrefabPathsDictionary()
    {
        if (_prefabPathsDictionary == null || _prefabPathsDictionary.Count == 0 && characterPrefabEntries.Count > 0)
        {
            _prefabPathsDictionary = new Dictionary<CharacterType, string>();
            foreach (CharacterPrefabEntry entry in characterPrefabEntries)
            {
                if (_prefabPathsDictionary.ContainsKey(entry.characterType))
                {
                    Debug.LogWarning($"Trùng lặp CharacterType '{entry.characterType}' trong CharacterPrefabCollection. Chỉ mục đầu tiên sẽ được sử dụng.");
                    continue;
                }
                _prefabPathsDictionary.Add(entry.characterType, entry.prefabPath);
            }
        }
        return _prefabPathsDictionary;
    }

    void OnEnable()
    {
        _prefabPathsDictionary = null;

        if (characterPrefabEntries.Count == 0)
        {
            Debug.Log("Initializing CharacterPrefabCollection with default entries.");
            characterPrefabEntries.Add(new CharacterPrefabEntry { characterType = CharacterType.KAVENT, prefabPath = "Players/PivotCharacter" });
            characterPrefabEntries.Add(new CharacterPrefabEntry { characterType = CharacterType.ALIA, prefabPath = "Players/PivotCharacterMage" });

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    public string GetPrefabPath(CharacterType type)
    {
        if (GetPrefabPathsDictionary().ContainsKey(type))
        {
            return GetPrefabPathsDictionary()[type];
        }
        return null;
    }
}