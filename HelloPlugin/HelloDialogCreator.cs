using Microsoft.Bot.Builder.Dialogs;
using PluginSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloPlugin
{
    [Plugin(Intent ="greet", Description ="To greet you, the user")]
    public class HelloDialogCreator : IDialogCreator
    {
        public bool ForwardMessage => false;

        public IDialog<object> CreateNewDialog(IDialogContext context)
        {
            return new HelloDialog();
        }
    }
}
