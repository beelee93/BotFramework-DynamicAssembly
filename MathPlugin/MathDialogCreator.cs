using Microsoft.Bot.Builder.Dialogs;
using PluginSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathPlugin
{
    [Plugin(Intent = "math", Description = "A simple math plugin")]
    public class MathDialogCreator : IDialogCreator
    {
        public bool ForwardMessage => false;

        public IDialog<object> CreateNewDialog(IDialogContext context)
        {
            return new MathDialog();
        }
    }
}
