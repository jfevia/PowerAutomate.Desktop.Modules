using System;
using Npgsql;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Checks parameterized statements and their error categories.
/// </summary>
[TestFixture]
public sealed class ExecuteActionTests : PostgreSqlActionFixture
{
    /// <summary>
    ///     Rejects empty parameter names before running a command.
    /// </summary>
    [Test]
    public void Execute_WhenNameIsEmpty_ReportsInvalidArgument()
    {
        var parameters = CreateParameters(string.Empty, 1);
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = parameters
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects database-null parameter names.
    /// </summary>
    [Test]
    public void Execute_WhenNameIsNull_ReportsInvalidArgument()
    {
        var parameters = CreateParameterTable();
        parameters.Rows.Add(DBNull.Value, 1);
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = parameters
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Accepts a table with no parameter rows.
    /// </summary>
    [Test]
    public void Execute_WhenParametersAreEmpty_UsesNoParameters()
    {
        var parameters = CreateParameterTable();
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "DELETE FROM records",
            Parameters = parameters
        };

        RunAction(action);

        Assert.That(Client.ParameterCount, Is.Zero);
    }

    /// <summary>
    ///     Surfaces server failures instead of a zero-row result.
    /// </summary>
    [Test]
    public void Execute_WhenServerFails_ReportsDatabaseError()
    {
        var failure = new NpgsqlException("server unavailable");
        Client.Failure = failure;
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1"
        };

        AssertError(ErrorCodes.Database, action);
    }

    /// <summary>
    ///     Rejects missing SQL before opening a connection.
    /// </summary>
    /// <param name="sql">The invalid SQL supplied by the flow.</param>
    [TestCase("")]
    [TestCase(null)]
    public void Execute_WhenSqlIsMissing_ReportsInvalidArgument(string? sql)
    {
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = sql
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects negative timeouts instead of waiting without a bound.
    /// </summary>
    [Test]
    public void Execute_WhenTimeoutIsNegative_ReportsInvalidArgument()
    {
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            TimeoutSeconds = -1
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Keeps SQL database nulls distinct from empty strings.
    /// </summary>
    [Test]
    public void Execute_WhenValueIsNull_ReturnsAffectedRows()
    {
        Client.AffectedRows = 3;
        var parameters = CreateParameters("@value", DBNull.Value);
        var action = new StubExecuteAction(Client)
        {
            ConnectionString = "Host=localhost",
            Sql = "UPDATE records SET value = @value",
            Parameters = parameters
        };

        RunAction(action);

        Assert.That(action.AffectedRows, Is.EqualTo(3));
        Assert.That(Client.ParameterValue, Is.EqualTo(DBNull.Value));
    }

    /// <summary>
    ///     Rejects a missing injected client before opening a connection.
    /// </summary>
    [Test]
    public void ExecuteAction_WhenClientIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var action = new StubExecuteAction(null);
            Assert.Fail($"Unexpected action: {action}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("client"));
    }

    /// <summary>
    ///     Uses the default timeout until a flow changes it.
    /// </summary>
    [Test]
    public void ExecuteAction_WhenCreated_HasSafeDefaults()
    {
        var action = new ExecuteAction();

        Assert.That(action.AffectedRows, Is.Zero);
        Assert.That(action.TimeoutSeconds, Is.EqualTo(30));
    }
}
