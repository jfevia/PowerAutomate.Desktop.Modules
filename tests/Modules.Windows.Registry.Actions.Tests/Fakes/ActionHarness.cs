// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

internal sealed class ActionHarness
{
    public ActionHarness()
    {
        Key = new FakeRegistryKey("HIVE\\Path");
        RegistryService = new FakeRegistryService(Key);
        Context = new RegistryContext(RegistryService);
    }

    public RegistryContext Context { get; }
    public FakeRegistryKey Key { get; }
    public FakeRegistryService RegistryService { get; }
}
