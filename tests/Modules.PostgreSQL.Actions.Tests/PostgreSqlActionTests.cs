using System;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Npgsql;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

[TestFixture]
public class PostgreSqlActionTests
{
    private FakePostgreSqlClient client = null!;

    [SetUp]
    public void SetUp() => client = new FakePostgreSqlClient();

    [Test]
    public void Query_UsesBoundParametersAndReturnsRows()
    {
        client.QueryResult.Rows.Add("record");
        var action = new QueryAction(client)
        {
            ConnectionString = "Host=localhost;Database=test",
            Sql = "SELECT name FROM records WHERE id = @id",
            Parameters = Parameters(("@id", 42)),
            TimeoutSeconds = 15
        };

        action.Execute(new ActionContext());

        Assert.That(action.Result.Rows[0]["name"], Is.EqualTo("record"));
        Assert.That(client.ConnectionString, Is.EqualTo("Host=localhost;Database=test"));
        Assert.That(client.CommandText, Is.EqualTo(action.Sql));
        Assert.That(client.TimeoutSeconds, Is.EqualTo(15));
        Assert.That(client.ParameterName, Is.EqualTo("@id"));
        Assert.That(client.ParameterValue, Is.EqualTo(42));
    }

    [Test]
    public void Query_WithoutParameters_UsesDefaultTimeout()
    {
        var action = new QueryAction(client) { ConnectionString = "Host=localhost", Sql = "SELECT 1" };

        action.Execute(new ActionContext());

        Assert.That(client.ParameterCount, Is.Zero);
        Assert.That(client.TimeoutSeconds, Is.EqualTo(30));
    }

    [Test]
    public void Execute_BindsNullAndReturnsAffectedRows()
    {
        client.AffectedRows = 3;
        var action = new ExecuteAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "UPDATE records SET value = @value",
            Parameters = Parameters(("@value", DBNull.Value))
        };

        action.Execute(new ActionContext());

        Assert.That(action.AffectedRows, Is.EqualTo(3));
        Assert.That(client.ParameterValue, Is.EqualTo(DBNull.Value));
    }

    [Test]
    public void Execute_WithEmptyParameterTable_Succeeds()
    {
        var action = new ExecuteAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "DELETE FROM records",
            Parameters = Parameters()
        };

        action.Execute(new ActionContext());

        Assert.That(client.ParameterCount, Is.Zero);
    }

    [Test]
    public void DefaultConstructors_UseProductionWiring()
    {
        Assert.That(new QueryAction().Result, Is.Null);
        Assert.That(new ExecuteAction().AffectedRows, Is.Zero);
    }

    [Test]
    public void Constructors_WithNullClient_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => new QueryAction(null!));
        Assert.Throws<ArgumentNullException>(() => new ExecuteAction(null!));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Query_WithBlankConnectionString_ReportsInvalidArgument(string connectionString)
    {
        var action = new QueryAction(client) { ConnectionString = connectionString, Sql = "SELECT 1" };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Execute_WithBlankSql_ReportsInvalidArgument()
    {
        var action = new ExecuteAction(client) { ConnectionString = "Host=localhost", Sql = "" };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Query_WithZeroTimeout_ReportsInvalidArgument()
    {
        var action = new QueryAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            TimeoutSeconds = 0
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Execute_WithNegativeTimeout_ReportsInvalidArgument()
    {
        var action = new ExecuteAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            TimeoutSeconds = -1
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Query_WithMissingNameColumn_ReportsInvalidArgument()
    {
        var parameters = new DataTable();
        parameters.Columns.Add("Value", typeof(object));
        var action = new QueryAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = parameters
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Query_WithMissingValueColumn_ReportsInvalidArgument()
    {
        var parameters = new DataTable();
        parameters.Columns.Add("Name", typeof(string));
        var action = new QueryAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = parameters
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Execute_WithUnnamedParameter_ReportsInvalidArgument()
    {
        var action = new ExecuteAction(client)
        {
            ConnectionString = "Host=localhost",
            Sql = "SELECT 1",
            Parameters = Parameters(("", 1))
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Query_WhenServerFails_ReportsDatabaseError()
    {
        client.Failure = new NpgsqlException("server unavailable");
        var action = new QueryAction(client) { ConnectionString = "Host=localhost", Sql = "SELECT 1" };

        AssertError(ErrorCodes.Database, action);
    }

    [Test]
    public void Execute_WhenServerFails_ReportsDatabaseError()
    {
        client.Failure = new NpgsqlException("server unavailable");
        var action = new ExecuteAction(client) { ConnectionString = "Host=localhost", Sql = "SELECT 1" };

        AssertError(ErrorCodes.Database, action);
    }

    [Test]
    public void Resources_HaveModuleAndActionNames()
    {
        var resources = Properties.Resources.ResourceManager;

        Assert.That(resources.GetString("PostgreSQL_FriendlyName"), Is.EqualTo("PostgreSQL"));
        Assert.That(resources.GetString("Query_FriendlyName"), Is.EqualTo("Query PostgreSQL"));
    }

    private static void AssertError(string code, PostgreSqlActionBase action)
    {
        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(code));
    }

    private static DataTable Parameters(params (string Name, object Value)[] values)
    {
        var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Value", typeof(object));
        foreach (var (name, value) in values)
        {
            table.Rows.Add(name, value);
        }

        return table;
    }

    private sealed class FakePostgreSqlClient : IPostgreSqlClient
    {
        public DataTable QueryResult { get; } = NewResult();
        public int AffectedRows { get; set; }
        public string? ConnectionString { get; private set; }
        public string? CommandText { get; private set; }
        public string? ParameterName { get; private set; }
        public object? ParameterValue { get; private set; }
        public int ParameterCount { get; private set; }
        public int TimeoutSeconds { get; private set; }
        public Exception? Failure { get; set; }

        public DataTable Query(string connectionString, NpgsqlCommand command)
        {
            Record(connectionString, command);
            if (Failure != null) throw Failure;
            return QueryResult;
        }

        public int Execute(string connectionString, NpgsqlCommand command)
        {
            Record(connectionString, command);
            if (Failure != null) throw Failure;
            return AffectedRows;
        }

        private void Record(string connectionString, NpgsqlCommand command)
        {
            ConnectionString = connectionString;
            CommandText = command.CommandText;
            TimeoutSeconds = command.CommandTimeout;
            ParameterCount = command.Parameters.Count;
            if (ParameterCount > 0)
            {
                ParameterName = command.Parameters[0].ParameterName;
                ParameterValue = command.Parameters[0].Value;
            }
        }

        private static DataTable NewResult()
        {
            var table = new DataTable();
            table.Columns.Add("name", typeof(string));
            return table;
        }
    }
}
