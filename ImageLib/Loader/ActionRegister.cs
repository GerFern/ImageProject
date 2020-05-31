using System;
using System.Collections.Generic;

//using System.Runtime.Loader;

namespace ImageLib.Loader
{
    public class ActionRegister : ItemRegister
    {
        public Action Action { get; }

        public bool SaveHistory { get; }

        public ActionRegister(Action action, IEnumerable<char> name, IEnumerable<char>[] nameGroups) :
            base(name, nameGroups)
        {
            Action = action;
        }

        public ActionRegister(Action action, bool saveHistory, IEnumerable<char> name, IEnumerable<char>[] nameGroups) :
           this(action, name, nameGroups)
        {
            SaveHistory = saveHistory;
        }
    }
}