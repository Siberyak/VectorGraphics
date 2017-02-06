using Shapes;

namespace Graph.Viewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private ViewPort _viewPort;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            _viewPort = new Shapes.ViewPort();
            this.SuspendLayout();
            // 
            // _viewPort
            // 
            _viewPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            _viewPort.ClipRectangle = ((System.Drawing.RectangleF)(resources.GetObject("_viewPort.ClipRectangle")));
            _viewPort.Location = new System.Drawing.Point(12, 99);
            _viewPort.Name = "_viewPort";
            _viewPort.Shapes = null;
            _viewPort.Size = new System.Drawing.Size(570, 295);
            _viewPort.TabIndex = 0;
            _viewPort.Text = "viewPort1";
            _viewPort.Zoom = 1F;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 406);
            this.Controls.Add(_viewPort);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion
    }
}