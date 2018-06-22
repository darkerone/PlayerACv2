using PlayerACv2.Enums;
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
        private Image _playImageResource;
        private Image _pauseImageResource;
        private PositionUpdateSourceEnum _lastPositionUpdateSource = PositionUpdateSourceEnum.Slider;

        #region Dependency Properties

        public static readonly DependencyProperty MusicPathNameProperty =
            DependencyProperty.Register("MusicPathName", typeof(string), typeof(PlayerAC), new FrameworkPropertyMetadata(OnMusicPathNamePropertyChanged));
        public string MusicPathName
        {
            get { return (string)GetValue(MusicPathNameProperty); }
            set { SetValue(MusicPathNameProperty, value); }
        }

        public static readonly DependencyProperty PlayPauseButtonVisibilityProperty =
            DependencyProperty.Register("PlayPauseButtonVisibility", typeof(Visibility), typeof(PlayerAC), new FrameworkPropertyMetadata(OnPlayPauseButtonVisibilityPropertyChanged));
        public Visibility PlayPauseButtonVisibility
        {
            get { return (Visibility)GetValue(PlayPauseButtonVisibilityProperty); }
            set { SetValue(PlayPauseButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty JumpButtonsVisibilityProperty =
            DependencyProperty.Register("JumpButtonsVisibility", typeof(Visibility), typeof(PlayerAC), new FrameworkPropertyMetadata(OnJumpButtonsVisibilityPropertyChanged));
        public Visibility JumpButtonsVisibility
        {
            get { return (Visibility)GetValue(JumpButtonsVisibilityProperty); }
            set { SetValue(JumpButtonsVisibilityProperty, value); }
        }

        public static readonly DependencyProperty PositionSliderVisibilityProperty =
            DependencyProperty.Register("PositionSliderVisibility", typeof(Visibility), typeof(PlayerAC), new FrameworkPropertyMetadata(OnPositionSliderVisibilityPropertyChanged));
        public Visibility PositionSliderVisibility
        {
            get { return (Visibility)GetValue(PositionSliderVisibilityProperty); }
            set { SetValue(PositionSliderVisibilityProperty, value); }
        }

        public static readonly DependencyProperty PositionTextVisibilityProperty =
            DependencyProperty.Register("PositionTextVisibility", typeof(Visibility), typeof(PlayerAC), new FrameworkPropertyMetadata(OnPositionTextVisibilityPropertyChanged));
        public Visibility PositionTextVisibility
        {
            get { return (Visibility)GetValue(PositionTextVisibilityProperty); }
            set { SetValue(PositionTextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty MaxPositionTextVisibilityProperty =
            DependencyProperty.Register("MaxPositionTextVisibility", typeof(Visibility), typeof(PlayerAC), new FrameworkPropertyMetadata(OnMaxPositionTextVisibilityPropertyChanged));
        public Visibility MaxPositionTextVisibility
        {
            get { return (Visibility)GetValue(MaxPositionTextVisibilityProperty); }
            set { SetValue(MaxPositionTextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(PlayerAC), new FrameworkPropertyMetadata());
        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            private set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty JumpForwardOffsetProperty =
            DependencyProperty.Register("JumpForwardOffset", typeof(double), typeof(PlayerAC), new FrameworkPropertyMetadata((double)10));
        public double JumpForwardOffset
        {
            get { return (double)GetValue(JumpForwardOffsetProperty); }
            set { SetValue(JumpForwardOffsetProperty, value); }
        }

        public static readonly DependencyProperty JumpBackwardOffsetProperty =
            DependencyProperty.Register("JumpBackwardOffset", typeof(double), typeof(PlayerAC), new FrameworkPropertyMetadata((double)10));
        public double JumpBackwardOffset
        {
            get { return (double)GetValue(JumpBackwardOffsetProperty); }
            set { SetValue(JumpBackwardOffsetProperty, value); }
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

            _playImageResource = (Image)this.FindResource("Image_Play");
            _pauseImageResource = (Image)this.FindResource("Image_Pause");

            if(_playImageResource == null ||
                _pauseImageResource == null)
            {
                throw new Exception("Resource(s) manquante(s).");
            }

            _mediaPlayer = new MediaPlayer();
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            _timer.Start();

            this.Slider_Position.Value = 0;
            this.Slider_Position.Maximum = 100;
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
        /// Défini l'état de la musique. Si True, joue la musique. Si False, pause la musique. Si null, inverse l'état actuel
        /// </summary>
        /// <param name="play"></param>
        public void PlayPause(bool? play)
        {
            if (play != null)
            {
                // Mode manuel
                if (play.Value)
                {
                    this.Play();
                }
                else
                {
                    this.Pause();
                }
            }
            else
            {
                // Mode automatique
                if (IsPLaying)
                {
                    this.Pause();
                }
                else
                {
                    this.Play();
                }
            }
        }

        /// <summary>
        /// Effectue un saut en arrière dans la musique
        /// </summary>
        public void JumpBackward()
        {
            _lastPositionUpdateSource = PositionUpdateSourceEnum.Jump;
            this.JumpToPosition(_mediaPlayer.Position.TotalSeconds - JumpBackwardOffset);
        }

        /// <summary>
        /// Effectue un saut en avan dans la musique
        /// </summary>
        public void JumpForward()
        {
            _lastPositionUpdateSource = PositionUpdateSourceEnum.Jump;
            this.JumpToPosition(_mediaPlayer.Position.TotalSeconds + JumpForwardOffset);
        }

        /// <summary>
        /// Effectue un saut vers la position passée en paramètre
        /// </summary>
        /// <param name="newPosition"></param>
        public void JumpToPosition(double newPosition)
        {
            int newPositionInt = Convert.ToInt32(newPosition);
            // Met à jour la position dans le lecteur
            int hours = newPositionInt / 3600;
            int minutes = (newPositionInt % 3600) / 60;
            int seconds = (newPositionInt % 3600) % 60;
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
            // Mise à jour de la position courante informative de la musique
            if (!IsUserDraggingSlider)
            {
                UpdatePositionInformationIfChanged();
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
                this.Text_MaxPosition.Text = this.GetTimeStringFromSeconds(Convert.ToInt32(maxPositionInSec));
                this.Slider_Position.Maximum = maxPositionInSec;
            }
        }

        /// <summary>
        /// Au clic sur le bouton "Play/Pause"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            this.PlayPause(null);
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
            _lastPositionUpdateSource = PositionUpdateSourceEnum.ThumbDrag;
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
            if (_lastPositionUpdateSource == PositionUpdateSourceEnum.Slider || 
                _lastPositionUpdateSource == PositionUpdateSourceEnum.ThumbDrag)
            {
                this.Text_Position.Text = this.GetTimeStringFromSeconds(Convert.ToInt32(this.Slider_Position.Value));
                this.JumpToPosition(this.Slider_Position.Value);
            }
            _lastPositionUpdateSource = PositionUpdateSourceEnum.Slider;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Récupère un horaire au format hh:mm:ss à partir d'un nombre total de secondes
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
        private string GetTimeStringFromSeconds(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = (totalSeconds % 3600) % 60;

            return hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
        }

        private void Play()
        {
            if (_mediaPlayer.HasAudio)
            {
                _mediaPlayer.Play();
                IsPLaying = true;
                this.Button_PlayPause.Content = this._pauseImageResource;
            }
        }

        private void Pause()
        {
            if (_mediaPlayer.HasAudio)
            {
                _mediaPlayer.Pause();
                IsPLaying = false;
                this.Button_PlayPause.Content = this._playImageResource;
            }
        }

        /// <summary>
        /// Met à jour les informations de la position si elle a changé
        /// </summary>
        private void UpdatePositionInformationIfChanged()
        {
            double positionInSec = _mediaPlayer.Position.TotalSeconds;
            // Si la position a changée
            if (Position != positionInSec)
            {
                // Met à jour le texte de la position
                this.Text_Position.Text = this.GetTimeStringFromSeconds(Convert.ToInt32(positionInSec));
                // Met à jour la position du thumb du slider
                this.Slider_Position.Value = positionInSec;
                // Met à jour la Dependency property 
                Position = positionInSec;
            }
        }

        #endregion

        #region Statics methods

        private static void OnMusicPathNamePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            string musicPathName = (string)e.NewValue;
            control.SetMusique(musicPathName);
        }

        private static void OnPlayPauseButtonVisibilityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            Visibility visibility = (Visibility)e.NewValue;
            control.Button_PlayPause.Visibility = visibility;
        }

        private static void OnJumpButtonsVisibilityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            Visibility visibility = (Visibility)e.NewValue;
            control.Button_JumpBackward.Visibility = visibility;
            control.Button_JumpForward.Visibility = visibility;
        }

        private static void OnPositionSliderVisibilityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            Visibility visibility = (Visibility)e.NewValue;
            control.Slider_Position.Visibility = visibility;
        }

        private static void OnPositionTextVisibilityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            Visibility visibility = (Visibility)e.NewValue;
            control.Text_Position.Visibility = visibility;
        }

        private static void OnMaxPositionTextVisibilityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerAC control = source as PlayerAC;
            Visibility visibility = (Visibility)e.NewValue;
            control.Text_MaxPosition.Visibility = visibility;
        }

        #endregion
    }
}
