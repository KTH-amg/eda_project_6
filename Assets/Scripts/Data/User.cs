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

        private User() 
        {
            holding_stock = new List<string>();  // 리스트 초기화 추가
        }
        
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

        public bool setStock(string stock, int price, int num_st)
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
                    while (reader.Read())
                    {
                        std = Convert.ToString(reader["std_code"]);
                    }
                }
            }

            int error = Convert.ToInt32(
                dbManager.insert("holding_stock", 
                    "user_id, std_code, price, num_of_stocks", 
                    $"'{user_id}', '{std}', {price}, {num_st}"));
            
            if (error == 0)
            {
                return true; // 모든 프로세스 성공
            }
            else
            {
                return false; // DB 오류
            }
        }

        public bool delStock(string stock)
        {
            holding_stock.Remove(stock);

            string std = "";
            using (var reader = dbManager.select("stock", "std_code", $"stock_name='{stock}'"))
            {
                if (reader == null || !reader.HasRows)  // 데이터가 없는 경우 체크
                {
                    return false; // 종목 찾기 실패
                }
                else
                {
                    while (reader.Read())
                    {
                        std = Convert.ToString(reader["std_code"]);
                    }
                }
            }

            int error = Convert.ToInt32(dbManager.delete("holding_stock", $"(user_id='{user_id} AND std_code='{std}')"));
            
            if (error == 0)
            {
                return true; // 모든 프로세스 성공
            }
            else
            {
                return false; // DB 오류
            }
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
            holding_stock.Clear(); // 기존 리스트 초기화
            
            using (var hold_reader = dbManager.select(
                "holding_stock h", "*", $"h.user_id='{user_id}'", "stock s", "h.std_code=s.std_code"))
            {
                if (hold_reader == null || !hold_reader.HasRows)  // 데이터가 없는 경우 체크
                {
                    return holding_stock; // 빈 리스트 반환
                }
                else
                {
                    while (hold_reader.Read())
                    {
                        string name = Convert.ToString(hold_reader["stock_name"]);
                        holding_stock.Add(name); // 각 std_code를 리스트에 추가
                    }
                }
            }

            return holding_stock;
        }

        public void delUser()
        {
            _instance = null;
        }
    }
}
