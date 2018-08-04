using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HelloPlugin
{
    [Serializable]
    public class HelloDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if(context.UserData.ContainsKey("user"))
            {
                string user = context.UserData.GetValue<string>("user");
                await context.PostAsync("Hello " + user);
                context.Done(true);
            }
            else
            {
                await context.PostAsync("I need your name");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var res = await result as Activity;
                await context.PostAsync("Hello " + res.Text);
                context.UserData.SetValue("user", res.Text);
                context.Done(true);
            }
            catch(Exception error)
            {
                context.Fail(error);
            }
        }
    }
}