using Assets.TutorialInfo.Scripts.Login;
using Assets.TutorialInfo.Scripts.Login.ImplementLogin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Button googleLogin;
    private LoginContext _loginContext;

    // Start is called before the first frame update
    void Start()
    {
        _loginContext = new LoginContext();
        googleLogin.onClick.AddListener(() =>
        {
            _loginContext.SetLoginStrategy(new GoogleLogin());
            _loginContext.ExecuteLogin();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
