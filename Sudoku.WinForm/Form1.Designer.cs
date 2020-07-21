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
            this.textCell10 = new System.Windows.Forms.TextBox();
            this.textCell03 = new System.Windows.Forms.TextBox();
            this.textCell30 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.btnSolve = new System.Windows.Forms.Button();
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
            this.textCell00.Location = new System.Drawing.Point(12, 29);
            this.textCell00.Name = "textCell00";
            this.textCell00.Size = new System.Drawing.Size(30, 20);
            this.textCell00.TabIndex = 1;
            // 
            // textCell10
            // 
            this.textCell10.Location = new System.Drawing.Point(48, 29);
            this.textCell10.Name = "textCell10";
            this.textCell10.Size = new System.Drawing.Size(30, 20);
            this.textCell10.TabIndex = 2;
            // 
            // textCell03
            // 
            this.textCell03.Location = new System.Drawing.Point(12, 118);
            this.textCell03.Name = "textCell03";
            this.textCell03.Size = new System.Drawing.Size(30, 20);
            this.textCell03.TabIndex = 3;
            // 
            // textCell30
            // 
            this.textCell30.Location = new System.Drawing.Point(138, 29);
            this.textCell30.Name = "textCell30";
            this.textCell30.Size = new System.Drawing.Size(30, 20);
            this.textCell30.TabIndex = 4;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(12, 81);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(30, 20);
            this.textBox5.TabIndex = 5;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(12, 55);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(30, 20);
            this.textBox6.TabIndex = 6;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 796);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textCell30);
            this.Controls.Add(this.textCell03);
            this.Controls.Add(this.textCell10);
            this.Controls.Add(this.textCell00);
            this.Controls.Add(this.txtStatus);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox textCell00;
        private System.Windows.Forms.TextBox textCell10;
        private System.Windows.Forms.TextBox textCell03;
        private System.Windows.Forms.TextBox textCell30;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button btnSolve;
    }
}

