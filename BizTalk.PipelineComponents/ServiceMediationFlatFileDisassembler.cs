// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceMediationFlatFileDisassembler.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Resources;
    using System.Runtime.InteropServices;

    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Component.Utilities;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Streaming;

    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution;

    /// <summary>
    /// Implements ESB service mediation in the context of the Flat File Disassembler pipeline component.
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [ComponentCategory(CategoryTypes.CATID_Streamer)]
    [Guid("5EEEF32A-5601-4D8D-B2DA-D939484F08E7")]
    public class ServiceMediationFlatFileDisassembler : BaseCustomTypeDescriptor, IBaseComponent, IPersistPropertyBag, IComponentUI, IDisassemblerComponent, IProbeMessage, IDisposable
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly ResourceManager ResourceManager;

        /// <summary>
        /// The Flat File disassembler component.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly FFDasmComp flatFileDasmComp;

        /// <summary>
        /// Instance of Service Mediation Disassembler
        /// </summary>
        private readonly ServiceMediation serviceMediationDasm;

        /// <summary>
        /// A list of disassembled messages from the FF disassembler
        /// </summary>
        private readonly List<IBaseMessage> innerDisassembledMessages = new List<IBaseMessage>();

        /// <summary>
        /// The current mediated message.
        /// </summary>
        private IBaseMessage currentMediatedMessage;
        
        /// <summary>
        /// Index of the current XML message provided by the FF disassembler.
        /// </summary>
        private int innerDisasembledMessageIndex;

        /// <summary>
        /// Initializes static members of the <see cref="ServiceMediationFlatFileDisassembler"/> class.
        /// </summary>
        static ServiceMediationFlatFileDisassembler()
        {
            ResourceManager = new ResourceManager("SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties.Resources", typeof(ServiceMediationFlatFileDisassembler).Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMediationFlatFileDisassembler"/> class.
        /// </summary>
        public ServiceMediationFlatFileDisassembler()
            : base(ResourceManager)
        {
            this.serviceMediationDasm = new ServiceMediation();
            this.flatFileDasmComp = new FFDasmComp();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMediationFlatFileDisassembler"/> class.
        /// </summary>
        /// <param name="dataReaderFunction">The data reader function.</param>
        public ServiceMediationFlatFileDisassembler(FFDasmComp.DataReaderFunction dataReaderFunction)
            : base(ResourceManager)
        {
            this.serviceMediationDasm = new ServiceMediation();
            this.flatFileDasmComp = new FFDasmComp(dataReaderFunction);
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
        [BtsDescription("DescFFDasmDescription")]
        [BtsPropertyName("PropFFDasmDescription")]
        public string Description
        {
            get
            {
                return Resources.EsbServiceMediationFlatFileDasmComponentDescription;
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
                return Resources.EsbServiceMediationFFDisassemblerIcon.Handle;
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
        [BtsDescription("DescFFDasmName")]
        [BtsPropertyName("PropFFDasmName")]
        public string Name
        {
            get
            {
                return Resources.EsbServiceMediationFlatFileDasmComponentName;
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
        [BtsDescription("DescFFDasmVersion")]
        [BtsPropertyName("PropFFDasmVersion")]
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// Gets or sets the schema to be applied to the document.
        /// </summary>
        [BtsDescription("DescDocumentSpecName")]
        [BtsPropertyName("PropDocumentSpecName")]
        [BtsCategory("FFDisassembly")]
        public SchemaWithNone DocumentSpecName
        {
            get
            {
                return this.flatFileDasmComp.DocumentSpecName;
            }

            set
            {
                this.flatFileDasmComp.DocumentSpecName = value;
            }
        }

        /// <summary>
        /// Gets or sets the header schema name that the flat file disassembler uses for parsing document headers. 
        /// </summary>
        [BtsDescription("DescHeaderSpecName")]
        [BtsPropertyName("PropHeaderSpecName")]
        [BtsCategory("FFDisassembly")]
        public SchemaWithNone HeaderSpecName
        {
            get
            {
                return this.flatFileDasmComp.HeaderSpecName;
            }

            set
            {
                this.flatFileDasmComp.HeaderSpecName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to preserve the headers 
        /// of the documents on the message context.
        /// </summary>
        [BtsDescription("DescPreserveHeader")]
        [BtsPropertyName("PropPreserveHeader")]
        [BtsCategory("FFDisassembly")]
        public bool PreserveHeader
        {
            get
            {
                return this.flatFileDasmComp.PreserveHeader;
            }

            set
            {
                this.flatFileDasmComp.PreserveHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the trailer schema name that the flat file disassembler uses for parsing document trailers.
        /// </summary>
        [BtsDescription("DescTrailerSpecName")]
        [BtsPropertyName("PropTrailerSpecName")]
        [BtsCategory("FFDisassembly")]
        public SchemaWithNone TrailerSpecName
        {
            get
            {
                return this.flatFileDasmComp.TrailerSpecName;
            }

            set
            {
                this.flatFileDasmComp.TrailerSpecName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the structure of the parsed document.
        /// </summary>
        [BtsDescription("DescValidateDocumentStructure")]
        [BtsPropertyName("PropValidateDocumentStructure")]
        [BtsCategory("FFDisassembly")]
        public bool ValidateDocumentStructure
        {
            get
            {
                return this.flatFileDasmComp.ValidateDocumentStructure;
            }

            set
            {
                this.flatFileDasmComp.ValidateDocumentStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the disassembler will attempt to 
        /// recover from errors during interchange processing.
        /// </summary>
        [BtsDescription("DescRecoverableInterchangeProcessing")]
        [BtsPropertyName("PropRecoverableInterchangeProcessing")]
        [BtsCategory("FFDisassembly")]
        public bool RecoverableInterchangeProcessing
        {
            get
            {
                return this.flatFileDasmComp.RecoverableInterchangeProcessing;
            }

            set
            {
                this.flatFileDasmComp.RecoverableInterchangeProcessing = value;
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
            classid = new Guid(Resources.EsbServiceMediationFlatFileDasmComponentClassId);
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
            this.flatFileDasmComp.Disassemble(pipelineContext, inMsg);
            this.innerDisassembledMessages.Clear();
            this.innerDisasembledMessageIndex = 0;
            this.currentMediatedMessage = default(IBaseMessage);

            // Fullt disassemble the flat file here and store each message in an 
            // in-memory collection.  See the GetNext() method for a description
            // of why this is being done here.
            while (true)
            {
                var nextMessage = this.flatFileDasmComp.GetNext(pipelineContext);

                // If there are no further disassembled XML messages, return null.
                if (nextMessage == null)
                {
                    return;
                }

                // Add a clone of the message to the in-memory collection.
                this.innerDisassembledMessages.Add(nextMessage.Clone(pipelineContext));
            }
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
            /* *************************************************************************************
             * This method was written specifically to handle a logical issue with Microsoft's
             * FF Disassembler component.  The root issue is that the FF Disassember is not safe
             * to use with downstream components.  Depending on how the downstream component  
             * behaves, it is possible for the FF disassember to loose internal integrity during 
             * its disassembly cycle.  An XML reader object used to read through the the XSD 
             * document spec during internal processing of FF records and their cnversion to XML 
             * can become deferenced and subject to garbage collection prematurely.  If a GC 
             * collection happens, the FF disassemble throws an error saying that the DataReader  
             * has been disposed.  One well-documented workaround is to switch on recoverable 
             * interchage processing.  This causes the FF disassembler to follow a route through 
             * the code that avoids the issue altogether.  However, it isn't an acceptable solution  
             * for a general-purpose component.
             * 
             * This code avoids the issue by completely disassembling the flat file in the 
             * Dissassemble() method and storing the resultant messages in an in-memory collection.  
             * This method processes that list, further disassembling each message and returning 
             * the messages one by one on each call.  Holding messages in-memory in this way is not 
             * ideal, but does have the merit of ensuring the the problems with Microsoft's FF 
             * Disassembler are dealt with internally inside this component and not propogated 
             * further through the pipeline.
             * *************************************************************************************/

            var messageCount = this.innerDisassembledMessages.Count;

            // Check to see if there are any more messages to process.
            if (messageCount == 0 ||
                this.innerDisasembledMessageIndex >= messageCount)
            {
                this.innerDisassembledMessages.Clear();
                this.innerDisasembledMessageIndex = 0;
                this.currentMediatedMessage = default(IBaseMessage);
                return null;
            }

            // If this is the first call to this method, call Disassemble on the 
            // Service Mediation component.
            if (this.innerDisasembledMessageIndex == 0 && this.currentMediatedMessage == null)
            {
                this.serviceMediationDasm.Disassemble(
                    pipelineContext, 
                    this.innerDisassembledMessages[this.innerDisasembledMessageIndex]);
            }

            // Get the next mediated message
            while (true)
            {
                this.currentMediatedMessage = this.serviceMediationDasm.GetNext(pipelineContext);

                if (this.currentMediatedMessage != null)
                {
                    return this.currentMediatedMessage;
                }

                this.innerDisasembledMessageIndex++;

                // Check to see if all disassembled FF messages have been processed
                if (this.innerDisasembledMessageIndex >= messageCount)
                {
                    this.innerDisassembledMessages.Clear();
                    this.innerDisasembledMessageIndex = 0;
                    this.currentMediatedMessage = default(IBaseMessage);
                    return null;
                }

                // If more diassembled FF messages exist, cycle to the next one.
                this.serviceMediationDasm.Disassemble(
                    pipelineContext, 
                    this.innerDisassembledMessages[++this.innerDisasembledMessageIndex]);
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

            // Let the Flat File disassembler read its properties from the property bag.
            this.flatFileDasmComp.Load(pb, errlog);
        }

        /// <summary>
        /// Probes the flat file message to determine if the structure of the document conforms to the document schema.
        /// </summary>
        /// <param name="pc">The IPipelineContext for the current pipeline.</param>
        /// <param name="inMsg">The BizTalk message.</param>
        /// <returns>true if the structure of the document conforms to the document schema; otherwise, false.</returns>
        public bool Probe(IPipelineContext pc, IBaseMessage inMsg)
        {
            return this.flatFileDasmComp.Probe(pc, inMsg);
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

            // Let the Flat File disassembler write its properties to the property bag.
            this.flatFileDasmComp.Save(pb, clearDirty, saveAllProperties);
        }

        /// <summary>
        /// Initialize the disassembler component.
        /// </summary>
        public void InitNew()
        {
            // Let the Service Mediation diassembler initialise.
            this.serviceMediationDasm.InitNew();

            // Let the Flat File diassembler initialise.
            this.flatFileDasmComp.InitNew();
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

            // Let the Flat File diassembler initialise.
            var listEnumFlatFile = this.flatFileDasmComp.Validate(obj);

            var listOut = new ArrayList();

            // An enumerator for a concatenation of the validation message lists.
            Func<IEnumerator> enumeratorForConcatenatedLists = () =>
            {
                while (listEnumServiceMediation.MoveNext())
                {
                    listOut.Add(listEnumServiceMediation.Current);
                }

                while (listEnumFlatFile.MoveNext())
                {
                    listOut.Add(listEnumFlatFile.Current);
                }

                listEnumServiceMediation.Reset();
                listEnumFlatFile.Reset();

                return listOut.GetEnumerator();
            };

            return listEnumServiceMediation == null
                       ? listEnumFlatFile
                       : listEnumFlatFile == null ? listEnumServiceMediation : enumeratorForConcatenatedLists();
        }

        /// <summary>
        /// Releases the resources used by ServiceMediationFlatFileDisassembler object.
        /// </summary>
        public void Dispose()
        {
            this.flatFileDasmComp.Dispose();
        }
    }
}
