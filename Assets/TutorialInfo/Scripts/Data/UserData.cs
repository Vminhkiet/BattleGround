using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class UserData
{
    public string username;
    public string email;
    public int money;
    public int score;
    public List<string> charactersOwned;
    public List<string> spellsOwned;
    public string characterSelected;
    public string spellSelected;
    public Timestamp createdAt;

    public static UserData FromDictionary(Dictionary<string, object> data)
    {
        return new UserData
        {
            username = data["username"].ToString(),
            email = data["email"].ToString(),
            money = int.Parse(data["money"].ToString()),
            score = int.Parse(data["score"].ToString()),
            charactersOwned = ConvertToStringList(data["charactersOwned"]),
            spellsOwned = ConvertToStringList(data["spellsOwned"]),
            characterSelected = data["characterSelected"].ToString(),
            spellSelected = data["spellSelected"].ToString(),
            createdAt = (Timestamp)data["createdAt"]
        };
    }

    private static List<string> ConvertToStringList(object listObj)
    {
        List<object> objList = listObj as List<object>;
        List<string> stringList = new List<string>();
        foreach (object o in objList)
        {
            stringList.Add(o.ToString());
        }
        return stringList;
    }
}
