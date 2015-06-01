namespace Solidsoft.BizTalk.Esb.Pipelines
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class EsbTransmit : Microsoft.BizTalk.PipelineOM.SendPipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>8c6b051c-0ff5-4fc2-9ae5-5016cb726282</CategoryId>  <FriendlyName>Transmit</FriendlyName"+
">  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Pre-Assemble\" minO"+
"ccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4101-4cce-4536-83fa-4a5040674ad6\" />      <Co"+
"mponents />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\"Assemb"+
"le\" minOccurs=\"0\" maxOccurs=\"1\" execMethod=\"All\" stageId=\"9d0e4107-4cce-4536-83fa-4a5040674ad6\" />  "+
"    <Components>        <Component>          <Name>Solidsoft.Esb.BizTalk.PipelineComponents.Governan"+
"ce,Solidsoft.Esb.BizTalk.PipelineComponents, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cb2a1e"+
"0509631d62</Name>          <ComponentName>Solidsoft ESB Governance</ComponentName>          <Descrip"+
"tion>Applies Solidsoft ESB governance within a BizTalk Server pipeline.</Description>          <Vers"+
"ion>1.0</Version>          <Properties>            <Property Name=\"providerName\" />            <Prop"+
"erty Name=\"serviceName\" />            <Property Name=\"bindingAccessPoint\" />            <Property Na"+
"me=\"bindingUrlType\" />            <Property Name=\"bodyContainerXPath\" />            <Property Name=\""+
"operationName\" />            <Property Name=\"messageType\" />            <Property Name=\"messageDirec"+
"tion\">              <Value xsi:type=\"xsd:string\">NotSpecified</Value>            </Property>        "+
"    <Property Name=\"messageRole\" />            <Property Name=\"rulePolicy\" />            <Property N"+
"ame=\"rulePolicyVersion\">              <Value xsi:type=\"xsd:string\" />            </Property>        "+
"    <Property Name=\"version\">              <Value xsi:type=\"xsd:string\">0.0</Value>            </Pro"+
"perty>          </Properties>          <CachedDisplayName>Solidsoft ESB Governance</CachedDisplayNam"+
"e>          <CachedIsManaged>true</CachedIsManaged>        </Component>      </Components>    </Stag"+
"e>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"3\" Name=\"Encode\" minOccurs=\"0\" maxOc"+
"curs=\"-1\" execMethod=\"All\" stageId=\"9d0e4108-4cce-4536-83fa-4a5040674ad6\" />      <Components />    "+
"</Stage>  </Stages></Document>";
        
        private const string _versionDependentGuid = "c0187b8b-7bee-4974-9465-eb0a8605452d";
        
        public EsbTransmit()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e4107-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.all);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("Solidsoft.Esb.BizTalk.PipelineComponents.Governance,Solidsoft.Esb.BizTalk.PipelineComponents, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cb2a1e0509631d62");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"providerName\" /"+
">    <Property Name=\"serviceName\" />    <Property Name=\"bindingAccessPoint\" />    <Property Name=\"bi"+
"ndingUrlType\" />    <Property Name=\"bodyContainerXPath\" />    <Property Name=\"operationName\" />    <"+
"Property Name=\"messageType\" />    <Property Name=\"messageDirection\">      <Value xsi:type=\"xsd:strin"+
"g\">NotSpecified</Value>    </Property>    <Property Name=\"messageRole\" />    <Property Name=\"rulePol"+
"icy\" />    <Property Name=\"rulePolicyVersion\">      <Value xsi:type=\"xsd:string\" />    </Property>  "+
"  <Property Name=\"version\">      <Value xsi:type=\"xsd:string\">0.0</Value>    </Property>  </Properti"+
"es></PropertyBag>";
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
