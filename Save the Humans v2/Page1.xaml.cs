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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Save_the_Humans_v2
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        readonly Random random = new Random();
        readonly DispatcherTimer enemyTimer = new DispatcherTimer();
        readonly DispatcherTimer targetTimer = new DispatcherTimer();
        private bool humanCaptured = false;
        public Page1()
        {
            InitializeComponent();

            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += TargetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
        }

        private void TargetTimer_Tick(object sender, EventArgs e)
        {
            progresBar.Value += 1;
            if (progresBar.Value >= progresBar.Maximum)
                EndTheGame();
        }
        // конец игры
        private void EndTheGame() 
        {
            if (!playArea.Children.Contains(gameOverText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                playArea.Children.Add(gameOverText);
            }
        }
        
        // время между вызовом медота добавления инопланитян
        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            AddEnemy();
        }
        // кнопка запуска игры
        private void StartButton_Click(object sender, RoutedEventArgs e) 
        {
            StartGame();
        }
        // зауск игры
        private void StartGame() 
        {
            human.IsHitTestVisible = true;
            humanCaptured = false;
            progresBar.Value = 0;
            startButton.Visibility=Visibility.Collapsed;
            playArea.Children.Clear();
            playArea.Children.Add(target);
            playArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();
        }
        // метод который добавляет инопланитян
        private void AddEnemy() 
        {
            ContentControl enemy = new ContentControl
            {
                Template = Resources["EnemyTemplate"] as ControlTemplate
            };
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100), random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)"); 
            playArea.Children.Add(enemy);
            enemy.MouseEnter += Enemy_MouseEnter;
        }

        private void Enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
                EndTheGame();
        }

        // метод который анимирует инопланитян
        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever};
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 5)))
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void human_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        private void target_MouseEnter(object sender, MouseEventArgs e)
        {
            if(targetTimer.IsEnabled && humanCaptured)
            {
                progresBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100,(int)playArea.ActualWidth -100));
                Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight -100));
                Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
                humanCaptured = false;
                human.IsHitTestVisible = true;
            }
        }

        private void playArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPoisition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPoisition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3) 
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                        {
                    humanCaptured = false;
                    human.IsHitTestVisible=true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void playArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }
    }
}
