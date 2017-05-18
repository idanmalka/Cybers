using Cybers.Infrustructure.interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Microsoft.Win32;

namespace ServicesModule
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IOService : IIOService
    {
        public void OpenFileDialog(EventHandler<ServiceResult<string>> callback)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                callback.Invoke(this, new ServiceResult<string>(openFileDialog.FileName));
            }
        }
    }
}
