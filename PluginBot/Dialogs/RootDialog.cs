using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Autofac;
using PluginSDK;
using System.Diagnostics;
using System.Linq;
using Autofac.Core;

namespace PluginBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            
            if(activity.Text == "help")
            {
                await ShowHelp(context);
            }
            else
            {
                ILifetimeScope scope = WebApiApplication.GetContainer();
                try
                {
                    IDialogCreator obj = scope.ResolveNamed<IDialogCreator>(activity.Text);
                    context.Call(obj.CreateNewDialog(context), AfterDialog);
                }
                catch(Exception err)
                {
                    await context.PostAsync(err.Message);
                    await context.PostAsync("I don't understand");
                    context.Wait(MessageReceivedAsync);
                }
            }
        }

        private async Task AfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                await result;
            }
            catch (Exception err)
            {
                await context.PostAsync(err.Message);
            }
            await context.PostAsync("Anything else I can do for you?");
            context.Wait(MessageReceivedAsync);
        }

        private async Task ShowHelp(IDialogContext context)
        {
            string message = "";
            foreach (var v in WebApiApplication.PluginDescriptions)
            {
                message += $"**{v.Key}** : {v.Value}\n";
            }
            await context.PostAsync(message);

            await context.PostAsync("Anything else I can do for you?");
            context.Wait(MessageReceivedAsync);
        }
    }
}