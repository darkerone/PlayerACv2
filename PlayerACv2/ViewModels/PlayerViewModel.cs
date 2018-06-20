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

        private string _positionInSec = "0";
        public string PositionInSec
        {
            get { return _positionInSec; }
            set
            {
                _positionInSec = value;
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

        public void setMusique(string musicPathName)
        {
            _mediaPlayer.Close();
            if (musicPathName != null && musicPathName != "")
            {
                _mediaPlayer.Open(new Uri(musicPathName));
            }
            IsPLaying = false;
        }

        #endregion

        #endregion region

        #region Privates methods

        private string GetStringOfTime(int hours, int minutes, int seconds)
        {
            return hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
        }

        #endregion region

        #region Events

        private void _timer_Tick(object sender, EventArgs e)
        {
            PositionInSec = GetStringOfTime(_mediaPlayer.Position.Hours, _mediaPlayer.Position.Minutes, _mediaPlayer.Position.Seconds);
        }

        #endregion
    }
}
