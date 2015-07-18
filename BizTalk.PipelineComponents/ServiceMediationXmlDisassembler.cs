// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMediationXmlDisassembler.cs" company="Solidsoft Reply Ltd.">
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
    /// Implements ESB service mediation in the context of the XML Disassembler pipeline component.
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [ComponentCategory(CategoryTypes.CATID_Streamer)]
    [Guid("DA2A8452-88E1-4C96-B5E7-8B04AABBFBF3")]
    public class ServiceMediationXmlDisassembler : BaseCustomTypeDescriptor, IBaseComponent, IDisassemblerComponent, IPersistPropertyBag, IProbeMessage, IComponentUI
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly ResourceManager ResourceManager;

        /// <summary>
        /// The XML disassembler component.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly XmlDasmComp xmlDasmComp;

        /// <summary>
        /// Instance of Service Mediation Disassembler
        /// </summary>
        private ServiceMediation serviceMediationDasm;

        /// <summary>
        /// Indicates whether the service mediation component has no further messages to disassemble.
        /// </summary>
        private bool serviceMediationDasmNoFurtherMessages;

        /// <summary>
        /// The current disassembled XML message.
        /// </summary>
        private IBaseMessage currentXmlMessage;

        /// <summary>
        /// The current mediated message.
        /// </summary>
        private IBaseMessage currentMediatedMessage;

        /// <summary>
        /// Property bag of the service mediation component.
        /// </summary>
        private IPropertyBag serviceMediationPropertyBag;

        /// <summary>
        /// Initializes static members of the <see cref="ServiceMediationXmlDisassembler"/> class.
        /// </summary>
        static ServiceMediationXmlDisassembler()
        {
            ResourceManager = new ResourceManager("SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties.Resources", typeof(ServiceMediationXmlDisassembler).Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMediationXmlDisassembler"/> class.
        /// </summary>
        public ServiceMediationXmlDisassembler()
            : base(ResourceManager)
        {
            this.serviceMediationDasm = new ServiceMediation();
            this.xmlDasmComp = new XmlDasmComp();
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBServiceMediation")]
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
        [BtsDescription("DescXmlDasmDescription")]
        [BtsPropertyName("PropXmlDasmDescription")]
        public string Description
        {
            get
            {
                return Resources.EsbServiceMediationXmlDasmComponentDescription;
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsDescription("DescXmlDasmName")]
        [BtsPropertyName("PropXmlDasmName")]
        public string Name
        {
            get
            {
                return Resources.EsbServiceMediationXmlDasmComponentName;
            }
        }

        /// <summary>
        /// Gets or sets the name of the service operation to be invoked.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescOperationName")]
        [BtsPropertyName("PropOperationName")]
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBServiceMediation")]
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
        [BtsCategory("ESBServiceMediation")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBResolutionValues")]
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
        [BtsCategory("ESBServiceMediation")]
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
        [BtsDescription("DescXmlDasmVersion")]
        [BtsPropertyName("PropXmlDasmVersion")]
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
        [BtsCategory("XMLDisassembly")]
        public bool AllowUnrecognizedMessage
        {
            get
            {
                return this.xmlDasmComp.AllowUnrecognizedMessage;
            }

            set
            {
                this.xmlDasmComp.AllowUnrecognizedMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema(s) to be applied to the document.
        /// </summary>
        [BtsDescription("DescDocumentSpecNames")]
        [BtsPropertyName("PropDocumentSpecNames")]
        [BtsCategory("XMLDisassembly")]
        public SchemaList DocumentSpecNames
        {
            get
            {
                return this.xmlDasmComp.DocumentSpecNames;
            }

            set
            {
                this.xmlDasmComp.DocumentSpecNames = value;
            }
        }

        /// <summary>
        /// Gets or sets the the schema(s) to be applied to the envelope.
        /// </summary>
        [BtsDescription("DescEnvelopeSpecNames")]
        [BtsPropertyName("PropEnvelopeSpecNames")]
        [BtsCategory("XMLDisassembly")]
        public SchemaList EnvelopeSpecNames
        {
            get
            {
                return this.xmlDasmComp.EnvelopeSpecNames;
            }

            set
            {
                this.xmlDasmComp.EnvelopeSpecNames = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the envelope and 
        /// document structures of incoming messages.
        /// </summary>
        [BtsDescription("DescValidate")]
        [BtsPropertyName("PropValidate")]
        [BtsCategory("XMLDisassembly")]
        public bool ValidateDocument
        {
            get
            {
                return this.xmlDasmComp.ValidateDocument;
            }

            set
            {
                this.xmlDasmComp.ValidateDocument = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the disassembler will attempt to 
        /// recover from errors during interchange processing.
        /// </summary>
        [BtsDescription("DescRecoverableInterchangeProcessing")]
        [BtsPropertyName("PropRecoverableInterchangeProcessing")]
        [BtsCategory("XMLDisassembly")]
        public bool RecoverableInterchangeProcessing
        {
            get
            {
                return this.xmlDasmComp.RecoverableInterchangeProcessing;
            }

            set
            {
                this.xmlDasmComp.RecoverableInterchangeProcessing = value;
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
            this.xmlDasmComp.Disassemble(pipelineContext, inMsg);
        }

        /// <summary>
        /// Gets the next message from the message set resulting from the disassembler execution.
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
            while (true)
            {
                // Check to see see if we have just processed a mediated message.
                if (this.currentMediatedMessage == null)
                {
                    // If not, get the next disassembled XML message
                    this.currentXmlMessage = this.xmlDasmComp.GetNext(pipelineContext);

                    // If there are no further disassembled XML messages, return null.
                    if (this.currentXmlMessage == null)
                    {
                        return null;
                    }

                    // Indicate that we have just got the next flat file message by ensuring that 
                    // the service mediation component is null.
                    this.serviceMediationDasmNoFurtherMessages = true;
                }

                // Check if we have just got the next flat file message.  If so, create and initialise
                // a new service mediation component.
                if (this.serviceMediationDasmNoFurtherMessages)
                {
                    this.InitialiseServiceMediation();
                    this.serviceMediationDasm.Disassemble(pipelineContext, this.currentXmlMessage);
                }

                // Get the next mediated message
                this.currentMediatedMessage = this.serviceMediationDasm.GetNext(pipelineContext);

                // If a mediated message was found, return it.
                if (this.currentMediatedMessage != null)
                {
                    return this.currentMediatedMessage;
                }
            }
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
            this.serviceMediationDasm.SafeLoadWhenWrapped(pb, errlog);

            // Capture the property values to a local property bag.
            this.serviceMediationPropertyBag = pb;

            // Let the XML disassembler read its properties from the property bag.
            this.xmlDasmComp.Load(pb, errlog);
        }

        /// <summary>
        /// Probes the message to determine if it is in XML format.
        /// </summary>
        /// <param name="pc">The IPipelineContext for the current pipeline.</param>
        /// <param name="inMsg">The BizTalk message.</param>
        /// <returns>true if the structure of the document conforms to the document schema; otherwise, false.</returns>
        public bool Probe(IPipelineContext pc, IBaseMessage inMsg)
        {
            return this.xmlDasmComp.Probe(pc, inMsg);
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
            this.xmlDasmComp.Save(pb, clearDirty, saveAllProperties);
        }

        /// <summary>
        ///     Not implemented.
        /// </summary>
        public void InitNew()
        {
            // Let the Service Mediation diassembler initialise.
            this.serviceMediationDasm.InitNew();

            // Let the XML diassembler initialise.
            this.xmlDasmComp.InitNew();
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
            var listEnumXml = this.xmlDasmComp.Validate(obj);

            var listOut = new ArrayList();

            // An enumerator for a concatenation of the validation message lists.
            Func<IEnumerator> enumeratorForConcatenatedLists = () =>
                {
                    while (listEnumServiceMediation.MoveNext())
                    {
                        listOut.Add(listEnumServiceMediation.Current);
                    }

                    while (listEnumXml.MoveNext())
                    {
                        listOut.Add(listEnumXml.Current);
                    }

                    listEnumServiceMediation.Reset();
                    listEnumXml.Reset();

                    return listOut.GetEnumerator();
                };

            return listEnumServiceMediation == null
                       ? listEnumXml
                       : listEnumXml == null ? listEnumServiceMediation : enumeratorForConcatenatedLists();
        }

        /// <summary>
        /// Initializes a new instance of the Service Mediation component.
        /// </summary>
        private void InitialiseServiceMediation()
        {
            this.serviceMediationDasm = new ServiceMediation();
            this.serviceMediationDasm.Load(this.serviceMediationPropertyBag, 0);
        }
    }
}
