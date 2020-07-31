namespace Sudoku.WinForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.textCell00 = new System.Windows.Forms.TextBox();
            this.textCell30 = new System.Windows.Forms.TextBox();
            this.btnSolve = new System.Windows.Forms.Button();
            this.txtSumBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(570, 0);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(421, 794);
            this.txtStatus.TabIndex = 0;
            // 
            // textCell00
            // 
            this.textCell00.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textCell00.ForeColor = System.Drawing.Color.DarkCyan;
            this.textCell00.Location = new System.Drawing.Point(12, 29);
            this.textCell00.MaxLength = 1;
            this.textCell00.Name = "textCell00";
            this.textCell00.Size = new System.Drawing.Size(50, 38);
            this.textCell00.TabIndex = 1;
            this.textCell00.TextChanged += new System.EventHandler(this.textCell00_TextChanged);
            // 
            // textCell30
            // 
            this.textCell30.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.textCell30.Enabled = false;
            this.textCell30.Location = new System.Drawing.Point(138, 29);
            this.textCell30.Multiline = true;
            this.textCell30.Name = "textCell30";
            this.textCell30.Size = new System.Drawing.Size(5, 400);
            this.textCell30.TabIndex = 4;
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(459, 12);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(75, 23);
            this.btnSolve.TabIndex = 7;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // txtSumBox
            // 
            this.txtSumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSumBox.Location = new System.Drawing.Point(243, 97);
            this.txtSumBox.Multiline = true;
            this.txtSumBox.Name = "txtSumBox";
            this.txtSumBox.Size = new System.Drawing.Size(71, 70);
            this.txtSumBox.TabIndex = 8;
            this.txtSumBox.Text = "\\  23\r\n12\\";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(243, 287);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(71, 20);
            this.textBox2.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(434, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 64);
            this.label1.TabIndex = 10;
            this.label1.Text = "23\r\n12";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 796);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.txtSumBox);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.textCell30);
            this.Controls.Add(this.textCell00);
            this.Controls.Add(this.txtStatus);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox textCell00;
        private System.Windows.Forms.TextBox textCell30;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.TextBox txtSumBox;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
    }
}

