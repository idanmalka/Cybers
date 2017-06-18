using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;

namespace Cybers.Infrustructure.interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IIOService
    {
        void OpenFileDialog(EventHandler<ServiceResult<string>> callback);
        IEnumerable<User> ReadUsersFromPath(string path);
        AlgorithmAttributesEventArgs ReadConfigurationFromFile(string path);
    }
}
