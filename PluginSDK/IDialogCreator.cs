using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginSDK
{
    public interface IDialogCreator
    {
        IDialog<object> CreateNewDialog(IDialogContext context);
        bool ForwardMessage { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute: Attribute
    {
        public string Intent { get; set; }
        public string Description { get; set; }
    }
}
