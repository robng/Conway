﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Conway
{
    public class Display : Form
    {
        // unclean, instead of timers use a loop
        private System.ComponentModel.IContainer components;
        private Timer timer;
        
        public readonly Grid Grid;
        
        public Display(Grid grid)
        {
            Grid = grid;
            ClientSize = new Size(grid.Width, grid.Height);
            Setup();
        }

        private void Setup()
        {
            InitializeComponent();
            Grid.OnUpdate += OnGridUpdate;
            timer.Start();
            timer.Interval = 1;
            timer.Tick += Timer_Tick;
            BackColor = Color.RoyalBlue;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            Show();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!Grid.IsSetup)
            {
                return;
            }

            Grid.Update();
        }

        private void OnGridUpdate()
        {
            Refresh();
        }
        
        protected override void OnPaint(PaintEventArgs args)
        {
            var now = DateTime.Now;
            var graphics = args.Graphics;
            var font = new Font("Courier New", 11);
            graphics.DrawString("Generation: " + Grid.Iteration, font, Brushes.White, new Point(0, 0));
            graphics.DrawString("Live Cells: " + Grid.AliveCellCount, font, Brushes.White, new Point(0, 12));
            
            foreach (var cell in Grid.Cells)
            {
                if (cell == null)
                {
                    continue;
                }

                if (cell.IsAlive)
                {
                    graphics.FillRectangle(Brushes.White, cell.AbsoluteX, cell.AbsoluteY, Grid.CellSize, Grid.CellSize);
                }
                else
                {
                    if (cell.WasVisited)
                    {
                        const float decaySpeed = 1F;
                        var color = Color.White;
                        
                        var secondsSinceLastVisit = (now - cell.LastVisited).TotalSeconds;
                        if (secondsSinceLastVisit < decaySpeed)
                        {
                            var intensity = 255 - ((int) ((255 / decaySpeed) * secondsSinceLastVisit));
                            using (var brush = new SolidBrush(Color.FromArgb(intensity, color.R, color.G, color.B)))
                            {
                                graphics.FillRectangle(brush, cell.AbsoluteX, cell.AbsoluteY, Grid.CellSize, Grid.CellSize);
                            }
                        }
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Display
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Display";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Display_KeyPress);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Display_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Display_MouseMove);
            this.ResumeLayout(false);

        }

        private void Display_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    Grid.ToggleCell(Grid.GetCellAtAbsolute(e.X, e.Y));
            //}
            //if (e.Button == MouseButtons.Right)
            //{
            //    Grid.IsRunning = !Grid.IsRunning;
            //}
        }

        private void Display_MouseMove(object sender, MouseEventArgs e)
        {
            var cell = Grid.GetCellAtAbsolute(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                Grid.SetCellAlive(cell, true);
            }
            if (e.Button == MouseButtons.Right)
            {
                Grid.SetCellAlive(cell, false);
            }

            Grid.Update();
        }

        private void Display_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                Grid.IsRunning = !Grid.IsRunning;
            }
            if (e.KeyChar == 'r')
            {
                Grid.Reset();
            }
            if (e.KeyChar == 's')
            {
                var dialog = new SaveFileDialog();
                dialog.ShowDialog();
                Grid.Save(dialog.FileName);
                //dialog.FileOk += (a, b) =>
                //{
                //    MessageBox.Show(123 + "");
                //};
            }
            if (e.KeyChar == 'l')
            {
                var dialog = new OpenFileDialog();
                dialog.ShowDialog();
                Grid.Load(dialog.FileName);
                //dialog.FileOk += (a, b) =>
                //{
                //    Grid.Load(dialog.FileName);
                //};
            }
        }
    }
}