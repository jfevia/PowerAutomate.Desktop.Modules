using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Supplies a hand-written Npgsql client stub for each action test.
/// </summary>
public abstract class PostgreSqlActionFixture
{
    /// <summary>
    ///     Records bound commands without contacting a server.
    /// </summary>
    protected PostgreSqlClientStub Client { get; private set; }

    /// <summary>
    ///     Initializes the client before NUnit setup.
    /// </summary>
    protected PostgreSqlActionFixture()
    {
        Client = new PostgreSqlClientStub();
    }

    /// <summary>
    ///     Prevents command state from leaking between tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        Client = new PostgreSqlClientStub();
    }

    /// <summary>
    ///     Checks the error category exposed to desktop flows.
    /// </summary>
    /// <param name="code">The expected PAD error code.</param>
    /// <param name="action">The action expected to fail.</param>
    protected void AssertError(string code, PostgreSqlActionBase action)
    {
        var exception = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(exception.Name, Is.EqualTo(code));
    }

    /// <summary>
    ///     Creates a parameter table with the required columns.
    /// </summary>
    /// <returns>An empty table ready for parameter rows.</returns>
    protected DataTable CreateParameterTable()
    {
        var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Value", typeof(object));
        return table;
    }

    /// <summary>
    ///     Creates a table containing one bound parameter.
    /// </summary>
    /// <param name="name">The SQL placeholder including its prefix.</param>
    /// <param name="value">The scalar or database-null value.</param>
    /// <returns>A parameter table with one row.</returns>
    protected DataTable CreateParameters(string name, object value)
    {
        var table = CreateParameterTable();
        table.Rows.Add(name, value);
        return table;
    }

    /// <summary>
    ///     Executes an action using a fresh PAD context.
    /// </summary>
    /// <param name="action">The action to run.</param>
    protected void RunAction(PostgreSqlActionBase action)
    {
        var context = new ActionContext();
        action.Execute(context);
    }
}
