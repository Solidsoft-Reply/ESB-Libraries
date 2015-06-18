// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsGovernance.cs" company="Solidsoft Reply Ltd.">
//   Copyright 2015 Solidsoft Reply Limited.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable UnusedMethodReturnValue.Global

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System;
    ////using System.Diagnostics;

    using BTS;

    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;

    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution;
    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;

    using Action = WCF.Action;

    /// <summary>
    /// Helper methods specific to Governance pipeline components.
    /// </summary>
    internal static class ExtensionMethodsGovernance
    {
        /// <summary>
        /// Handles messages that must be queued for a currently closed service window.
        /// </summary>
        /// <param name="inMsg">The pipeline message.</param>
        /// <param name="directive">The resolver directive.</param>
        /// <returns>True, if the message was enqueued.</returns>
        public static bool QueueForServiceWindow(this IBaseMessage inMsg, Directive directive)
        {
            // Service Window
            if (!directive.InServiceWindow)
            {
                // The message should be queued and resent when the window opens.
                // There is no simple way in a pipeline to enqueue a message to wait 
                // for a service window.
                // TODO: Extend the logic to invert control to a pluggable enqueuing component. 
            }

            return false;
        }

        /// <summary>
        /// Promotes properties defined in an XSD schema for an XML message.
        /// </summary>
        /// <param name="message">the XML message.</param>
        /// <param name="pc">Pipeline context.</param>
        /// <returns>The message with promoted properties.</returns>
        public static IBaseMessage PromotePropertiesBySchema(this IBaseMessage message, IPipelineContext pc)
        {
            // Write the PromotePropertiesOnly property on the message
            message.Context.Write("PromotePropertiesOnly", Resources.UriXmlNormProperties, true);

            var xmlDasmComp = new XmlDasmComp { AllowUnrecognizedMessage = true, ValidateDocument = false };
            xmlDasmComp.Disassemble(pc, message);
            var outMsg = xmlDasmComp.GetNext(pc);
            outMsg.BodyPart.Data.Position = 0;
            return outMsg;
        }

        /// <summary>
        /// Write and/or promote and BTS properties defined by the policy
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="directive">A resolver directive.</param>
        /// <returns>The message with written and/or promoted BTS properties.</returns>
        public static IBaseMessage WriteAndPromoteBtsProperties(this IBaseMessage message, Directive directive)
        {
            // [fvar] Sets properties on the message.
            Func<BtsProperties, IBaseMessage> setProperties = properties =>
                {
                    foreach (var btsProperty in properties)
                    {
                        if (btsProperty.Value.Promoted)
                        {
                            message.SetPromotedProperty(
                                btsProperty.Value.Name,
                                btsProperty.Value.Namespace,
                                btsProperty.Value.Value);
                        }
                        else
                        {
                            message.SetProperty(
                                btsProperty.Value.Name,
                                btsProperty.Value.Namespace,
                                btsProperty.Value.Value);
                        }
                    }

                    return message;
                };

            return directive.BtsProperties == null ? message : setProperties(directive.BtsProperties);
        }

        /// <summary>
        /// Set the transport type and location properties on a pipeline message.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="directive">the resolver directive.</param>
        /// <returns>The pipeline message with transport type and location properties.</returns>
        public static IBaseMessage SetDynamicPortProperties(this IBaseMessage message, Directive directive)
        {
            // [fvar] The outbound transport type BizTalk property.
            var outboundTransportType = ExtensionMethodsGeneral.AsCachedVar(() => new OutboundTransportType());

            // [fvar] The outbound transport location BizTalk property.
            var outboundTransportLocation = ExtensionMethodsGeneral.AsCachedVar(() => new OutboundTransportLocation());

            // [fvar] The message with the dynamic port TransportType property, as required.
            Func<IBaseMessage> messageWithOutboundTransportProperty =
                () =>
                !string.IsNullOrEmpty(directive.TransportType)
                    ? message.SetProperty(
                        outboundTransportType().Name.Name,
                        outboundTransportType().Name.Namespace,
                        directive.TransportType)
                    : message;

            // Return the message with the dynamic port TransportType and TransportLocation properties, as required.
            return !string.IsNullOrEmpty(directive.EndPoint)
                       ? messageWithOutboundTransportProperty().SetProperty(
                           outboundTransportLocation().Name.Name,
                           outboundTransportLocation().Name.Namespace,
                           directive.EndPoint)
                       : messageWithOutboundTransportProperty();
        }

        /// <summary>
        /// Sets the SOAP action property on the message.  Sets properties for legacy
        /// SOAP adapter, WSE adapter and WCF adapter.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="directive">The resolver directive.</param>
        /// <returns>The pipeline message with SOAP action properties set.</returns>
        public static IBaseMessage SetSoapAction(this IBaseMessage message, Directive directive)
        {
            // [fvar] The cached BizTalk legacy SOAP adapter Action property.
            var btsSoapAction = ExtensionMethodsGeneral.AsCachedVar(() => new SOAPAction());

            // [fvar] The cached BizTalk legacy WSE adapter Action property
            var wcfSoapAction = ExtensionMethodsGeneral.AsCachedVar(() => new Action());

            // Return the message with the legacy SOAP, legacy WSE 2.0 and WCF adapter SOAP action properties.
            return string.IsNullOrEmpty(directive.SoapAction)
                ? message
                : message.SetProperty(btsSoapAction().Name.Name, btsSoapAction().Name.Namespace, directive.SoapAction)
                    .SetProperty("SoapAction", Resources.UriWseProperties, directive.SoapAction)
                    .SetProperty(wcfSoapAction().Name.Name, wcfSoapAction().Name.Namespace, directive.SoapAction);
        }

        /// <summary>
        /// Sets retry properties on a pipeline message.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="directive">The resolver directive.</param>
        /// <returns>A pipeline message with retry properties.</returns>
        public static IBaseMessage SetRetryProperties(this IBaseMessage message, Directive directive)
        {
            // If retry level is other than level zero, do not process further.
            if (directive.RetryLevel != 0)
            {
                return message;
            }

            // [fvar] RetryCount BizTalk property.
            var retryCount = ExtensionMethodsGeneral.AsCachedVar(() => new RetryCount());

            // [fvar] RetryInterval BizTalk property.
            var retryInterval = ExtensionMethodsGeneral.AsCachedVar(() => new RetryInterval());

            // [fvar] Message with retry count property set, as required.
            Func<IBaseMessage> messageWithRetryCountProp = 
                () =>
                directive.RetryCount > 0
                          ? message.SetProperty(
                              retryCount().Name.Name,
                              retryCount().Name.Namespace,
                              directive.RetryCount)
                          : message;

            // Return message with retry count and retry interval properties set, as required.
            return directive.RetryInterval > 0
                       ? messageWithRetryCountProp().SetProperty(
                           retryInterval().Name.Name,
                           retryInterval().Name.Namespace,
                           directive.RetryInterval)
                       : messageWithRetryCountProp();
        }

        /// <summary>
        /// Performs safe invocation on a function for processing pipeline messages.
        /// </summary>
        /// <param name="processMessage">The function for processing pipeline messages.</param>
        /// <param name="pc">The pipeline context.</param>
        /// <param name="inMsg">The pipeline message.</param>
        /// <param name="outMsgPerDirective">Indicates whether the disassembler should provide a separate message per directive.</param>
        /// <returns>Indicates if per-directive messaging is required.</returns>
        public static IBaseMessage SafeInvoke(this Func<IPipelineContext, IBaseMessage, bool, IBaseMessage> processMessage, IPipelineContext pc, IBaseMessage inMsg, bool outMsgPerDirective)
        {
            try
            {
                return processMessage(pc, inMsg, outMsgPerDirective);
            }
            catch (Exception ex)
            {
                try
                {
                    EventLog.WriteEntry("Application", ex.ToString(), System.Diagnostics.EventLogEntryType.Error, 3);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }

                throw;
            }
        }
    }
}
