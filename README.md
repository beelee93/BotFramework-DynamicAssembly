# PluginBot

This solution shows how to incorporate dynamic assembly loading into a MS Bot Framework project.

The issue to tackle is that when an externally-defined dialog is deserialized, the framework will be unable 
to perform the deserialization as it fails to resolve an external assembly name (eventhough it has already been loaded 
into the Application Domain)

As a workaround, a static record of all loaded assemblies is maintained in the WebApiApplication class. Then a handler for 
the current application domain's ResolveAssembly is added to aid the lookup for the external assembly.

Please consult Global.asax.cs to see how it is done.

The resolution of the loaded services are done in the RootDialog.cs