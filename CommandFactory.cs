using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EncodingConvertor
{
    public class CommandFactory
    {
        public delegate bool CanExecuteF(object parameter);
        public delegate void ExecuteF(object parameter);
        public delegate void OnCanExecuteChanged(EventArgs args);

        public static ICommand Create(ExecuteF exe, CanExecuteF canExe = default) =>
            new FuncCommand(exe, canExe);

        public static ICommand Create(ExecuteF exe, CanExecuteF canExe, out OnCanExecuteChanged onExeChanged) =>
            new FuncCommand(exe, canExe, out onExeChanged);

        private class FuncCommand : ICommand
        {
            private readonly CanExecuteF CanExecuteFunc;
            private readonly ExecuteF ExecuteFunc;

            public event EventHandler CanExecuteChanged;

            public FuncCommand(ExecuteF exe, CanExecuteF canExe)
            {
                if(null == exe)
                {
                    throw new ArgumentNullException();
                }
                CanExecuteFunc = canExe;
                ExecuteFunc = exe;
            }

            public FuncCommand(ExecuteF exe, CanExecuteF canExe, out OnCanExecuteChanged onExeChanged):
                this(exe, canExe)
            {
                onExeChanged = (args) => CanExecuteChanged?.Invoke(this, args);
            }

            public bool CanExecute(object parameter) =>
                CanExecuteFunc?.Invoke(parameter) ?? true;

            public void Execute(object parameter) =>
                ExecuteFunc.Invoke(parameter);
        }
    }
}
