using System;
using System.Collections.Generic;
using System.Linq;

//using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Text;

namespace ImageLib.Loader
{
    public class MethodRegister : ItemRegister
    {
        /// <summary>
        /// Класс, производный от <see cref="ImageMethod"/>
        /// </summary>
        public virtual Type Type { get; }

        /// <summary>
        /// Показывать редактор параметров перед вызовом метода
        /// </summary>
        public bool ShowEditParams { get; }

        public MethodRegister(Type type, bool showEditParams, IEnumerable<char> name, IEnumerable<char>[] nameGroups) :
            base(name, nameGroups)
        {
            Type = type;
            ShowEditParams = showEditParams;
        }

        //protected MethodRegister(SerializationInfo serializationInfo, StreamingContext streamingContext)
        //    : base(serializationInfo, streamingContext)
        //{
        //    Type = Type.GetType(serializationInfo.GetString(nameof(Type)));
        //}

        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //    info.AddValue(nameof(Type), Type.FullName);
        //}
    }
}