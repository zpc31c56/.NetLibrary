# SqlBulkCopy 

可讓您有效率地大量載入具有另一個來源的資料之 SQL Server 資料表。

此方法可以一次將大量的來源資料寫入MSSQL，來源可以是 DataRow[] 、DataTable、DataReader，本次的使用方式為取得A資料表資料後，將資料轉成要新增入資料庫的DataTable，再將DataTable使用WriteToServer方法將DataTable內的資料複製到SqlBulkCopy物件的目標資料表中。

https://docs.microsoft.com/zh-tw/dotnet/api/system.data.sqlclient.sqlbulkcopy?view=dotnet-plat-ext-5.0

# Dapper

- 基本 Query 寫法
// Query Model
using var conn = new SqlConnection("ConnectionString");
var sql = "SELECT * FROM Users";
var results = conn.Query<Users>(sql).ToList();
// Query Anonymous
using var conn = new SqlConnection("ConnectionString");
var sql = "SELECT * FROM Users";
var results = conn.Query(sql).ToList();
// 這時候 results 的型別會是 dynamic
// QueryFirst
using var conn = new SqlConnection("ConnectionString");
var sql = "SELECT * FROM Users";
var results = conn.QueryFirst<Users>(sql);
// QueryFirst() 取回符合條件的第一筆資料，如果沒有符合會拋出錯誤
// QueryFirstOrDefault() 會將符合條件的第一筆回傳回來，如果沒有符合回傳 null
// QuerySingle() 查詢唯一符合條件的資料，如果沒有符合或符合條件為多筆時會拋出錯誤
// QuerySingleOrDefault()，查詢唯一符合條件的資料，如果沒有符合回傳 null，但如果符合條件為多筆時會拋出錯誤
// QueryMultiple
var sql = "SELECT * FROM Users; SELECT * FROM Account;";
using var conn = new SqlConnection("ConnectionString");
using var results = conn.QueryMultiple(sql);
// 第一段 SQL
var users = results.Read<Users>().ToList();
// 第二段 SQL
var accounts = results.Read<Account>().ToList();
使用 Parameter​ 參數查詢
// Anonymous
// 單一參數
using var conn = new SqlConnection("ConnectionString");
var results = conn.Execute(
"MyStoredProcedure",
new { Param1 = 1, Param2 = " ImParam" },
commandType: CommandType.StoredProcedure
);

// 多組參數
using var conn = new SqlConnection("ConnectionString");
var results = conn.Execute(
"MyStoredProcedure",
new[] { new { Param1 = 1, Param2 = " ImParam" }, new { Param1 = 2, Param2 = " N2" } },
commandType: CommandType.StoredProcedure
);
// Dynamic
using var conn = new SqlConnection("ConnectionString");
// 設定參數
var parameters = new DynamicParameters();
parameters.Add("@Param1", "abc", DbType.String, ParameterDirection.Input);
parameters.Add("@Return1", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
conn.Execute("MyStoredProcedure", parameters, commandType: CommandType.StoredProcedure);
int result = parameters.Get<int>("@Return1");
// DynamicParameters 也可以接回 Return 值
// List
using var conn = new SqlConnection("ConnectionString");
string sql = "SELECT * FROM Users WHERE UserId IN @ids";
var results = conn.Query<Users>(sql, new { ids = new[] { "001", "002", "004", "008" } }).ToList();
// String
using var conn = new SqlConnection("ConnectionString");
var sql = "SELECT * FROM Users WHERE UserId = @id";
var results = conn.Query<Users>(
sql,
new { id = new DbString { Value = "002", IsFixedLength = false, Length = 3, IsAnsi = true } }
).ToList();
// Dapper 如果使用暱名型別預設 String 會轉成 NVARCHAR，效能會稍差，指定型別效能比較好
- 使用 Execute 執行預存程序
用於執行 Insert、Update、Delete、Stored Procedure 時。

// Stored Procedure
using var conn = new SqlConnection("ConnectionString");
// 準備參數
var parameters = new DynamicParameters();
parameters.Add("@Param1", "abc", DbType.String, ParameterDirection.Input);
parameters.Add("@OutPut1", dbType: DbType.Int32, direction: ParameterDirection.Output);
parameters.Add("@Return1", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
conn.Execute("MyStoredProcedure", parameters, commandType: CommandType.StoredProcedure);
// 接回 Output 值
var outputResult = parameters.Get<int>("@OutPut1");
// 接回 Return 值
var returnResult = parameters.Get<int>("@Return1");
// Stored Procedure 會用到的 input、output、return 都可以用
// INSERT statement
// 新增多筆資料
using var conn = new SqlConnection("ConnectionString");
var sql = "INSERT INTO Users(col1,col2) VALUES (@c1,@c2);";
var datas = new[]{
new { c1 = "A", c2 = "A2" },
new { c1 = "B", c2 = "B2" },
new { c1 = "C", c2 = "C2" }
};
conn.Execute(sql, datas);
// UPDATE statement
// 修改多筆資料
using var conn = new SqlConnection("ConnectionString");
var strSql = " UPDATE Users SET col1=@c1 WHERE col2=@c2";
var datas = new[]{
new { c1 = "A", c2 = "A2" },
new { c1 = "B", c2 = "B2" },
new { c1 = "C", c2 = "C2" }
};
conn.Execute(strSql, datas);
// DELETE statement
// 刪除多筆資料
using var conn = new SqlConnection("ConnectionString");
var sql = " DELETE Users WHERE col2=@c2";
var datas = new[]{
new {c2 = "A2" },
new {c2 = "B2" },
new {c2 = "C2" }
};
conn.Execute(sql, datas);
使用 Transaction​ 交易
// Transaction
// 單一資料庫時建議使用(效能較好)
using var transaction = conn.BeginTransaction();
using var conn = new SqlConnection("ConnectionString");
var sql = " UPDATE Users SET col1=@c1 WHERE col2=@c2";
var datas = new[]{
new { c1 = "A", c2 = "A2" },
new { c1 = "B", c2 = "B2" },
new { c1 = "C", c2 = "C2" }
};
conn.Execute(sql, datas);
transaction.Commit();
- // TransactionScope
// 用於異質資料庫交易
using var conn = new SqlConnection("ConnectionString");
using var transactionScope = new TransactionScope();
var sql = " UPDATE Users SET col1=@c1 WHERE col2=@c2";
var datas = new[]{
new { c1 = "A", c2 = "A2" },
new { c1 = "B", c2 = "B2" },
new { c1 = "C", c2 = "C2" }
};
conn.Execute(sql, datas);
transactionScope.Complete();

https://dotblogs.com.tw/OldNick/2018/01/15/Dapper

