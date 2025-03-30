using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RiskDegree : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI riskText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (riskText == null)
        {
            riskText = GameObject.Find("risk_data").GetComponent<TextMeshProUGUI>();
        }

        StockDataManager.OnRiskLevelUpdated += SetRiskLevel;
    }

    private void SetRiskLevel(string riskLevel)
    {
        switch (riskLevel)
        {
            case "낮음":
                progressBar.fillAmount = 0.25f;
                progressBar.color = new Color(0, 0.7f, 1f); // 연푸른색
                break;
            case "중간":
                progressBar.fillAmount = 0.5f;
                progressBar.color = new Color(1f, 0.92f, 0.016f); // 노란색
                break;
            case "높음":
                progressBar.fillAmount = 0.75f;
                progressBar.color = new Color(1f, 0.3f, 0.3f); // 붉은색
                break;
            default:
                progressBar.fillAmount = 0f;
                break;
        }
    }

    void OnDestroy()
    {
        StockDataManager.OnRiskLevelUpdated -= SetRiskLevel;
    }
}
