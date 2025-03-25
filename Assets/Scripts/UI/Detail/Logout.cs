using UnityEngine;
using UnityEngine.SceneManagement;
using user;

public class Logout : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickLogout()
    {
        User.Instance.delUser();
        // Login 씬으로 전환
        SceneManager.LoadScene("Login");
    }
}
