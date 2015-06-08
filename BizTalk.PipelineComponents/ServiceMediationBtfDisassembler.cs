// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMediationBtfDisassembler.cs" company="Solidsoft Reply Ltd.">
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

    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Component.Utilities;
    using Microsoft.BizTalk.Message.Interop;
    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution;

    /// <summary>
    /// Implements ESB service mediation in the context of the BTF Disassembler pipeline component.
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [Guid("38B0756D-099E-4E08-B5D9-BA6460DF7C98")]
    public class ServiceMediationBtfDisassembler : BaseCustomTypeDescriptor, IBaseComponent, IDisassemblerComponent, IPersistPropertyBag, IProbeMessage, IComponentUI, IDisposable
    {
        /// <summary>
        /// SOAP Schema namespace
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string SoapSchemaNS = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// BTF v2 Schema namespace
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string Btf2SchemaNS = "http://schemas.biztalk.org/btf-2-0/envelope";

        /// <summary>
        /// Receipt topic.
        /// </summary>
        public const string ReceiptTopic = "http://schemas.biztalk.org/btf-2-0/receipts/deliveryReceipt";

        /// <summary>
        /// Address type.
        /// </summary>
        public const string AddressType = "biz:URL";

        /// <summary>
        /// Date-time firmat information.
        /// </summary>
        public const string DateTimeFormatInfo = "s";

        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly ResourceManager ResourceManager;

        /// <summary>
        /// The BTF disassembler component.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly BtfDasmComp btfDasmComp;

        /// <summary>
        /// Instance of Service Mediation Disassembler
        /// </summary>
        private ServiceMediation serviceMediationDasm;

        /// <summary>
        /// The current XML message.
        /// </summary>
        private IBaseMessage currentBtfMessage;

        /// <summary>
        /// Initializes static members of the <see cref="ServiceMediationBtfDisassembler"/> class.
        /// </summary>
        static ServiceMediationBtfDisassembler()
        {
            ResourceManager = new ResourceManager("SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties.Resources", typeof(ServiceMediationBtfDisassembler).Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMediationBtfDisassembler"/> class.
        /// </summary>
        public ServiceMediationBtfDisassembler()
            : base(ResourceManager)
        {
            this.serviceMediationDasm = new ServiceMediation();
            this.btfDasmComp = new BtfDasmComp();
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
        /// Gets the description of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescBtfDasmDescription")]
        [BtsPropertyName("PropBtfDasmDescription")]
        public string Description
        {
            get
            {
                return Resources.EsbServiceMediationBtfDasmComponentDescription;
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
                return Resources.EsbServiceMediationXmlDisassemblerIcon.Handle;
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
        /// Gets the name of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescBtfDasmName")]
        [BtsPropertyName("PropBtfDasmName")]
        public string Name
        {
            get
            {
                return Resources.EsbServiceMediationBtfDasmComponentName;
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
        ///     Gets or sets the resolution data setting.  This controls the facts that are
        ///     asserted to the resolver.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescResolutionDataName")]
        [BtsPropertyName("PropResolutionDataName")]
        public ResolutionData ResolutionData
        {
            get
            {
                return this.serviceMediationDasm.ResolutionData;
            }

            set
            {
                this.serviceMediationDasm.ResolutionData = value;
            }
        }

        /// <summary>
        ///     Gets or sets a list of properties used as resolution facts.
        /// </summary>
        [BtsDescription("DescResolutionDataPropertiesName")]
        [BtsPropertyName("PropResolutionDataPropertiesName")]
        public ResolutionDataPropertyList ResolutionDataProperties
        {
            get
            {
                return this.serviceMediationDasm.ResolutionDataProperties;
            }

            set
            {
                this.serviceMediationDasm.ResolutionDataProperties = value;
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
        /// Gets the version of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescBtfDasmVersion")]
        [BtsPropertyName("PropBtfDasmVersion")]
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow messages that do not have 
        /// a recognized schema to be passed through the disassembler.
        /// </summary>
        [BtsDescription("DescAllowUnrecognizedMessage")]
        [BtsPropertyName("PropAllowUnrecognizedMessage")]
        public bool AllowUnrecognizedMessage
        {
            get
            {
                return this.btfDasmComp.AllowUnrecognizedMessage;
            }

            set
            {
                this.btfDasmComp.AllowUnrecognizedMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema(s) to be applied to the document.
        /// </summary>
        [BtsDescription("DescDocumentSpecNames")]
        [BtsPropertyName("PropDocumentSpecNames")]
        public SchemaList DocumentSpecNames
        {
            get
            {
                return this.btfDasmComp.DocumentSpecNames;
            }

            set
            {
                this.btfDasmComp.DocumentSpecNames = value;
            }
        }

        /// <summary>
        /// Gets or sets the the schema(s) to be applied to the envelope.
        /// </summary>
        [BtsDescription("DescEnvelopeSpecNames")]
        [BtsPropertyName("PropEnvelopeSpecNames")]
        public SchemaList EnvelopeSpecNames
        {
            get
            {
                return this.btfDasmComp.EnvelopeSpecNames;
            }

            set
            {
                this.btfDasmComp.EnvelopeSpecNames = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the envelope and 
        /// document structures of incoming messages.
        /// </summary>
        [BtsDescription("DescValidate")]
        [BtsPropertyName("PropValidate")]
        public bool ValidateDocument
        {
            get
            {
                return this.btfDasmComp.ValidateDocument;
            }

            set
            {
                this.btfDasmComp.ValidateDocument = value;
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
            classid = new Guid(Resources.EsbServiceMediationXmlDasmComponentClassId);
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
            this.btfDasmComp.Disassemble(pipelineContext, inMsg);
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
                if (this.currentBtfMessage == null)
                {
                    this.currentBtfMessage = this.btfDasmComp.GetNext(pipelineContext);

                    if (this.currentBtfMessage == null)
                    {
                        break;
                    }

                    this.InitialiseServiceMediation();
                    this.serviceMediationDasm.Disassemble(pipelineContext, this.currentBtfMessage);
                }

                var nextMsg = this.serviceMediationDasm.GetNext(pipelineContext);

                if (nextMsg != null)
                {
                    return nextMsg;
                }

                this.currentBtfMessage = null;
            }

            return null;
        }

        /// <summary>
        /// Performs the DO method.
        /// </summary>
        public void DoDone()
        {
            this.btfDasmComp.DoDone();
        }

        /// <summary>
        /// Performs the LOAD method.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <param name="inMsg">The base message.</param>
        public void DoLoad(IPipelineContext pc, IBaseMessage inMsg)
        {
            this.btfDasmComp.DoLoad(pc, inMsg);
        }

        /// <summary>
        /// Performs the NEXT method.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>The performed method.</returns>
        public IBaseMessage DoNext(IPipelineContext pc)
        {
            return this.btfDasmComp.DoNext(pc);
        }

        /// <summary>
        /// Performs the READ method.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>The performed method.</returns>
        public IBaseMessage DoRead(IPipelineContext pc)
        {
            return this.btfDasmComp.DoRead(pc);
        }

        /// <summary>
        /// Performs the REST method.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>The performed method.</returns>
        public IBaseMessage DoRest(IPipelineContext pc)
        {
            return this.btfDasmComp.DoRest(pc);
        }

        /// <summary>
        /// Performs the SEND method.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>The performed method.</returns>
        public IBaseMessage DoSend(IPipelineContext pc)
        {
            return this.btfDasmComp.DoSend(pc);
        }

        /// <summary>
        /// Performs the STOP method.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>The performed method.</returns>
        public IBaseMessage DoStop(IPipelineContext pc)
        {
            return this.btfDasmComp.DoStop(pc);
        }

        /// <summary>
        /// Specifies whether this component is the original component.
        /// </summary>
        /// <returns>true if this component is the original component; otherwise, false.</returns>
        public bool IsOriginal()
        {
            return this.btfDasmComp.IsOriginal();
        }

        /// <summary>
        /// Specifies whether this instance is a sender.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>true if this instance is a sender; otherwise, false.</returns>
        public bool IsSender(IPipelineContext pc)
        {
            return this.btfDasmComp.IsSender(pc);
        }

        /// <summary>
        /// Probes the incoming message.
        /// </summary>
        /// <param name="pc">The pipeline context.</param>
        /// <param name="inMsg">The incoming message.</param>
        /// <returns>The probed incoming message.</returns>
        public bool Probe(IPipelineContext pc,	IBaseMessage inMsg)
        {
            return this.btfDasmComp.Probe(pc, inMsg);
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

            // Let the BTF disassembler read its properties from the property bag.
            this.btfDasmComp.Load(pb, errlog);
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

            // Let the BTF disassembler write its properties to the property bag.
            this.btfDasmComp.Save(pb, clearDirty, saveAllProperties);
        }

        /// <summary>
        ///     Not implemented.
        /// </summary>
        public void InitNew()
        {
            // Let the Service Mediation diassembler initialise.
            this.serviceMediationDasm.InitNew();

            // Let the BTF diassembler initialise.
            this.btfDasmComp.InitNew();
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
            // Let the Service Mediation diassembler initialise.
            var listEnumServiceMediation = this.serviceMediationDasm.Validate(obj);

            // Let the BTF diassembler initialise.
            var listEnumBtf = this.btfDasmComp.Validate(obj);

            var listOut = new ArrayList();

            // An enumerator for a concatenation of the validation message lists.
            Func<IEnumerator> enumeratorForConcatenatedLists = () =>
                {
                    while (listEnumServiceMediation.MoveNext())
                    {
                        listOut.Add(listEnumServiceMediation.Current);
                    }

                    while (listEnumBtf.MoveNext())
                    {
                        listOut.Add(listEnumBtf.Current);
                    }

                    listEnumServiceMediation.Reset();
                    listEnumBtf.Reset();

                    return listOut.GetEnumerator();
                };

            return listEnumServiceMediation == null
                       ? listEnumBtf
                       : listEnumBtf == null ? listEnumServiceMediation : enumeratorForConcatenatedLists();
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
                ResolutionData = this.ResolutionData,
                ResolutionDataProperties = this.ResolutionDataProperties,
                ServiceName = this.ServiceName
            };
        }

        /// <summary>
        /// Releases the resources used by ServiceMediationBtfDisassembler object.
        /// </summary>
        void IDisposable.Dispose()
        {
            ((IDisposable)this.btfDasmComp).Dispose();
        }
    }
}
