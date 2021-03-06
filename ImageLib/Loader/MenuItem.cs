﻿//using System;
//using System.Collections.Generic;
//using System.Runtime.Serialization;
//using System.Text;

//namespace ImageLib.Loader
//{
//    public class MenuRegister : ItemRegister
//    {
//        Dictionary<string, MenuRegister> autoGeneratedMenu;
//        List<MenuRegister> menuChilds;
//        List<ItemRegister> itemChilds;

//        internal MenuRegister GetMenuOrCreate(string[] directory, int index = 0)
//        {
//            if (directory.Length == index) return this;
//            string key = directory[index];
//            if (autoGeneratedMenu.TryGetValue(key, out MenuRegister menu)) return menu.GetMenu(directory, index + 1);
//            else
//            {
//                MenuRegister mr = new MenuRegister(key, this);
//                autoGeneratedMenu.Add(key, mr);
//                return mr.GetMenuOrCreate(directory, index + 1);
//            }
//        }

//        public MenuRegister GetMenu(string[] directory, int index = 0)
//        {
//            if (directory.Length == index) return this;
//            string key = directory[index];
//            if (autoGeneratedMenu.TryGetValue(key, out MenuRegister menu)) return menu.GetMenu(directory, index + 1);
//            else return null;
//        }

//        public void AddItem(ItemRegister item)
//        {
//            if(item is MenuRegister menuRegister)
//            {
//                menuRegister.menuChilds.Add(item);
//            }
//        }

//        public MenuRegister(IEnumerable<char> name, IEnumerable<char>[] nameGroups) : base(name, nameGroups)
//        {
//        }

//        public MenuRegister(IEnumerable<char> name, MenuRegister parent) : base(name, parent)
//        {
//        }
//    }
//}