# Task Manager Application

This application is a task management system utilizing ASP.NET with Entity Framework. 
It provides a user interface built with Razor pages and covers basic CRUD operations alongside additional features for enhanced functionality.

## Features

- **CRUD Operations**: Create, read, update, and delete tasks.
- **Task Filtering**: Filter tasks by start date, closing date, and the programmer's name or surname.
- **Statistics Display**: Show a summary of hour estimations for tasks, grouped by state, along with the programmer's name and surname.
- **Task Hierarchy**: Support for subtasks within tasks, with each task having the ability to include one or more subtasks and only one parent task.

## Getting Started with the Database

To set up the application's database:

1. Ensure you have an empty database ready for creating the required tables.
2. Confirm the installation of the following NuGet packages:
   - `Microsoft.EntityFrameworkCore`
   - `Microsoft.EntityFrameworkCore.SqlServer`
   - `Microsoft.EntityFrameworkCore.Tools`
3. Configure the `appsettings.json` file with your database connection string:

 ```json
 "ConnectionStrings": {
   "DefaultConnection": "Your database connection string goes here"
 }
```
4. Execute the `Update-Database` command in the Package Manager Console.
5. Run the application and test the provided features.
