using System;
using System.Data;
using System.IO;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions.Tests;

/// <summary>
///     Isolates each database action test in a disposable local file.
/// </summary>
public abstract class SQLiteDatabaseFixture
{
    /// <summary>
    ///     Identifies the database used by the current test.
    /// </summary>
    protected string DatabasePath { get; private set; }

    /// <summary>
    ///     Initializes the path before NUnit runs a test.
    /// </summary>
    protected SQLiteDatabaseFixture()
    {
        DatabasePath = string.Empty;
    }

    /// <summary>
    ///     Gives each test a distinct local database path.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var testId = TestContext.CurrentContext.Test.ID + ".db";
        DatabasePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, testId);
    }

    /// <summary>
    ///     Deletes only the file owned by the completed test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        File.Delete(DatabasePath);
    }

    /// <summary>
    ///     Creates a database with one integer column.
    /// </summary>
    protected void CreateDatabase()
    {
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            AllowCreate = true,
            Sql = "CREATE TABLE records (value INTEGER)"
        };
        RunAction(action);
    }

    /// <summary>
    ///     Creates a parameter table with the columns expected by both actions.
    /// </summary>
    /// <returns>A table ready for optional parameter rows.</returns>
    protected DataTable CreateParameterTable()
    {
        var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Value", typeof(object));
        return table;
    }

    /// <summary>
    ///     Creates one named parameter row.
    /// </summary>
    /// <param name="name">The placeholder name, including its prefix.</param>
    /// <param name="value">The value to bind to the placeholder.</param>
    /// <returns>A table with one named parameter.</returns>
    protected DataTable CreateParameters(string name, object value)
    {
        var parameters = CreateParameterTable();
        parameters.Rows.Add(name, value);
        return parameters;
    }

    /// <summary>
    ///     Executes an action with a fresh desktop-flow context.
    /// </summary>
    /// <param name="action">The database action to run.</param>
    protected void RunAction(ActionBase action)
    {
        var context = new ActionContext();
        action.Execute(context);
    }
}
