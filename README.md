# DBExtension
A Simple ORM for SQL Server (Only)

# Usuage

## DBTable (Example)

```cs
public class DBTable
{
    public int UniqueIndex {get; set;} //Not Nullable
    public string UserID {get;set;}
    public string UserPWD {get;set;}
    public int? UserType {get;set;} //Nullable
    public DateTime? RegisterDate {get;set;} = null; //Nullable
}
```

## Select
```cs
var con = new SqlConnection(ConnectionStrings);

DBTable value = con.Select<DBTable>("UniqueIndex = '{0}'", 1);
```

## Selects
```cs
var con = new SqlConnection(ConnectionStrings);

List<DBTable> value = con.Selects<DBTable>("UserType = '{0}'", -1);
```

## Insert
```cs
var con = new SqlConnection(ConnectionStrings);

DBTable value = new DBTable();

value.UniqueIndex = 2;
value.UserID = "Test";
value.UserPWD = "TestPWD";
value.UserType = -1;
value.DateTime = DateTime.Now;

con.Insert<DBTable>(value);
```

## Update
```cs
var con = new SqlConnection(ConnectionStrings);

DBTable value = con.Select<DBTable>("UniqueIndex = '{0}'", 1);
value.UserID = "UpdatedID";
con.Update<DBTable>(value);
```
