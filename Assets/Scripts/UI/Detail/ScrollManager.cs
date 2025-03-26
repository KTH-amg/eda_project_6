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
    private const int MAX_ITEMS_PER_GROUP = 2;         // 그룹당 최대 아이템 수
    private List<string> holding_stock_List;

    // 각 종목의 매수가와 수량을 저장할 구조체 정의
    private class StockPurchaseInfo
    {
        public string PurchasePrice { get; set; }
        public string NumberOfStocks { get; set; }
    }

    private Dictionary<string, StockPurchaseInfo> stockPurchaseInfos = new Dictionary<string, StockPurchaseInfo>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        if (User.Instance != null)
        {
            holding_stock_List = User.Instance.getStock();
            if(holding_stock_List != null)
            {
                int holding_stock_num = holding_stock_List.Count;
                
                for (int i = 0; i < holding_stock_num; i++)
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

        string stockName = holding_stock_List[itemsInCurrentGroup];
        GameObject newView = Instantiate(viewPrefab, currentGroup.transform);
        await SetStockItemData(newView, stockName);
        
        itemsInCurrentGroup++;
    }

    private async Task SetStockItemData(GameObject stockItem, string stockName)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        StockInfo stockInfo = new StockInfo(today, today);
        List<StockDetail> stock_data_arr = await stockInfo.get_stock_info(stockName);


        // 각 TMP 컴포넌트 찾기
        TextMeshProUGUI name = stockItem.transform.Find("name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI cur_price = stockItem.transform.Find("cur_price").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI my_price = stockItem.transform.Find("my_price").GetComponent<TextMeshProUGUI>();
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
                my_price.text = purchaseInfo.PurchasePrice;
            
            // 수익률 계산
            if (margin != null && Convert.ToSingle(stock_data_arr[0].closing_price) as currentPrice
                && float.TryParse(purchaseInfo.PurchasePrice, out float purchasePrice))
            {
                float marginRate = ((currentPrice - purchasePrice) / purchasePrice) * 100;
                margin.text = $"{marginRate:F2}%";
            }
            
            // 필요한 경우 수량 정보도 표시
            // TextMeshProUGUI stockAmount = stockItem.transform.Find("amount").GetComponent<TextMeshProUGUI>();
            // if (stockAmount != null)
            //     stockAmount.text = purchaseInfo.NumberOfStocks;
        }

        if (fluct != null) fluct.text = Convert.ToString(stock_data_arr[0].fluctuation_rate);
        if (attr != null) attr.text = stock_data_arr[0].abbr; 
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

        // 리스트 다시 가져오기
        if (User.Instance != null)
        {
            holding_stock_List = User.Instance.getStock();
            if(holding_stock_List != null)
            {
                int holding_stock_num = holding_stock_List.Count;
                
                for (int i = 0; i < holding_stock_num; i++)
                {
                    await AddNewItem();
                }
            }
        }
    }
}
