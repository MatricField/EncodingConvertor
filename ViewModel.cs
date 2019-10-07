using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace EncodingConvertor
{
    class ViewModel :
        INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        internal byte[] Data { get; set; }

        public ObservableCollection<ITextEncodingConvertor> Convertors { get; }

        public ICommand AddConvertorCommand { get; }

        public ViewModel()
        {
            Data = Array.Empty<byte>();
            Convertors = new ObservableCollection<ITextEncodingConvertor>();
            AddConvertorCommand = CommandFactory.Create(AddConvertorCommandExecute);
            var defaultEncoding = Encoding.GetEncoding(CultureInfo.CurrentUICulture.TextInfo.OEMCodePage).WebName;
            var convertor = new TextEncodingConvertor(this)
            {
                EncodingName = defaultEncoding
            };
            Convertors.Add(convertor);

            var rand = new Random();
            var randEnco = TextEncodingConvertor.AllEncodings[rand.Next(TextEncodingConvertor.AllEncodings.Count)];
            while(randEnco == defaultEncoding)
            {
                randEnco = TextEncodingConvertor.AllEncodings[rand.Next(TextEncodingConvertor.AllEncodings.Count)];
            }
            convertor = new TextEncodingConvertor(this)
            {
                EncodingName = randEnco
            };
            Convertors.Add(convertor);
        }

        private void AddConvertorCommandExecute(object _) =>
            Convertors.Add(new TextEncodingConvertor(this));
    }
}
