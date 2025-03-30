using UnityEngine;
using UnityEngine.UI;
using user;
using System.Collections.Generic;
using TMPro;
using stockinfo;
using System;
using stockdetail;
using System.Threading.Tasks;
public class ScrollManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;     // 스크롤뷰 컴포넌트
    [SerializeField] private GameObject groupPrefab;    // 그룹 프리팹
    [SerializeField] private GameObject viewPrefab;     // 뷰 프리팹
    
    private GameObject currentGroup;                    // 현재 활성화된 그룹
    private int itemsInCurrentGroup = 0;               // 현재 그룹에 들어있는 아이템 수
    private int totalItemsAdded = 0;
    private const int MAX_ITEMS_PER_GROUP = 2;         // 그룹당 최대 아이템 수
    private List<User.HoldingStockInfo> holdingStocks;
    private List<Toggle> stockToggles = new List<Toggle>();

    // 각 종목의 매수가와 수량을 저장할 구조체 정의
    private class StockPurchaseInfo
    {
        public int PurchasePrice { get; set; }
        public int NumberOfStocks { get; set; }
    }

    private Dictionary<string, StockPurchaseInfo> stockPurchaseInfos = new Dictionary<string, StockPurchaseInfo>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        if (User.Instance != null)
        {
            holdingStocks = User.Instance.getStock();
            if(holdingStocks != null && holdingStocks.Count > 0)
            {
                // 보유 주식 정보를 딕셔너리에 저장
                foreach (var stockInfo in holdingStocks)
                {
                    stockPurchaseInfos[stockInfo.StockName] = new StockPurchaseInfo 
                    { 
                        PurchasePrice = stockInfo.PurchasePrice,
                        NumberOfStocks = stockInfo.NumberOfStocks
                    };
                }
                
                // 각 주식에 대해 아이템 생성
                for (int i = 0; i < holdingStocks.Count; i++)
                {
                    await AddNewItem();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task AddNewItem()
    {
        if (currentGroup == null || itemsInCurrentGroup >= MAX_ITEMS_PER_GROUP)
        {
            CreateNewGroup();
        }

        string stockName = holdingStocks[totalItemsAdded].StockName;
        GameObject newView = Instantiate(viewPrefab, currentGroup.transform);
        await SetStockItemData(newView, stockName);
        
        totalItemsAdded++;
        itemsInCurrentGroup++;
    }

    private async Task SetStockItemData(GameObject stockItem, string stockName)
    {
        string targetDate = await GetMostRecentTradingDay(stockName);
        StockInfo stockInfo = new StockInfo(targetDate, targetDate);
        List<StockDetail> stock_data_arr = await stockInfo.get_stock_info(stockName);

        // 각 TMP 컴포넌트 찾기
        TextMeshProUGUI name = stockItem.transform.Find("name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI cur_price = stockItem.transform.Find("cur_price").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI my_price = stockItem.transform.Find("mine").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI margin = stockItem.transform.Find("margin").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI fluct = stockItem.transform.Find("fluc").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI attr = stockItem.transform.Find("attr").GetComponent<TextMeshProUGUI>();

        // 데이터 설정
        if (name != null) name.text = stockName;
        // 여기에 다른 데이터 설정 추가
        // 예: 현재가, 매수가, 수익률 등
        if (cur_price != null) cur_price.text = Convert.ToString(stock_data_arr[0].closing_price);

        // 매수가와 수량 정보 설정
        if (stockPurchaseInfos.ContainsKey(stockName))
        {
            var purchaseInfo = stockPurchaseInfos[stockName];
            
            if (my_price != null)
                my_price.text = purchaseInfo.PurchasePrice.ToString();
            
            // 수익률 계산
            if (margin != null)
            {
                float currentPrice = (float)stock_data_arr[0].closing_price;
                float purchasePrice = float.Parse(purchaseInfo.PurchasePrice.ToString());
                float marginRate = ((currentPrice - purchasePrice) / purchasePrice) * 100;
                string sign = marginRate >= 0 ? "+" : "";
                margin.text = $"{sign}{marginRate:F2}%";
                margin.color = marginRate >= 0 ? Color.red : new Color(0, 0.7f, 1f);
            }
            
            // 필요한 경우 수량 정보도 표시
            // TextMeshProUGUI stockAmount = stockItem.transform.Find("amount").GetComponent<TextMeshProUGUI>();
            // if (stockAmount != null)
            //     stockAmount.text = purchaseInfo.NumberOfStocks;
        }

        if (fluct != null) 
        {
            float fluctRate = stock_data_arr[0].fluctuation_rate;
            string arrow = fluctRate >= 0 ? " ▲" : " ▼";
            fluct.text = $"{fluctRate:F2}%{arrow}";
            fluct.color = fluctRate >= 0 ? Color.red : new Color(0, 0.7f, 1f);
        }

        if (attr != null) attr.text = stock_data_arr[0].abbr;

        // Toggle 컴포넌트 찾아서 리스트에 추가
        Toggle stockToggle = stockItem.GetComponentInChildren<Toggle>();
        if (stockToggle != null)
        {
            stockToggle.gameObject.SetActive(false); // 초기에는 숨김
            stockToggles.Add(stockToggle);
        }
    }

    private void CreateNewGroup()
    {
        // 스크롤뷰의 content 트랜스폼을 부모로 새 그룹 생성
        currentGroup = Instantiate(groupPrefab, scrollRect.content);
        itemsInCurrentGroup = 0;
    }

    private void OnEnable()
    {
        TabManager.OnStockAdded += RefreshStockList;
    }

    private void OnDisable()
    {
        TabManager.OnStockAdded -= RefreshStockList;
    }

    private async void RefreshStockList(string newStock, int purchasePrice, int numOfStock)
    {
        // 매수 정보 저장
        stockPurchaseInfos[newStock] = new StockPurchaseInfo 
        { 
            PurchasePrice = purchasePrice,
            NumberOfStocks = numOfStock
        };

        // 기존 그룹과 아이템들 제거
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }
        
        // 변수 초기화
        currentGroup = null;
        itemsInCurrentGroup = 0;
        totalItemsAdded = 0;
        stockToggles.Clear();

        // 리스트 다시 가져오기
        if (User.Instance != null)
        {
            holdingStocks = User.Instance.getStock();
            if(holdingStocks != null && holdingStocks.Count > 0)
            {
                for (int i = 0; i < holdingStocks.Count; i++)
                {
                    await AddNewItem();
                }
            }
        }
    }
    
    // 체크박스 표시/숨김 토글 메서드
    public void ToggleCheckboxes(bool show)
    {
        foreach (Toggle toggle in stockToggles)
        {
            if (toggle != null)
            {
                toggle.gameObject.SetActive(show);
                toggle.isOn = false; // 체크박스 초기화
            }
        }
    }

    // 선택된 주식 목록 반환
    public List<string> GetSelectedStocks()
    {
        List<string> selectedStocks = new List<string>();
        for (int i = 0; i < stockToggles.Count; i++)
        {
            if (stockToggles[i] != null && stockToggles[i].isOn)
            {
                selectedStocks.Add(holdingStocks[i].StockName);
            }
        }
        return selectedStocks;
    }

    // 삭제 후 목록 새로고침
    public async void RefreshAfterDeletion()
    {
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }
        
        currentGroup = null;
        itemsInCurrentGroup = 0;
        totalItemsAdded = 0;
        stockToggles.Clear();

        if (User.Instance != null)
        {
            holdingStocks = User.Instance.getStock();
            if(holdingStocks != null && holdingStocks.Count > 0)
            {
                for (int i = 0; i < holdingStocks.Count; i++)
                {
                    await AddNewItem();
                }
            }
        }
    }

    private async Task<string> GetMostRecentTradingDay(string stockName)
    {
        DateTime currentDate = DateTime.Now;
        int maxAttempts = 10; // 최대 10일 전까지만 확인
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // 주말 체크
            if (currentDate.DayOfWeek == DayOfWeek.Saturday)
            {
                currentDate = currentDate.AddDays(-1);
                attempts++;
                continue;
            }
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                currentDate = currentDate.AddDays(-2);
                attempts++;
                continue;
            }

            // 해당 날짜의 데이터가 있는지 확인
            string dateString = currentDate.ToString("yyyyMMdd");
            try
            {
                StockInfo testInfo = new StockInfo(dateString, dateString);
                List<StockDetail> testData = await testInfo.get_stock_info(stockName);
                
                // 데이터가 존재하면 해당 날짜 반환
                if (testData != null && testData.Count > 0)
                {
                    return dateString;
                }
            }
            catch
            {
                // 에러 발생 시 (데이터가 없는 경우) 이전 날짜 확인
            }

            currentDate = currentDate.AddDays(-1);
            attempts++;
        }

        // 기본값으로 현재 날짜 반환 (모든 시도가 실패한 경우)
        return DateTime.Now.ToString("yyyyMMdd");
    }
}
