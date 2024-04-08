using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Trpo1._1
{
    internal abstract class Cell_Prototype
    {
        public int Hp { get; protected set; }
        public Brush color;

        public virtual Cell_Prototype Clone()
        {
            var clone = this.MemberwiseClone() as Cell_Prototype;
            return clone;
        }
        public virtual void Hp_decreace()
        {
            Hp--;
        }
    }

    internal class Standart_Cell:Cell_Prototype
    {
        public Standart_Cell()
        {
            Hp = 1;
            color = new SolidBrush(Color.Black);
        }
    }
    internal class Cell_with_largeHp:Cell_Prototype
    {
        public Cell_with_largeHp()
        {
            Hp = 7;
            color = new SolidBrush(Color.Black);
        }
    }

    internal class Cell_with_randomHpdecreace:Cell_Prototype
    {
        public Cell_with_randomHpdecreace()
        {
            this.Hp = 9;
            color = new SolidBrush(Color.Black);
        }
        public override void Hp_decreace()
        {
            Random rnd = new Random();
            Hp -= rnd.Next(1, 3);
        }
    }
    //-------------------------------------------------------
    internal abstract class Game_Template
    {
        public Dictionary<(int, int), Cell_Prototype> Cells = new Dictionary<(int, int), Cell_Prototype>();
        protected Dictionary<(int,int), int> neighbors = new Dictionary<(int, int), int>();
        protected Cell_Prototype CellSample;
        public bool GameEnd { get; protected set; }

        public void Initialize_Cells(List<int> x, List<int> y, Cell_Prototype init_cell)
        {
            Cells.Clear();
            CellSample = init_cell.Clone();
            for(int i = 0; i < x.Count; i++)
            {
                Cells.Add((x[i], y[i]), init_cell.Clone());
            }
            GameEnd = false;
        }
        public void GameCicle()
        {
            if (Cells.Count != 0)
            {
                foreach (var cell in Cells)
                {
                    CellProcess(cell);
                }

                foreach (var cell in neighbors)
                {
                    ProcessNeigbors(cell);
                }
                CleanUp();
                neighbors.Clear();
            }
            else
            {
                GameEnd = true;
            }
        }
        abstract protected void CellProcess(KeyValuePair<(int, int), Cell_Prototype> cell);
        abstract protected void ProcessNeigbors(KeyValuePair<(int, int), int> cell);
        abstract protected void CleanUp();
    }

    
    internal class StandartGame:Game_Template
    {
        protected override void CellProcess(KeyValuePair<(int, int), Cell_Prototype> cell)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1) { neighbors.TryAdd(cell.Key, 0); continue; }
                    int x = cell.Key.Item1 + (i - 1);
                    int y = cell.Key.Item2 + (j - 1);
                    if (x < 0 || x > 19) continue;
                    if (y < 0 || y > 19) continue;
                    if (!neighbors.TryAdd((x,y), 1)) { neighbors[(x, y)]++; }
                }
            }
        }
        protected override void ProcessNeigbors(KeyValuePair<(int, int), int> cell)
        {
            int y = cell.Key.Item2;
            int x = cell.Key.Item1;
            bool found = false;

            if (Cells.ContainsKey((x,y)))
            {
                found = true;
                if (cell.Value > 3 || cell.Value < 2)
                {
                    Cells[(x,y)].Hp_decreace();
                }
            }
            
            if(!found)
            {
                if(cell.Value == 3)
                {
                    Cells.Add((x, y), CellSample.Clone());
                }
            }
        }
        protected override void CleanUp()
        {
            var copy = new Dictionary<(int, int), Cell_Prototype>(Cells);
            foreach (var cell in copy)
            {
                if (cell.Value.Hp <= 0)
                {
                    Cells.Remove(cell.Key);
                }
            }
        }
    }

    internal class CrossGame:StandartGame
    {
        protected override void CellProcess(KeyValuePair<(int, int), Cell_Prototype> cell)
        {
            if (cell.Key.Item1 != 0)
                if (!neighbors.TryAdd((cell.Key.Item1 - 1, cell.Key.Item2), 1))
                    neighbors[(cell.Key.Item1 - 1, cell.Key.Item2)]++;
            if (cell.Key.Item1 != 19)
                if (!neighbors.TryAdd((cell.Key.Item1 + 1, cell.Key.Item2), 1))
                    neighbors[(cell.Key.Item1 + 1, cell.Key.Item2)]++;
            if (cell.Key.Item2 != 0)
                if (!neighbors.TryAdd((cell.Key.Item1, cell.Key.Item2 - 1), 1))
                    neighbors[(cell.Key.Item1, cell.Key.Item2 - 1)]++;
            if (cell.Key.Item2 != 19)
                if (!neighbors.TryAdd((cell.Key.Item1, cell.Key.Item2 + 1), 1))
                    neighbors[(cell.Key.Item1, cell.Key.Item2 + 1)]++;
        }
    }

    internal abstract class OneDimenshionGame : Game_Template
    {
        private int current_row = 0;
        protected override void CellProcess(KeyValuePair<(int, int), Cell_Prototype> cell)
        {
            if (cell.Key.Item2 == current_row & current_row != 19)
            {
                for (int i = 0; i < 20; i++)
                {
                    neighbors.TryAdd((i, current_row + 1), 0);
                }

                if (cell.Key.Item1 != 0)

                    neighbors[(cell.Key.Item1 - 1, current_row + 1)] += 100;

                neighbors[(cell.Key.Item1, current_row + 1)] += 10;

                if (cell.Key.Item1 != 19)

                    neighbors[(cell.Key.Item1 + 1, current_row + 1)] += 1;
            }
        }
        
        protected override void CleanUp()
        {
            current_row++;
            neighbors.Clear();
        }
    }
    internal class Rule161 : OneDimenshionGame
    {
        protected override void ProcessNeigbors(KeyValuePair<(int, int), int> cell)
        {
            for(int i = 0; i < 20; i++)
            {
                neighbors.TryAdd((i, cell.Key.Item2), 0);
            }
            if (cell.Value == 0 || cell.Value == 101 || cell.Value == 111)
            {
                Cells.TryAdd(cell.Key, CellSample.Clone());
            }
        }
    }
    internal class Rule30:OneDimenshionGame
    {
        protected override void ProcessNeigbors(KeyValuePair<(int, int), int> cell)
        {
            if (cell.Value == 100 || cell.Value == 11 || cell.Value == 10 || cell.Value == 1)
            {
                Cells.TryAdd(cell.Key, CellSample.Clone());
            }
        }
    }
    internal class Rule90 : OneDimenshionGame
    {
        protected override void ProcessNeigbors(KeyValuePair<(int, int), int> cell)
        {
            if (cell.Value == 110 || cell.Value == 100 || cell.Value == 11 || cell.Value == 1)
            {
                Cells.TryAdd(cell.Key, CellSample.Clone());
            }
        }
    }
    internal class Rule110 : OneDimenshionGame
    {
        protected override void ProcessNeigbors(KeyValuePair<(int, int), int> cell)
        {
            if (cell.Value == 110 || cell.Value == 101 || cell.Value == 11 || cell.Value == 10 || cell.Value == 1)
            {
                Cells.TryAdd(cell.Key, CellSample.Clone());
            }
        }
    }

}
