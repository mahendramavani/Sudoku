using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Kakuro.Core;

namespace Kakuro.CoreForm
{
    public partial class KakuroForm : Form, IKakuroView
    {
        private TextBox[,] _txtBoxes = new TextBox[0,0];
        private TextBox _txtXSize;
        private TextBox _txtYSize;
        private Button _btnDraw;
        private Button _btnSolve;
        private Button _btnClear;
        private Button _btnSample1;

        private int _tabStopIndex = 1;
        private int _xSize;
        private int _ySize;

        public KakuroForm()
        {
            InitializeComponent();
            CreateForm();
        }

        private void CreateForm()
        {
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(988, 796);
            Name = "frmKakuroForm";
            Text = "KakuroForm";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
            PerformLayout();

            _txtXSize = new TextBox
            {
                Font = new Font("Times New Roman", 20F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkCyan,
                Location = new Point(5, 5),
                Size = new Size(50, 40),
                MaxLength = 2,
                Name = "txtXSize",
                TabIndex = _tabStopIndex++,
            };
            Controls.Add(_txtXSize);

            _txtYSize = new TextBox
            {
                Font = new Font("Times New Roman", 20F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkCyan,
                Location = new Point(60, 5),
                Size = new Size(50, 40),
                MaxLength = 2,
                Name = "txtYSize",
                TabIndex = _tabStopIndex++,
            };
            Controls.Add(_txtYSize);

            _btnDraw = new Button
            {
                Location = new Point(120, 5),
                Name = "btnDraw",
                Size = new Size(75, 40),
                TabIndex = _tabStopIndex++,
                Text = "Draw",
                UseVisualStyleBackColor = true
            };
            _btnDraw.Click += btnDraw_Click;
            Controls.Add(_btnDraw);
            
            _btnSolve = new Button
            {
                Location = new Point(200, 5),
                Name = "btnSolve",
                Size = new Size(75, 40),
                Text = "Solve",
                UseVisualStyleBackColor = true,
                Enabled = false,
            };
            _btnSolve.Click += btnSolve_Click;
            Controls.Add(_btnSolve);
            
            _btnClear = new Button
            {
                Location = new Point(280, 5),
                Name = "btnClear",
                Size = new Size(75, 40),
                Text = "Clear",
                UseVisualStyleBackColor = true,
                Enabled = false,
            };
            _btnClear.Click += btnClear_Click;
            Controls.Add(_btnClear);

            _btnSample1 = new Button
            {
                Location = new Point(360, 5),
                Name = "btnSample1",
                Size = new Size(100, 40),
                Text = "Sample (8x10)",
                UseVisualStyleBackColor = true,
                Enabled = true,
            };  
            _btnSample1.Click += btnSample1_Click;
            Controls.Add(_btnSample1);
        }

        private void btnSample1_Click(object sender, EventArgs e)
        {
            var hardCodedInputs = new[,]
            {
                { @"x",   @"x",    @"\12",   @"\10", @"x",     @"x",     @"\6",  @"\17",   @"\29", @"x"   },
                { @"x",   @"10\",  @"",      @"",    @"\10",   @"15\",   @"",    @"",      @"",    @"\16" },
                { @"x",   @"6\",   @"",      @"",    @"",      @"29\24", @"",    @"",      @"",    @""    },
                { @"x",   @"x",    @"15\29", @"",    @"",      @"",      @"\16", @"17\18", @"",    @""    },
                { @"x",   @"41\9", @"",      @"",    @"",      @"",      @"",    @"",      @"",    @"x"   },
                { @"16\", @"",     @"",      @"\11", @"16\10", @"",      @"",    @"",      @"\9",  @"x"   },
                { @"11\", @"",     @"",      @"",    @"",      @"23\",   @"",    @"",      @"",    @"x"   },
                { @"x",   @"24\",  @"",      @"",    @"",      @"x",     @"3\",  @"",      @"",    @"x"   },
            };

            var xSize = hardCodedInputs.GetLength(0);
            var ySize = hardCodedInputs.GetLength(1);

            _txtXSize.Text = xSize.ToString();
            _txtYSize.Text = ySize.ToString();

            btnDraw_Click(_btnDraw, EventArgs.Empty);

            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    _txtBoxes[x, y].Text = hardCodedInputs[x, y];
                }
            }

            btnSolve_Click(_btnSolve, EventArgs.Empty);
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(_txtXSize.Text, out _xSize))
            {
                MessageBox.Show("Invalid input: XSize","Input Error",MessageBoxButtons.OK);
                _txtXSize.Focus();
                return;
            }
            
            if (!int.TryParse(_txtYSize.Text, out _ySize))
            {
                MessageBox.Show("Invalid input: YSize","Input Error",MessageBoxButtons.OK);
                _txtYSize.Focus();
                return;
            }

            _txtBoxes = new TextBox[_xSize,_ySize];

            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    var txtBox = new TextBox()
                    {
                        Location = new Point(60 + x*75, 50 + y*65),
                        Name = $"txtYSize_{x}_{y}",
                        TabIndex = _tabStopIndex++,

                        Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0),
                        ForeColor = Color.DarkCyan,
                        Size = new Size(70, 60),
                        Multiline = true,
                        MaxLength = 7,
                    };
                    Controls.Add(txtBox);
                    _txtBoxes[x,y] = txtBox;
                }
            }

            _btnDraw.Enabled = false;

            _btnSolve.Enabled = true;
            _btnSolve.TabIndex = _tabStopIndex++;

            _btnClear.Enabled = true;
            _btnClear.TabIndex = _tabStopIndex++;

            HardCodedKakuroInputs();
        }

        private void HardCodedKakuroInputs()
        {
            
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            _btnSolve.Enabled = false;
            var kakuroBoard = new KakuroBoard(this,_xSize, _ySize);
            kakuroBoard.Solve();
        }

        public string[,] UserInputs
        {
            get
            {
                var inputs = new string[_xSize, _ySize];

                for (var y = 0; y < _ySize; y++)
                {
                    for (var x = 0; x < _xSize; x++)
                    {
                        var textBox = _txtBoxes[x, y];
                        inputs[x, y] = textBox.Text.Trim();
                    }
                }

                return inputs;
            }
        }

        public void DisplayCurrentStatus(IList<SumCell> sumCells)
        {
            foreach (var sumCell in sumCells)
            {
                var sumTextBox = _txtBoxes[sumCell.XPosition,sumCell.YPosition];

                sumTextBox.BackColor = Color.DarkGray;
                sumTextBox.Font = new Font("Times New Roman", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
                sumTextBox.Enabled = false;
                sumTextBox.Text = sumCell.GetDisplayValue();

                foreach (var gameCell in sumCell.GetXPartCells())
                {
                    var gameTextBox = _txtBoxes[gameCell.XPosition, gameCell.YPosition];

                    if (gameCell.IsSolved)
                    {
                        gameTextBox.Font = new Font("Times New Roman", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
                        gameTextBox.Text = gameCell.DisplaySolutionValue();
                    }
                    else
                    {
                        gameTextBox.Font = new Font("Times New Roman", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
                        gameTextBox.MaxLength = 15;
                        gameTextBox.Text = gameCell.GetEliminationValueDisplayValue();
                    }
                }
            }
        }
        public void RemoveCell(Cell cell)
        {
            Controls.Remove(_txtBoxes[cell.XPosition,cell.YPosition]);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    var textBox = _txtBoxes[x, y];
                    Controls.Remove(textBox);
                }
            }

            _btnDraw.Enabled = true;

            _btnClear.Enabled = false;
            _btnSolve.Enabled = false;

            _txtXSize.Text = string.Empty;
            _txtYSize.Text = string.Empty;
            
            _txtXSize.Focus();
        }
    }
}
