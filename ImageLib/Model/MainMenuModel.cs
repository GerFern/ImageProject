using Avalonia.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Model
{
    public class MainMenuModel : AvaloniaList<MenuModel>
    {
        public static MainMenuModel Instance { get; private set; }

        public MainMenuModel()
        {
            if (Instance == null) Instance = this;
        }
    }
}