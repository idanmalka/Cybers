using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Cybers.Infrustructure
{
    public static class Extensions
    {
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container)
        {
            container.RegisterType(typeof(object), typeof(T), typeof(T).FullName);
        }
    }
}
