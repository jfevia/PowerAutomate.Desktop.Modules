// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using PowerAutomate.Desktop.PetStore.Client;

namespace PowerAutomate.Desktop.Modules.PetStore.Actions;

public abstract class PetStoreActionBase : ActionBase
{
    protected PetStoreActionBase() : this(PetStoreContext.CreateDefault())
    {
    }

    protected PetStoreActionBase(PetStoreContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected PetStoreContext Context { get; }

    public override void Execute(ActionContext context)
    {
        try
        {
            Execute(Context.ClientFactory.CreateClient(), context);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    protected abstract void Execute(IPetStoreClient client, ActionContext context);
}