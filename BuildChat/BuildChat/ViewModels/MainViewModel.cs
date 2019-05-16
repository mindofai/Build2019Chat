using BuildChat.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildChat.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        private string _message;
        private ObservableCollection<MessageModel> _messages;
        private bool _isConnected;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }


        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageModel> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageModel>();
            SendMessageCommand = new Command(async () => { await SendMessage(Name, Message); });
            ConnectCommand = new Command(async () => await Connect());
            DisconnectCommand = new Command(async () => await Disconnect());

            IsConnected = false;
        }

        async Task Connect()
        {
            Messages.Add(new MessageModel() { User = Name, Message = $"{Name} has joined", IsOwnMessage = true });
            IsConnected = true;
        }

        async Task SendMessage(string user, string message)
        {
                Messages.Add(new MessageModel() { User = Name, Message = Message, IsOwnMessage = true });
        }

        async Task Disconnect()
        {
                Messages.Add(new MessageModel() { User = Name, Message = $"{Name} has joined", IsOwnMessage = true });
                IsConnected = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
