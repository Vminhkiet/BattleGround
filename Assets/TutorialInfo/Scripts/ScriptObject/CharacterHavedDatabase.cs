using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterHavedDatabase", menuName = "Haved/ Character Haved Database")]
public class CharacterHavedDatabase : ScriptableObject
{
    public List<CharacterType> characters = new List<CharacterType>();

    public int CharactersCount
    {
        get { return characters.Count; }
    }

    public void AddCharacter(CharacterType type)
    {
        characters.Add(type);
    }

    public bool checkNameCharacter(CharacterType type)
    {
        return characters.Contains(type);
    }
}
