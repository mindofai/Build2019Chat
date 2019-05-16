using BuildChat.Models;
using Microsoft.AspNetCore.SignalR.Client;
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
        private string _group;
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
        public string Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
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

        private HubConnection hubConnection;

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

            hubConnection = new HubConnectionBuilder()
         .WithUrl($"http://signalrdemomsph.azurewebsites.net/chatHub")
         .Build();


            hubConnection.On<string>("JoinGroupMessage", (user) =>
            {
                Messages.Add(new MessageModel() { User = Name, Message = $"{user} has joined the chat group", IsSystemMessage = true });
            });

            hubConnection.On<string>("LeaveGroupMessage", (user) =>
            {
                Messages.Add(new MessageModel() { User = Name, Message = $"{user} has left the chat group", IsSystemMessage = true });
            });

            hubConnection.On<string,string>("ReceiveGroupMessage", (user, message) =>
            {
                Messages.Add(new MessageModel() { User = user, Message = message, IsSystemMessage = false, IsOwnMessage = Name == user });
            });


        }

        async Task Connect()
        {
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("JoinGroupChat", Group, Name);

            IsConnected = true;
        }

        async Task SendMessage(string user, string message)
        {
            await hubConnection.InvokeAsync("SendGroupMessage", Group, user, message);
        }

        async Task Disconnect()
        {
            await hubConnection.InvokeAsync("LeaveGroupChat", Group, Name);
            await hubConnection.StopAsync();

            IsConnected = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
