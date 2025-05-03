using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InfoPlayer", menuName = "Info/Info Player")]
public class InfoPlayerDatabase : ScriptableObject
{
    public string playerName;
    public int Score;
    public int Money;
    public CharacterType characterType = CharacterType.ARAST;
    private List<string> items;

    public void setMoney(int money)
    {
        Money = money;
    }
    public void setScore(int score)
    {
        Score = score;
    }
    
    public void setName(string name)
    {
        playerName = name;
    }
    
    public void setCharacterType(CharacterType type)
    {
        characterType = type;
    }
    public int getScore()
    {
        return Score;
    }
    public int getMoney()
    {
        return Money;
    }
    public string getName()
    {
        return playerName;
    }
    public CharacterType getCharacterType()
    {
        return characterType;
    }
}
