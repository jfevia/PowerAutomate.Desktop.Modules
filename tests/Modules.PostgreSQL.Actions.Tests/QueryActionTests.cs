using System;
using System.Data;
using Npgsql;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Checks parameter binding and PostgreSQL query results.
/// </summary>
[TestFixture]
public sealed class QueryActionTests : PostgreSqlActionFixture
{
    /// <summary>
    ///     Sends a bound value with the requested command timeout.
    /// </summary>
    [Test]
    public void Execute_WhenParameterIsBound_ReturnsRows()
    {
        Client.QueryResult.Rows.Add("record");
        var parameters = CreateParameters("@id", 42);
        var action = new StubQueryAction(Client)
        {
            ConnectionString = "Host=localhost;Database=test",
            Sql = "SELECT name FROM records WHERE id = @id",
            Parameters = parameters,
            TimeoutSeconds = 15
        };

        RunAction(action);

        Assert.That(action.Result.Rows[0]["name"], Is.EqualTo("record"));
        Assert.That(Client.ConnectionString, Is.EqualTo("Host=localhost;Database=test"));
        Assert.That(Client.CommandText, Is.EqualTo(action.Sql));
        Assert.That(Client.TimeoutSeconds, Is.EqualTo(15));
        Assert.That(Client.ParameterName, Is.EqualTo("@id"));
        Assert.That(Client.ParameterValue, Is.EqualTo(42));
    }

    /// <summary>
    ///     Uses the documented timeout when none is supplied.
    /// </summary>
    [Test]
    public void Execute_WhenParametersAreMissing_UsesDefaultTimeout()
    {
        var action = new StubQueryAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1"
        };

        RunAction(action);

        Assert.That(Client.ParameterCount, Is.Zero);
        Assert.That(Client.TimeoutSeconds, Is.EqualTo(30));
    }

    /// <summary>
    ///     Initializes the output table before the flow executes.
    /// </summary>
    [Test]
    public void QueryAction_WhenCreated_HasEmptyResult()
    {
        var action = new QueryAction();

        Assert.That(action.Result.Rows.Count, Is.Zero);
        Assert.That(action.TimeoutSeconds, Is.EqualTo(30));
    }

    /// <summary>
    ///     Rejects a missing injected client before running SQL.
    /// </summary>
    [Test]
    public void QueryAction_WhenClientIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var action = new StubQueryAction(null);
            Assert.Fail($"Unexpected action: {action}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("client"));
    }

    /// <summary>
    ///     Rejects a missing connection string before creating a command.
    /// </summary>
    /// <param name="connectionString">The missing connection settings.</param>
    [TestCase("")]
    [TestCase(" ")]
    public void Execute_WhenConnectionStringIsBlank_ReportsInvalidArgument(string connectionString)
    {
        var action = new StubQueryAction(Client)
        {
            ConnectionString = connectionString,
            Sql = "SELECT 1"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects a zero timeout before contacting the server.
    /// </summary>
    [Test]
    public void Execute_WhenTimeoutIsZero_ReportsInvalidArgument()
    {
        var action = new StubQueryAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            TimeoutSeconds = 0
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects parameter rows without a Name column.
    /// </summary>
    [Test]
    public void Execute_WhenNameColumnIsMissing_ReportsInvalidArgument()
    {
        var parameters = new DataTable();
        parameters.Columns.Add("Value", typeof(object));
        var action = new StubQueryAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = parameters
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects parameter rows without a Value column.
    /// </summary>
    [Test]
    public void Execute_WhenValueColumnIsMissing_ReportsInvalidArgument()
    {
        var parameters = new DataTable();
        parameters.Columns.Add("Name", typeof(string));
        var action = new StubQueryAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = parameters
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Surfaces a database failure rather than an empty result.
    /// </summary>
    [Test]
    public void Execute_WhenServerFails_ReportsDatabaseError()
    {
        var failure = new NpgsqlException("server unavailable");
        Client.Failure = failure;
        var action = new StubQueryAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1"
        };

        AssertError(ErrorCodes.Database, action);
    }
}
