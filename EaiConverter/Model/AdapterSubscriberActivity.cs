using System;
using EaiConverter.Parser;
using System.Xml.Linq;
using EaiConverter.Model;

namespace EaiConverter.Model
{
    public class AdapterSubscriberActivity  : Activity
    {
        public string AdapterService
        {
            get;
            set;
        }

        public AdapterSubscriberActivity(string name, ActivityType type) : base (name, type)
        {
        }

        public AdapterSubscriberActivity() 
        {
        }

        public string TpPluginEndpointName
        {
            get;
            set;
        }

        public int? RvCmSessionDefaultTimeLimit
        {
            get;
            set;
        }
	
        public bool? UseRequestReply
        {
            get;
            set;
        }

        public string TransportChoice
        {
            get;
            set;
        }

        public bool? RvCmSessionRequireOldMessages
        {
            get;
            set;
        }

        public bool? RvCmSessionSyncLedger
        {
            get;
            set;
        }

        public string TransportType
        {
            get;
            set;
        }

        public string RvSubject
        {
            get;
            set;
        }

        public string RvSessionService
        {
            get;
            set;
        }
        public string RvSessionNetwork
        {
            get;
            set;
        }
        public string RvSessionDaemon
        {
            get;
            set;
        }
        public string MessageFormat
        {
            get;
            set;
        }
        public string RvCmSessionLedgerFile
        {
            get;
            set;
        }

        public string RvCmSessionName
        {
            get;
            set;
        }

        public string RvCmSessionRelayAgent
        {
            get;
            set;
        }

        public string OutputSchema
        {
            get;
            set;
        }
	}

}

