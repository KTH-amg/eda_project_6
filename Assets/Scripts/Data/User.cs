namespace user
{
    using System;
    using System.Collections.Generic;

    public class User
{
    private string user_id;
    private string user_pw;
    private string user_name;
    private List<string> holding_stock;

    private User() {}
    public static User _instance;

    public static User Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new User();
            }
            return _instance;
        }
    }

    public void setId(string id)
    {
        user_id = id;
    }

    public void setPw(string pw)
    {
        user_pw = pw;
    }

    public void setName(string name)
    {
        user_name = name;
    }

    public bool setStock(string stock)
    {
        holding_stock.Add(stock);

        string std = "";
        using (var reader = dbManager.select("stock", "std_code", $"stock_name='{stock}'"))
        {
            if (reader == null || !reader.HasRows)  // 데이터가 없는 경우 체크
            {
                return false; // 종목 찾기 실패
            }
            else
            {
                std = Convert.ToString(reader["std_code"]);
            }
        }

        int error = Convert.ToInt32(dbManager.insert("holding_stock", "user_id, std_code", $"'{user_id}', '{std}'"));
        if (error == 0)
        {
            return true; // 모든 프로세스 성공
        }
        else
        {
            return false; // DB 오류
        }
    }

    public void delStock(string stock)
    {
        holding_stock.Remove(stock);
    }

    public string getId()
    {
        return user_id;
    }

    public string getPw()
    {
        return user_pw;
    }
    public string getName()
    {
        return user_name;
    }

    public List<string> getStock()
    {
        return holding_stock;
    }

    public void delUser()
    {
        _instance = null;
    }
    }
}
