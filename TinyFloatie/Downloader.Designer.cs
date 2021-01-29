namespace TinyFloatie
{
    partial class Downloader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Downloader));
            this.lbFloatieText = new System.Windows.Forms.Label();
            this.pbExpand = new System.Windows.Forms.ProgressBar();
            this.lbBytes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbFloatieText
            // 
            this.lbFloatieText.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFloatieText.ForeColor = System.Drawing.Color.White;
            this.lbFloatieText.Location = new System.Drawing.Point(22, 18);
            this.lbFloatieText.Name = "lbFloatieText";
            this.lbFloatieText.Size = new System.Drawing.Size(242, 20);
            this.lbFloatieText.TabIndex = 0;
            this.lbFloatieText.Text = "Floatie needs to download itself";
            this.lbFloatieText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbExpand
            // 
            this.pbExpand.Location = new System.Drawing.Point(24, 57);
            this.pbExpand.Name = "pbExpand";
            this.pbExpand.Size = new System.Drawing.Size(238, 71);
            this.pbExpand.TabIndex = 1;
            // 
            // lbBytes
            // 
            this.lbBytes.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lbBytes.ForeColor = System.Drawing.Color.White;
            this.lbBytes.Location = new System.Drawing.Point(22, 132);
            this.lbBytes.Name = "lbBytes";
            this.lbBytes.Size = new System.Drawing.Size(242, 20);
            this.lbBytes.TabIndex = 2;
            this.lbBytes.Text = "_";
            this.lbBytes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Downloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(285, 166);
            this.Controls.Add(this.lbBytes);
            this.Controls.Add(this.pbExpand);
            this.Controls.Add(this.lbFloatieText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Downloader";
            this.Text = "Floatie";
            this.Load += new System.EventHandler(this.Downloader_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbFloatieText;
        private System.Windows.Forms.ProgressBar pbExpand;
        private System.Windows.Forms.Label lbBytes;
    }
}

