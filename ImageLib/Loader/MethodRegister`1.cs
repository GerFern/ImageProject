using System;
using System.Collections.Generic;

//using System.Runtime.Loader;
using System.Runtime.Serialization;

namespace ImageLib.Loader
{
    public class MethodRegister<T> : MethodRegister where T : BaseMethod
    {
        public MethodRegister(bool showEditParams, IEnumerable<char> name, IEnumerable<char>[] nameGroups)
            : base(typeof(T), showEditParams, name, nameGroups) { }
    }
}