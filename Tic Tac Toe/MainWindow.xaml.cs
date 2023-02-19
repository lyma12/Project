using System;
using System.Collections.Generic;

using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool gameOver;
        private bool soundOn, musicOn;
        private MediaPlayer music = new MediaPlayer();
        private MediaPlayer musicBegin = new MediaPlayer();
        private MediaPlayer sound = new MediaPlayer();
        private GameState gameState = new GameState();
        private readonly Image[,] imageControls = new Image[3, 3];
        private readonly Dictionary<Player, ImageSource> imageSources = new Dictionary<Player, ImageSource>()
        {
            {Player.X, new BitmapImage(new Uri("pack://application:,,,/img\\X15.png")) },
            {Player.O, new BitmapImage(new Uri("pack://application:,,,/img\\O15.png")) }
        };
        private readonly Dictionary<Player, ObjectAnimationUsingKeyFrames> animation = new Dictionary<Player, ObjectAnimationUsingKeyFrames>
        {
            {Player.X, new ObjectAnimationUsingKeyFrames()},
            {Player.O, new ObjectAnimationUsingKeyFrames()}
        };

        public MainWindow()
        {
            
            InitializeComponent();
            setUpAnimation();
            setUpImageGrid();
            Uri sourceMusic = new Uri(@"C:\Users\linhl\OneDrive\Máy tính\New folder\Tic Tac Toe\Tic Tac Toe\music\fun-129202.wav");
            musicBegin.Open(sourceMusic);
            musicBegin.MediaEnded += new EventHandler(MediaBegin_ended);
            musicBegin.Play();



            gameState.OnMoveMadeState += OnMoveMade;
            gameState.EndGame += EndGameGrid;
            gameState.GameNoWin += EndGameState;
            
        }

        private void MediaBegin_ended(object sender, EventArgs e)
        {
            musicBegin.Position = TimeSpan.Zero;
            musicBegin.Play();
        }
        private void Media_ended(object sender, EventArgs e)
        {
            music.Position = TimeSpan.Zero;
            music.Play();
        }





        private void setUpAnimation()
        {
            animation[Player.X].Duration = TimeSpan.FromSeconds(0.25);
            animation[Player.O].Duration = TimeSpan.FromSeconds(0.25);
            for(int i = 0; i<16; i++)
            {
                Uri uriX = new Uri($"pack://application:,,,/img/X{i}.png");
                Uri uriO = new Uri($"pack://application:,,,/img/O{i}.png");
                BitmapImage xImg = new BitmapImage(uriX);
                DiscreteObjectKeyFrame xKeyFrame = new DiscreteObjectKeyFrame(xImg);
                BitmapImage oImg = new BitmapImage(uriO);
                DiscreteObjectKeyFrame oKeyFrame = new DiscreteObjectKeyFrame(oImg);
                animation[Player.X].KeyFrames.Add(xKeyFrame);
                animation[Player.O].KeyFrames.Add(oKeyFrame);
            }
        }

        private void setUpImageGrid()
        {
            for(int r = 0; r < 3; r++)
            {
                for(int c = 0; c <3; c++)
                {
                    Image imageControl = new Image();
                    uniformGrid.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
        }
        private void turnOff(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Play_Again(object sender, RoutedEventArgs e)
        {
            gameOver = false;
            StackGird.Visibility = Visibility.Visible;
            EndState.Visibility = Visibility.Hidden;

        }
        private void gamePlayStart(object sender, RoutedEventArgs e)
        {
            StatePlay.Visibility = Visibility.Visible;
            StateStart.Visibility = Visibility.Hidden;
        }

        
        private void UniformGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
                double sizeGridChildren = uniformGrid.Width / 3;
                Point positionClick = e.GetPosition(uniformGrid);
                int r = (int)(positionClick.X / sizeGridChildren);
                int c = (int)(positionClick.Y / sizeGridChildren);
            if (!gameOver)
            {
                if (soundOn)
                {
                    Uri sourceSound = new Uri(@"C:\\Users\\linhl\\OneDrive\\Máy tính\\New folder\\Tic Tac Toe\\Tic Tac Toe\\music\\click_mouse.wav");
                    sound.Open(sourceSound);
                    sound.Play();
                }
                gameState.MakeMove(c, r);
            }

        }

        private void OnMoveMade(int r, int c)
        {
            Player player = gameState.playerGrid[r, c];
            imageControls[r, c].BeginAnimation(Image.SourceProperty, animation[player]);
            PlayerImage.Source = imageSources[gameState.playerCurrent];
        }

        private async void EndGameGrid(WinType winType, int r, int c)
        {
            gameOver = true;
            Point start = new Point(0, 0);
            Point end = new Point(0, 0);
            int squareSize = (int) (uniformGrid.Width / 3);
            int margin = squareSize / 2;
            if (winType == WinType.columns)
            {
                int xp =  (c * squareSize + margin);
                start = new Point(xp, 0);
                end = new Point(xp, uniformGrid.Height);

            }
            if (winType == WinType.row)
            {
                int yp =  ( r * squareSize + margin);
                start = new Point(0, yp);
                end = new Point(uniformGrid.Width, yp);

            }
            if (winType == WinType.diag1)
            {
                start = new Point(0, 0);
                end = new Point(uniformGrid.Width, uniformGrid.Height);

            }
            if (winType == WinType.diag2)
            {
                start = new Point(uniformGrid.Width, 0);
                end = new Point(0, uniformGrid.Height);

            }

            LineCheck.X1 = start.X;
            LineCheck.Y1 = start.Y;

            DoubleAnimation x = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.25),
                From = start.X,
                To = end.X
            };
            DoubleAnimation y = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.25),
                From = start.Y,
                To = end.Y
            };

            LineCheck.Visibility = Visibility.Visible;
            LineCheck.BeginAnimation(Line.X2Property, x);
            LineCheck.BeginAnimation(Line.Y2Property, y);


            await Task.Delay(x.Duration.TimeSpan);
            EndGameState();
        }
        private async void EndGameState()
        {
            await Task.Delay(1000);
            StackGird.Visibility = Visibility.Hidden;
            if (gameState.winer != Player.none)
                TextInfor.Text = "Winner: " + gameState.winer.ToString() + "!";
            else TextInfor.Text = "It's tie!!!";
            EndState.Visibility = Visibility.Visible;

            for(int i = 0; i<3; i++)
            {
                for(int j = 0; j<3; j++)
                {
                    imageControls[i, j].BeginAnimation(Image.SourceProperty, null);
                    imageControls[i, j].Source = null;
                }
            }
            PlayerImage.Source = imageSources[Player.X];
            LineCheck.Visibility = Visibility.Hidden;
            gameState.Reset();
        }

        private void Music_Click(object sender, RoutedEventArgs e)
        {
            musicOn = !musicOn;
            if (musicOn)
            {
                Uri sourceMusic = new Uri(@"C:\\Users\\linhl\\OneDrive\\Máy tính\\New folder\\Tic Tac Toe\\Tic Tac Toe\\music\\hail-126903.wav");
                music.Open(sourceMusic);
                music.MediaEnded += new EventHandler(Media_ended);
                music.Play();
            }
            else
            {
                music.Close();
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    imageControls[i, j].BeginAnimation(Image.SourceProperty, null);
                    imageControls[i, j].Source = null;
                }
            }
            PlayerImage.Source = imageSources[Player.X];
            LineCheck.Visibility = Visibility.Hidden;
            gameState.Reset();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            StatePlay.Visibility = Visibility.Hidden;
            StateStart.Visibility = Visibility.Visible;
            music.Close();
            Uri sourceMusic = new Uri("C:\\Users\\linhl\\OneDrive\\Máy tính\\New folder\\Tic Tac Toe\\Tic Tac Toe\\music\\fun-129202.wav");
            musicBegin.Open(sourceMusic);
            musicBegin.MediaEnded += new EventHandler(MediaBegin_ended);
            musicBegin.Play();
        }

        private void PLAY_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            gameOver = false;
            soundOn = true;
            musicOn = true;
            musicBegin.Close();
            Uri sourceMusic = new Uri(@"C:\Users\linhl\OneDrive\Máy tính\New folder\Tic Tac Toe\Tic Tac Toe\music\hail-126903.wav");
            music.Open(sourceMusic);
            music.MediaEnded += new EventHandler(Media_ended);
            music.Play();

            StateStart.Visibility = Visibility.Hidden;
            StatePlay.Visibility = Visibility.Visible;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    imageControls[i, j].BeginAnimation(Image.SourceProperty, null);
                    imageControls[i, j].Source = null;
                }
            }
            PlayerImage.Source = imageSources[Player.X];
            LineCheck.Visibility = Visibility.Hidden;
            gameState.Reset();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help_game.Visibility = Visibility.Visible;
            StateStart.Visibility = Visibility.Hidden;
        }

        private void Back_Help_Click(object sender, RoutedEventArgs e)
        {
            StateStart.Visibility = Visibility.Visible;
            Help_game.Visibility = Visibility.Hidden;
        }

        private void Sound_Click(object sender, RoutedEventArgs e)
        {
            soundOn = !soundOn;
            
        }
    }
}
