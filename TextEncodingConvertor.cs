using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace EncodingConvertor
{
    class TextEncodingConvertor :
        ITextEncodingConvertor
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ViewModel parent;

        private Encoding encoding;

        public static ObservableCollection<string> AllEncodings { get; } =
            new ObservableCollection<string>(Encoding.GetEncodings().Select(x => x.Name));

        public ICommand CloseCommand { get; }

        public TextEncodingConvertor(ViewModel parent)
        {
            CloseCommand = CommandFactory.Create(CloseCommandExecute);
            this.parent = parent;
            parent.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.Data))
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
                if (null == value)
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
                if (null != encoding)
                {
                    parent.Data = encoding.GetBytes(value);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseCommandExecute(object _)
        {
            parent.Convertors.Remove(this);
        }
    }
}
