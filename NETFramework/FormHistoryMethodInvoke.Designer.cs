namespace NETFramework
{
    partial class FormHistoryMethodInvoke
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
            this.imageHistoryView1 = new NETFramework.ImageHistoryView();
            this.imageHistoryView2 = new NETFramework.ImageHistoryView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // imageHistoryView1
            // 
            this.imageHistoryView1.Location = new System.Drawing.Point(12, 12);
            this.imageHistoryView1.Name = "imageHistoryView1";
            this.imageHistoryView1.Size = new System.Drawing.Size(190, 278);
            this.imageHistoryView1.TabIndex = 0;
            // 
            // imageHistoryView2
            // 
            this.imageHistoryView2.Location = new System.Drawing.Point(213, 12);
            this.imageHistoryView2.Name = "imageHistoryView2";
            this.imageHistoryView2.Size = new System.Drawing.Size(190, 278);
            this.imageHistoryView2.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(247, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Выполнить";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(328, 296);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // FormHistoryMethodInvoke
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(415, 327);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.imageHistoryView2);
            this.Controls.Add(this.imageHistoryView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormHistoryMethodInvoke";
            this.Text = "FormHistoryMethodInvoke";
            this.ResumeLayout(false);

        }

        #endregion

        private ImageHistoryView imageHistoryView1;
        private ImageHistoryView imageHistoryView2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}