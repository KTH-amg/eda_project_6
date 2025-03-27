using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 관리를 위해 필요
using TMPro;

public class LoadResult : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    private string stock_name;
    
    private void Start()
    {
        // 이 스크립트가 붙어있는 게임오브젝트의 부모를 DontDestroyOnLoad로 설정
        DontDestroyOnLoad(gameObject);
    }

    public void OnClickLoadResult()
    {
        stock_name = dropdown.options[dropdown.value].text;
        SceneManager.LoadScene("ResultScene");  // ResultScene으로 이동
    }

    public string GetStockName()
    {
        string tempStockName = stock_name;
        Destroy(gameObject);  // 부모 오브젝트를 파괴
        return tempStockName;
    }
    
}