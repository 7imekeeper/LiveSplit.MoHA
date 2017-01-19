namespace LiveSplit.MoHA
{
	partial class MoHASettings
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitBriefingsCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// splitBriefingsCheckBox
			// 
			this.splitBriefingsCheckBox.AutoSize = true;
			this.splitBriefingsCheckBox.Location = new System.Drawing.Point(3, 3);
			this.splitBriefingsCheckBox.Name = "splitBriefingsCheckBox";
			this.splitBriefingsCheckBox.Size = new System.Drawing.Size(114, 17);
			this.splitBriefingsCheckBox.TabIndex = 0;
			this.splitBriefingsCheckBox.Text = "Split After Briefings";
			this.splitBriefingsCheckBox.UseVisualStyleBackColor = true;
			// 
			// MoHASettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitBriefingsCheckBox);
			this.Name = "MoHASettings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox splitBriefingsCheckBox;
	}
}
