using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathPlugin
{
    [Serializable]
    public class MathDialog : IDialog<object>
    {
        private int phase = 0;
        private int[] operands = { 0, 0 };
        private char oper = '+';

        public async Task StartAsync(IDialogContext context)
        {
            await MessageReceivedAsync(context, null);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity res;
            try
            {
                switch (phase)
                {
                    case 0:
                        await context.PostAsync("Type the first integer operand");
                        phase = 1;
                        context.Wait(MessageReceivedAsync);
                        break;
                    case 1:
                        res = await result as Activity;
                        operands[0] = Int32.Parse(res.Text);
                        await context.PostAsync("Type the operator");
                        phase = 2;
                        context.Wait(MessageReceivedAsync);
                        break;
                    case 2:
                        res = await result as Activity;
                        oper = res.Text[0];
                        await context.PostAsync("Type the second integer operand");
                        phase = 3;
                        context.Wait(MessageReceivedAsync);
                        break;
                    case 3:
                        res = await result as Activity;
                        operands[1] = Int32.Parse(res.Text);
                        float x = Calculate();
                        await context.PostAsync("Result : " + x.ToString());
                        context.Done(true);
                        break;
                }
            }
            catch(Exception err)
            {
                context.Fail(err);
            }
        }

        private float Calculate()
        {
            switch(oper)
            {
                case '+':
                    return operands[0] + operands[1];
                case '-':
                    return operands[0] - operands[1];
                case '*':
                    return operands[0] * operands[1];
                case '/':
                    return operands[0] / operands[1];
            }
            throw new ArgumentException("Unknown operator");
        }
    }
}
