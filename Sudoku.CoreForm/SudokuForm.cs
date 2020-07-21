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
        private TextBox txtVerticalSeparator1;
        private TextBox txtVerticalSeparator2;
        private TextBox txtHorizontalSeparator1;
        private TextBox txtHorizontalSeparator2;
        private TextBox[,] textCells;
        private Core.Sudoku _sudoku;

        public SudokuForm()
        {
            InitializeComponent();
            
            SuspendLayout();

            components = new System.ComponentModel.Container();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 550);
            Name = "SudokuForm";
            Text = "Sudoku";

            txtStatus = new TextBox
            {
                Location = new System.Drawing.Point(570, 10),
                Multiline = true,
                Name = "txtStatus",
                Size = new Size(420, 500),
                Enabled = false,
                TabStop = false
            };
            Controls.Add(txtStatus);

            txtVerticalSeparator1 = new TextBox
            {
                BackColor = SystemColors.InactiveCaptionText,
                Enabled = false,
                Multiline = true,
                Name = "txtVerticalSeparator1",
                Size = new Size(5, 450),
                Location = new System.Drawing.Point(190, 40)
            };
            Controls.Add(txtVerticalSeparator1);
            
            txtVerticalSeparator2 = new TextBox
            {
                BackColor = SystemColors.InactiveCaptionText,
                Enabled = false,
                Multiline = true,
                Name = "txtVerticalSeparator2",
                Size = new Size(5, 450),
                Location = new System.Drawing.Point(340, 40)
            };
            Controls.Add(txtVerticalSeparator2);
            
            txtHorizontalSeparator1 = new TextBox
            {
                BackColor = SystemColors.InactiveCaptionText,
                Enabled = false,
                Multiline = true,
                Name = "txtHorizontalSeparator1",
                Size = new Size(450, 5),
                Location = new System.Drawing.Point(40, 190)
            };
            Controls.Add(txtHorizontalSeparator1);

            txtHorizontalSeparator2 = new TextBox
            {
                BackColor = SystemColors.InactiveCaptionText,
                Enabled = false,
                Multiline = true,
                Name = "txtHorizontalSeparator2",
                Size = new Size(450, 5),
                Location = new System.Drawing.Point(40, 340)
            };
            Controls.Add(txtHorizontalSeparator2);

            var tabIndex = 1;
            textCells = new TextBox[Core.Sudoku.SIZE, Core.Sudoku.SIZE];
            for (var y = 0; y < Core.Sudoku.SIZE; y++)
            {
                for (var x = 0; x < Core.Sudoku.SIZE; x++)
                {
                    textCells[x, y] = new TextBox
                    {
                        Location = new System.Drawing.Point((x + 1) * 50, (y + 1) * 50),
                        Name = "Cell" + x + y,
                        Size = new Size(35, 30),
                        TabIndex = tabIndex++,
                        Font = new Font("Times New Roman", 20F, FontStyle.Bold, GraphicsUnit.Point, 0),
                        MaxLength = 1,
                    };
                    textCells[x,y].TextChanged += textCells_TextChanged;
                    Controls.Add(textCells[x,y]);
                }
            }

            btnClear = new Button
            {
                Location = new System.Drawing.Point(150, 500),
                Name = "btnClear",
                Size = new Size(75, 23),
                TabIndex = tabIndex,
                Text = "Clear",
                UseVisualStyleBackColor = true
            };
            btnClear.Click += btnClear_Click;
            Controls.Add(btnClear);

            btnSample1 = new Button
            {
                Location = new System.Drawing.Point(250, 500),
                Name = "btnSample1",
                Size = new Size(75, 23),
                TabIndex = tabIndex,
                Text = "Sample 1",
                UseVisualStyleBackColor = true
            };
            btnSample1.Click += btnSample1_Click;
            Controls.Add(btnSample1);
            
            btnSample2 = new Button
            {
                Location = new System.Drawing.Point(350, 500),
                Name = "btnSample2",
                Size = new Size(75, 23),
                TabIndex = tabIndex,
                Text = "Sample 2",
                UseVisualStyleBackColor = true
            };
            btnSample2.Click += btnSample2_Click;
            Controls.Add(btnSample2);

            btnSolve = new Button
            {
                Location = new System.Drawing.Point(450, 500),
                Name = "btnSolve",
                Size = new Size(75, 23),
                TabIndex = tabIndex,
                Text = "Solve",
                UseVisualStyleBackColor = true
            };
            btnSolve.Click += btnSolve_Click;
            Controls.Add(btnSolve);
            
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
                        textCells[x, y].BackColor = Color.LightGreen;
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


        private void textCells_TextChanged(object sender, System.EventArgs e)
        {
            SendKeys.Send("\t");
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

            textCells[0, 0].Focus();
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
