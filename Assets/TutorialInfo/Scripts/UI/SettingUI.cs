using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [SerializeField] GameObject UI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenUI()
    {
        UI.SetActive(true);
    }
    public void CloseUI()
    {
        UI.SetActive(false);
    }
}
