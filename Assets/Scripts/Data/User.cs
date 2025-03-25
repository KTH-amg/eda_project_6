using System;
using System.Collections.Generic;

public class User
{
    private string user_id;
    private string user_pw;
    private List<string> holding_stock;

    void setId(string id)
    {
        user_id = id;
    }

    void setPw(string pw)
    {
        user_pw = pw;
    }

    void setStock(string stock)
    {
        holding_stock.Add(stock);
    }

    void delStock(string stock)
    {
        holding_stock.Remove(stock);
    }

    string getId()
    {
        return user_id;
    }

    string getPw()
    {
        return user_pw;
    }

    List<string> getStock()
    {
        return holding_stock;
    }
}
