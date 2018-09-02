namespace DMR
{
	partial class DMRIDForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DMRIDForm));
			this.btnDownload = new System.Windows.Forms.Button();
			this.btnReadFromGD77 = new System.Windows.Forms.Button();
			this.btnWriteToGD77 = new System.Windows.Forms.Button();
			this.txtRegionId = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.lblMessage = new System.Windows.Forms.Label();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnDownload
			// 
			this.btnDownload.Location = new System.Drawing.Point(12, 35);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(125, 23);
			this.btnDownload.TabIndex = 0;
			this.btnDownload.Text = "Download";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			// 
			// btnReadFromGD77
			// 
			this.btnReadFromGD77.Location = new System.Drawing.Point(12, 545);
			this.btnReadFromGD77.Name = "btnReadFromGD77";
			this.btnReadFromGD77.Size = new System.Drawing.Size(123, 23);
			this.btnReadFromGD77.TabIndex = 1;
			this.btnReadFromGD77.Text = "Read from GD-77";
			this.btnReadFromGD77.UseVisualStyleBackColor = true;
			this.btnReadFromGD77.Click += new System.EventHandler(this.btnReadFromGD77_Click);
			// 
			// btnWriteToGD77
			// 
			this.btnWriteToGD77.Location = new System.Drawing.Point(294, 545);
			this.btnWriteToGD77.Name = "btnWriteToGD77";
			this.btnWriteToGD77.Size = new System.Drawing.Size(123, 23);
			this.btnWriteToGD77.TabIndex = 2;
			this.btnWriteToGD77.Text = "Write to GD-77";
			this.btnWriteToGD77.UseVisualStyleBackColor = true;
			this.btnWriteToGD77.Click += new System.EventHandler(this.btnWriteToGD77_Click);
			// 
			// txtRegionId
			// 
			this.txtRegionId.Location = new System.Drawing.Point(160, 37);
			this.txtRegionId.Name = "txtRegionId";
			this.txtRegionId.Size = new System.Drawing.Size(100, 20);
			this.txtRegionId.TabIndex = 3;
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(292, 35);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(125, 23);
			this.btnClear.TabIndex = 4;
			this.btnClear.Text = "Clear list";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// lblMessage
			// 
			this.lblMessage.Location = new System.Drawing.Point(12, 9);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(405, 23);
			this.lblMessage.TabIndex = 5;
			this.lblMessage.Text = "lblMessage";
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(13, 66);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(404, 473);
			this.dataGridView1.TabIndex = 6;
			// 
			// DMRIDForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 580);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.txtRegionId);
			this.Controls.Add(this.btnWriteToGD77);
			this.Controls.Add(this.btnReadFromGD77);
			this.Controls.Add(this.btnDownload);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DMRIDForm";
			this.Text = "DMR ID";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DMRIDFormNew_FormClosing);
			this.Load += new System.EventHandler(this.DMRIDForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.Button btnReadFromGD77;
		private System.Windows.Forms.Button btnWriteToGD77;
		private System.Windows.Forms.TextBox txtRegionId;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.DataGridView dataGridView1;
	}
}