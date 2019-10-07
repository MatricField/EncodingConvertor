using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EncodingConvertor
{
    class ViewModel :
        INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private byte[] Data { get; set; }

        public ObservableCollection<Convertor> Convertors { get; }

        public ICommand GetAddConvertorCommand => new AddConvertorCommand(this);

        public ViewModel()
        {
            Data = Array.Empty<byte>();
            Convertors = new ObservableCollection<Convertor>();
            var defaultEncoding = Encoding.GetEncoding(CultureInfo.CurrentUICulture.TextInfo.OEMCodePage).WebName;
            var convertor = new Convertor(this)
            {
                EncodingName = defaultEncoding
            };
            Convertors.Add(convertor);

            var rand = new Random();
            var randEnco = Convertor.AllEncodings[rand.Next(Convertor.AllEncodings.Count)];
            while(randEnco == defaultEncoding)
            {
                randEnco = Convertor.AllEncodings[rand.Next(Convertor.AllEncodings.Count)];
            }
            convertor = new Convertor(this)
            {
                EncodingName = randEnco
            };
            Convertors.Add(convertor);
        }

        private class AddConvertorCommand : ICommand
        {
            private readonly ViewModel VM;

            public event EventHandler CanExecuteChanged;

            public AddConvertorCommand(ViewModel VM) =>
                this.VM = VM;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                VM.Convertors.Add(new Convertor(VM));
            }
        }

        public class Convertor :
            INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private ViewModel parent;

            private Encoding encoding;

            public static ObservableCollection<string> AllEncodings { get; } =
                new ObservableCollection<string>(Encoding.GetEncodings().Select(x => x.Name));

            public ICommand GetCloseCommand { get; }

            internal Convertor(ViewModel parent)
            {
                GetCloseCommand = new CloseCommand(this);
                this.parent = parent;
                parent.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Data))
                    {
                        OnPropertyChanged(nameof(Text));
                    }
                };
            }

            public string EncodingName
            {
                get => encoding?.WebName ?? "";
                set
                {
                    if(null == value)
                    {
                        encoding = null;
                        OnPropertyChanged();
                        OnPropertyChanged(nameof(Text));
                    }
                    else
                    {
                        var newEncoding = Encoding.GetEncoding(value);
                        if (newEncoding != encoding)
                        {
                            encoding = newEncoding;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Text));
                        }
                    }
                }
            }

            public string Text
            {
                get => encoding?.GetString(parent.Data);
                set
                {
                    if(null != encoding)
                    {
                        parent.Data = encoding.GetBytes(value);
                    }
                }
            }

            protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            class CloseCommand : ICommand
            {
                private Convertor parent;

                public event EventHandler CanExecuteChanged;

                public CloseCommand(Convertor parent) =>
                    this.parent = parent;

                public bool CanExecute(object parameter) => true;

                public void Execute(object parameter) =>
                    parent.parent.Convertors.Remove(parent);
            }
        }
    }
}
