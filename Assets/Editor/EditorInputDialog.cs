using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class EditorInputDialog : EditorWindow
{
    string description, inputText;
    string okButton, cancelButton;
    System.Action<string> onOK;
    bool initialized = false;

    public static string Show(string title, string description, string defaultText = "", string okButton = "OK", string cancelButton = "Cancel")
    {
        string result = null;
        EditorInputDialog window = GetWindow<EditorInputDialog>(true, title, true); // true for utility window
        window.description = description;
        window.inputText = defaultText;
        window.okButton = okButton;
        window.cancelButton = cancelButton;
        window.onOK = (text) => result = text;
        window.ShowModal(); // Sử dụng ShowModal để nó chặn tương tác cho đến khi đóng
        return result;
    }

    void OnGUI()
    {
        if (!initialized)
        {
            // Set the window size (optional, adjust as needed)
            minSize = new Vector2(350, 150);
            maxSize = new Vector2(350, 150);
            initialized = true;
        }
        EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        inputText = EditorGUILayout.TextField(inputText);
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(okButton))
        {
            if (onOK != null) onOK(inputText);
            Close();
        }
        if (GUILayout.Button(cancelButton))
        {
            Close(); // Result sẽ vẫn là null
        }
        GUILayout.EndHorizontal();
    }
}