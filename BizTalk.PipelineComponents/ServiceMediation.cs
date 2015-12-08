// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMediation.cs" company="Solidsoft Reply Ltd.">
//   Copyright (c) 2015 Solidsoft Reply Limited. All rights reserved.
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Xml;

    using BTS;

    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Streaming;
    using Microsoft.RuleEngine;
    using Microsoft.Win32;
    using Microsoft.XLANGs.BaseTypes;

    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;
    using SolidsoftReply.Esb.Libraries.Facts;
    using SolidsoftReply.Esb.Libraries.Resolution;
    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;

    using Directive = SolidsoftReply.Esb.Libraries.Resolution.Directive;
    using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;

    /// <summary>
    ///     Implements ESB service mediation in the context of a pipeline component.
    /// </summary>
    /// <remarks>
    ///     This class implements a pipeline component that can be used in receive and
    ///     send BizTalk pipelines. The pipeline component gets a data stream, enforces
    ///     service mediation policy and outputs modified streams.  It can be used as
    ///     a disassembler or normal pipeline component.
    /// </remarks>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [Guid("59CFD96B-20EE-40ad-BFD0-319B59A0DDBC")]
    [ComVisible(false)]
    [DefaultProperty("Policy")]
    public class ServiceMediation : BaseCustomTypeDescriptor, IBaseComponent, IComponent, IPersistPropertyBag, IComponentUI, IDisassemblerComponent
    {
        #region Static Fields

        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly ResourceManager ResourceManager;

        /// <summary>
        /// MessageType BizTalk property.
        /// </summary>
        private static readonly PropertyBase MessageTypeProp = new MessageType();

        /// <summary>
        /// Schema Strong Name BizTalk property.
        /// </summary>
        private static readonly PropertyBase SchemaStrongNameProp = new SchemaStrongName();

        #endregion

        #region Fields

        /// <summary>
        ///     Dictionary of rule engine configuration values.
        /// </summary>
        private static readonly IDictionary ConfigValues;

        /// <summary>
        /// The schema strong name property.
        /// </summary>
        private static readonly PropertyBase SchemaStrongNameProperty = new SchemaStrongName();

        /// <summary>
        ///     Index value for current resolver directive being processed.
        /// </summary>
        private int current;

        /// <summary>
        ///     The original message.   This is used during disassembly.
        /// </summary>
        private IBaseMessage pipelineInMsg;

        /// <summary>
        ///     A resolver directive.
        /// </summary>
        private Directives directives;

        /// <summary>
        ///     Version of the pipeline component.
        /// </summary>
        private Version version = new Version(1, 0);

        /// <summary>
        /// The name of policy to be executed.
        /// </summary>
        private string policy;

        /// <summary>
        /// The version of policy to be executed.   If empty, the
        ///     latest version will be executed.
        /// </summary>
        private Version policyVersion;

        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="ServiceMediation"/> class.
        /// </summary>
        static ServiceMediation()
        {
            ResourceManager = new ResourceManager("SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties.Resources", typeof(ServiceMediation).Assembly);
            ConfigValues = ConfigurationManager.GetSection("Microsoft.RuleEngine") as IDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMediation"/> class.
        /// </summary>
        public ServiceMediation()
            : base(ResourceManager)
        {
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets an identifier for a binding access point.   The identifier should  be
        ///     in the form of a URL.   This may be used when the endpoint URL is known,
        ///     but other resolution is required, or as a 'virtual' URL.
        ///     This is based on UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescBindingAccessPoint")]
        [BtsPropertyName("PropBindingAccessPoint")]
        [BtsCategory("ESBResolutionValues")]
        public string BindingAccessPoint { get; set; }

        /// <summary>
        ///     Gets or sets the type (scheme) of URL for the target service.   This is based on
        ///     UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescBindingUrlType")]
        [BtsPropertyName("PropBindingUrlType")]
        [BtsCategory("ESBResolutionValues")]
        public string BindingUrlType { get; set; }

        /// <summary>
        ///     Gets or sets an XPath that addresses the element that contains the body of the message.
        /// </summary>
        /// <remarks>
        ///     If blank, the conceptual 'Root' node (parent of the Document Element) is used.
        /// </remarks>
        [Browsable(true)]
        [BtsDescription("DescBodyContainerXPath")]
        [BtsPropertyName("PropBodyContainerXPath")]
        [BtsCategory("ESBServiceMediation")]
        public string BodyContainerXPath { get; set; }

        /// <summary>
        ///     Gets the description of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescDescription")]
        [BtsPropertyName("PropDescription")]
        public string Description
        {
            get
            {
                return Resources.EsbServiceMediationComponentDescription;
            }
        }

        /// <summary>
        ///     Gets the component icon to use in BizTalk Editor.
        /// </summary>
        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return Resources.EsbServiceMediationDisassemberIcon.Handle;
            }
        }

        /// <summary>
        ///     Gets or sets the direction of message.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescMessageDirection")]
        [BtsPropertyName("PropMessageDirection")]
        [BtsCategory("ESBResolutionValues")]
        public MessageDirectionTypes MessageDirection { get; set; }

        /// <summary>
        ///     Gets or sets a role specifier for the message.   Equivalent to
        ///     messageLabel in WSDL 2.0
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        [Browsable(true)]
        [BtsDescription("DescMessageRole")]
        [BtsPropertyName("PropMessageRole")]
        [BtsCategory("ESBResolutionValues")]
        public string MessageRole { get; set; }

        /// <summary>
        ///     Gets or sets the type of message.   In a BizTalk context, this should generally be
        ///     equivalent to the BTS.MessageType property.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescMessageType")]
        [BtsPropertyName("PropMessageType")]
        [BtsCategory("ESBResolutionValues")]
        public string MessageType { get; set; }

        /// <summary>
        ///     Gets the name of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescName")]
        [BtsPropertyName("PropName")]
        public string Name
        {
            get
            {
                return Resources.EsbServiceMediationComponentName;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the service operation to be invoked.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescOperationName")]
        [BtsPropertyName("PropOperationName")]
        [BtsCategory("ESBResolutionValues")]
        public string OperationName { get; set; }

        /// <summary>
        /// Gets or sets the name of policy to be executed.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescPolicy")]
        [BtsPropertyName("PropPolicy")]
        [BtsCategory("ESBServiceMediation")]
        public string Policy
        {
            get
            {
                // Determine the policy name
                if (!string.IsNullOrWhiteSpace(this.policy))
                {
                    return this.policy;
                }

                try
                {
                    this.policy = ConfigurationManager.AppSettings[Resources.AppSettingsEsbDefaultPolicy];
                }
                catch
                {
                    this.policy = string.Empty;
                }

                return this.policy;
            }

            set
            {
                this.policy = value;
            }
        }

        /// <summary>
        ///     Gets or sets the version of Policy to be executed.   If empty, the
        ///     latest version will be executed.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescPolicyVersion")]
        [BtsPropertyName("PropPolicyVersion")]
        [BtsCategory("ESBServiceMediation")]
        public string PolicyVersion
        {
            get
            {
                if (this.policyVersion == null || (this.policyVersion.Major == 0 && this.policyVersion.Minor == 0))
                {
                    return string.Empty;
                }

                return this.policyVersion.ToString(2);
            }

            set
            {
                this.policyVersion = string.IsNullOrEmpty(value) ? null : new Version(value);
            }
        }

        /// <summary>
        ///     Gets or sets the human-friendly name of service provider.   This is equivalent to
        ///     the business entity name in UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescProviderName")]
        [BtsPropertyName("PropProviderName")]
        [BtsCategory("ESBResolutionValues")]
        public string ProviderName { get; set; }

        /// <summary>
        ///     Gets or sets the resolution data setting.  This controls the facts that are
        ///     asserted to the resolver.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescResolutionDataName")]
        [BtsPropertyName("PropResolutionDataName")]
        [BtsCategory("ESBResolutionValues")]
        public ResolutionData ResolutionData { get; set; }

        /// <summary>
        ///     Gets or sets a list of properties used as resolution facts.
        /// </summary>
        [BtsDescription("DescResolutionDataPropertiesName")]
        [BtsPropertyName("PropResolutionDataPropertiesName")]
        [BtsCategory("ESBResolutionValues")]
        public ResolutionDataPropertyList ResolutionDataProperties { get; set; }
        
        /// <summary>
        ///     Gets or sets the human-friendly name for target service.   This is equivalent to
        ///     the business service name in UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescServiceName")]
        [BtsPropertyName("PropServiceName")]
        [BtsCategory("ESBResolutionValues")]
        public string ServiceName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the BAM event stream should be synchronized with
        ///     the pipeline context.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescSynchronizeBam")]
        [BtsPropertyName("PropSynchronizeBam")]
        [BtsCategory("ESBServiceMediation")]
        public bool SynchronizeBam { get; set; }

        /// <summary>
        ///     Gets the version of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescVersion")]
        [BtsPropertyName("PropVersion")]
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Processes the original message and invokes the resolver
        /// </summary>
        /// <param name="pipelineContext">
        /// The IPipelineContext containing the current pipeline context.
        /// </param>
        /// <param name="inMsg">
        /// The IBaseMessage containing the message to be disassembled.
        /// </param>
        public void Disassemble(IPipelineContext pipelineContext, IBaseMessage inMsg)
        {
            this.directives = null;
            this.current = 0;
            this.pipelineInMsg = this.PrepareMessage(pipelineContext, this.ProcessMessage(pipelineContext, inMsg, true));
        }

        /// <summary>
        /// Implements Microsoft.BizTalk.Component.Interop.IComponent.Execute method.
        /// </summary>
        /// <param name="context">
        /// The pipeline context.
        /// </param>
        /// <param name="inMsg">
        /// The inbound message.
        /// </param>
        /// <returns>
        /// Processed input message with appended or prepended data.
        /// </returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        ///     the processing of the message in pipeline component.
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public IBaseMessage Execute(IPipelineContext context, IBaseMessage inMsg)
        {
            this.directives = null;
            Func<IPipelineContext, IBaseMessage, bool, IBaseMessage> pm = this.ProcessMessage;
            this.pipelineInMsg = pm.SafeInvoke(context, inMsg, false);
            return this.pipelineInMsg;
        }

        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component.
        /// </param>
        public void GetClassID(out Guid classid)
        {
            classid = new Guid(Resources.EsbServiceMediationComponentClassId);
        }

        /// <summary>
        /// Gets the next message from the message set resulting from the disassembler execution.
        /// A message is created per directive returned by the resolver.
        /// </summary>
        /// <param name="pipelineContext">
        /// The IPipelineContext containing the current pipeline context.
        /// </param>
        /// <returns>
        /// A pointer to the IBaseMessage containing the next message from the disassembled document.
        /// Returns NULL if there are no more messages left.
        /// </returns>
        public IBaseMessage GetNext(IPipelineContext pipelineContext)
        {
            return this.ProcessMessagePerDirective(pipelineContext);
        }

        /// <summary>
        /// Initialize the disassembler component.
        /// </summary>
        public void InitNew()
        {
            this.ProviderName = string.Empty;
            this.ServiceName = string.Empty;
            this.BindingAccessPoint = string.Empty;
            this.BindingUrlType = string.Empty;
            this.BodyContainerXPath = string.Empty;
            this.OperationName = string.Empty;
            this.MessageType = string.Empty;
            this.MessageDirection = MessageDirectionTypes.NotSpecified;
            this.MessageRole = string.Empty;
            this.Policy = string.Empty;
            this.PolicyVersion = string.Empty;
            this.version = new Version(1, 0);
        }

        /// <summary>
        /// Loads configuration property for component.
        /// </summary>
        /// <param name="pb">
        /// Configuration property bag.
        /// </param>
        /// <param name="errlog">
        /// Error status (not used in this code).
        /// </param>
        public void Load(IPropertyBag pb, int errlog)
        {
            this.Load(pb, errlog, false);
        }

        /// <summary>
        /// Safely loads configuration property for a wrapped component.
        /// </summary>
        /// <param name="pb">
        /// Configuration property bag.
        /// </param>
        /// <param name="errlog">
        /// Error status (not used in this code).
        /// </param>
        public void SafeLoadWhenWrapped(IPropertyBag pb, int errlog)
        {
            this.Load(pb, errlog, true);
        }

        /// <summary>
        /// Saves the current component configuration into the property bag.
        /// </summary>
        /// <param name="pb">
        /// Configuration property bag.
        /// </param>
        /// <param name="clearDirty">
        /// The parameter is not used.
        /// </param>
        /// <param name="saveAllProperties">
        /// The parameter is not used.
        /// </param>
        public void Save(IPropertyBag pb, bool clearDirty, bool saveAllProperties)
        {
            WritePropertyBag(pb, "providerName", this.ProviderName);
            WritePropertyBag(pb, "serviceName", this.ServiceName);
            WritePropertyBag(pb, "bindingAccessPoint", this.BindingAccessPoint);
            WritePropertyBag(pb, "bindingUrlType", this.BindingUrlType);
            WritePropertyBag(pb, "bodyContainerXPath", this.BodyContainerXPath);
            WritePropertyBag(pb, "operationName", this.OperationName);
            WritePropertyBag(pb, "messageType", this.MessageType);
            WritePropertyBag(pb, "messageDirection", this.MessageDirection.ToString());
            WritePropertyBag(pb, "messageRole", this.MessageRole);
            WritePropertyBag(pb, "rulePolicy", this.Policy);
            WritePropertyBag(pb, "rulePolicyVersion",  string.IsNullOrWhiteSpace(this.PolicyVersion) ? null : this.PolicyVersion);
            WritePropertyBag(pb, "resolutionData", this.ResolutionData.ToString());
            WritePropertyBag(pb, "resolutionDataProperties", this.ResolutionDataProperties.Count > 0 ? this.ResolutionDataProperties.Aggregate((p1, p2) => string.Format("{0}¦{1}", p1, p2)) : string.Empty);
            WritePropertyBag(pb, "synchronizeBam", this.SynchronizeBam ? bool.TrueString : bool.FalseString);
            WritePropertyBag(pb, "version", (this.version == null) ? null : this.version.ToString(2));
        }

        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build
        ///     of a BizTalk project.
        /// </summary>
        /// <param name="obj">
        /// Project system.
        /// </param>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        ///     of this component.
        /// </returns>
        public IEnumerator Validate(object obj)
        {
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare a message for Disassembly.  If multiple directives have been defined and the message
        /// contains non-seekable streams,the message is cloned using seekable streams.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <param name="inMsg">The BizTalk message.</param>
        /// <returns>The prepared BizTalk message.</returns>
        private IBaseMessage PrepareMessage(IPipelineContext pc, IBaseMessage inMsg)
        {
            if (this.directives.Count <= 1)
            {
                return inMsg;
            }

            var outMsg = inMsg;
            var nonSeekableParts = new List<int>();

            for (int partIdx = 0; partIdx < inMsg.PartCount; partIdx++)
            {
                if (!inMsg.Part(partIdx).GetOriginalDataStream().CanSeek)
                {
                    nonSeekableParts.Add(partIdx);
                }
            }

            if (nonSeekableParts.Count > 0)
            {
                // Create the cloned message
                var messageFactory = pc.GetMessageFactory();
                outMsg = messageFactory.CreateMessage();

                // Clone each part
                for (var partIdx = 0; partIdx < inMsg.PartCount; partIdx++)
                {
                    string partName;
                    var part = inMsg.GetPartByIndex(partIdx, out partName);

                    // Create and initilialize the new part
                    var newPart = messageFactory.CreateMessagePart();
                    newPart.Charset = part.Charset;
                    newPart.ContentType = part.ContentType;

                    // Get the original uncloned data stream
                    var originalStream = part.GetOriginalDataStream();
                    // Add the original stream to the Resource tracker to prevent it being
                    // disposed, in case we need to clone the same stream multiple times.
                    pc.ResourceTracker.AddResource(originalStream);

                    // Create the new part with a Virtual Stream, and add the the resource tracker
                    if (nonSeekableParts.Contains(partIdx))
                    {
                        newPart.Data = new VirtualStream(new ReadOnlySeekableStream(originalStream));
                    }
                    else 
                    {
                        newPart.Data = new VirtualStream(originalStream);
                    }

                    pc.ResourceTracker.AddResource(newPart.Data);

                    // Create and populate the property bag for the new message part.
                    newPart.GetOriginalDataStream().StreamAtStart();
                    newPart.PartProperties = messageFactory.CreatePropertyBag();
                    var partPoperties = part.PartProperties;

                    for (var propertyNo = 0; propertyNo < partPoperties.CountProperties; propertyNo++)
                    {
                        string propertyName, propertyNamespace;
                        var property = partPoperties.ReadAt(propertyNo, out propertyName, out propertyNamespace);
                        newPart.PartProperties.Write(propertyName, propertyNamespace, property);
                    }

                    // Add the new part to the cloned message
                    outMsg.AddPart(partName, newPart, partName == inMsg.BodyPartName);
                }

                // Copy the context from old to new
                outMsg.Context = inMsg.Context;
            }

            return outMsg;
        }

        /// <summary>
        /// Adds additional parts from the original message.
        /// </summary>
        /// <param name="inMsg">
        /// The original message.
        /// </param>
        /// <param name="outMsg">
        /// The per-directive outbound message.
        /// </param>
        /// <param name="bodyPartAdded">
        /// Flag indicates if a body part has already been added.
        /// </param>
        private static void AddPartsFromInMsg(IBaseMessage inMsg, IBaseMessage outMsg, bool bodyPartAdded)
        {
            if (!bodyPartAdded)
            {
                outMsg.AddPart("Body", inMsg.BodyPart, true);
            }

            // Add any other parts to outbound message
            for (var idx = 0; idx < inMsg.PartCount; idx++)
            {
                string partName;
                var part = inMsg.GetPartByIndex(idx, out partName);

                if (part != inMsg.BodyPart)
                {
                    outMsg.AddPart(partName, part, false);
                }
            }
        }

        /// <summary>
        /// Copy all properties from the in message to the out message.
        /// </summary>
        /// <param name="inMsg">
        /// The inbound message.
        /// </param>
        /// <param name="outMsg">
        /// The outbound message.
        /// </param>
        /// <returns>
        /// An <see cref="IBaseMessage"/>.
        /// </returns>
        private static IBaseMessage GetMsgWithCopiedProperties(IBaseMessage inMsg, IBaseMessage outMsg)
        {
            if (inMsg == null || outMsg == null)
            {
                return null;
            }

            for (var idx = 0; idx < inMsg.Context.CountProperties; idx++)
            {
                string name, nameSpace;
                var inProperty = inMsg.Context.ReadAt(idx, out name, out nameSpace);

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(nameSpace))
                {
                    continue;
                }

                if ((name == MessageTypeProp.Name.Name && nameSpace == MessageTypeProp.Name.Namespace) ||
                    (name == SchemaStrongNameProp.Name.Name && nameSpace == SchemaStrongNameProp.Name.Namespace))
                {
                    // Provide special handling for message type and schema strong name, which may have 
                    // been set after transform.
                    var outMsgTypeProperty = outMsg.Context.Read(name, nameSpace);

                    if (outMsgTypeProperty != null)
                    {
                        if (!outMsg.Context.IsPromoted(name, nameSpace))
                        {
                            outMsg.Context.Promote(name, nameSpace, inProperty);
                        }
                    }
                    else
                    {
                        outMsg.Context.Promote(name, nameSpace, inProperty);
                    }
                }
                else if (inMsg.Context.IsPromoted(name, nameSpace))
                {
                    outMsg.Context.Promote(name, nameSpace, inProperty);
                }
                else
                {
                    outMsg.Context.Write(name, nameSpace, inProperty);
                }
            }

            return outMsg;
        }

        /// <summary>
        /// Reads property value from property bag.
        /// </summary>
        /// <param name="pb">
        /// Property bag.
        /// </param>
        /// <param name="propName">
        /// Name of property.
        /// </param>
        /// <returns>
        /// Value of the property.
        /// </returns>
        private static object ReadPropertyBag(IPropertyBag pb, string propName)
        {
            object val = null;

            try
            {
                pb.Read(propName, out val, 0);
            }
            catch (ArgumentException)
            {
                return val;
            }
            catch (Exception ex)
            {
                throw new EsbPipelineComponentException(ex.Message);
            }

            return val;
        }

        /// <summary>
        /// Writes property values into a property bag.
        /// </summary>
        /// <param name="pb">
        /// Property bag.
        /// </param>
        /// <param name="propName">
        /// Name of property.
        /// </param>
        /// <param name="val">
        /// Value of property.
        /// </param>
        private static void WritePropertyBag(IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (Exception ex)
            {
                throw new EsbPipelineComponentException(ex.Message);
            }
        }

        /// <summary>
        /// Copies message context properties from one message to another.
        /// </summary>
        /// <param name="inMsg">The source message.</param>
        /// <param name="outMsg">The destination message.</param>
        private static void CopyContext(IBaseMessage inMsg, IBaseMessage outMsg)
        {
            outMsg.Context = PipelineUtil.CloneMessageContext(inMsg.Context);

            // Set the BTS.MessageType property.  This may be overridden by BtsProperties below.
            outMsg.Context.Promote("MessageID", Resources.UriBtsSystemProperties, outMsg.MessageID.ToString());
        }

        /// <summary>
        ///     Indicates whether static support has been configured for the rule engine.
        /// </summary>
        /// <returns>
        ///     True, if static support is being used.  Otherwise false.
        /// </returns>
        private static bool IsStaticSupport()
        {
            const string Key = "StaticSupport";

            if (ConfigValues != null && ConfigValues.Contains(Key))
            {
                return int.Parse((string)ConfigValues[Key], CultureInfo.CurrentCulture) > 0;
            }

            // Test width of integer pointer to determine 32 or 64 bit OS.
            var size = IntPtr.Size == 8;
            var registryKey =
                Registry.LocalMachine.OpenSubKey(
                    size
                        ? Resources.RegKeyWow6432
                        : Resources.RegKey);

            if (registryKey == null)
            {
                return false;
            }

            var value = registryKey.GetValue(Key);
            registryKey.Close();

            if (value != null)
            {
                return (int)value > 0;
            }

            return false;
        }

        /// <summary>
        ///     Build the connection string to the business rule store
        /// </summary>
        /// <returns>Connection string for the business rule store</returns>
        private static string GetRuleStoreConnectionString()
        {
            RegistryKey regKey = null;
            object dataBaseName;
            object dataBaseServer;

            try
            {
                regKey = Registry.LocalMachine.OpenSubKey(Resources.RegKey);

                if (regKey == null)
                {
                    regKey = Registry.LocalMachine.OpenSubKey(Resources.RegKeyWow6432);

                    if (regKey == null)
                    {
                        throw new RuleStoreConnectionParametersException(Resources.ExceptionNoConnectionParameters);
                    }
                }

                dataBaseName = regKey.GetValue(Resources.RegKeyDatabaseName);
                dataBaseServer = regKey.GetValue(Resources.RegKeyDatabaseServer);

                if (dataBaseName == null || dataBaseServer == null)
                {
                    throw new RuleStoreConnectionParametersException(Resources.ExceptionNoConnectionParameters);
                }
            }
            finally
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }

            return string.Format(Resources.ConnectionString, dataBaseServer, dataBaseName);
        }

        /// <summary>
        /// Validate an XML document using a BRE policy.
        /// </summary>
        /// <param name="xmlDocument">
        /// The XML message document to be validated.
        /// </param>
        /// <param name="documentType">
        /// The document type of the XML document, as used in the validation rules.
        /// </param>
        /// <param name="directive">
        /// The directive currently being processed.
        /// </param>
        /// <returns>
        /// A <see cref="Validations"/> object containing the results of all validations.
        /// </returns>
        private static Validations ValidateDocument(XmlNode xmlDocument, string documentType, Directive directive)
        {
            var validations = new Validations();

            if (directive == null)
            {
                return validations;
            }

            if (string.IsNullOrWhiteSpace(directive.ValidationPolicyName))
            {
                return validations;
            }

            var trace = Convert.ToBoolean(ConfigurationManager.AppSettings[Resources.AppSettingsEsbBreTrace]);
            var tempFileName = string.Empty;

            if (trace)
            {
                var traceFileFolder = ConfigurationManager.AppSettings[Resources.AppSettingsEsbBreTraceFileLocation];

                if (!string.IsNullOrWhiteSpace(traceFileFolder))
                {
                    while (traceFileFolder.EndsWith(@"\"))
                    {
                        traceFileFolder = traceFileFolder.Substring(0, traceFileFolder.Length - 1);
                    }
                }

                if (string.IsNullOrWhiteSpace(traceFileFolder))
                {
                    traceFileFolder = @".";
                }

                tempFileName = string.Format(
                    @"{0}\ValidationPolicyTrace_{1}_{2}.txt",
                    traceFileFolder,
                    DateTime.Now.ToString("yyyyMMdd"),
                    Guid.NewGuid());
            }

            // Determine if static support is being used by rule engine and create the array of short-term facts 
            var shortTermFacts = IsStaticSupport()
                                          ? new object[]
                                                {
                                                   new TypedXmlDocument(documentType, xmlDocument), validations 
                                                }
                                          : new object[]
                                                {
                                                    new TypedXmlDocument(documentType, xmlDocument), validations, 
                                                    new XmlHelper()
                                                };

            // Assert the XML messsage and a validation object.
            var policyName = directive.ValidationPolicyName;
            Version validationPolicyVersion;
            System.Version.TryParse(directive.ValidationPolicyVersion, out validationPolicyVersion);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings[Resources.AppSettingsEsbBrePolicyTester]))
            {
                PolicyTester policyTester = null;

                int min;
                int maj;

                if (validationPolicyVersion == null)
                {
                    maj = 1;
                    min = 0;
                }
                else
                {
                    maj = validationPolicyVersion.Major;
                    min = validationPolicyVersion.Minor;
                }

                try
                {
                    // Use PolicyTester
                    var srs = new SqlRuleStore(GetRuleStoreConnectionString());
                    var rsi = new RuleSetInfo(policyName, maj, min);
                    var ruleSet = srs.GetRuleSet(rsi);

                    if (ruleSet == null)
                    {
                        throw new RuleSetNotFoundException(
                            string.Format(Resources.ExceptionRsNotInStore, policyName, maj, min));
                    }

                    policyTester = new PolicyTester(ruleSet);

                    if (trace)
                    {
                        // Create the debug tracking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policyTester.Execute(shortTermFacts, dti);

                        Trace.Write(
                            "[Esb.BizTalk.Orchestration.Directive] ValidateMessage Trace: " + dti.GetTraceOutput());
                    }
                    else
                    {
                        // Execute the policy
                        policyTester.Execute(shortTermFacts);
                    }
                }
                finally
                {
                    if (policyTester != null)
                    {
                        policyTester.Dispose();
                    }
                }
            }
            else
            {
                var policy = validationPolicyVersion == null
                                    ? new Policy(policyName)
                                    : new Policy(policyName, validationPolicyVersion.Major, validationPolicyVersion.Minor);

                try
                {
                    if (trace)
                    {
                        // Create the debug tacking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policy.Execute(shortTermFacts, dti);

                        Trace.Write(
                            "[Esb.BizTalk.Orchestration.Directive] ValidateMessage Trace: " + dti.GetTraceOutput());
                    }
                    else
                    {
                        // Execute the policy
                        policy.Execute(shortTermFacts);
                    }
                }
                finally
                {
                    policy.Dispose();
                }
            }

            return validations;
        }

        /// <summary>
        /// Validate an message part content using a BRE policy.
        /// </summary>
        /// <param name="bizTalkMessagePart">
        /// The message part.
        /// </param>
        /// <param name="documentType">
        /// The document type of the message to be validated.
        /// </param>
        /// <param name="directive">
        /// The directive currently being processed.
        /// </param>
        /// <returns>
        /// A <see cref="Validations"/> object containing the results of all validations.
        /// </returns>
        private static Validations ValidateMessagePart(IBaseMessagePart bizTalkMessagePart, string documentType, Directive directive)
        {
            var validations = new Validations();

            if (directive == null)
            {
                return validations;
            }

            if (string.IsNullOrWhiteSpace(directive.ValidationPolicyName))
            {
                return validations;
            }

            var xmlDocument = bizTalkMessagePart.AsXmlDocument();
            return xmlDocument.ChildNodes.Count == 0 ? validations : ValidateDocument(xmlDocument, documentType, directive);
        }

        /// <summary>
        /// Loads configuration property for component.
        /// </summary>
        /// <param name="pb">
        /// Configuration property bag.
        /// </param>
        /// <param name="errlog">
        /// Error status (not used in this code).
        /// </param>
        /// <param name="wrapped">
        /// Flag indicates if the service mediation pipeline component is wrapped with
        /// other pipeline components.  This is a workaround for Microsoft's code that
        /// disposes property bags  The service mediation component supports this
        /// same pattern, but the pattern fails when the component is wrapped with
        /// another component that also disposes the property bag.  If set to true, the
        /// code does not dispose the property bag.  NB., the service mediation
        /// component's Load() property should be called before calling Load on any
        /// other component that disposes the property bag.  See the Load method of the
        /// ESB service mediation disassemblers for an example.
        /// </param>
        // ReSharper disable once UnusedParameter.Local
        private void Load(IPropertyBag pb, int errlog, bool wrapped)
        {
            if (pb == null)
            {
                throw new ArgumentNullException(Resources.ExceptionNullPropertyBag);
            }

            var disposableObj = wrapped ? new object() : pb;

            using (new DisposableObjectWrapper(disposableObj))
            {
                var val = ReadPropertyBag(pb, "providerName");

                if (val != null)
                {
                    this.ProviderName = (string)val;
                }

                val = ReadPropertyBag(pb, "serviceName");

                if (val != null)
                {
                    this.ServiceName = (string)val;
                }

                val = ReadPropertyBag(pb, "bindingAccessPoint");

                if (val != null)
                {
                    this.BindingAccessPoint = (string)val;
                }

                val = ReadPropertyBag(pb, "bindingUrlType");

                if (val != null)
                {
                    this.BindingUrlType = (string)val;
                }

                val = ReadPropertyBag(pb, "bodyContainerXPath");

                if (val != null)
                {
                    this.BodyContainerXPath = (string)val;
                }

                val = ReadPropertyBag(pb, "operationName");

                if (val != null)
                {
                    this.OperationName = (string)val;
                }

                val = ReadPropertyBag(pb, "messageType");

                if (val != null)
                {
                    this.MessageType = (string)val;
                }

                val = ReadPropertyBag(pb, "messageDirection");

                if (val != null)
                {
                    var enumValue = (string)val;
                    this.MessageDirection =
                        (MessageDirectionTypes)Enum.Parse(typeof(MessageDirectionTypes), enumValue, true);
                }

                val = ReadPropertyBag(pb, "messageRole");

                if (val != null)
                {
                    this.MessageRole = (string)val;
                }

                val = ReadPropertyBag(pb, "rulePolicy");

                if (val != null)
                {
                    this.Policy = (string)val;
                }

                val = ReadPropertyBag(pb, "rulePolicyVersion");

                if (string.IsNullOrWhiteSpace((string)val))
                {
                    this.PolicyVersion = null;
                }
                else
                {
                    this.PolicyVersion = (string)val;
                }

                val = ReadPropertyBag(pb, "resolutionData");

                if (string.IsNullOrWhiteSpace((string)val))
                {
                    this.ResolutionData = ResolutionData.ValuesOnly;
                }
                else
                {
                    this.ResolutionData = (ResolutionData)Enum.Parse(typeof(ResolutionData), (string)val);
                }

                val = ReadPropertyBag(pb, "resolutionDataProperties");

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (string.IsNullOrWhiteSpace((string)val))
                {
                    this.ResolutionDataProperties = new ResolutionDataPropertyList();
                }
                else
                {
                    this.ResolutionDataProperties = new ResolutionDataPropertyList(((string)val).Split('¦').ToList());
                }

                val = ReadPropertyBag(pb, "synchronizeBam");

                this.SynchronizeBam = val == null ? false : bool.Parse((string)val);

                val = ReadPropertyBag(pb, "version");

                this.version = val == null ? null : new Version((string)val);
            }
        }

        /// <summary>
        /// Applies all resolver directives to a single outbound message.   If multiple directives overlap
        ///     in terms of the properties they set, the final state of the outbound message is governed according
        ///     to the order in which directives are applied.  This order should be considered arbitrary.
        /// </summary>
        /// <param name="pc">
        /// The pipeline context.
        /// </param>
        /// <param name="inMsg">
        /// The original message
        /// </param>
        /// <returns>
        /// The outbound message.
        /// </returns>
        private IBaseMessage ApplyDirectivesToSingleMessage(IPipelineContext pc, IBaseMessage inMsg)
        {
            var outMsg = pc.GetMessageFactory().CreateMessage();
            var bodyPartAdded = false;

            // Copy the context from old to new
            CopyContext(inMsg, outMsg);

            foreach (Directive directive in this.directives)
            {
                this.ProcessDirective(pc, inMsg, outMsg, directive, ref bodyPartAdded);
            }

            AddPartsFromInMsg(inMsg, outMsg, bodyPartAdded);

            return outMsg;
        }

        /// <summary>
        /// Processes a single directive returned from the resolver
        /// </summary>
        /// <param name="pc">
        /// The pipeline context.
        /// </param>
        /// <param name="inMsg">
        /// The original message.
        /// </param>
        /// <param name="outMsg">
        /// The outbound message.
        /// </param>
        /// <param name="directive">
        /// The resolver directive to be processed
        /// </param>
        /// <param name="bodyPartAssigned">
        /// A flag indicating if a body part is added and/or assigned to the outbound message.
        /// </param>
        private void ProcessDirective(
            IPipelineContext pc, 
            IBaseMessage inMsg, 
            IBaseMessage outMsg, 
            Directive directive, 
            ref bool bodyPartAssigned)
        {
            if (inMsg.QueueForServiceWindow(directive))
            {
                // Currently unreachable.  There is no simple way in a pipeline to 
                // enqueue a message to wait for a service window.
                // TODO: Extend the logic in the extension method to invert control to a pluggable enqueuing component. 
                return;
            }

            // [var] The message part to be validated and transformed.
            var messagePart = inMsg.BodyOrFirstPartOrDefault();

            // Populate a dictionary with message properties
            var properties = new Dictionary<string, object>();

            for (var idx = 0; idx < inMsg.Context.CountProperties; idx++)
            {
                string propertyName, propertyNamespace;
                var property = inMsg.Context.ReadAt(idx, out propertyName, out propertyNamespace);
                properties.Add(string.Format("{0}#{1}", propertyNamespace, propertyName), property);
            }

            // [fvar] Transformed XML if the part has valid XML content.  If BAM interception is 
            //        defined in the directive, this will be used. 
            Func<XmlDocument, XmlDocument> transform = xmlDocument => xmlDocument.HasChildNodes
                                           ? directive.TransformWithInterception(xmlDocument, properties)
                                           : xmlDocument;

            // We will only transform and do a post-transformation BAM interception if a 
            // transformation has been defined.
            if (!string.IsNullOrWhiteSpace(directive.ValidationPolicyName))
            {
                var schemaStrongName =
                    (string)
                    inMsg.Context.Read(SchemaStrongNameProperty.Name.Name, SchemaStrongNameProperty.Name.Namespace);

                var documentType = pc.GetDocumentSpecByName(schemaStrongName).DocSpecName;

                // Perform validation
                var validations = ValidateMessagePart(messagePart, documentType, directive);

                // [fvar] promote a validation count property
                Action<int, string> promoteValidationCountProperty =
                    (count, propertyName) =>
                    outMsg.Context.Promote(propertyName, "http://solidsoftreply.com/esb/2015/validation-properties", validations.ErrorCount);

                // [fvar] write a validation entries property
                Action<ValidationLevel, string> writeValidationEntryProperty = (validationLevel, propertyName) =>
                    {
                        var messages = validations.ToString(validationLevel).Replace("\r\n", "|");
                        messages = messages.Substring(0, messages.Length - 1);
                        outMsg.Context.Write(
                            propertyName,
                            "http://solidsoftreply.com/esb/2015/validation-properties",
                            messages);
                    };

                // Add error validation entries as properties
                if (validations.ErrorCount > 0)
                {
                    promoteValidationCountProperty(validations.ErrorCount, "ErrorsCount");
                    writeValidationEntryProperty(ValidationLevel.Error, "Errors");
                }

                // Add warning validation entries as properties
                if (validations.WarningCount > 0)
                {
                    promoteValidationCountProperty(validations.WarningCount, "WarningsCount");
                    writeValidationEntryProperty(ValidationLevel.Warning, "Warnings");
                }

                // Add information validation entries as properties
                if (validations.InformationCount > 0)
                {
                    promoteValidationCountProperty(validations.InformationCount, "InformationCount");
                    writeValidationEntryProperty(
                        ValidationLevel.Information,
                        "Information");
                }

                // Throw an exception if dictated by the policy
                if (directive.ErrorOnInvalid && validations.ErrorCount > 0)
                {
                    throw new ValidationException(string.Format("\r\nValidation Errors:\r\n{0}", validations.ToString(ValidationLevel.Error)));
                }
            }

            // [var] Transformed XML, or an empty XML document is the content cannot be transformed.
            var transformedDoc = messagePart == null
                                     ? new XmlDocument()
                                     : transform(messagePart.AsXmlDocument());

            if (!bodyPartAssigned)
            {
                // Override the buffering specified in a directive and use the event stream returned by the 
                // pipeline context (the Messaging Event Stream - MES).  This is a buffered event stream
                // synchronised with the pipeline.
                if (this.SynchronizeBam)
                {
                    directive.EventStream = pc.GetEventStream();
                }

                if (transformedDoc.HasChildNodes)
                {
                    // Format the document as required
                    transformedDoc = directive.Format(transformedDoc);

                    // Assign the transformed message to the body part of the outbound message
                    bodyPartAssigned = outMsg.PopulateBodyPartFromXmlDocument(transformedDoc, pc);

                    if (bodyPartAssigned)
                    {
                        // Set the BTS.MessageType property.  This may be overridden by BtsProperties below.
                        outMsg.Context.Promote("MessageType", Resources.UriBtsSystemProperties, transformedDoc.TypeSpecifier());

                        // Set the schema strong name property.  This may be overridden by BtsProperties below.
                        if (directive.MapTargetSchemaStrongNames != null)
                        {
                            outMsg.Context.Promote(
                                "SchemaStrongName",
                                Resources.UriBtsSystemProperties,
                                directive.MapTargetSchemaStrongNames.FirstOrDefault());
                        }
                    }
                }
                else
                {
                    bodyPartAssigned = outMsg.PopulateBodyPartFromExistingPart(messagePart, pc);
                }
            }

            // Write and/or promote any BTS properties set by the policy, together with dynamic
            // port properties for routing, the SOAP action property and retry properties.
            outMsg.PromotePropertiesBySchema(pc)
                .WriteAndPromoteBtsProperties(directive)
                .SetDynamicPortProperties(directive)
                .SetSoapAction(directive)
                .SetRetryProperties(directive);
            outMsg.BodyPart.Data.Position = 0;
        }

        /// <summary>
        /// Process a message.  This method invokes the resolver.   When used in other stages than
        /// disassembly, it produces a single outbound message.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <param name="inMsg">The pipeline message.</param>
        /// <param name="outMsgPerDirective">Indicates if per-directive messaging is required.</param>
        /// <returns>The processed message.</returns>
        private IBaseMessage ProcessMessage(IPipelineContext pc, IBaseMessage inMsg, bool outMsgPerDirective)
        {
            // If no policy is defined, log a warning.
            if (string.IsNullOrWhiteSpace(this.Policy))
            {
                // TODO: Log a warning
                return inMsg;
            }

            // [var] The part to be processed, de-enveloped if required.  If no part exists, an empty part assigned.
            var inPart =
                (inMsg.BodyPart ?? inMsg.Part(0) ?? pc.GetMessageFactory().CreateMessagePart()).RemoveEnvelope(
                    this.BodyContainerXPath);

            var msgType = this.MessageType;

            // Use the message type if not overriden and if not de-enveloped.
            if (string.IsNullOrWhiteSpace(msgType))
            {
                msgType = string.IsNullOrWhiteSpace(this.BodyContainerXPath)
                    ? inMsg.MessageType()
                    : inPart.AsXmlDocument().TypeSpecifier();
            }

            // Ensure message type is promoted on inbound message
            if (string.IsNullOrWhiteSpace(inMsg.MessageType()) && !string.IsNullOrWhiteSpace(msgType))
            {
                inMsg.Context.Promote(MessageTypeProp.Name.Name, MessageTypeProp.Name.Namespace, msgType);
            }

            var parameters = new Parameters();

            switch (this.ResolutionData)
            {
                case ResolutionData.ValuesWithAllListedProperties:
                    foreach (var property in inMsg.Properties().Where(
                        p => this.ResolutionDataProperties.Contains(string.Format("{0}|{1}", p.NameSpace, p.Name))))
                    {
                        // Set the key based on the namespace and name of the property
                        parameters.Add(property.NameSpace + "#" + property.Name, property.Value);
                    }

                    break;
                case ResolutionData.ValuesWithListedPromotedProperties:
                    foreach (var property in inMsg.Properties().Where(
                        p => this.ResolutionDataProperties.Contains(string.Format("{0}|{1}", p.NameSpace, p.Name)) && p.IsPromoted))
                    {
                        // Set the key based on the namespace and name of the property
                        parameters.Add(property.NameSpace + "#" + property.Name, property.Value);
                    }

                    break;
                case ResolutionData.ValuesWithAllProperties:
                    foreach (var property in inMsg.Properties())
                    {
                        // Set the key based on the namespace and name of the property
                        parameters.Add(property.NameSpace + "#" + property.Name, property.Value);
                    }

                    break;
                case ResolutionData.ValuesWithAllPromotedProperties:
                    foreach (var property in inMsg.Properties().Where(p => p.IsPromoted))
                    {
                        // Set the key based on the namespace and name of the property
                        parameters.Add(property.NameSpace + "#" + property.Name, property.Value);
                    }

                    break;
            }

            var policyVersionString = this.policyVersion == null ? null : this.policyVersion.ToString(2);

            // Construct the facts for resolution
            var facts = new Facts
                            {
                                ProviderName = this.ProviderName,
                                ServiceName = this.ServiceName,
                                BindingAccessPoint = this.BindingAccessPoint,
                                BindingUrlType = this.BindingUrlType,
                                MessageType = msgType,
                                OperationName = this.OperationName,
                                MessageRole = this.MessageRole,
                                MessageDirection = this.MessageDirection,
                                Parameters = parameters.Count == 0 ? null : parameters
                            };

            // Call the resolver
            this.directives = Resolver.Resolve(
                facts,
                this.policy,
                policyVersionString);

            // [var] The out message.
            IBaseMessage outMsg = null;

            // Apply directives to the message if if will not be disassembled per directive.
            if (!outMsgPerDirective)
            {
                outMsg = this.ApplyDirectivesToSingleMessage(pc, inMsg);
            }

            // Return the message.
            return outMsg == null ? inMsg : GetMsgWithCopiedProperties(inMsg, outMsg);
        }

        /// <summary>
        /// Used during disassembly to process a message per directive.   Each directive
        /// results in a separate message.
        /// </summary>
        /// <param name="pc">
        /// The pipeline context.
        /// </param>
        /// <returns>
        /// A per-directive message.
        /// </returns>
        private IBaseMessage ProcessMessagePerDirective(IPipelineContext pc)
        {
            if (this.directives == null && !string.IsNullOrWhiteSpace(this.policy))
            {
                // TODO: Log a warning
            }

            // If there are no directives, then return the current message
            if (this.current == 0 && (this.directives == null || this.directives.Count == 0))
            {
                this.current++;
                return this.pipelineInMsg;
            }

            // If there are no more directives to process, return null.
            if (this.directives == null || this.current >= this.directives.Count)
            {
                return null;
            }

            var inMsg = this.pipelineInMsg.Clone(pc);
            var outMsg = pc.GetMessageFactory().CreateMessage();
            var bodyPartAdded = false;

            // Copy the context from old to new
            CopyContext(inMsg, outMsg);

            var directive = this.directives.GetDirective(this.current);

            this.ProcessDirective(pc, inMsg, outMsg, directive, ref bodyPartAdded);

            AddPartsFromInMsg(inMsg, outMsg, bodyPartAdded);

            this.current++;
            return outMsg;
        }

        #endregion
    }
}