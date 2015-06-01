namespace Esb.Validation {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Property)]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Errors", @"Warnings", @"Information", @"ErrorsCount", @"WarningsCount", @"InformationCount"})]
    public sealed class Properties : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://solidsoft.com/esb/2014/validation-properties"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""http://solidsoft.com/esb/2014/validation-properties"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:annotation>
    <xs:appinfo>
      <b:schemaInfo schema_type=""property"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" />
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Errors"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""86a13531-4c35-4d0e-b695-350e497bcab9"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""Warnings"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""6dfb7dd4-742d-4c93-9950-3f78d5ba2b84"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""Information"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""4005778c-e9d5-40de-abf5-e05dfcb8664c"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""ErrorsCount"" type=""xs:integer"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""b863bade-c70f-4000-a6f8-679b0ea7066f"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""WarningsCount"" type=""xs:integer"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""a1dc4c71-00f8-421a-bd76-a9945d28d76b"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""InformationCount"" type=""xs:integer"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""0d232aad-cd19-483e-924e-67912a4ffb55"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
</xs:schema>";
        
        public Properties() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [6];
                _RootElements[0] = "Errors";
                _RootElements[1] = "Warnings";
                _RootElements[2] = "Information";
                _RootElements[3] = "ErrorsCount";
                _RootElements[4] = "WarningsCount";
                _RootElements[5] = "InformationCount";
                return _RootElements;
            }
        }
        
        protected override object RawSchema {
            get {
                return _rawSchema;
            }
            set {
                _rawSchema = value;
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"Errors",@"http://solidsoft.com/esb/2014/validation-properties","string","System.String")]
    [PropertyGuidAttribute(@"86a13531-4c35-4d0e-b695-350e497bcab9")]
    public sealed class Errors : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"Errors", @"http://solidsoft.com/esb/2014/validation-properties");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"Warnings",@"http://solidsoft.com/esb/2014/validation-properties","string","System.String")]
    [PropertyGuidAttribute(@"6dfb7dd4-742d-4c93-9950-3f78d5ba2b84")]
    public sealed class Warnings : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"Warnings", @"http://solidsoft.com/esb/2014/validation-properties");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"Information",@"http://solidsoft.com/esb/2014/validation-properties","string","System.String")]
    [PropertyGuidAttribute(@"4005778c-e9d5-40de-abf5-e05dfcb8664c")]
    public sealed class Information : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"Information", @"http://solidsoft.com/esb/2014/validation-properties");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"ErrorsCount",@"http://solidsoft.com/esb/2014/validation-properties","integer","System.Decimal")]
    [PropertyGuidAttribute(@"b863bade-c70f-4000-a6f8-679b0ea7066f")]
    public sealed class ErrorsCount : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"ErrorsCount", @"http://solidsoft.com/esb/2014/validation-properties");
        
        private static decimal PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(decimal);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"WarningsCount",@"http://solidsoft.com/esb/2014/validation-properties","integer","System.Decimal")]
    [PropertyGuidAttribute(@"a1dc4c71-00f8-421a-bd76-a9945d28d76b")]
    public sealed class WarningsCount : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"WarningsCount", @"http://solidsoft.com/esb/2014/validation-properties");
        
        private static decimal PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(decimal);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"InformationCount",@"http://solidsoft.com/esb/2014/validation-properties","integer","System.Decimal")]
    [PropertyGuidAttribute(@"0d232aad-cd19-483e-924e-67912a4ffb55")]
    public sealed class InformationCount : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"InformationCount", @"http://solidsoft.com/esb/2014/validation-properties");
        
        private static decimal PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(decimal);
            }
        }
    }
}
