﻿using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using OpenCvSharp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Model
{
    [View(typeof(PanAndZoomBuildView))]
    public class PanAndZoomModel : ReactiveObject, IModelDataContainer
    {
        [Reactive] public object ModelData { get; set; }

        //[Bind(nameof(ZoomBorder.MaxOffsetX))]
        //[Reactive] public double MaxOffsetX { get; set; } = double.PositiveInfinity;

        //[Bind(nameof(ZoomBorder.MinOffsetX))]
        //[Reactive]
        //public double MinOffsetX { get; set; } = double.NegativeInfinity;

        //[Bind(nameof(ZoomBorder.MaxOffsetY))]
        //[Reactive]
        //public double MaxOffsetY { get; set; } = double.PositiveInfinity;

        //[Bind(nameof(ZoomBorder.MinOffsetY))]
        //[Reactive]
        //public double MinOffsetY { get; set; } = double.NegativeInfinity;

        //[Bind(nameof(ZoomBorder.MaxZoomX))]
        //[Reactive]
        //public double MaxZoomX { get; set; } = 1000;

        //[Bind(nameof(ZoomBorder.MinZoomX))]
        //[Reactive]
        //public double MinZoomX { get; set; } = 0.001;

        //[Bind(nameof(ZoomBorder.MaxZoomY))]
        //[Reactive]
        //public double MaxZoomY { get; set; } = 1000;

        //[Bind(nameof(ZoomBorder.MinZoomY))]
        //[Reactive]
        //public double MinZoomY { get; set; } = 0.001;

        //[Bind(nameof(ZoomBorder.ZoomSpeed))]
        //[Reactive]
        //public double ZoomSpeed { get; set; } = 1;

        //[Bind(nameof(ZoomBorder.EnableGestureZoom))]
        //[Reactive]
        //public bool EnableGestureZoom { get; set; } = true;

        //[Bind(nameof(ZoomBorder.EnableGestureTranslation))]
        //[Reactive]
        //public bool EnableGestureTranslate { get; set; } = true;

        //[Bind(nameof(ZoomBorder.EnableGestureRotation))]
        //[Reactive]
        //public bool EnableGestureRotation { get; set; } = true;
    }

    public class PanAndZoomBuildView : CustomBuildView
    {
        public override Control Build(object data)
        {
            var model = (PanAndZoomModel)data;
            Control control = (Control)ViewLocator.Instance.Build(model.ModelData);
            control.DataContext = model.ModelData;

            ZoomBorder zoomBorder = new ZoomBorder
            {
                Child = control,
                EnableGestureZoom = false,
                ZoomSpeed = 2,
                ClipToBounds = true,
                Focusable = true
            };

            model.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IModelDataContainer.ModelData))
                {
                    zoomBorder.Child = (Control)ViewLocator.Instance.Build(model.ModelData);
                    zoomBorder.Child.DataContext = model.ModelData;
                }
            };

            return zoomBorder;
        }
    }

    public interface IModelDataContainer
    {
        public object ModelData { get; set; }
        //public Dictionary<string, object> Tags { get; set; }
    }

    //public interface IHistoryModelDataContainer : IModelDataContainer
    //{
    //    //public AvaloniaList<HistoryItem>
    //}

    public class HistoryItem
    {
        public string Message { get; }
        public object Data { get; }

        public HistoryItem(string message, object data)
        {
            Message = message;
            Data = data;
        }
    }
}