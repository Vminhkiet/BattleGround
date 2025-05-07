using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public Button btnSignIn;
    public Button btnSignUp;
    public Button btnToSignUp;
    public Button btnToSignIn;
    public Button btnCloseSignIn;
    public Button btnCloseSignUp;
    public GameObject signInPanel;
    public GameObject signUpPanel;

    private void Awake()
    {
        btnSignIn.onClick.AddListener(OpenSignIn);
        btnSignUp.onClick.AddListener(OpenSignUp);
        btnToSignIn.onClick.AddListener(OpenSignIn);
        btnToSignUp.onClick.AddListener(OpenSignUp);
        btnCloseSignIn.onClick.AddListener(CloseSignIn);
        btnCloseSignUp.onClick.AddListener(CloseSignUp);
    }

    void OpenSignUp ()
    {
        if(signInPanel.activeInHierarchy)
         signInPanel.SetActive(false);

        signUpPanel.SetActive(true);
    }

    void OpenSignIn()
    {
        if (signUpPanel.activeInHierarchy)
            signUpPanel.SetActive(false);
        signInPanel.SetActive(true);
    }

    void CloseSignUp()
    {
        signUpPanel.SetActive(false);
    }

    void CloseSignIn()
    {
        signInPanel.SetActive(false);
    }
}
