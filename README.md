# Loyalty Automation

Task goal is to implement simple API for managing loyalty bonus program for some store. Stubs for client methods are provided in `LoyaltyClient.cs`.

**Deadline** for this task is extended for one more day &mdash; until Oct 8th instead of Oct 7th &mdash; due to publish delay :)

## Working with database
This task implies usage of the database, in this case &mdash; SQLite. SQLite is a one-file-database: everything is stored in a single file (which is a DB itself) and no installations are needed. Starter project comes with two databases for you: `loyalty.db` &mdash; main database, for normal work of application, and `test.db` &mdash; database used by automatic tests.

Database operations in .NET Core are typically implemented with special framework &mdash; Entity Framework Core. EF Core is an ORM framework &mdash; special type of software capable of translating native DB operations (SQL) into application code (C# in our case). For this EF Core provides you with a good abstraction &mdash; `DbContext`. Implementing DbContext in inherited class lets you define objects you want to store in DB. EF Core does the rest: you work with object collections as with usual collections in C#, and EF Core performs DB operations for you, executing SELECTs, UPDATEs, INSERTs, or DELETEs (aka CRUD) where necessary. Starter project comes with class that inherits from `DbContext` and defines data model for Loyalty application &mdash; it is in `Data/LoyaltyContext.cs`. EF Core as an abstraction layer between database and application supports various database engines like SQL Server, PostgreSql, SQLite etc. Check getting-started-guide: [EF Core with SQLite](https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite)

## Implementation note
You **do not need** to setup data model and initialize the database &mdash; it's all ready and provided in starter project. DB file `loyalty.db` which you see in project folder is created based on the model defined in `LoyaltyContext.cs`. This model is enough to work with the task. You don't need to modify files in `Data` folder. Simply use provided `LoyaltyContext` for your operations with data. And just implement rest of the necessary code: it might be just simple _Transaction Script_ [Fow] implementation straight in `LoyaltyClient`, or more sophisticated solution with separate classes for _Domain Model_ [Fow] with domain logic. It's up to you as a designer! **_There are no right or wrong decisions in this task!_**

## Running tests
For running tests in this project use only `dotnet test` command (`dotnet xunit` may not work).
