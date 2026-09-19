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
        private int bestScore;
        private bool gameOver;

        // 3 × 3 = 9 tiles
        private const int TotalTiles = 9;

        // Number of bombs
        private const int TotalBombs = 1;

        // 9 - 1 = 8 safe tiles
        private const int TotalSafeTiles = TotalTiles - TotalBombs;


        // ============================================================
        // CACHED COLORS
        // ============================================================

        private static readonly SolidColorBrush WinIconBgBrush =
            new SolidColorBrush(Color.FromRgb(0xE8, 0xFF, 0xF3));

        private static readonly SolidColorBrush WinIconFgBrush =
            new SolidColorBrush(Color.FromRgb(0x0D, 0xAD, 0x68));

        private static readonly SolidColorBrush LoseIconBgBrush =
            new SolidColorBrush(Color.FromRgb(0xFF, 0xE8, 0xEC));

        private static readonly SolidColorBrush LoseIconFgBrush =
            new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x5B));


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public MainWindow()
        {
            InitializeComponent();

            StartGame();
        }


        // ============================================================
        // START / RESET GAME
        // ============================================================

        private void StartGame()
        {
            score = 0;
            safeTilesFound = 0;
            gameOver = false;

            bombs.Clear();
            tiles.Clear();

            GameBoard.Children.Clear();

            ResultOverlay.Visibility = Visibility.Collapsed;


            // --------------------------------------------------------
            // Generate random bomb
            // --------------------------------------------------------

            int bombIndex = random.Next(TotalTiles);

            bombs.Add(bombIndex);


            // --------------------------------------------------------
            // Create game tiles
            // --------------------------------------------------------

            for (int i = 0; i < TotalTiles; i++)
            {
                Button tile = new Button
                {
                    Content = "?",
                    Tag = i,
                    IsEnabled = true,
                    Style = (Style)FindResource("GameTileStyle")
                };

                tile.Click += Tile_Click;

                tiles.Add(tile);

                GameBoard.Children.Add(tile);
            }


            // Update UI
            UpdateStats();
        }


        // ============================================================
        // TILE CLICK
        // ============================================================

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            // Game already finished
            if (gameOver)
                return;


            // Make sure sender is a button
            if (sender is not Button clickedTile)
                return;


            // Get tile index
            if (clickedTile.Tag is not int index)
                return;


            // Already clicked
            if (!clickedTile.IsEnabled)
                return;


            // Disable immediately
            clickedTile.IsEnabled = false;


            // ========================================================
            // BOMB
            // ========================================================

            if (bombs.Contains(index))
            {
                RevealTile(
                    clickedTile,
                    "💣",
                    "BombTileStyle"
                );


                // Show remaining safe tiles
                RevealAllRemainingSafeTiles();


                // Game over
                EndGame(false);

                return;
            }


            // ========================================================
            // SAFE TILE
            // ========================================================

            RevealTile(
                clickedTile,
                "✓",
                "SafeTileStyle"
            );


            safeTilesFound++;

            score += 10;


            // Update score / safe tiles
            UpdateStats();


            // ========================================================
            // WIN CONDITION
            // ========================================================

            if (safeTilesFound >= TotalSafeTiles)
            {
                EndGame(true);
            }
        }


        // ============================================================
        // REVEAL TILE
        // ============================================================

        private void RevealTile(
            Button tile,
            string content,
            string styleKey)
        {
            tile.Content = content;

            tile.Style = (Style)FindResource(styleKey);
        }


        // ============================================================
        // REVEAL REMAINING SAFE TILES
        // ============================================================

        private void RevealAllRemainingSafeTiles()
        {
            for (int i = 0; i < TotalTiles; i++)
            {
                // Don't reveal bomb as safe
                if (bombs.Contains(i))
                    continue;


                // Already revealed
                if (tiles[i].Content?.ToString() == "✓")
                    continue;


                RevealTile(
                    tiles[i],
                    "✓",
                    "SafeTileStyle"
                );
            }
        }


        // ============================================================
        // END GAME
        // ============================================================

        private void EndGame(bool won)
        {
            gameOver = true;


            // Disable all tiles
            DisableAllTiles();


            // Update best score
            if (score > bestScore)
            {
                bestScore = score;
            }


            // ========================================================
            // WIN
            // ========================================================

            if (won)
            {
                ResultIconBg.Background = WinIconBgBrush;

                ResultIcon.Text = "🏆";

                ResultIcon.Foreground = WinIconFgBrush;

                ResultTitle.Text = "You Win!";

                ResultSubtitle.Text =
                    "You found every safe tile.";
            }


            // ========================================================
            // LOSS
            // ========================================================

            else
            {
                ResultIconBg.Background = LoseIconBgBrush;

                ResultIcon.Text = "💣";

                ResultIcon.Foreground = LoseIconFgBrush;

                ResultTitle.Text = "Boom!";

                ResultSubtitle.Text =
                    "You hit the bomb. Try again!";
            }


            // Update result screen
            ResultScore.Text = score.ToString();

            ResultBest.Text = bestScore.ToString();


            // Show overlay
            ResultOverlay.Visibility = Visibility.Visible;
        }


        // ============================================================
        // DISABLE ALL TILES
        // ============================================================

        private void DisableAllTiles()
        {
            foreach (Button tile in tiles)
            {
                tile.IsEnabled = false;
            }
        }


        // ============================================================
        // UPDATE GAME STATS
        // ============================================================

        private void UpdateStats()
        {
            // Score
            ScoreText.Text = score.ToString();


            // Bomb count
            BombText.Text = TotalBombs.ToString();


            // Remaining safe tiles
            int remainingSafeTiles =
                TotalSafeTiles - safeTilesFound;


            if (remainingSafeTiles < 0)
            {
                remainingSafeTiles = 0;
            }


            SafeTilesText.Text =
                remainingSafeTiles.ToString();
        }


        // ============================================================
        // NEW GAME BUTTON
        // ============================================================

        private void NewGame_Click(
            object sender,
            RoutedEventArgs e)
        {
            StartGame();
        }
    }
}