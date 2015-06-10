// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resolver.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Reflection;
    using System.Xml;

    using Microsoft.BizTalk.XLANGs.BTXEngine;
    using Microsoft.XLANGs.BaseTypes;

    using SolidsoftReply.Esb.Libraries.Resolution;
    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;

    /// <summary>
    /// Class to resolve and get info from the Service Directory
    /// </summary>
    [Serializable]
    public class Resolver
    {
        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="serviceName">Service name</param>
        /// <param name="messageType">Message type</param>        
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(string serviceName, string messageType)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                serviceName,
                null,
                null,
                messageType,
                null,
                null,
                MessageDirectionTypes.NotSpecified,
                null,
                string.Empty,
                null,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="serviceName">Service name</param>
        /// <param name="messageType">Message type</param>    
        /// <param name="messageDirection">Direction of the message</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(string serviceName, string messageType, MessageDirectionTypes messageDirection)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                serviceName,
                null,
                null,
                messageType,
                null,
                null,
                messageDirection,
                null,
                string.Empty,
                null,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                MessageDirectionTypes.NotSpecified,
                null,
                string.Empty,
                null,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="policyName">Policy name</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(string policyName, Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                MessageDirectionTypes.NotSpecified,
                null,
                policyName,
                null,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string policyName, Version version, Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                MessageDirectionTypes.NotSpecified,
                null,
                policyName,
                version,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string messageType, MessageDirectionTypes messageDirection, XLANGMessage messageIn)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                null,
                null,
                null,
                messageType,
                null,
                null,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                string.Empty,
                null,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="policyName">Policy name</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string messageType, MessageDirectionTypes messageDirection, XLANGMessage messageIn, string policyName)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                null,
                null,
                null,
                messageType,
                null,
                null,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                policyName,
                null,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string messageType,
            MessageDirectionTypes messageDirection,
            XLANGMessage messageIn,
            string policyName,
            Version version)
        {
            return new Directives(Resolution.Resolver.Resolve(
                null,
                null,
                null,
                null,
                messageType,
                null,
                null,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                policyName,
                version,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="policyName">Policy name</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            XLANGMessage messageIn,
            string policyName)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                policyName,
                null,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            XLANGMessage messageIn,
            string policyName,
            Version version)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                policyName,
                version,
                null));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                null,
                string.Empty,
                null,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            string policyName,
            Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                null,
                policyName,
                null,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            string policyName,
            Version version,
            Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                null,
                policyName,
                version,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            XLANGMessage messageIn,
            Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                string.Empty,
                null,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            XLANGMessage messageIn,
            string policyName,
            Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                policyName,
                null,
                parameters));
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="messageIn">Message to be transformed</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            XLANGMessage messageIn,
            string policyName,
            Version version,
            Parameters parameters)
        {
            return new Directives(Resolution.Resolver.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                messageDirection,
                GetXmlDocumentFromXlangMessage(messageIn),
                policyName,
                version,
                parameters));
        }

        /// <summary>
        /// Invalidate the cache
        /// </summary>
        public static void InvalidateCache()
        {
            Resolution.Resolver.InvalidateCache();
        }

        /// <summary>
        /// Return an XML document from an XLANG message.
        /// </summary>
        /// <param name="bizTalkMessage">
        /// The BizTalk message.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        private static XmlDocument GetXmlDocumentFromXlangMessage(XLANGMessage bizTalkMessage)
        {
            // Invoke private method to unwrap message.
            var unwrappedBizTalkMessage =
            (BTXMessage)
                bizTalkMessage.GetType()
                    .GetMethod("Unwrap", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(bizTalkMessage, null);

            // Get the XML content (if any) of the body part
            var part = unwrappedBizTalkMessage.BodyPart ?? unwrappedBizTalkMessage[0];

            if (part != null)
            {
                try
                {
                    return (XmlDocument)part.RetrieveAs(typeof(XmlDocument));
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}
