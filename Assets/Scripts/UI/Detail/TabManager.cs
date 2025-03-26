using UnityEngine;
using UnityEngine.UI;
using TMPro;
using user;

public class TabManager : MonoBehaviour
{
    [SerializeField] public GameObject onPickedEffect; // 선택 효과
    [SerializeField] private GameObject dashboardComponent;
    [SerializeField] private GameObject selectComponent;
    [SerializeField] private GameObject mystockComponent;
    [SerializeField] private GameObject addStockPopup; // 종목 추가 팝업 UI
    [SerializeField] private TMP_Dropdown stockDropdown; // 종목 선택 드롭다운
    [SerializeField] private TMP_InputField purchasePriceInput; // 매수가 입력 필드
    [SerializeField] private TMP_InputField numOfStockInput;    // 주식 수량 입력 필드

    private Vector3[] tabPositions = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, -110, 0),
        new Vector3(0, -220, 0)
    };

    // 선택된/선택되지 않은 텍스트 색상
    private Color selectedColor = new Color32(0x64, 0xFF, 0xDA, 0xFF); // 0x80은 255의 50%인 128
    private Color unselectedColor = new Color(0.5f, 0.5f, 0.5f); // 회색

    // 종목 추가 이벤트를 위한 델리게이트와 이벤트 선언
    public delegate void StockAddedEventHandler(string stockName, string purchasePrice, string numOfStock);
    public static event StockAddedEventHandler OnStockAdded;

    void Start()
    {
        // 시작할 때 모든 텍스트 색상을 unselectedColor로 초기화
        SetTabTextColors(0);  // 0번(Dashboard) 탭이 선택된 상태로 시작

        if (onPickedEffect != null)
        {
            onPickedEffect.SetActive(true);
            // 컴포넌트 활성화/비활성화
            dashboardComponent.SetActive(true);
            selectComponent.SetActive(false);
            mystockComponent.SetActive(false);
            // 이펙트 이동
            MoveEffectToTab(0);
        }
    }

    private void SetTabTextColors(int selectedTab)
    {
        try 
        {
            var dashboardText = GameObject.Find("dashboard_txt");
            var selectText = GameObject.Find("select_txt");
            var mystockText = GameObject.Find("mystock_txt");

            var dashboardTMP = dashboardText.GetComponent<TMP_Text>();
            var selectTMP = selectText.GetComponent<TMP_Text>();
            var mystockTMP = mystockText.GetComponent<TMP_Text>();

            // 모든 텍스트를 60% 불투명도의 흰색으로 초기화 (alpha: 153)
            Color defaultColor = new Color32(255, 255, 255, 153);
            dashboardTMP.color = defaultColor;
            selectTMP.color = defaultColor;
            mystockTMP.color = defaultColor;

            // 선택된 탭의 텍스트만 색상 변경
            switch(selectedTab)
            {
                case 0:
                    dashboardTMP.color = selectedColor;
                    break;
                case 1:
                    selectTMP.color = selectedColor;
                    break;
                case 2:
                    mystockTMP.color = selectedColor;
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in SetTabTextColors: {e.Message}");
        }
    }

    public void OnClickDashboardTab()
    {
        MoveEffectToTab(0);
        SetTabTextColors(0);
        dashboardComponent.SetActive(true);
        selectComponent.SetActive(false);
        mystockComponent.SetActive(false);
    }

    public void OnClickSelectTab()
    {
        MoveEffectToTab(1);
        SetTabTextColors(1);
        dashboardComponent.SetActive(false);
        selectComponent.SetActive(true);
        mystockComponent.SetActive(false);
    }

    public void OnClickMyStockTab()
    {
        MoveEffectToTab(2);
        SetTabTextColors(2);
        dashboardComponent.SetActive(false);
        selectComponent.SetActive(false);
        mystockComponent.SetActive(true);
    }

    private void MoveEffectToTab(int tabIndex)
    {
        if (onPickedEffect != null)
        {
            onPickedEffect.GetComponent<RectTransform>().anchoredPosition = tabPositions[tabIndex];
        }
    }

    // 팝업 활성화
    public void OnClickAddButton()
    {
        if (addStockPopup != null)
        {
            addStockPopup.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Add Stock Popup is not assigned!");
        }
    }

    // 팝업 비활성화
    public void OnClickClosePopup()
    {
        if (addStockPopup != null)
        {
            addStockPopup.SetActive(false);
        }
    }

    public void OnClickAddStock()
    {
        if (User.Instance == null)
        {
            Debug.LogWarning("User instance is not initialized!");
            return;
        }

        if (addStockPopup != null && stockDropdown != null)
        {
            string selectedStock = stockDropdown.options[stockDropdown.value].text;
            string purchasePrice = purchasePriceInput.text;
            string numOfStock = numOfStockInput.text;
            
            Debug.Log($"Selected stock: {selectedStock}, Purchase price: {purchasePrice}, Number of stocks: {numOfStock}");
            User.Instance.setStock(selectedStock, purchasePrice, numOfStock);
            
            // 이벤트 호출 시 모든 정보 전달
            OnStockAdded?.Invoke(selectedStock, purchasePrice, numOfStock);
            
            OnClickClosePopup();
        }
    }
}