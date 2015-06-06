// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMediationEdiDisassembler.cs" company="Solidsoft Reply Ltd.">
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

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Text;

    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Edi.Pipelines;
    using Microsoft.BizTalk.Message.Interop;

    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution;

    /// <summary>
    /// Implements ESB service mediation in the context of the EDI Disassembler pipeline component.
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [ComponentCategory(CategoryTypes.CATID_Streamer)]
    [Guid("5F5C38E0-F9BB-4EC7-BE96-D9E4F671E057")]
    public class ServiceMediationEdiDisassembler : BaseCustomTypeDescriptor, IBaseComponent, IDisassemblerComponent, IPersistPropertyBag, IProbeMessage, IComponentUI
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly ResourceManager ResourceManager;

        /// <summary>
        /// The EDI disassembler component.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly EdiDisassembler ediDasmComp;

        /// <summary>
        /// Instance of Service Mediation Disassembler
        /// </summary>
        private ServiceMediation serviceMediationDasm;

        /// <summary>
        /// The current XML message.
        /// </summary>
        private IBaseMessage currentEdiMessage;

        /// <summary>
        /// Specifies whether encoding is for inbound.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Encoding mEncodingInbound;

        /// <summary>
        /// Initializes static members of the <see cref="ServiceMediationEdiDisassembler"/> class.
        /// </summary>
        static ServiceMediationEdiDisassembler()
        {
            ResourceManager = new ResourceManager("SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties.Resources", typeof(ServiceMediationEdiDisassembler).Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMediationEdiDisassembler"/> class.
        /// </summary>
        public ServiceMediationEdiDisassembler()
            : base(ResourceManager)
        {
            this.serviceMediationDasm = new ServiceMediation();
            this.ediDasmComp = new EdiDisassembler();
        }

        /// <summary>
        /// Gets or sets an identifier for a binding access point.   The identifier should  be
        /// in the form of a URL.   This may be used when the endpoint URL is known,
        /// but other resolution is required, or as a 'virtual' URL.
        /// This is based on UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescBindingAccessPoint")]
        [BtsPropertyName("PropBindingAccessPoint")]
        public string BindingAccessPoint
        {
            get
            {
                return this.serviceMediationDasm.BindingAccessPoint;
            }

            set
            {
                this.serviceMediationDasm.BindingAccessPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the type (scheme) of URL for the target service.   This is based on
        /// UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescBindingUrlType")]
        [BtsPropertyName("PropBindingUrlType")]
        public string BindingUrlType
        {
            get
            {
                return this.serviceMediationDasm.BindingUrlType;
            }

            set
            {
                this.serviceMediationDasm.BindingUrlType = value;
            }
        }

        /// <summary>
        /// Gets or sets an XPath that addresses the element that contains the body of the message.
        /// </summary>
        /// <remarks>
        /// If blank, the conceptual 'Root' node (parent of the Document Element) is used.
        /// </remarks>
        [Browsable(true)]
        [BtsDescription("DescBodyContainerXPath")]
        [BtsPropertyName("PropBodyContainerXPath")]
        public string BodyContainerXPath
        {
            get
            {
                return this.serviceMediationDasm.BodyContainerXPath;
            }

            set
            {
                this.serviceMediationDasm.BodyContainerXPath = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the disassembler allows trailing delimiters.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("AllowTrailingDelimitersDescription")]
        [BtsPropertyName("AllowTrailingDelimitersName")]
        public bool AllowTrailingDelimiters
        {
            get
            {
                return this.ediDasmComp.AllowTrailingDelimiters;
            }

            set
            {
                this.ediDasmComp.AllowTrailingDelimiters = value;
            }
        }

        /// <summary>
        /// Gets or sets the character set used by the disassembler for processing the interchange.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("CharacterSetDescription")]
        [BtsPropertyName("CharacterSetName")]
        public CharacterSetOptionList CharacterSet
        {
            get
            {
                return this.ediDasmComp.CharacterSet;
            }

            set
            {
                this.ediDasmComp.CharacterSet = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to convert implied decimal format Nn to base 10 numeric value.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("ConvertToImpliedDecimalDescription")]
        [BtsPropertyName("ConvertToImpliedDecimal")]
        public bool ConvertToImpliedDecimal
        {
            get
            {
                return this.ediDasmComp.ConvertToImpliedDecimal;
            }
            set
            {
                this.ediDasmComp.ConvertToImpliedDecimal = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether XML tag is created for trailing separators.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("CreateXmlTagForTrailingSeparatorsDescription")]
        [BtsPropertyName("CreateXmlTagForTrailingSeparators")]
        public bool CreateXmlTagForTrailingSeparators
        {
            get
            {
                return this.ediDasmComp.CreateXmlTagForTrailingSeparators;
            }
            set
            {
                this.ediDasmComp.CreateXmlTagForTrailingSeparators = value;
            }
        }

        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescEdiDasmDescription")]
        [BtsPropertyName("PropEdiDasmDescription")]
        public string Description
        {
            get
            {
                return Resources.EsbServiceMediationEdiDasmComponentDescription;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the disassembler detects multiple interchanges.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DetectMultipleInterchangesDescription")]
        [BtsPropertyName("DetectMultipleInterchangesName")]
        public bool DetectMultipleInterchanges
        {
            get
            {
                return this.ediDasmComp.DetectMultipleInterchanges;
            }
            set
            {
                this.ediDasmComp.DetectMultipleInterchanges = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the EDI data is validated.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("EdiDataValidationDescription")]
        [BtsPropertyName("EdiDataValidationName")]
        public bool EdiDataValidation
        {
            get
            {
                return this.ediDasmComp.EdiDataValidation;
            }
            set
            {
                this.ediDasmComp.EdiDataValidation = value;
            }
        }

        /// <summary>
        /// Gets or sets the delimiters for EDIFACT-encoded messages.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("EfactDelimitersDescription")]
        [BtsPropertyName("EfactDelimitersName")]
        public string EfactDelimiters
        {
            get
            {
                return this.ediDasmComp.EfactDelimiters;
            }
            set
            {
                this.ediDasmComp.EfactDelimiters = value;
            }
        }

        /// <summary>
        /// Gets the component icon to use in BizTalk Editor.
        /// </summary>
        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return Resources.EsbServiceMediationEdiDisassemblerIcon.Handle;
            }
        }

        /// <summary>
        /// Gets or sets the direction of message.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescMessageDirection")]
        [BtsPropertyName("PropMessageDirection")]
        public MessageDirectionTypes MessageDirection
        {
            get
            {
                return this.serviceMediationDasm.MessageDirection;
            }

            set
            {
                this.serviceMediationDasm.MessageDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets a role specifier for the message.   Equivalent to
        /// messageLabel in WSDL 2.0
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [Browsable(true)]
        [BtsDescription("DescMessageRole")]
        [BtsPropertyName("PropMessageRole")]
        public string MessageRole
        {
            get
            {
                return this.serviceMediationDasm.MessageRole;
            }

            set
            {
                this.serviceMediationDasm.MessageRole = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of message.   In a BizTalk context, this should generally be
        /// equivalent to the BTS.MessageType property.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescMessageType")]
        [BtsPropertyName("PropMessageType")]
        public string MessageType
        {
            get
            {
                return this.serviceMediationDasm.MessageType;
            }

            set
            {
                this.serviceMediationDasm.MessageType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether security information is masked to protect sensitive information.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("MaskSecurityInformationDescription")]
        [BtsPropertyName("MaskSecurityInformation")]
        public bool MaskSecurityInformation
        {
            get
            {
                return this.ediDasmComp.MaskSecurityInformation;
            }
            set
            {
                this.ediDasmComp.MaskSecurityInformation = value;
            }
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescEdiDasmName")]
        [BtsPropertyName("PropEdiDasmName")]
        public string Name
        {
            get
            {
                return Resources.EsbServiceMediationEdiDasmComponentName;
            }
        }

        /// <summary>
        /// Gets or sets the name of the service operation to be invoked.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescOperationName")]
        [BtsPropertyName("PropOperationName")]
        public string OperationName
        {
            get
            {
                return this.serviceMediationDasm.OperationName;
            }

            set
            {
                this.serviceMediationDasm.OperationName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of policy to be executed.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescPolicy")]
        [BtsPropertyName("PropPolicy")]
        public string Policy
        {
            get
            {
                return this.serviceMediationDasm.Policy;
            }

            set
            {
                this.serviceMediationDasm.Policy = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of Policy to be executed.   If empty, the
        /// latest version will be executed.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescPolicyVersion")]
        [BtsPropertyName("PropPolicyVersion")]
        public string PolicyVersion
        {
            get
            {
                return this.serviceMediationDasm.PolicyVersion;
            }

            set
            {
                this.serviceMediationDasm.PolicyVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the human-friendly name of service provider.   This is equivalent to
        /// the business entity name in UDDI.
        /// </summary>
        [Browsable(true), Category("Service"), Description(
            "Human-friendly name of service provider.   This is equivalent to the business entity name in UDDI.")]
        [BtsDescription("DescProviderName")]
        [BtsPropertyName("PropProviderName")]
        public string ProviderName
        {
            get
            {
                return this.serviceMediationDasm.ProviderName;
            }

            set
            {
                this.serviceMediationDasm.ProviderName = value;
            }
        }

        /// <summary>
        /// Gets or sets the human-friendly name for target service.   This is equivalent to
        /// the business service name in UDDI.
        /// </summary>
        [Browsable(true), Category("Service"), Description(
            "Human-friendly name for target service.   This is equivalent to the business service name in UDDI.")]
        [BtsDescription("DescServiceName")]
        [BtsPropertyName("PropServiceName")]
        public string ServiceName
        {
            get
            {
                return this.serviceMediationDasm.ServiceName;
            }

            set
            {
                this.serviceMediationDasm.ServiceName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the BAM event stream should be synchronized with
        /// the pipeline context.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescSynchronizeBam")]
        [BtsPropertyName("PropSynchronizeBam")]
        public bool SynchronizeBam
        {
            get
            {
                return this.serviceMediationDasm.SynchronizeBam;
            }

            set
            {
                this.serviceMediationDasm.SynchronizeBam = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether transaction set 997 is overridden by transaction set 999.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("Override997With999Description")]
        [BtsPropertyName("Override997With999Name")]
        public bool Override997With999
        {
            get
            {
                return this.ediDasmComp.Override997With999;
            }
            set
            {
                this.ediDasmComp.Override997With999 = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the fallback settings are overridden.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("OverrideFallbackSettingsDescription")]
        [BtsPropertyName("OverrideFallbackSettingsName")]
        public bool OverrideFallbackSettings
        {
            get
            {
                return this.ediDasmComp.OverrideFallbackSettings;
            }
            set
            {
                this.ediDasmComp.OverrideFallbackSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether batched interchange is preserved.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("BiboModeDescription")]
        [BtsPropertyName("BiboModeName")]
        public bool PreserveInterchange
        {
            get
            {
                return this.ediDasmComp.PreserveInterchange;
            }
            set
            {
                this.ediDasmComp.PreserveInterchange = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to route ACK to send pipeline on request-response receive port.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("RouteAckOn2WayPortDescription")]
        [BtsPropertyName("RouteAckOn2WayPort")]
        public bool RouteAckOn2WayPort
        {
            get
            {
                return this.ediDasmComp.RouteAckOn2WayPort;
            }
            set
            {
                this.ediDasmComp.RouteAckOn2WayPort = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to use dot as a decimal separator for internal EDIFACT representation.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("UseDotAsDecimalSeparatorForInternalEDIFACTRepesentationDescription")]
        [BtsPropertyName("UseDotAsDecimalSeparatorForInternalEDIFACTRepesentationName")]
        // ReSharper disable once InconsistentNaming
        public bool UseDotAsDecimalSeparatorForInternalEDIFACTRepesentation
        {
            get
            {
                return this.ediDasmComp.UseDotAsDecimalSeparatorForInternalEDIFACTRepesentation;
            }
            set
            {
                this.ediDasmComp.UseDotAsDecimalSeparatorForInternalEDIFACTRepesentation = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to use ISA11 as repetition separator.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("UseIsa11AsRepetitionSeparatorDescription")]
        [BtsPropertyName("UseIsa11AsRepetitionSeparator")]
        public bool UseIsa11AsRepetitionSeparator
        {
            get
            {
                return this.ediDasmComp.UseIsa11AsRepetitionSeparator;
            }
            set
            {
                this.ediDasmComp.UseIsa11AsRepetitionSeparator = value;
            }
        }

        /// <summary>
        /// Gets the version of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescEdiDasmVersion")]
        [BtsPropertyName("PropEdiDasmVersion")]
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// Gets or set whether XML schema is validated.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("XmlSchemaValidationDescription")]
        [BtsPropertyName("XmlSchemaValidationName")]
        public bool XmlSchemaValidation
        {
            get
            {
                return this.ediDasmComp.XmlSchemaValidation;
            }
            set
            {
                this.ediDasmComp.XmlSchemaValidation = value;
            }
        }
        
        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component.
        /// </param>
        // ReSharper disable once InconsistentNaming
        public void GetClassID(out Guid classid)
        {
            classid = new Guid(Resources.EsbServiceMediationEdiDasmComponentClassId);
        }

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
            this.ediDasmComp.mEncodingInbound = this.mEncodingInbound;
            this.ediDasmComp.Disassemble(pipelineContext, inMsg);
        }

        /// <summary>
        /// The get next.
        /// </summary>
        /// <param name="pipelineContext">
        /// The pipeline context.
        /// </param>
        /// <returns>
        /// The <see cref="IBaseMessage"/>.
        /// </returns>
        public IBaseMessage GetNext(IPipelineContext pipelineContext)
        {
            while (true)
            {
                if (this.currentEdiMessage == null)
                {
                    this.currentEdiMessage = this.ediDasmComp.GetNext(pipelineContext);

                    if (this.currentEdiMessage == null)
                    {
                        break;
                    }

                    this.InitialiseServiceMediation();
                    this.serviceMediationDasm.Disassemble(pipelineContext, this.currentEdiMessage);
                }

                var nextMsg = this.serviceMediationDasm.GetNext(pipelineContext);

                if (nextMsg != null)
                {
                    return nextMsg;
                }

                this.currentEdiMessage = null;
            }

            return null;
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
            // Let the Service Mediation disassembler read its properties from the property bag.
            this.serviceMediationDasm.Load(pb, errlog);

            // Let the XML disassembler read its properties from the property bag.
            this.ediDasmComp.Load(pb, errlog);
        }

        /// <summary>
        /// Probes the message to determine if it is in XML format.
        /// </summary>
        /// <param name="pc">The IPipelineContext for the current pipeline.</param>
        /// <param name="inMsg">The BizTalk message.</param>
        /// <returns>true if the structure of the document conforms to the document schema; otherwise, false.</returns>
        public bool Probe(IPipelineContext pc, IBaseMessage inMsg)
        {
            return this.ediDasmComp.Probe(pc, inMsg);
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
            // Let the Service Mediation disassembler write its properties to the property bag.
            this.serviceMediationDasm.Save(pb, clearDirty, saveAllProperties);

            // Let the XML disassembler write its properties to the property bag.
            this.ediDasmComp.Save(pb, clearDirty, saveAllProperties);
        }

        /// <summary>
        ///     Not implemented.
        /// </summary>
        public void InitNew()
        {
            // Let the Service Mediation diassembler initialise.
            this.serviceMediationDasm.InitNew();

            // Let the XML diassembler initialise.
            this.ediDasmComp.InitNew();
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
            // Let the ServiceMediation diassembler initialise.
            var listEnumServiceMediation = this.serviceMediationDasm.Validate(obj);

            // Let the XML diassembler initialise.
            var listEnumEdi = this.ediDasmComp.Validate(obj);

            var listOut = new ArrayList();

            // An enumerator for a concatenation of the validation message lists.
            Func<IEnumerator> enumeratorForConcatenatedLists = () =>
                {
                    while (listEnumServiceMediation.MoveNext())
                    {
                        listOut.Add(listEnumServiceMediation.Current);
                    }

                    while (listEnumEdi.MoveNext())
                    {
                        listOut.Add(listEnumEdi.Current);
                    }

                    listEnumServiceMediation.Reset();
                    listEnumEdi.Reset();

                    return listOut.GetEnumerator();
                };

            return listEnumServiceMediation == null
                       ? listEnumEdi
                       : listEnumEdi == null ? listEnumServiceMediation : enumeratorForConcatenatedLists();
        }

        /// <summary>
        /// Initializes a new instance of the Service Mediation component.
        /// </summary>
        private void InitialiseServiceMediation()
        {
            this.serviceMediationDasm = new ServiceMediation
            {
                BindingAccessPoint = this.BindingAccessPoint,
                BindingUrlType = this.BindingUrlType,
                BodyContainerXPath = this.BodyContainerXPath,
                MessageDirection = this.MessageDirection,
                MessageRole = this.MessageRole,
                MessageType = this.MessageType,
                OperationName = this.OperationName,
                Policy = this.Policy,
                PolicyVersion = this.PolicyVersion,
                ProviderName = this.ProviderName,
                ServiceName = this.ServiceName
            };
        }
    }
}
