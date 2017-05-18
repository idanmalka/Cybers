using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure
{
    public class ServiceResult<T> : EventArgs
    {
        public T Object { get; }

        public ServiceResult(T obj)
        {
            Object = obj;
        }
    }
}
