using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Util.ServiceProxy;
using System.ServiceModel;

namespace Distributor.Service.Src.Util
{
    /// <summary>
    /// use proxy implement AOP
    /// </summary>
    public static class ServiceProxyFactory
   {
       public static T Create<T>(string endpointName)
       {
           return Create<T>(null, endpointName, string.Empty);
       }

       public static T Create<T>(string endpointName, string uri)
       {
           return Create<T>(null, endpointName, uri);
       }

       public static T Create<T>(InstanceContext callbackInstance, string endpointName)
       {
           return Create<T>(callbackInstance, endpointName, string.Empty);
       }

       public static T Create<T>(InstanceContext callbackInstance, string endpointName, string uri)
       {
           Guard.NotNullOrEmpty(() => endpointName);

           return (T)(new ServiceRealProxy<T>(callbackInstance, endpointName, uri).GetTransparentProxy());
       } 
   }
}
