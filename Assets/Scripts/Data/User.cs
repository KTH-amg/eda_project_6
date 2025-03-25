using System;
using System.Collections.Generic;

public class User
{
    private string user_id;
    private string user_pw;
    private List<string> holding_stock;

    public static User instance;

    private User() {}

    public static User Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new User();
            }
            return instance;
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

    public List<string> getStock()
    {
        return holding_stock;
    }
}
