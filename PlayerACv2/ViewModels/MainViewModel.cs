using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace PlayerACv2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        private string _musicPathName;
        public string MusicPathName
        {
            get { return _musicPathName; }
            set
            {
                _musicPathName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            MusicPathName = currentDirectory + "\\Imagine Dragons - Thunder.mp3";
        }

        #region Commands


        #endregion region

        #region Private methods


        #endregion region
    }
}
