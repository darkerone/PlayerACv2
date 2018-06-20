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

        private PlayerViewModel _player;
        public PlayerViewModel Player
        {
            get { return _player; }
            set
            {
                _player = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            _player = new PlayerViewModel();
            _player.setMusique(@"C:\Users\ChaupinAn\Downloads\Imagine Dragons - Thunder.mp3");
        }

        #region Commands


        #endregion region

        #region Private methods


        #endregion region
    }
}
