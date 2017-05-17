using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IIOService
    {
        string OpenFileDialog();
    }
}
