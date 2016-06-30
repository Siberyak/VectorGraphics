using Shapes;

namespace D3.Viewer
{
    partial class D3Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(D3Form));
            this._fillViewPort = new Shapes.ViewPort();
            this._topViewPort = new Shapes.ViewPort();
            this.SuspendLayout();
            // 
            // _fillViewPort
            // 
            this._fillViewPort.ClipRectangle = ((System.Drawing.RectangleF)(resources.GetObject("_fillViewPort.ClipRectangle")));
            this._fillViewPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this._fillViewPort.Location = new System.Drawing.Point(0, 268);
            this._fillViewPort.Name = "_fillViewPort";
            this._fillViewPort.Shapes = null;
            this._fillViewPort.Size = new System.Drawing.Size(1548, 288);
            this._fillViewPort.TabIndex = 1;
            this._fillViewPort.Text = "userControl12";
            this._fillViewPort.Zoom = 1F;
            // 
            // _topViewPort
            // 
            this._topViewPort.ClipRectangle = ((System.Drawing.RectangleF)(resources.GetObject("_topViewPort.ClipRectangle")));
            this._topViewPort.Dock = System.Windows.Forms.DockStyle.Top;
            this._topViewPort.Location = new System.Drawing.Point(0, 0);
            this._topViewPort.Name = "_topViewPort";
            this._topViewPort.Shapes = null;
            this._topViewPort.Size = new System.Drawing.Size(1548, 268);
            this._topViewPort.TabIndex = 0;
            this._topViewPort.Text = "userControl11";
            this._topViewPort.Zoom = 1F;
            // 
            // D3Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1548, 556);
            this.Controls.Add(this._fillViewPort);
            this.Controls.Add(this._topViewPort);
            this.Name = "Form2";
            this.Text = "D3Form";
            this.ResumeLayout(false);

        }

        #endregion

        private ViewPort _topViewPort;
        private ViewPort _fillViewPort;
    }
}