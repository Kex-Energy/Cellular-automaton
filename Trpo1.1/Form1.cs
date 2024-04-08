namespace Trpo1._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private CheckBox[,] Grid;
        private static readonly int GridX = 20, GridY = 20;
        private Game_Template game;
        private bool stop = false;
        private bool started = false;
        object _locker = new object();

        private void HideCheckBoxes()
        {
            for(int i = 0; i < GridX; i++)
            {
                for(int j = 0; j < GridY; j++)
                {
                    Grid[i, j].Hide();
                }
            }
        }
        private void ShowCheckBoxes(bool f)
        {
            if (!f)
            {
                for (int i = 0; i < GridX; i++)
                {
                    for (int j = 0; j < GridY; j++)
                    {
                        Grid[i, j].Show();
                    }
                }
            }
            else
            {
                for (int j = 0; j < GridY; j++)
                {
                    Grid[0, j].Show();
                }
            }
        }
       
        private void StartGame(int selected_cell, int selected_game)
        {
            stop = false;
            
            List<int> x = new List<int>();
            List<int> y = new List<int>();
            Cell_Prototype cell;
            for (int i = 0; i < GridY; i++)
            {
                for (int j = 0; j < GridX; j++)
                {
                    if (Grid[i, j].Checked)
                    {
                        y.Add(i);
                        x.Add(j);
                        
                    }
                }
            }

            switch (selected_game)
            {
                case 0:
                    game = new StandartGame();
                    break;
                case 1:
                    game = new CrossGame();
                    break;
                case 2:
                    game = new Rule161();
                    break;
                case 3:
                    game = new Rule110();
                    break;
                case 4:
                    game = new Rule90();
                    break;
                case 5:
                    game = new Rule30();
                    break;
                default:
                    game = new StandartGame();
                    break;
            }

            switch (selected_cell)
            {
                case 0:
                    cell = new Standart_Cell();
                    break;
                case 1:
                    cell = new Cell_with_randomHpdecreace();
                    break;
                case 2:
                    cell = new Cell_with_largeHp();
                    break;
                default:
                    cell = new Standart_Cell();
                    break;
            }
                
            
            game.Initialize_Cells(x, y, cell);
            started = true;
            while (!game.GameEnd && !stop)
            {
                lock (_locker)
                {
                    game.GameCicle();
                }
                
                Invoke(groupBox1.Invalidate);
                
                Thread.Sleep(500);
            }
            started = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            HideCheckBoxes();
            button1.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            int c = comboBox1.SelectedIndex;
            int g = comboBox2.SelectedIndex;
            await Task.Run(() => StartGame(c,g));
            bool one = comboBox2.SelectedIndex > 1;
            
            ShowCheckBoxes(one);
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            groupBox1.Invalidate();
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            groupBox1.Invalidate();
            stop = true;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            if (started)
            {
                lock (_locker)
                {
                    foreach (var c in game.Cells)
                    {
                        e.Graphics.FillRectangle(
                            c.Value.color,
                            10 + c.Key.Item1 * ((groupBox1.Size.Width - 5) / GridX),
                            20 + c.Key.Item2 * ((groupBox1.Size.Height - 20) / GridY),
                            15, 14);
                    }
                }
            }
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex > 1)
            {
                comboBox1.Hide();
                for (int i = 1; i < GridY; i++)
                {
                    for (int j = 0; j < GridX; j++)
                    {
                        Grid[i, j].Checked = false;
                        Grid[i, j].Hide();
                    }
                }
            }
            else
            {
                comboBox1.Show();
                for (int i = 1; i < GridY; i++)
                {
                    for (int j = 0; j < GridX; j++)
                    {
                        Grid[i, j].Show();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Grid = new CheckBox[GridY, GridX];
            for(int i = 0; i < GridY; i++)
            {
                for (int j = 0; j < GridX; j++)
                {
                    
                    Grid[i, j] = new CheckBox();
                    Grid[i, j].AutoSize = true;
                    Grid[i, j].Location = new System.Drawing.Point(10 + j * ((groupBox1.Size.Width - 5) / GridX), 20 + i * ((groupBox1.Size.Height - 20) / GridY));
                    Grid[i, j].Name = "checkBox" + Convert.ToString(i * 10 + j + 2);
                    Grid[i, j].Size = new System.Drawing.Size(15, 14);
                    Grid[i, j].TabIndex = 0;
                    Grid[i, j].UseVisualStyleBackColor = true;
                    Grid[i, j].Enabled = true;
                    groupBox1.Controls.Add(Grid[i, j]);
                    
                }
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }
    }
}