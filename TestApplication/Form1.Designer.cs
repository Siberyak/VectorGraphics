using Shapes;

namespace TestApplication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this._userControl1 = new Shapes.ViewPort();
            this._userControl2 = new Shapes.ViewPort();
            this._userControl3 = new Shapes.ViewPort();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._userControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1007, 493);
            this.splitContainer1.SplitterDistance = 479;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this._userControl2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this._userControl3);
            this.splitContainer2.Size = new System.Drawing.Size(524, 493);
            this.splitContainer2.SplitterDistance = 249;
            this.splitContainer2.TabIndex = 2;
            // 
            // _userControl1
            // 
            this._userControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._userControl1.Location = new System.Drawing.Point(0, 0);
            this._userControl1.Name = "_userControl1";
            this._userControl1.Shapes = null;
            this._userControl1.Size = new System.Drawing.Size(479, 493);
            this._userControl1.TabIndex = 0;
            this._userControl1.Zoom = 1F;
            // 
            // _userControl2
            // 
            this._userControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._userControl2.Location = new System.Drawing.Point(0, 0);
            this._userControl2.Name = "_userControl2";
            this._userControl2.Shapes = null;
            this._userControl2.Size = new System.Drawing.Size(524, 249);
            this._userControl2.TabIndex = 1;
            this._userControl2.Text = "userControl11";
            this._userControl2.Zoom = 1F;
            // 
            // _userControl3
            // 
            this._userControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this._userControl3.Location = new System.Drawing.Point(0, 0);
            this._userControl3.Name = "_userControl3";
            this._userControl3.Shapes = null;
            this._userControl3.Size = new System.Drawing.Size(524, 240);
            this._userControl3.TabIndex = 0;
            this._userControl3.Text = "userControl11";
            this._userControl3.Zoom = 1F;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 505);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ViewPort _userControl1;
        private ViewPort _userControl2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ViewPort _userControl3;
        //private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}

