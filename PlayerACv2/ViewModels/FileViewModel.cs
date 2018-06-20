using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixerACv2.ViewModels
{
    public class FileViewModel : BaseViewModel
    {
        private string _folderPath;
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                OnPropertyChanged();
            }
        }

        private string _oldName;
        public string OldName
        {
            get { return _oldName; }
            set
            {
                _oldName = value;
                OnPropertyChanged();
            }
        }

        private string _newName;
        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                OnPropertyChanged();
            }
        }

        public FileViewModel(string filePath)
        {
            FolderPath = Path.GetDirectoryName(filePath);
            OldName = Path.GetFileName(filePath);
            NewName = Path.GetFileName(filePath);
        }
    }
}
