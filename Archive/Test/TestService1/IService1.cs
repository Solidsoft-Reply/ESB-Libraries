using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TestService1
{
    [ServiceContract(Namespace="http://solidsoft.com/schemas/testservice1/service1")]
    public interface IService1
    {
        [OperationContract]
        CompositeTypeResponse Hello(CompositeTypeRequest request);
    }

    [DataContract]
    public class CompositeTypeRequest
    {
        string myName = "";

        [DataMember]
        public string MyName
        {
            get { return myName; }
            set { myName = value; }
        }
    }

    [DataContract]
    public class CompositeTypeResponse
    {
        bool success = true;
        string hello = "Hello ";

        [DataMember]
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        [DataMember]
        public string Hello
        {
            get { return hello; }
            set { hello = value; }
        }
    }
}
