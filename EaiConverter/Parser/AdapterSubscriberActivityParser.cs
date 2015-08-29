using System;

namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class AdapterSubscriberActivityParser : IActivityParser
    {

        public const string AeSubscriberPropertyPrefix = "ae.aepalette.SharedProperties.";

        public Activity Parse(XElement inputElement)
        {
            var activity = new AdapterSubscriberActivity();

            activity.Name = inputElement.Attribute("name").Value;
            activity.Type = (ActivityType)inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element("config");

            activity.TpPluginEndpointName = XElementParserUtils.GetStringValue(configElement.Element("tpPluginEndpointName"));

            activity.UseRequestReply = XElementParserUtils.GetBoolValue(configElement.Element(AeSubscriberPropertyPrefix + "useRequestReply"));
            activity.TransportChoice = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "transportChoice"));
            activity.AdapterService = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "adapterService"));
            activity.TransportType = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "transportType"));
            activity.RvSubject = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvSubject"));
            activity.RvSessionService = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvSessionService"));
            activity.RvSessionNetwork = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvSessionNetwork"));
            activity.RvSessionDaemon = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvSessionDaemon"));
            activity.MessageFormat = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "msgFormat"));
            activity.RvCmSessionDefaultTimeLimit = XElementParserUtils.GetIntValue(configElement.Element(AeSubscriberPropertyPrefix + "rvCmSessionDefaultTimeLimit"));
            activity.RvCmSessionSyncLedger = XElementParserUtils.GetBoolValue(configElement.Element(AeSubscriberPropertyPrefix + "rvCmSessionSyncLedger"));
            activity.RvCmSessionLedgerFile = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvCmSessionLedgerFile"));
            activity.RvCmSessionName = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvCmSessionName"));
            activity.RvCmSessionRelayAgent = XElementParserUtils.GetStringValue(configElement.Element(AeSubscriberPropertyPrefix + "rvCmSessionRelayAgent"));
            activity.RvCmSessionRequireOldMessages = XElementParserUtils.GetBoolValue(configElement.Element(AeSubscriberPropertyPrefix + "rvCmSessionRequireOldMessages"));

            var OutputSchemaElement = configElement.Element(AeSubscriberPropertyPrefix + "outputMeta");

            activity.OutputSchema = XElementParserUtils.GetStringValue(OutputSchemaElement.Element("aeMeta"));

            return activity;
        }
    }
}
