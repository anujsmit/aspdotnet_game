using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BoomAvoider
{
    public partial class MainWindow : Window
    {
        private readonly Random random = new Random();
        private readonly List<Button> tiles = new List<Button>();
        private readonly HashSet<int> bombs = new HashSet<int>();

        private int score;
        private int safeTilesFound;

        private const int TotalTiles = 25;
        private const int TotalBombs = 5;

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
        }

        private void StartGame()
        {
            score = 0;
            safeTilesFound = 0;

            bombs.Clear();
            tiles.Clear();
            GameBoard.Children.Clear();

            while (bombs.Count < TotalBombs)
            {
                bombs.Add(random.Next(TotalTiles));
            }

            for (int i = 0; i < TotalTiles; i++)
            {
                Button tile = new Button
                {
                    Content = "?",
                    FontSize = 36,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    MinHeight = 65,
                    MinWidth = 65,
                    Background = new SolidColorBrush(Color.FromRgb(232, 232, 232)),
                    Foreground = new SolidColorBrush(Color.FromRgb(34, 34, 34)),
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(153, 153, 153)),
                    Tag = i,
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                tile.Click += Tile_Click;

                tiles.Add(tile);
                GameBoard.Children.Add(tile);
            }

            UpdateStats();
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Button clickedTile = (Button)sender;
            int index = (int)clickedTile.Tag;

            if (!clickedTile.IsEnabled)
                return;

            clickedTile.IsEnabled = false;

            if (bombs.Contains(index))
            {
                
                clickedTile.Content = "✗";
                clickedTile.Foreground = new SolidColorBrush(Color.FromRgb(204, 0, 0));
                clickedTile.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                clickedTile.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                clickedTile.BorderThickness = new Thickness(3);

                ShowAllBombs();

                MessageBox.Show(
                    $"BOOM! 💥\n\nYour Score: {score}",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                DisableAllTiles();
                return;
            }

            
            clickedTile.Content = "✓";
            clickedTile.Foreground = new SolidColorBrush(Color.FromRgb(0, 170, 0));
            clickedTile.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            clickedTile.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            clickedTile.BorderThickness = new Thickness(3);

            safeTilesFound++;
            score += 10;

            UpdateStats();

            if (safeTilesFound == TotalTiles - TotalBombs)
            {
                ShowAllSafeTiles();

                MessageBox.Show(
                    $"YOU WIN! 🏆\n\nFinal Score: {score}",
                    "Congratulations",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DisableAllTiles();
            }
        }

        private void ShowAllBombs()
        {
            foreach (int bombIndex in bombs)
            {
                Button tile = tiles[bombIndex];
                tile.Content = "✗";
                tile.Foreground = new SolidColorBrush(Color.FromRgb(204, 0, 0));
                tile.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                tile.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                tile.BorderThickness = new Thickness(3);
            }
        }

        private void ShowAllSafeTiles()
        {
            for (int i = 0; i < TotalTiles; i++)
            {
                if (!bombs.Contains(i))
                {
                    Button tile = tiles[i];
                    tile.Content = "✓";
                    tile.Foreground = new SolidColorBrush(Color.FromRgb(0, 170, 0));
                    tile.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    tile.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                    tile.BorderThickness = new Thickness(3);
                }
            }
        }

        private void DisableAllTiles()
        {
            foreach (Button tile in tiles)
            {
                tile.IsEnabled = false;
            }
        }

        private void UpdateStats()
        {
            ScoreText.Text = score.ToString();
            BombText.Text = TotalBombs.ToString();

            int remainingSafeTiles = (TotalTiles - TotalBombs) - safeTilesFound;
            SafeTilesText.Text = remainingSafeTiles.ToString();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }
    }
}