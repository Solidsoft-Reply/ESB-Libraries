namespace Solidsoft.BizTalk.Esb.Pipelines
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class EsbReceive : Microsoft.BizTalk.PipelineOM.ReceivePipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>f66b9f5e-43ff-4f5f-ba46-885348ae1b4e</CategoryId>  <FriendlyName>Receive</FriendlyName>"+
"  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Decode\" minOccurs=\""+
"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4103-4cce-4536-83fa-4a5040674ad6\" />      <Component"+
"s />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\"Disassemble\" "+
"minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"FirstMatch\" stageId=\"9d0e4105-4cce-4536-83fa-4a5040674ad6\" "+
"/>      <Components>        <Component>          <Name>Solidsoft.Esb.BizTalk.PipelineComponents.XmlD"+
"isassemblerWithGovernance,Solidsoft.Esb.BizTalk.PipelineComponents, Version=1.0.0.0, Culture=neutral"+
", PublicKeyToken=cb2a1e0509631d62</Name>          <ComponentName>XML disassembler with Solidsoft ESB"+
" Governance</ComponentName>          <Description>Applies Solidsoft ESB governance in the context of"+
" an XML Disassembler within a BizTalk Server pipeline.</Description>          <Version>1.0</Version>"+
"          <Properties>            <Property Name=\"providerName\" />            <Property Name=\"servic"+
"eName\" />            <Property Name=\"bindingAccessPoint\" />            <Property Name=\"bindingUrlTyp"+
"e\" />            <Property Name=\"bodyContainerXPath\" />            <Property Name=\"operationName\" />"+
"            <Property Name=\"messageType\" />            <Property Name=\"messageDirection\">           "+
"   <Value xsi:type=\"xsd:string\">NotSpecified</Value>            </Property>            <Property Nam"+
"e=\"messageRole\" />            <Property Name=\"rulePolicy\" />            <Property Name=\"rulePolicyVe"+
"rsion\">              <Value xsi:type=\"xsd:string\" />            </Property>            <Property Nam"+
"e=\"version\">              <Value xsi:type=\"xsd:string\">0.0</Value>            </Property>           "+
" <Property Name=\"EnvelopeSpecNames\">              <Value xsi:type=\"xsd:string\" />            </Prope"+
"rty>            <Property Name=\"EnvelopeSpecTargetNamespaces\">              <Value xsi:type=\"xsd:str"+
"ing\" />            </Property>            <Property Name=\"DocumentSpecNames\">              <Value xs"+
"i:type=\"xsd:string\" />            </Property>            <Property Name=\"DocumentSpecTargetNamespace"+
"s\">              <Value xsi:type=\"xsd:string\" />            </Property>            <Property Name=\"A"+
"llowUnrecognizedMessage\">              <Value xsi:type=\"xsd:boolean\">false</Value>            </Prop"+
"erty>            <Property Name=\"ValidateDocument\">              <Value xsi:type=\"xsd:boolean\">false"+
"</Value>            </Property>            <Property Name=\"RecoverableInterchangeProcessing\">       "+
"       <Value xsi:type=\"xsd:boolean\">false</Value>            </Property>            <Property Name="+
"\"HiddenProperties\">              <Value xsi:type=\"xsd:string\">EnvelopeSpecTargetNamespaces,DocumentS"+
"pecTargetNamespaces</Value>            </Property>          </Properties>          <CachedDisplayNam"+
"e>XML disassembler with Solidsoft ESB Governance</CachedDisplayName>          <CachedIsManaged>true<"+
"/CachedIsManaged>        </Component>      </Components>    </Stage>    <Stage>      <PolicyFileStag"+
"e _locAttrData=\"Name\" _locID=\"3\" Name=\"Validate\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stage"+
"Id=\"9d0e410d-4cce-4536-83fa-4a5040674ad6\" />      <Components />    </Stage>    <Stage>      <Policy"+
"FileStage _locAttrData=\"Name\" _locID=\"4\" Name=\"ResolveParty\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod"+
"=\"All\" stageId=\"9d0e410e-4cce-4536-83fa-4a5040674ad6\" />      <Components />    </Stage>  </Stages><"+
"/Document>";
        
        private const string _versionDependentGuid = "86069ea8-124c-4d4b-8503-5472a67ff9a9";
        
        public EsbReceive()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e4105-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.firstRecognized);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("Solidsoft.Esb.BizTalk.PipelineComponents.XmlDisassemblerWithGovernance,Solidsoft.Esb.BizTalk.PipelineComponents, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cb2a1e0509631d62");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"providerName\" /"+
">    <Property Name=\"serviceName\" />    <Property Name=\"bindingAccessPoint\" />    <Property Name=\"bi"+
"ndingUrlType\" />    <Property Name=\"bodyContainerXPath\" />    <Property Name=\"operationName\" />    <"+
"Property Name=\"messageType\" />    <Property Name=\"messageDirection\">      <Value xsi:type=\"xsd:strin"+
"g\">NotSpecified</Value>    </Property>    <Property Name=\"messageRole\" />    <Property Name=\"rulePol"+
"icy\" />    <Property Name=\"rulePolicyVersion\">      <Value xsi:type=\"xsd:string\" />    </Property>  "+
"  <Property Name=\"version\">      <Value xsi:type=\"xsd:string\">0.0</Value>    </Property>    <Propert"+
"y Name=\"EnvelopeSpecNames\">      <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"E"+
"nvelopeSpecTargetNamespaces\">      <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name="+
"\"DocumentSpecNames\">      <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"Document"+
"SpecTargetNamespaces\">      <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"AllowU"+
"nrecognizedMessage\">      <Value xsi:type=\"xsd:boolean\">false</Value>    </Property>    <Property Na"+
"me=\"ValidateDocument\">      <Value xsi:type=\"xsd:boolean\">false</Value>    </Property>    <Property "+
"Name=\"RecoverableInterchangeProcessing\">      <Value xsi:type=\"xsd:boolean\">false</Value>    </Prope"+
"rty>    <Property Name=\"HiddenProperties\">      <Value xsi:type=\"xsd:string\">EnvelopeSpecTargetNames"+
"paces,DocumentSpecTargetNamespaces</Value>    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp0XmlProperties);;
                ((IPersistPropertyBag)(comp0)).Load(pb, 0);
            }
            this.AddComponent(stage, comp0);
        }
        
        public override string XmlContent
        {
            get
            {
                return _strPipeline;
            }
        }
        
        public override System.Guid VersionDependentGuid
        {
            get
            {
                return new System.Guid(_versionDependentGuid);
            }
        }
    }
}
