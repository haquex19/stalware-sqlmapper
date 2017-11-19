# Release Unstable
Use at your own risk...

# Version
Not yet released as a usable nuget package

# Before Reading
- Lots of examples are provided in the unit test project. Reference this for more complex examples compared to the very basic examples provided in this document.

# Table of Contents
Work in progress

## Purpose
The purpose of this library is to create SQL statements leveraging .NET's expression capabilities. This library 
will prevent you from having to manually write down queries and instead use objects and LINQ expressions with 
full intellisense to build out queries. This can prevent mistyping a column name or table for example and 
generally improve the time it takes to write queries.

## Selects
This section will describe and provide examples for the `SelectBuilder`.

### Alias
In the `SelectBuilder`, the alias is very important. In fact, all column names and tables will be automatically 
aliased. The alias that the library determines is the name of the parameter that you decide. Take the following 
code for example:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Build();
```

In the `.Select` method we passed a lambda expression. The parameter name that is being used here is `x`. Thus, 
The `Users` table and its columns will now be aliased with `x`. The result of the above statement is:

```csharp
Console.WriteLine(result.Query);
//Outputs: SELECT x.* FROM Users AS x
```

The alias is important because this is what you will need to reference when doing joins and indicating which table 
a column belongs to. Let's look at the following code below:

```csharp
var result = new SelectBuilder<Users>()
    .Select(user => new {user.FirstName, user.LastName})
    .Join<BankAccounts>((user, bank) => user.Id == bank.UserId, bank => new {})
    .Build();
```

For the `Users` table here, we gave it an alias of `user`. We then joined another table called `BankAccounts`
and gave it the alias `bank`. If you look closely at the `.Join` method you will see that the same alias, 
`user`, is used just as it was used in the `.Select` method. Simlarily if there are more tables to be joined 
and the `BankAccounts` table in the example needs to be referenced, it is important to use the same parameter 
name as was used in the `.Join` method, which is `bank`. The result of the above statement is:

```csharp
Console.WriteLine(result.Query);
/*Outputs: 
SELECT user.FirstName, user.LastName, bank.*
FROM Users AS user
JOIN BankAccounts AS bank ON (user.Id = bank.UserId)
*/
```

If we used a different parameter name in the `.Join` method and instead used `ganondorf`, our alias's will 
mismatch and give us the wrong query. Here's the call written using the wrong alias:

```csharp
var result = new SelectBuilder<Users>()
    .Select(user => new {user.FirstName, user.LastName})
    .Join<BankAccounts>((ganondorf, bank) => ganondorf.Id == bank.UserId, bank => new {})
    .Build();
```

This will result in the query below, which is wrong. You will get a SQL error that says an object with the alias 
`ganondorf` could not be found.

```csharp
Console.WriteLine(result.Query);
/* Outputs:
SELECT user.FirstName, user.LastName, bank.*
FROM Users AS user
JOIN BankAccounts AS bank ON (ganondorf.Id = bank.UserId)
*/
```

Basically the idea is to keep your alias names consistent and match the correct table you are trying to reference.

### Parameters
All queries are parameterized. Any constant values in a compare operator will be parameterized. The parameter names 
in the queries themeselves are prefixed with `PARAM` and then a number starting with `0`. This number increments 
for every new value that needs to be parameterized. (EX: PARAM0, PARAM1, PARAM2, etc.)

Let's look at the following code:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Where(x => x.FirstName == "Ganondorf" && x.LastName == "Dragmire")
    .Build();
```

When viewing `result.Query` it will output:

```sql
SELECT x.*
FROM Users AS x
WHERE ((x.FirstName = @PARAM0) AND (x.LastName = @PARAM1))
```

There is another property in the `result` object named `Parameters` which is a simple list of key value pairs, 
`List<KeyValuePair<string, object>>`. This object will contain the `PARAM` keys and its value, in this case:

```csharp
Console.WriteLine($"{result.Parameters[0].Key} | {result.Parameters[0].Value}");
//Outputs: "PARAM0 | Ganondorf"

Console.WriteLine($"{result.Parameters[1].Key} | {result.Parameters[1].Value}");
//Outputs: "PARAM1 | Dragmire"
```

### Initialize Select Builder
Instantiate the select builder by passing in a type parameter for your initial table. This adds the table name to 
the `FROM` clause when the `Select` method is called:

```csharp
var builder = new SelectBuilder<Users>(); //will add FROM Users
var builder = new SelectBuilder<Games>(); //will add FROM Games
```

This type is then set as the default type for that class throughout the method chaining. Thus, any LINQ expressions 
that pass a parameter without specifying a type on that method will pass the type used to instantiate the builder.
For example, suppose we instantiate the builder like this:

`var builder = new SelectBuilder<Users>();`

When we call the `Where` method instead of the `Where<T>` method, the `Users` type will be passed. If we used 
`Where<T>` however, the type passed in `T` will be passed instead.

```csharp
builder.Where(x => /*some condition*/); //x here is of type Users
builder.Where<Games>(x => /*some condition*/); //x here is of type Games
```

**Note: Not all methods also contain a *`method`*`<T>` equivalent. This can change depending on the circumstance 
in later releases.**

### Selecting Columns
You call the `Select` command where you will create a lambda expression that indicates the alias and the columns 
to retrieve for that table. In the expression, return an anonymous object that contains the columns to retrieve.

```csharp
var builder = new SelectBuilder<Users>()
    .Select(x => new {x.Id, x.FirstName});
    //Produces SELECT x.Id, x.FirstName FROM Users AS x
```

To return all columns, simply return an anonymous object with no properties:

```csharp
var builder = new SelectBuilder<Users>()
    .Select(x => new {});
    //Produces SELECT x.* FROM Users AS x
```

### The WHERE Clause
Use the `Where` method to indicate the conditions. Use it like any LINQ expression:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Where(x => x.Active && x.Kills > 500)
    .Build();
```

The `result.Query` property produces:

```sql
SELECT x.*
FROM Users AS x
WHERE (x.Active = 1 AND (x.Kills > @PARAM0))
```

### Method Calls
Your LINQ expressions can contain method calls, for example:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Where(x => x.FirstName.StartsWith("ganon") || x.LastName.Contains("hyrule"))
    .Build();
```

The `result.Query` property will result in:

```sql
SELECT x.* 
FROM Users AS x 
WHERE (x.FirstName LIKE @PARAM0 OR x.LastName LIKE @PARAM1)
```

In `result.Parameters`, PARAM0 will contain the value `ganon%` and PARAM1 will contain `%hyrule%`.

**NOTE: Only a select amount of methods are supported. (Currently, very few). More methods will be added in 
later releases.**

You can chain the `Where` method and also supply your own type in `Where<T>` when you want to use a different 
table for conditions rather than the default used when you instantiated the `SelectBuilder`. This is usually
used in joins. A `WhereOr` and `WhereOr<T>` method is also provided. Look at the documentation in code to 
understand their purpose.

### The ORDER BY Clause
You can use the `.OrderBy` method to add an ORDER BY clause:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .OrderBy(x => x.Id)
    .Build();
```

The `result.Query` property contains:

```sql
SELECT x.* 
FROM Users AS x
ORDER BY x.Id
```

Multiple columns can also be supplied at the same time:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .OrderByMultiple(x => new {x.Id, x.FirstName})
    .Build();
```

The `result.Query` property contains:

```sql
SELECT x.* 
FROM Users AS x
ORDER BY x.Id, x.FirstName
```

The descending equivalent methods are also provided: `OrderByDesc`, `OrderByMultipleDesc`. 

Additionally, you can provide a type to target a particular table in the `OrderBy<T>` and `OrderByMultiple<T>`
and their descending equivalents.

You can also chain these order by methods.

### Joins
To start a join, you will need to pass in a type that indicates the table you'd like to join:

```csharp
    .Join<Games> //Indicates you want to join the Games table
    .Join<Accounts> //Indicates you want to join the Accounts table
```

You will then need to pass in two LINQ expressions. The first expression indicates what sort of conditions you 
want to join by, AKA the ON clause. In this expression, the first parameter indicates the table you want to join 
with conditions on. The second parameter indicates the table you want to join, this also sets the alias name as 
well. (Refer to the alias section) Let's look at the code below:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Join<Games>((x, game) => x.Id == game.UserId, /*discussed later*/)
    .Build();
```

Let's break this apart:
- The `.Join<Games>` part indicates that we want to join with the `Games` table
- The expression `(x, game) =>` says a few things
  - The first parameter `x` indicates the `Users` table because that's the alias we set initally at the `Select` method. (Refer to the alias section)
  - The second paramater `game` is a new alias that we are setting for the `Games` table. (Refer to the alias section)
  - This gives us the ability to create an ON clause between the `Users` table and `Games` table.
- Finally `x.Id == game.UserId` is where we specify the condition.
- The JOIN clause will result in: `JOIN Games AS game ON (x.Id = game.UserId)`

The second argument in the method indicates what columns to select for the joining table. Think of it as the 
`Select` method argument passed here:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Join<Games>((x, game) => x.Id == game.UserId, game => new {game.Id})
    .Build();
```

In the second argument, we have a LINQ expression that returns an object. This is interpreted as selecting the `Id`
column in the `Games` table.

The entire query will look like below:

```sql
SELECT x.*, game.Id 
FROM Users AS x
JOIN Games AS game ON (x.Id = game.UserId)
```

You can chain join methods. In fact, this raises the scenario where you want to add conditions on two tables 
that do not include the default table used to instantiate the builder. A method with an additional type is 
provided for that:

```csharp
var result = new SelectBuilder<Users>()
    .Select(x => new {})
    .Join<Games>((user, game) => user.Id == game.UserId, game => new {})
    .Join<Games, Platform>((game, platform) => game.SupportedPlatform == platform.Name, platform => new {})
    .Build();
```

This results in the query:

```sql
SELECT x.*, game.*, platform.*
FROM Users AS x 
JOIN Games AS game ON (user.Id = game.UserId) 
JOIN Platform AS platform ON (game.SupportedPlatform = platform.Name)
```

Other methods including `LeftJoin` and `RightJoin` are also provided.

**Note: Chaining joins can often require more than just two tables for the condition. This will be supported in 
later releases.**

## Inserts
This section will describe the two provided implementations of the `IInsertBuilder`

### MySQL and SQL Server
When inserting a new record into a table, the syntax is mostly the same with these two different databases. The 
issue we face is when we want to retrieve the primary key id value that was just inserted into that table. These
databases have different ways of obtaining that. 

SQL Server recommends using the `OUTPUT` clause to retrieve columns that were just inserted. MySQL however does 
not have this functionality and we must use other means. While SQL Server makes it easy to retrieve a value on a 
column that was just inserted, we need to implement different methods depending on the type of id used in MySQL. 

If the id is a `Guid` we must save a guid into a variable like so: `SET @temp = SELECT UUID();` We then must 
use this variable in the insert clause.

If the id is an integer *and* it auto increments (if it does not auto increment, this method does not work) then 
we must use the built in MySQL function `LAST_INSERT_ID()` to get this id.

These different implementations are discussed below.

### `IdColumn` Attribute
Since there is no set rule on how to name a primary key column, every database may have a different name. Thus, 
the library needs a way to know which property in an object represents the primary key of a table. Add the 
`[IdColumn]` attribute to the primary key property:

```csharp
public class Users 
{
    [IdColumn]
    public long Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
}
```

### Insert All Columns
To insert all columns of an object into a table, use the following code:

```csharp
public class Users 
{
    [IdColumn]
    public long Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
}

var user = new Users {FirstName = "Ganondorf"};
var result = new SQLServerInsertBuilder<Users>(user)
    .InsertAll()
    .Build();
```

The `result.Query` property holds:
```sql
INSERT INTO Users (FirstName, LastName)
VALUES (@FirstName, @LastName)
```

### Auto Increment Ids
You may be wondering why the above piece of code did not generate the `Id` column in the `INSERT` clause. This is 
because the library assumes the id columns are auto-generated. If your database does not auto-generate ids and 
instead your program explicitly sets this id in code, then you have the option of adding a bool argument like this:

```csharp
var result = new SQLServerInsertBuilder<Users>(user)
    .InsertAll(true)
    .Build();
```

The `.InsertAll` method by default does not include the id column, the parameter it accepts is an optional 
parameter. You can pass `true` to inclue the id column. The above query outputs the query:

```sql
INSERT INTO Users (Id, FirstName, LastName) 
VALUES (@Id, @FirstName, @LastName)
```

### Insert All Except Some Columns
You may choose to insert all columns be disregard just a few:

```csharp
var result = new SQLServerInsertBuilder<Users>(user)
    .InsertAllExcept(x => new {x.LastName}, true)
    .Build();
```

This results in:
```sql
INSERT INTO Users (Id, FirstName)
VALUES (@Id, @FirstName)
```

### Insert Only Specific Columns
You may choose to only insert certain columns:

```csharp
var result = new SQLServerInsertBuilder<Users>(user)
    .InsertOnly(x => new {x.LastName})
    .Build();
```

This results in:
```sql
INSERT INTO Users (LastName) 
VALUES (@LastName)
```

## Disclaimer
There are quite a few libraries out there that achieve the same thing this library tries to achieve. However, 
they are not free. I am currently the only developer on this library, so updates and new features can come quite 
slowly. There will surely be several specific cases where the current implementation of this library cannot handle
the case. For these cases, please send me an email and I'll try to implement them. Or, do it yourself and create 
a PR :), this is open source after all and everyone benefits. For the most part however, most queries can be built 
with this library. There's still more statements that will eventually be supported in later releases. (EX: The 
group by and having clause)

Additionally, there is so much possibility with LINQ expressions. Some of the statements may not be supported just 
yet but will be in later releases, gradually.

This library is also built with my current understanding of expressions and how to traverse them. If you know of a 
better or more efficient way to traverse an expression tree, do let me know in an email. And of course, go ahead 
and implement it yourself and create a PR if you want :).