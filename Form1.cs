using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampoMinado
{
    public partial class Form1 : Form
    {
        private const int gridSize = 5;
        private const int bombCount = 5;

        private Button[,] grid = new Button[gridSize, gridSize];
        private bool[,] revealed = new bool[gridSize, gridSize];

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            PlaceBombs();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Name = "Form1";
            this.Text = "Campo Minado";

            this.ResumeLayout(false);
        }

        private void InitializeGrid()
        {
            int buttonSize = 50;
            int startX = 10;
            int startY = 10;

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Button button = new Button();
                    button.Size = new Size(buttonSize, buttonSize);
                    button.Location = new Point(startX + col * buttonSize, startY + row * buttonSize);
                    button.Tag = row * gridSize + col;
                    button.Click += Button_Click;

                    this.Controls.Add(button);
                    grid[row, col] = button;
                }
            }
        }

        private void PlaceBombs()
        {
            Random random = new Random();

            for (int i = 0; i < bombCount; i++)
            {
                int row, col;

                do
                {
                    row = random.Next(0, gridSize);
                    col = random.Next(0, gridSize);
                } while ((int)grid[row, col].Tag == -1);

                grid[row, col].Tag = -1;
            }
        }

        private int GetNearbyBombs(int row, int col)
        {
            int bombCount = 0;

            for (int i = Math.Max(0, row - 1); i <= Math.Min(row + 1, gridSize - 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(col + 1, gridSize - 1); j++)
                {
                    if ((int)grid[i, j].Tag == -1)
                    {
                        bombCount++;
                    }
                }
            }

            return bombCount;
        }

        private void RevealEmptyCells(int row, int col)
        {
            for (int i = Math.Max(0, row - 1); i <= Math.Min(row + 1, gridSize - 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(col + 1, gridSize - 1); j++)
                {
                    if (!revealed[i, j])
                    {
                        revealed[i, j] = true;
                        grid[i, j].Enabled = false;
                        int nearbyBombs = GetNearbyBombs(i, j);

                        if (nearbyBombs > 0)
                        {
                            grid[i, j].Text = nearbyBombs.ToString();
                        }
                        else
                        {
                            RevealEmptyCells(i, j);
                        }

                        grid[i, j].BackColor = Color.DarkGray;
                    }
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            int row = index / gridSize;
            int col = index % gridSize;

            if (revealed[row, col])
            {
                return;
            }

            if ((int)button.Tag == -1) //acredito que esse é um bom ponto para começar a estudar o bug
            {
                MessageBox.Show("Você perdeu!");
                RestartGame();
                return;
            }

            revealed[row, col] = true;
            button.Enabled = false;
            button.BackColor = Color.DarkGray;

            int remainingCells = 0;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (!revealed[i, j])
                    {
                        remainingCells++;
                    }
                }
            }

            if (remainingCells == bombCount)
            {
                MessageBox.Show("Fim de jogo!");
                RestartGame();
                return;
            }

            int nearbyBombs = GetNearbyBombs(row, col);

            if (nearbyBombs > 0)
            {
                button.Text = nearbyBombs.ToString();
            }
            else
            {
                RevealEmptyCells(row, col);
            }
        }

        private void RestartGame()
        {
            foreach (Button button in grid)
            {
                button.Enabled = true;
                button.Text = string.Empty;
                button.Tag = 0;
                button.BackColor = default(Color);
            }

            Array.Clear(revealed, 0, revealed.Length);
            PlaceBombs();
        }
    }
}
