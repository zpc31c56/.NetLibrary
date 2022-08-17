SqlBulkCopy 可讓您有效率地大量載入具有另一個來源的資料之 SQL Server 資料表。

此方法可以一次將大量的來源資料寫入MSSQL，來源可以是 DataRow[] 、DataTable、DataReader，本次的使用方式為取得A資料表資料後，將資料轉成要新增入資料庫的DataTable，再將DataTable使用WriteToServer方法將DataTable內的資料複製到SqlBulkCopy物件的目標資料表中。


https://docs.microsoft.com/zh-tw/dotnet/api/system.data.sqlclient.sqlbulkcopy?view=dotnet-plat-ext-5.0