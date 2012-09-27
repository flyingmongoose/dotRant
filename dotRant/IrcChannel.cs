﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotRant
{
    class IrcChannel : IIrcChannel
    {
        internal enum State
        {
            WaitingForJoin,
            WaitingForNames,
            Joined
        }

        readonly IrcConnection _connection;
        readonly string _name;
        readonly List<Guid> _users;
        readonly Dictionary<Guid, string> _userStates;

        internal readonly TaskCompletionSource<IIrcChannel> _loaded;
        internal volatile State _state;

        internal string _topic = "";
        internal string _topicCreator = null;
        internal DateTime _topicTime = DateTime.MinValue;

        public IrcChannel(IrcConnection connection, string name)
        {
            _connection = connection;
            _name = name;
            _users = new List<Guid>();
            _userStates = new Dictionary<Guid, string>();
            _loaded = new TaskCompletionSource<IIrcChannel>();
            _state = State.WaitingForJoin;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Topic
        {
            get { return _topic; }
            set
            {
                if (_topic != value)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public string TopicCreator
        {
            get { return _topicCreator };
        }

        public DateTime TopicTime
        {
            get { return _topicTime; }
        }

        public IIrcConnection Connection
        {
            get { return _connection; }
        }

        public IList<string> Users
        {
            get
            {
                return _users
                    .Select(_connection.Name)
                    .ToList();
            }
        }

        public Task Send(string message)
        {
            return _connection.SendCommand("PRIVMSG", _name, message.AsMultiParameter());
        }

        internal void AddName(string name)
        {
            var prefixes = ParsePrefixes(ref name);
            Guid nameGuid = _connection.Name(name);
            _users.Add(nameGuid);
            _userStates.Add(nameGuid, prefixes);
        }

        private string ParsePrefixes(ref string name)
        {
            StringBuilder prefixes = new StringBuilder();
            while (name[0] == '+' || name[0] == '@' || name[0] == '!')
            {
                prefixes.Append(name[0]);
                name = name.Substring(1);
            }
            return prefixes.ToString();
        }
    }
}
