using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public LoginManager authManager;

    public void OnLoginPressed()
    {
        authManager.Login(emailField.text, passwordField.text);
    }

    public void OnRegisterPressed()
    {
        authManager.Register(emailField.text, passwordField.text);
    }
}
