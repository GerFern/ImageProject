namespace Shared.WinFormsPlatform
{
    partial class MatrixImageView
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            components = new System.ComponentModel.Container();
            vScrollBar = new System.Windows.Forms.VScrollBar();
            hScrollBar = new System.Windows.Forms.HScrollBar();
            panel = new System.Windows.Forms.Panel();

            vScrollBar.Visible = false;
            hScrollBar.Visible = false;
            panel.Visible = false;

            vScrollBar.Anchor =
                System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Right
                | System.Windows.Forms.AnchorStyles.Bottom;

            hScrollBar.Anchor =
                System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right
                | System.Windows.Forms.AnchorStyles.Bottom;

            this.Controls.Add(vScrollBar);
            this.Controls.Add(hScrollBar);
            this.Controls.Add(panel);

            vScrollBar.Top = 0;
            vScrollBar.Left = this.Width - vScrollBar.Width;
            vScrollBar.Height = this.Height;

            hScrollBar.Top = this.Height - hScrollBar.Height;
            hScrollBar.Left = 0;
            hScrollBar.Width = this.Width;

            panel.Size = new System.Drawing.Size(vScrollBar.Width, hScrollBar.Height);
            panel.Anchor = 
                System.Windows.Forms.AnchorStyles.Bottom 
                | System.Windows.Forms.AnchorStyles.Right;

        }

        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.Panel panel;
        #endregion
    }
}
