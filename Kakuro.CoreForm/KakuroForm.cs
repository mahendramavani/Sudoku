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
        private int _tabStopIndex = 1;
        private int _xSize;
        private int _ySize;

        public KakuroForm()
        {
            InitializeComponent();
            CreateForm();

            HardCodedInputs();
        }

        private void HardCodedInputs()
        {
            _txtXSize.Text = "8";
            _txtYSize.Text = "10";
            btnDraw_Click(_btnDraw,EventArgs.Empty);
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
            _txtBoxes[0, 0].Text = @"x";
            _txtBoxes[1, 0].Text = @"x";
            _txtBoxes[2, 0].Text = @"x";
            _txtBoxes[3, 0].Text = @"x";
            _txtBoxes[4, 0].Text = @"x";
            _txtBoxes[5, 0].Text = @"16\";
            _txtBoxes[6, 0].Text = @"11\";
            _txtBoxes[7, 0].Text = @"x";

            _txtBoxes[0, 1].Text = @"x";
            _txtBoxes[1, 1].Text = @"10\";
            _txtBoxes[2, 1].Text = @"6\";
            _txtBoxes[3, 1].Text = @"x";
            _txtBoxes[4, 1].Text = @"41\9";
            _txtBoxes[5, 1].Text = @"";
            _txtBoxes[6, 1].Text = @"";
            _txtBoxes[7, 1].Text = @"24\";

            _txtBoxes[0, 2].Text = @"\12";
            _txtBoxes[1, 2].Text = @"";
            _txtBoxes[2, 2].Text = @"";
            _txtBoxes[3, 2].Text = @"15\29";
            _txtBoxes[4, 2].Text = @"";
            _txtBoxes[5, 2].Text = @"";
            _txtBoxes[6, 2].Text = @"";
            _txtBoxes[7, 2].Text = @"";

            _txtBoxes[0, 3].Text = @"\10";
            _txtBoxes[1, 3].Text = @"";
            _txtBoxes[2, 3].Text = @"";
            _txtBoxes[3, 3].Text = @"";
            _txtBoxes[4, 3].Text = @"";
            _txtBoxes[5, 3].Text = @"\11";
            _txtBoxes[6, 3].Text = @"";
            _txtBoxes[7, 3].Text = @"";

            _txtBoxes[0, 4].Text = @"x";
            _txtBoxes[1, 4].Text = @"\10";
            _txtBoxes[2, 4].Text = @"";
            _txtBoxes[3, 4].Text = @"";
            _txtBoxes[4, 4].Text = @"";
            _txtBoxes[5, 4].Text = @"16\10";
            _txtBoxes[6, 4].Text = @"";
            _txtBoxes[7, 4].Text = @"";

            _txtBoxes[0, 5].Text = @"x";
            _txtBoxes[1, 5].Text = @"15\";
            _txtBoxes[2, 5].Text = @"29\24";
            _txtBoxes[3, 5].Text = @"";
            _txtBoxes[4, 5].Text = @"";
            _txtBoxes[5, 5].Text = @"";
            _txtBoxes[6, 5].Text = @"23\";
            _txtBoxes[7, 5].Text = @"x";

            _txtBoxes[0, 6].Text = @"\6";
            _txtBoxes[1, 6].Text = @"";
            _txtBoxes[2, 6].Text = @"";
            _txtBoxes[3, 6].Text = @"\16";
            _txtBoxes[4, 6].Text = @"";
            _txtBoxes[5, 6].Text = @"";
            _txtBoxes[6, 6].Text = @"";
            _txtBoxes[7, 6].Text = @"3\";

            _txtBoxes[0, 7].Text = @"\17";
            _txtBoxes[1, 7].Text = @"";
            _txtBoxes[2, 7].Text = @"";
            _txtBoxes[3, 7].Text = @"17\18";
            _txtBoxes[4, 7].Text = @"";
            _txtBoxes[5, 7].Text = @"";
            _txtBoxes[6, 7].Text = @"";
            _txtBoxes[7, 7].Text = @"";

            _txtBoxes[0, 8].Text = @"\29";
            _txtBoxes[1, 8].Text = @"";
            _txtBoxes[2, 8].Text = @"";
            _txtBoxes[3, 8].Text = @"";
            _txtBoxes[4, 8].Text = @"";
            _txtBoxes[5, 8].Text = @"\9";
            _txtBoxes[6, 8].Text = @"";
            _txtBoxes[7, 8].Text = @"";

            _txtBoxes[0, 9].Text = @"x";
            _txtBoxes[1, 9].Text = @"\16";
            _txtBoxes[2, 9].Text = @"";
            _txtBoxes[3, 9].Text = @"";
            _txtBoxes[4, 9].Text = @"x";
            _txtBoxes[5, 9].Text = @"x";
            _txtBoxes[6, 9].Text = @"x";
            _txtBoxes[7, 9].Text = @"x";
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
