using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayerACv2
{
    /// <summary>
    /// Logique d'interaction pour PlayerAC.xaml
    /// </summary>
    public partial class PlayerAC : System.Windows.Controls.UserControl
    {
        private MediaPlayer _mediaPlayer;
        private System.Windows.Forms.Timer _timer;
        private TextBlock _positionInSecTextControl;
        private TextBlock _maxPositionInSecTextControl;
        private Button _playPauseButtonControl;
        private Button _jumpBackwardButtonControl;
        private Button _jumpForwardButtonControl;
        private Slider _positionSliderControl;

        #region Dependency Properties

        public static readonly DependencyProperty MusicPathNameProperty =
            DependencyProperty.Register("MusicPathName", typeof(string), typeof(PlayerAC), new FrameworkPropertyMetadata(OnMusicPathPropertyChanged));

        public string MusicPathName
        {
            get { return (string)GetValue(MusicPathNameProperty); }
            set { SetValue(MusicPathNameProperty, value); }
        }

        #endregion

        #region Properties

        private bool _isPlaying = false;
        public bool IsPLaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
            }
        }

        private bool _isUserDraggingSlider = false;
        public bool IsUserDraggingSlider
        {
            get { return _isUserDraggingSlider; }
            set
            {
                _isUserDraggingSlider = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructeur
        /// </summary>
        public PlayerAC()
        {
            InitializeComponent();
            _positionInSecTextControl = (TextBlock)this.FindName("Text_Position");
            _maxPositionInSecTextControl = (TextBlock)this.FindName("Text_MaxPosition");
            _playPauseButtonControl = (Button)this.FindName("Button_PlayPause");
            _jumpBackwardButtonControl = (Button)this.FindName("Button_JumpBackward");
            _jumpForwardButtonControl = (Button)this.FindName("Button_JumpForward");
            _positionSliderControl = (Slider)this.FindName("Slider_Position");

            if (_positionInSecTextControl == null ||
                _maxPositionInSecTextControl == null ||
                _playPauseButtonControl == null ||
                _jumpBackwardButtonControl == null ||
                _jumpForwardButtonControl == null ||
                _positionSliderControl == null)
            {
                throw new Exception("Au moins un des contrôles n'a pas été trouvé.");
            }

            _mediaPlayer = new MediaPlayer();
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            _timer.Start();

            _positionSliderControl.Value = 0;
            _positionSliderControl.Maximum = 100;
        }

        #region Public methods

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

        /// <summary>
        /// Joue la musique si elle est en pause et inversement
        /// </summary>
        public void PlayPause()
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
        }

        /// <summary>
        /// Effectue un saut en arrière dans la musique
        /// </summary>
        public void JumpBackward()
        {
            TimeSpan destinationPosition = new TimeSpan(0, _mediaPlayer.Position.Minutes, _mediaPlayer.Position.Seconds - 10);
            _mediaPlayer.Position = destinationPosition;
        }

        /// <summary>
        /// Effectue un saut en avan dans la musique
        /// </summary>
        public void JumpForward()
        {
            TimeSpan destinationPosition = new TimeSpan(0, _mediaPlayer.Position.Minutes, _mediaPlayer.Position.Seconds + 10);
            _mediaPlayer.Position = destinationPosition;
        }

        /// <summary>
        /// Effectue un saut vers la position passée en paramètre
        /// </summary>
        /// <param name="position"></param>
        public void JumpToPosition(int position)
        {
            int hours = position / 3600;
            int minutes = (position % 3600) / 60;
            int seconds = (position % 3600) % 60;
            TimeSpan destinationPosition = new TimeSpan(hours, minutes, seconds);
            _mediaPlayer.Position = destinationPosition;
        }

        #endregion
        
        #region Events

        /// <summary>
        /// A chaque tick du timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            // Mise à jour de la position courante de la musique
            if (!IsUserDraggingSlider)
            {
                double positionInSec = _mediaPlayer.Position.TotalSeconds;
                _positionInSecTextControl.Text = this.GetTimeStringFromSeconds(Convert.ToInt32(positionInSec));
                _positionSliderControl.Value = positionInSec;
            }
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
                double maxPositionInSec = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                _maxPositionInSecTextControl.Text = this.GetTimeStringFromSeconds(Convert.ToInt32(maxPositionInSec));
                _positionSliderControl.Maximum = maxPositionInSec;
            }
        }

        /// <summary>
        /// Au clic sur le bouton "Play/Pause"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            this.PlayPause();
        }

        /// <summary>
        /// Au clic sur le bouton "En arrière"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_JumpBackward_Click(object sender, RoutedEventArgs e)
        {
            this.JumpBackward();
        }

        /// <summary>
        /// Au click sur le bouton "En avant"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_JumpForward_Click(object sender, RoutedEventArgs e)
        {
            this.JumpForward();
        }

        /// <summary>
        /// Quand le drag du thumb commence
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Position_DragStarted(object sender, RoutedEventArgs e)
        {
            IsUserDraggingSlider = true;
        }

        /// <summary>
        /// Quand le drag du thumb est terminé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Position_DragCompleted(object sender, RoutedEventArgs e)
        {
            IsUserDraggingSlider = false;
        }

        /// <summary>
        /// Quand la valeur du slider change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsUserDraggingSlider)
            {
                _positionInSecTextControl.Text = this.GetTimeStringFromSeconds(Convert.ToInt32(_positionSliderControl.Value));
                this.JumpToPosition(Convert.ToInt32(_positionSliderControl.Value));
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Récupère un horaire au format hh:mm:ss à partir d'un nombre total de secondes
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
        public string GetTimeStringFromSeconds(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = (totalSeconds % 3600) % 60;

            return hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
        }

        #endregion

        #region Statics methods

        private static void OnMusicPathPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            string musicPathName = (string)e.NewValue;
            control.SetMusique(musicPathName);
        }



        #endregion
    }
}
