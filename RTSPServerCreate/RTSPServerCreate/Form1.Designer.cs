namespace RTSPServerCreate
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
            this.btnServerCreate = new System.Windows.Forms.Button();
            this.txbServerStatus = new System.Windows.Forms.TextBox();
            this.btnServerStop = new System.Windows.Forms.Button();
            this.txbConnMax = new System.Windows.Forms.TextBox();
            this.txbConnected = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnServerCreate
            // 
            this.btnServerCreate.Location = new System.Drawing.Point(184, 152);
            this.btnServerCreate.Name = "btnServerCreate";
            this.btnServerCreate.Size = new System.Drawing.Size(75, 23);
            this.btnServerCreate.TabIndex = 0;
            this.btnServerCreate.Text = "Create";
            this.btnServerCreate.UseVisualStyleBackColor = true;
            this.btnServerCreate.Click += new System.EventHandler(this.btnServerCreate_Click);
            // 
            // txbServerStatus
            // 
            this.txbServerStatus.Location = new System.Drawing.Point(10, 8);
            this.txbServerStatus.Multiline = true;
            this.txbServerStatus.Name = "txbServerStatus";
            this.txbServerStatus.ReadOnly = true;
            this.txbServerStatus.Size = new System.Drawing.Size(322, 131);
            this.txbServerStatus.TabIndex = 1;
            // 
            // btnServerStop
            // 
            this.btnServerStop.Location = new System.Drawing.Point(265, 152);
            this.btnServerStop.Name = "btnServerStop";
            this.btnServerStop.Size = new System.Drawing.Size(75, 23);
            this.btnServerStop.TabIndex = 2;
            this.btnServerStop.Text = "Shutdown";
            this.btnServerStop.UseVisualStyleBackColor = true;
            this.btnServerStop.Click += new System.EventHandler(this.btnServerStop_Click);
            // 
            // txbConnMax
            // 
            this.txbConnMax.Location = new System.Drawing.Point(59, 155);
            this.txbConnMax.Name = "txbConnMax";
            this.txbConnMax.ReadOnly = true;
            this.txbConnMax.Size = new System.Drawing.Size(119, 20);
            this.txbConnMax.TabIndex = 3;
            // 
            // txbConnected
            // 
            this.txbConnected.Location = new System.Drawing.Point(59, 180);
            this.txbConnected.Name = "txbConnected";
            this.txbConnected.ReadOnly = true;
            this.txbConnected.Size = new System.Drawing.Size(119, 20);
            this.txbConnected.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Max:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Current:";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(265, 181);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 7;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 213);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbConnected);
            this.Controls.Add(this.txbConnMax);
            this.Controls.Add(this.btnServerStop);
            this.Controls.Add(this.txbServerStatus);
            this.Controls.Add(this.btnServerCreate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnServerCreate;
        private System.Windows.Forms.TextBox txbServerStatus;
        private System.Windows.Forms.Button btnServerStop;
        private System.Windows.Forms.TextBox txbConnMax;
        private System.Windows.Forms.TextBox txbConnected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnTest;
    }
}

