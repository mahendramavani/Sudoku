using System.Drawing;
using System.Windows.Forms;
using Sudoku.Core;

namespace Sudoku.CoreForm
{
    public partial class SudokuForm : Form, IView
    {
        private Button btnSample1;
        private Button btnSample2;
        private Button btnSolve;
        private Button btnClear;
        private TextBox txtStatus;
        private TextBox[,] textCells;
        private Core.Sudoku _sudoku;

        public SudokuForm()
        {
            InitializeComponent();

            var tabIndex = 1;

            txtStatus = new TextBox();
            btnSample1 = new Button();
            btnSample2 = new Button();
            btnSolve = new Button();
            btnClear = new Button();
            textCells = new TextBox[Core.Sudoku.SIZE, Core.Sudoku.SIZE];
            SuspendLayout();

            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Font;
            
            txtStatus.Location = new System.Drawing.Point(570, 0);
            txtStatus.Multiline = true;
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(421, 500);
            txtStatus.Enabled = false;
            txtStatus.TabStop = false;
            
            for (var y = 0; y < Core.Sudoku.SIZE; y++)
            {
                for (var x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    textCells[x, y] = new TextBox
                    {
                        Location = new System.Drawing.Point((x + 1) * 50, (y + 1) * 30),
                        Name = "Cell" + x + y,
                        Size = new Size(35, 20),
                        TabIndex = tabIndex++,
                    };

                    Controls.Add(textCells[x,y]);
                }
            }

            btnClear.Location = new System.Drawing.Point(150, 400);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 23);
            btnClear.TabIndex = tabIndex;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;

            btnSample1.Location = new System.Drawing.Point(250, 400);
            btnSample1.Name = "btnSample1";
            btnSample1.Size = new Size(75, 23);
            btnSample1.TabIndex = tabIndex;
            btnSample1.Text = "Sample 1";
            btnSample1.UseVisualStyleBackColor = true;
            btnSample1.Click += btnSample1_Click;

            btnSample2.Location = new System.Drawing.Point(350, 400);
            btnSample2.Name = "btnSample2";
            btnSample2.Size = new Size(75, 23);
            btnSample2.TabIndex = tabIndex;
            btnSample2.Text = "Sample 2";
            btnSample2.UseVisualStyleBackColor = true;
            btnSample2.Click += btnSample2_Click;

            btnSolve.Location = new System.Drawing.Point(450, 400);
            btnSolve.Name = "btnSolve";
            btnSolve.Size = new Size(75, 23);
            btnSolve.TabIndex = tabIndex;
            btnSolve.Text = "Solve";
            btnSolve.UseVisualStyleBackColor = true;
            btnSolve.Click += btnSolve_Click;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 550);
            Name = "SudokuForm";
            Text = "Sudoku";

            Controls.Add(btnSolve);
            Controls.Add(btnClear);
            Controls.Add(btnSample1);
            Controls.Add(btnSample2);
            Controls.Add(txtStatus);
            
            ResumeLayout(false);
            PerformLayout();
        }

        private void LoadValues(int[,] values)
        {
            for (var y = 0; y < Core.Sudoku.SIZE; y++)
            {
                for (var x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    var value = values[x, y];

                    if (value != 0)
                    {
                        textCells[x, y].BackColor = Color.Green;
                        textCells[x, y].Text = value.ToString();
                    }
                    else
                    {
                        textCells[x, y].ResetBackColor();
                        textCells[x, y].Text = string.Empty;
                    }
                }
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            for (var y = 0; y < Core.Sudoku.SIZE; y++)
            {
                for (var x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    textCells[x, y].Text = string.Empty;
                    textCells[x,y].ResetBackColor();
                }
            }
        }

        private void btnSample1_Click(object sender, System.EventArgs e)
        {
            var sampleInput = new[,]
            {
                {2, 9, 5,   7, 0, 0,   8, 6, 0},
                {0, 3, 1,   8, 6, 5,   9, 2, 0},
                {8, 0, 6,   0, 0, 0,   0, 0, 0},
                
                {0, 0, 7,   0, 5, 0,   0, 0, 6},
                {0, 0, 0,   3, 8, 7,   0, 0, 5},
                {5, 0, 0,   0, 1, 6,   7, 0, 0},
                    
                {0, 0, 0,   5, 0, 0,   1, 0, 9},
                {0, 2, 0,   6, 0, 0,   3, 0, 0},
                {0, 5, 4,   0, 0, 8,   6, 7, 2},
            };
            LoadValues(sampleInput);
        }

        private void btnSample2_Click(object sender, System.EventArgs e)
        {
            var sampleInput = new[,]
            {
                // {0, 1, 9, 0, 0, 0, 0, 0, 0},
                // {6, 0, 0, 0, 0, 3, 0, 4, 0},
                // {7, 3, 4, 5, 8, 9, 6, 1, 0},
                // {0, 0, 6, 8, 1, 7, 4, 3, 0},
                // {0, 5, 1, 3, 9, 0, 0, 0, 0},
                // {4, 0, 3, 2, 0, 0, 0, 0, 0},
                // {0, 0, 0, 4, 7, 0, 1, 0, 3},
                // {0, 0, 2, 9, 0, 0, 8, 6, 7},
                // {0, 9, 0, 0, 3, 0, 0, 2, 0},

                {0, 0, 0, 0, 0, 9, 0, 0, 6},
                {0, 0, 0, 0, 0, 3, 8, 5, 1},
                {0, 6, 2, 0, 1, 5, 0, 0, 0},
                {0, 0, 7, 0, 0, 0, 0, 6, 0},
                {0, 2, 1, 9, 7, 6, 3, 8, 0},
                {0, 3, 0, 0, 0, 0, 1, 0, 0},
                {0, 0, 0, 4, 5, 0, 9, 7, 0},
                {2, 5, 8, 6, 0, 0, 0, 0, 0},
                {4, 0, 0, 3, 0, 0, 0, 0, 0},
            };
            LoadValues(sampleInput);
        }

        private void btnSolve_Click(object sender, System.EventArgs e)
        {
            txtStatus.Text = string.Empty;
            _sudoku = new Core.Sudoku(this);
            _sudoku.Solve();
        }


        public int[,] Inputs
        {
            get
            {
                var inputs = new int[Core.Sudoku.SIZE, Core.Sudoku.SIZE];
                for (var y = 0; y < Core.Sudoku.SIZE; y++)
                {
                    for (var x = 0; x < Core.Sudoku.SIZE; x++)
                    {
                        var text = textCells[x, y].Text.Trim();
                        if (string.IsNullOrWhiteSpace(text))
                            inputs[x, y] = 0;

                        int entry;
                        if (int.TryParse(text, out entry))
                        {
                            inputs[x,y] = entry;
                        }
                        else
                        {
                            inputs[x, y] = 0;
                        }
                    }
                }

                return inputs;
            }
        }

        public void AppendStatus(string status)
        {
            txtStatus.Text += "\r\n==>" + status;
        }

        public void Print(Core.Point[,] points)
        {
            for (var y = 0; y < Core.Sudoku.SIZE; y++)
            {
                for (var x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    var point = points[y,x];
                    var textCell = textCells[x, y];
                    

                    textCell.Text = point.Value.ToString();

                    if (point.Type == ValueType.Given)
                    {
                        textCell.BackColor = Color.LightGreen;
                    }
                    else if (point.Type == ValueType.Calculated)
                    {
                        textCell.BackColor = Color.LightBlue;
                    }
                    else if (point.Type == ValueType.Guessed)
                    {
                        textCell.BackColor = Color.LightPink;
                    }
                }
            }
        }

        public void InValidInput(string message)
        {
            MessageBox.Show(message);
        }
    }
}
