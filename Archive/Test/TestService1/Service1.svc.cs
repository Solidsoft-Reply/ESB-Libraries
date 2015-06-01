using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TestService1
{
    public class Service1 : IService1
    {
        #region IService1 Members

        public CompositeTypeResponse Hello(CompositeTypeRequest request)
        {
            CompositeTypeResponse response = new CompositeTypeResponse();

            try
            {
                if (request == null)
                    throw new ArgumentNullException("request");

                response.Success = true;
                response.Hello = "Hello " + request.MyName;
            }
            catch
            {
                response.Success = false;
            }

            return response;
        }

        #endregion
    }
}
