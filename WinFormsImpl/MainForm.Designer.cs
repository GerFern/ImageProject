namespace PlatformImpl.WinForms
{
    partial class MainForm
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
        /// Required method for Designer support - do modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            if(CreateMatrixViewer) this.matrixImageView = new MatrixImageView();
            if(CreateHistoryViewer) this.imageHistoryView = new ImageHistoryView();
            this.SuspendLayout();
            PreInit(resources);
            // 
            // matrixImageView
            // 
            if (this.matrixImageView != null)
            {
                this.matrixImageView.Name = "matrixImageView";
            }
            //
            // imageHistoryView
            //
            if (this.imageHistoryView != null)
            {
                this.imageHistoryView.Name = "imageHistoryView";
            }
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";

            Init(resources);

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        /// <summary>
        /// Панель историй
        /// </summary>
        private ImageHistoryView imageHistoryView;
        /// <summary>
        /// Окно просмотра изображений
        /// </summary>
        private MatrixImageView matrixImageView;
    }
}

