using System.ComponentModel;
using System.Windows.Input;

namespace EncodingConvertor
{
    public interface ITextEncodingConvertor : INotifyPropertyChanged
    {
        ICommand CloseCommand { get; }
        string EncodingName { get; set; }
        string Text { get; set; }
    }
}