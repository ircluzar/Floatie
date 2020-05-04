namespace Floatie
{
    partial class Container
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Container));
            this.pnDrag = new System.Windows.Forms.Panel();
            this.pnDisplayHeader = new System.Windows.Forms.Panel();
            this.pnDisplayKnob = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnDrag
            // 
            this.pnDrag.AllowDrop = true;
            this.pnDrag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnDrag.BackColor = System.Drawing.Color.Transparent;
            this.pnDrag.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pnDrag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnDrag.Location = new System.Drawing.Point(0, 0);
            this.pnDrag.Name = "pnDrag";
            this.pnDrag.Size = new System.Drawing.Size(256, 256);
            this.pnDrag.TabIndex = 0;
            this.pnDrag.DragDrop += new System.Windows.Forms.DragEventHandler(this.pnDrag_DragDrop);
            this.pnDrag.DragEnter += new System.Windows.Forms.DragEventHandler(this.pnContainer_DragEnter);
            this.pnDrag.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Container_MouseDown);
            // 
            // pnDisplayHeader
            // 
            this.pnDisplayHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnDisplayHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.pnDisplayHeader.Location = new System.Drawing.Point(0, 0);
            this.pnDisplayHeader.Name = "pnDisplayHeader";
            this.pnDisplayHeader.Size = new System.Drawing.Size(256, 8);
            this.pnDisplayHeader.TabIndex = 0;
            this.pnDisplayHeader.Visible = false;
            // 
            // pnDisplayKnob
            // 
            this.pnDisplayKnob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnDisplayKnob.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.pnDisplayKnob.Location = new System.Drawing.Point(248, 248);
            this.pnDisplayKnob.Name = "pnDisplayKnob";
            this.pnDisplayKnob.Size = new System.Drawing.Size(8, 8);
            this.pnDisplayKnob.TabIndex = 0;
            this.pnDisplayKnob.Visible = false;
            // 
            // Container
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.Controls.Add(this.pnDisplayHeader);
            this.Controls.Add(this.pnDisplayKnob);
            this.Controls.Add(this.pnDrag);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Container";
            this.Text = "Floatie";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Container_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.Container_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Container_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Container_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Container_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Container_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Container_MouseMove);
            this.Resize += new System.EventHandler(this.Container_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnDrag;
        public System.Windows.Forms.Panel pnDisplayKnob;
        public System.Windows.Forms.Panel pnDisplayHeader;
    }
}

