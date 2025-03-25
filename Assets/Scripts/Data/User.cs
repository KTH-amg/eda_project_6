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

    public void setStock(string stock)
    {
        holding_stock.Add(stock);
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
