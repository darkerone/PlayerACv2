using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
//using System.Timers;

namespace PlayerACv2.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private string _playIconPath = @"D:\Users\Antoine\Documents\Mes projets\MetaAC\MetaAC\Icones\iconPlayer\play.png";
        private string _pauseIconPath = @"D:\Users\Antoine\Documents\Mes projets\MetaAC\MetaAC\Icones\iconPlayer\pause.png";

        private MediaPlayer _mediaPlayer;
        private Timer _timer;
        
        #region Properties
        
        private bool _isPlaying = false;
        public bool IsPLaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                if (value)
                {
                    PlayPauseIconPath = _pauseIconPath;
                }
                else
                {
                    PlayPauseIconPath = _playIconPath;
                }
                OnPropertyChanged();
            }
        }

        private string _playPauseIconPath;
        public string PlayPauseIconPath
        {
            get { return _playPauseIconPath; }
            set
            {
                _playPauseIconPath = value;
                OnPropertyChanged();
            }
        }

        private double _positionInSec = 0;
        public double PositionInSec
        {
            get { return _positionInSec; }
            set
            {
                _positionInSec = value;
                OnPropertyChanged();
            }
        }

        private double _maxPositionInSec = 100;
        public double MaxPositionInSec
        {
            get { return _maxPositionInSec; }
            set
            {
                _maxPositionInSec = value;
                OnPropertyChanged();
            }
        }

        #endregion


        public PlayerViewModel()
        {
            _mediaPlayer = new MediaPlayer();
            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            _timer.Start();

            PlayPauseIconPath = _playIconPath;
        }

        #region Commands

        public ICommand PlayPause
        {
            get
            {
                return new DelegateCommand((param) =>
                {
                    if (_mediaPlayer.HasAudio)
                    {
                        if (IsPLaying)
                        {
                            _mediaPlayer.Pause();
                            IsPLaying = false;
                        }
                        else
                        {
                            _mediaPlayer.Play();
                            IsPLaying = true;
                        }

                    }
                });
            }
        }

        public ICommand Stop
        {
            get
            {
                return new DelegateCommand((param) =>
                {
                    _mediaPlayer.Stop();
                    IsPLaying = false;
                });
            }
        }

        public ICommand JumpForward
        {
            get
            {
                return new DelegateCommand((param) =>
                {
                    TimeSpan actualPosition = new TimeSpan(0, _mediaPlayer.Position.Minutes, _mediaPlayer.Position.Seconds + 10);
                    _mediaPlayer.Position = actualPosition;
                });
            }
        }

        public ICommand JumpBackward
        {
            get
            {
                return new DelegateCommand((param) =>
                {
                    TimeSpan actualPosition = new TimeSpan(0, _mediaPlayer.Position.Minutes, _mediaPlayer.Position.Seconds - 10);
                    _mediaPlayer.Position = actualPosition;
                });
            }
        }

        #region Publics methods

        /// <summary>
        /// Initialise une musique dans le lecteur
        /// </summary>
        /// <param name="musicPathName">Chemin de la musique</param>
        public void SetMusique(string musicPathName)
        {
            _mediaPlayer.Close();
            if (musicPathName != null && musicPathName != "")
            {
                _mediaPlayer.Open(new Uri(musicPathName));

                _mediaPlayer.MediaOpened -= _mediaPlayer_MediaOpened;
                _mediaPlayer.MediaOpened += _mediaPlayer_MediaOpened;
            }
            IsPLaying = false;
        }
        
        #endregion

        #endregion region

        #region Privates methods

        #endregion region

        #region Events

        /// <summary>
        /// A chaque tick du timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            // Mise à jour de la position courante de la musique
            PositionInSec = _mediaPlayer.Position.TotalSeconds;
        }

        /// <summary>
        /// Une fois que le média est ouvert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            // Cette propriété TimeSpan ne peut avoir une valeur qu'une fois le média ouvert
            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                MaxPositionInSec = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        #endregion
    }
}
