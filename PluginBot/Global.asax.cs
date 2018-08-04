using System.Web.Http;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using Microsoft.Bot.Connector;
using System.Reflection;
using System.IO;
using System;
using PluginSDK;
using System.Diagnostics;
using System.Collections.Generic;

namespace PluginBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IContainer Externals { get; private set; }
        private static ILifetimeScope _container = null;

        private static Dictionary<string, Assembly> loaded = null;
        private static Dictionary<string, string> descriptions = null;
        
        public static Dictionary<string, string> PluginDescriptions
        {
            get
            {
                return descriptions;
            }
        }

        public static ILifetimeScope GetContainer()
        {
            if (_container == null)
            {
                Debug.WriteLine(">> Creating Lifetime Scope...");
                _container = Externals.BeginLifetimeScope();
            }
            return _container;
        }

        protected void Application_Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Conversation.UpdateContainer(
            builder =>
            {
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: [https://github.com/Microsoft/BotBuilder-Azure](https://github.com/Microsoft/BotBuilder-Azure)
                var store = new InMemoryDataStore();

                // Other storage options
                // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
                // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();
            });
            LoadDependencies();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                return loaded[args.Name];
            }
            catch
            {
                return null;
            }
        }

        protected void LoadDependencies()
        {
            if (loaded == null) loaded = new Dictionary<string, Assembly>();
            if (descriptions == null) descriptions = new Dictionary<string, string>();

            var bd = new ContainerBuilder();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Project\\PluginBot\\plugins\\");
            string[] files = Directory.GetFiles(path, "*.dll");

            foreach(string file in files)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(file);
                }
                finally { }

                if (assembly == null)
                    continue;

                foreach (Type type in assembly.ExportedTypes)
                {
                    if (!typeof(IDialogCreator).IsAssignableFrom(type))
                        continue;

                    PluginAttribute attr = null;

                    try
                    {
                        attr = type.GetCustomAttribute<PluginAttribute>();
                    }
                    finally { }

                    if (attr == null)
                        continue;

                    bd.RegisterType(type)
                        .As<IDialogCreator>()
                        .SingleInstance()
                        .Named(attr.Intent, typeof(IDialogCreator));

                    Debug.WriteLine($">> Registered plugin [{attr.Intent}]");

                    if(!descriptions.ContainsKey(attr.Intent))
                        descriptions.Add(attr.Intent, attr.Description);
                }

                if (!loaded.ContainsKey(assembly.GetName().Name))
                    loaded.Add(assembly.FullName, assembly);
                
            }

            Externals = bd.Build();
        }

    }
}
