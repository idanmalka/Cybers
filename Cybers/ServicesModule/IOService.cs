using Cybers.Infrustructure.interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ServicesModule
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IOService : IIOService
    {
        public string OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
