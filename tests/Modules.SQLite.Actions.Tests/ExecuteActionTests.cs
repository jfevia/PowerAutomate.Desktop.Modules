using System;
using System.Data;
using System.IO;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions.Tests;

/// <summary>
///     Verifies SQLite statement outcomes and file creation boundaries.
/// </summary>
[TestFixture]
public sealed class ExecuteActionTests : SQLiteDatabaseFixture
{
    /// <summary>
    ///     Prevents a new database file unless the flow opts in.
    /// </summary>
    [Test]
    public void Execute_WhenDatabaseIsMissing_CreatesOnlyIfAllowed()
    {
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            Sql = "CREATE TABLE records (value INTEGER)"
        };
        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.Database));
        Assert.That(File.Exists(DatabasePath), Is.False);

        action.AllowCreate = true;
        RunAction(action);
        Assert.That(File.Exists(DatabasePath), Is.True);
    }

    /// <summary>
    ///     Rejects placeholders whose names contain no characters.
    /// </summary>
    [Test]
    public void Execute_WhenNameIsEmpty_ReportsInvalidArgument()
    {
        CreateDatabase();
        var parameters = CreateParameters(string.Empty, 1);
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT 1",
            Parameters = parameters
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    /// <summary>
    ///     Rejects database-null placeholder names.
    /// </summary>
    [Test]
    public void Execute_WhenNameIsNull_ReportsInvalidArgument()
    {
        CreateDatabase();
        var parameters = CreateParameterTable();
        parameters.Rows.Add(DBNull.Value, 1);
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT 1",
            Parameters = parameters
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    /// <summary>
    ///     Accepts an empty parameter table for statements with no placeholders.
    /// </summary>
    [Test]
    public void Execute_WhenParametersAreEmpty_ReturnsZeroRows()
    {
        CreateDatabase();
        var parameters = CreateParameterTable();
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            Sql = "DELETE FROM records",
            Parameters = parameters
        };

        RunAction(action);

        Assert.That(action.AffectedRows, Is.Zero);
    }

    /// <summary>
    ///     Rejects invalid parameter tables without creating a file.
    /// </summary>
    [Test]
    public void Execute_WhenParametersAreInvalid_DoesNotCreateDatabase()
    {
        var parameters = new DataTable();
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            AllowCreate = true,
            Sql = "SELECT 1",
            Parameters = parameters
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
        Assert.That(File.Exists(DatabasePath), Is.False);
    }

    /// <summary>
    ///     Rejects paths that could resolve relative to the runner.
    /// </summary>
    /// <param name="databasePath">The invalid path to check.</param>
    [TestCase("")]
    [TestCase("relative.db")]
    [TestCase("C:relative.db")]
    [TestCase("\\root-relative.db")]
    public void Execute_WhenPathIsInvalid_ReportsInvalidArgument(string databasePath)
    {
        var action = new ExecuteAction
        {
            DatabasePath = databasePath,
            Sql = "SELECT 1"
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    /// <summary>
    ///     Rejects blank SQL before opening the database file.
    /// </summary>
    [Test]
    public void Execute_WhenSqlIsBlank_DoesNotCreateDatabase()
    {
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            AllowCreate = true,
            Sql = " "
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
        Assert.That(File.Exists(DatabasePath), Is.False);
    }

    /// <summary>
    ///     Surfaces SQL syntax errors through the database error contract.
    /// </summary>
    [Test]
    public void Execute_WhenSqlIsInvalid_ReportsDatabaseError()
    {
        CreateDatabase();
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            Sql = "NOT VALID SQL"
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.Database));
    }

    /// <summary>
    ///     Binds a scalar rather than concatenating it into SQL.
    /// </summary>
    [Test]
    public void Execute_WhenValueIsBound_ReturnsAffectedRows()
    {
        CreateDatabase();
        var parameters = CreateParameters("$value", 42);
        var action = new ExecuteAction
        {
            DatabasePath = DatabasePath,
            Sql = "INSERT INTO records (value) VALUES ($value)",
            Parameters = parameters
        };

        RunAction(action);

        Assert.That(action.AffectedRows, Is.EqualTo(1));
        var query = new QueryAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT value FROM records"
        };
        RunAction(query);
        Assert.That(Convert.ToInt64(query.Result.Rows[0]["value"]), Is.EqualTo(42));
    }
}
