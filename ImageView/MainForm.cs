using ImageLib.Controller;
using ImageLib.Loader;
using ImageView;
using OxyPlot.WindowsForms;
using Shared.WinFormsPlatform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared.WinFormsPlatform
{
    public partial class MainForm : Form
    {
        partial void PreInit(ComponentResourceManager resources)
        {
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            panel = new Panel();
            plotView = new PlotView();
            ((ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
        }

        partial void Init(ComponentResourceManager resources)
        {
            Text = "NIRS";
            // splitContainer1
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // splitContainer1.Panel1
            splitContainer1.Panel1.Controls.Add(matrixImageView);
            // splitContainer1.Panel2
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.SplitterDistance = 719;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 1;
            // splitContainer2
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel2;
            splitContainer2.Margin = new Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // splitContainer2.Panel1
            splitContainer2.Panel1.Controls.Add(imageHistoryView);
            // splitContainer2.Panel2
            splitContainer2.Panel2.Controls.Add(panel);
            splitContainer2.SplitterDistance = 274;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 1;
            // panel
            panel.Controls.Add(plotView);
            panel.BorderStyle = BorderStyle.Fixed3D;
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(4, 3, 4, 3);
            panel.Name = "panel";
            panel.TabIndex = 1;
            // plotView
            plotView.Dock = DockStyle.Fill;
            plotView.Name = "plotView";
            // matrixImageView
            matrixImageView.Dock = DockStyle.Fill;
            matrixImageView.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            // imageHistoryView
            imageHistoryView.Dock = DockStyle.Fill;
            // MainForm
            Controls.Add(splitContainer1);
            //splitContainer1.Panel2.ResumeLayout(false);
            //((ISupportInitialize)splitContainer1).EndInit();
            //splitContainer1.ResumeLayout(false);
            //splitContainer2.Panel2.ResumeLayout(false);
            //((ISupportInitialize)splitContainer2).EndInit();
            //splitContainer2.ResumeLayout(false);

            ((ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.Panel1.ResumeLayout();
            splitContainer1.Panel2.ResumeLayout();
            splitContainer1.ResumeLayout();
            ((ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.Panel1.ResumeLayout();
            splitContainer2.Panel2.ResumeLayout();
            splitContainer2.ResumeLayout();

            ResumeLayout(false);
            PerformLayout();
        }

        partial void AfterInit()
        {
            //imageHistoryView.Histories = MainController.CurrentController.HistoryController.Histories;
            MainController.CurrentControllerChanged += () =>
            {
                imageHistoryView.HistoryController = MainController.CurrentController.HistoryController;
                matrixImageView.Controller = MainController.CurrentController;
            };
            matrixImageView.BitmapHandlerChanged += (_, e) =>
            {
                plotView.Model = e.PlotModel;
            };
            //imageHistoryView.HistorySelected += (_, history) =>
            //{
            //    MainController.CurrentController.HistoryController.SetHead(history.index);
            //};
        }

        partial void BitmapHandlerChanged(BitmapHandler handler)
        {
            plotView.Model = handler.PlotModel;
        }

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private Panel panel;
        private PlotView plotView;
    }
}