using ImageLib.Controller;
using ImageLib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

//using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageLib.Loader
{
    //[Obsolete]
    //public class LoaderContext : AssemblyLoadContext
    //{
    //    public InitLoaderAttribute Attribute { get; set; }
    //    protected override Assembly Load(AssemblyName assemblyName)
    //    {
    //        return base.Load(assemblyName);
    //    }
    //}

    /// <summary>
    /// Загрузчик библиотек
    /// </summary>
    public static class LibLoader
    {
        public static string LibPath { get; set; }

        static private Dictionary<Assembly, LibInfo> libs;
        static private Dictionary<AppDomain, LibInfo> domainLibs;
        static public ReadOnlyDictionary<AppDomain, LibInfo> DomainLibs { get; }
        static public ReadOnlyDictionary<Assembly, LibInfo> Libs { get; }
        static public MenuModelOld RootMethodItem { get; }

        static LibLoader()
        {
            domainLibs = new Dictionary<AppDomain, LibInfo>();
            DomainLibs = new ReadOnlyDictionary<AppDomain, LibInfo>(domainLibs);
            libs = new Dictionary<Assembly, LibInfo>();
            Libs = new ReadOnlyDictionary<Assembly, LibInfo>(libs);
            RootMethodItem = new MenuModelOld() { Name = "root" };
            AppDomain.CurrentDomain.AssemblyLoad += (_, e) =>
            {
                Load(e.LoadedAssembly);
            };
        }

        public static PlatformRegister Platform { get; private set; }

        public static void PlatformInit(PlatformRegister platformRegister)
        {
            if (Platform == null)
            {
                Platform = platformRegister;
                Platform._mainWindowInstance = Platform.CreateMainWindow();
                RootMethodItem.MenuElement = Platform.CreateMainMenuItem(RootMethodItem);
            }
            else throw new Exception("Платформа уже зарегистрированна. Нельзя зарегистрировать больше одной платформы");
        }

        public static void LoadDir(string directory)
        {
            foreach (var item in GetDllFiless(directory))
            //new EnumerationOptions { RecurseSubdirectories = true }))
            {
                Load(item);
            }
        }

        public static IEnumerable<string> GetDllFiless(string directory)
        {
            return Directory.EnumerateFileSystemEntries(directory, "*.dll");
        }

        public static void Load(string libName)
        {
            //AppDomain appDomain = AppDomain.CreateDomain("LibDomain_" + Path.GetFileName(libName));
            //AssemblyName assemblyName = new AssemblyName(libName);
            //var core = typeof(object).Assembly;
            //var corepath = core.CodeBase;

            //PathAssemblyResolver resolver = new PathAssemblyResolver(new string[] { Path.GetDirectoryName(corepath), $"{Environment.CurrentDirectory}" });

            ////MetadataAssemblyResolver resolver =
            //MetadataLoadContext context = new MetadataLoadContext(resolver, "mscorlib");
            //Assembly asm = context.LoadFromAssemblyPath(libName);
            //var list = asm.CustomAttributes;

            bool haveMyAttribute = false;
            using (FileStream fileStream = new FileStream(libName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PEReader peReader = new PEReader(fileStream, PEStreamOptions.LeaveOpen);
                MetadataReader reader = peReader.GetMetadataReader();
                CustomAttributeHandleCollection customAttributes = reader.CustomAttributes;
                foreach (CustomAttributeHandle item in customAttributes)
                {
                    try
                    {
                        CustomAttribute attribute = reader.GetCustomAttribute(item);
                        MemberReference ctor = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
                        TypeReference attrType = reader.GetTypeReference((TypeReferenceHandle)ctor.Parent);
                        string nsName = reader.GetString(attrType.Namespace);
                        string typeName = reader.GetString(attrType.Name);
                        Debug.WriteLine($"{nsName}.{typeName}");
                        if (nsName == "ImageLib.Loader" && typeName == "InitLoaderAttribute")
                        {
                            haveMyAttribute = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                peReader.Dispose();

                //var asm = Assembly.ReflectionOnlyLoadFrom(libName);
                //if (asm.CustomAttributes.Any(a => a.AttributeType == typeof(InitLoaderAttribute)))
                //{
                //    Assembly assembly = Assembly.LoadFrom(libName);
                //    Load(assembly);
                //}

                if (haveMyAttribute)
                {
                    var context = new AssemblyLoadContext(null, true);
                    fileStream.Position = 0;
                    var asm = context.LoadFromStream(fileStream);
                    Load(asm, null);
                    context.Unloading += a =>
                    {
                    };
                    context.Unload();
                }
            }
        }

        public static void Load(Assembly assembly, AppDomain appDomain = null)
        {
            if (libs.ContainsKey(assembly)) return;
            var attrData = assembly.CustomAttributes
               .FirstOrDefault(a => a.AttributeType == typeof(InitLoaderAttribute));
            if (attrData != null)
            {
                var p1 = attrData.ConstructorArguments[0];
                var initLoaderAtt = (InitLoaderAttribute)Activator.CreateInstance(
                    typeof(InitLoaderAttribute),
                    p1.Value);

                object t;
                if (appDomain is null)
                {
                    t = Activator.CreateInstance(initLoaderAtt.InitType);
                }
                else
                {
                    throw new NotImplementedException();
                    //t = appDomain.CreateInstanceAndUnwrap(
                    //    assembly.FullName,
                    //    initLoaderAtt.InitType.FullName);
                }

                if (t is LibInitializer init)
                {
                    LibInfo libInfo = new LibInfo(appDomain, assembly, init);
                    if (!(appDomain is null))
                        domainLibs.Add(appDomain, libInfo);
                    libs.Add(assembly, libInfo);
                    libInfo.Registers.ItemRegistered += Registers_MethodRegistered;
                    PreLoad?.Invoke(libInfo);
                    if (init is PlatformInitializer platformInitializer)
                    {
                        var platform = (PlatformRegister)Activator.CreateInstance(platformInitializer.PlatformType);
                        platformInitializer.Platform = platform;
                        libInfo.Registers.RegisterPlatform(platform);
                    }
                    init.Initialize(libInfo.Registers);
                    OnLoad?.Invoke(libInfo);
                }
            }
        }

        private static void Registers_MethodRegistered(ItemRegister obj)
        {
            RootMethodItem.AddRegister(obj);
        }

        public static event Action<LibInfo> PreLoad;

        public static event Action<LibInfo> OnLoad;

        public static event Action<LibInfo> Unload;

        //public static void Load(string path)
        //{
        //    LoaderContext lc = new LoaderContext();
        //    lc.Resolving += (a, b) =>
        //    {
        //        return null;
        //    };
        //    var assembly = lc.LoadFromAssemblyPath(path);
        //    if (assembly == null) return;
        //    var attrData = assembly.CustomAttributes
        //        .Where(a => a.AttributeType == typeof(InitLoaderAttribute))
        //        .FirstOrDefault();
        //    if (attrData != null)
        //    {
        //        var p1 = attrData.ConstructorArguments[0];
        //        var initLoaderAtt = (InitLoaderAttribute)Activator.CreateInstance(
        //            typeof(InitLoaderAttribute),
        //            p1.Value);
        //        lc.Attribute = initLoaderAtt;
        //    }
        //    else
        //    {
        //        lc.Unload();
        //    }
        //}
    }

    /// <summary>
    /// Информация о библиотеке
    /// </summary>
    public class LibInfo
    {
        public Assembly Assembly { get; }
        public Registers Registers { get; }
        public AppDomain Domain { get; }

        public LibInfo(AppDomain domain, Assembly assembly, LibInitializer initializer)
        {
            Domain = domain;
            Assembly = assembly;
            Registers = new Registers(MainMenuModel.Instance);
        }
    }

    /// <summary>
    /// Инициализатор библиотеки
    /// </summary>
    //[Serializable]
    public abstract class LibInitializer
    {
        public abstract void Initialize(Registers registers);
    }

    public abstract class PlatformInitializer : LibInitializer
    {
        public PlatformRegister Platform { get; internal set; }
        public abstract Type PlatformType { get; }
    }

    /// <summary>
    /// Регистрация объектов
    /// </summary>
    //[Serializable]
    public class Registers
    {
        public MainMenuModel MainMenu { get; }

        public Registers(MainMenuModel menuModel) : this()
        {
            MainMenu = menuModel;
        }

        public void AddMenu(MenuModel menuModel, MenuModel parent = null)
        {
            if (parent == null)
                MainMenu.Add(menuModel);
            else
            {
                parent.Items.Add(menuModel);
                menuModel.Parent = parent;
            }
        }

        public void AddMenu(MenuModel menuModel, string[] directory)
        {
            if (directory.Length == 0)
                AddMenu(menuModel);
            else AddMenu(menuModel, Find(directory));
        }

        public Registers WithMenuModel(MenuModel menuModel, MenuModel parent = null)
        {
            AddMenu(menuModel, parent);
            return this;
        }

        public Registers WithMenuModel(MenuModel menuModel, string[] directory)
        {
            AddMenu(menuModel, directory);
            return this;
        }

        public MenuModel Find(params string[] directory)
        {
            MenuModel current = MainMenu.Where(a => a.Header == directory[0]).FirstOrDefault();
            foreach (var item in directory.Skip(1))
            {
                var tmp = current.Items.Where(a => a.Header == item).FirstOrDefault();
                if (tmp == null)
                {
                    tmp = new MenuModel()
                    {
                        Header = item,
                        Parent = current
                    };
                    current.Items.Add(tmp);
                }
                current = tmp;
            }
            return current;
        }

        private List<ActionRegister> actionRegisters = new List<ActionRegister>();
        private List<ItemRegister> methodRegisters = new List<ItemRegister>();
        private List<LocaleString> localeRegisters = new List<LocaleString>();

        //List<IImageTypeInfo> imageTypeInfos = new List<IImageTypeInfo>();
        //Dictionary<Type, IImageTypeInfo> imageTypeInfosFromTImage = new Dictionary<Type, IImageTypeInfo>();
        //Dictionary<Type, IImageTypeInfo> imageTypeInfosFromTLayer = new Dictionary<Type, IImageTypeInfo>();
        public IReadOnlyCollection<ActionRegister> ActionRegisters { get; }

        public IReadOnlyCollection<ItemRegister> MethodRegisters { get; }
        public IReadOnlyCollection<LocaleString> LocaleRegisters { get; }

        public PlatformRegister Platform { get; private set; }

        //public IReadOnlyCollection<IImageTypeInfo> FormateRegisters { get; }
        //
        //#region FormatReg
        //public Registers RegisterFormat(IImageTypeInfo formatRegister)
        //{
        //    imageTypeInfos.Add(formatRegister);
        //    imageTypeInfosFromTImage.Add(formatRegister.ImageType, formatRegister);
        //    imageTypeInfosFromTLayer.Add(formatRegister.LayerType, formatRegister);
        //    FormateRegistered?.Invoke(formatRegister);
        //    return this;
        //}
        //#endregion FormatReg

        public Registers RegisterPlatform(PlatformRegister platformRegister)
        {
            if (PlatformRegister._instance == null)
            {
                Platform = platformRegister;
                PlatformRegister._instance = platformRegister;
                platformRegister._mainWindowInstance = platformRegister.CreateMainWindow();
                LibLoader.RootMethodItem.MenuElement = platformRegister._mainMenuElement
                    = platformRegister.CreateMainMenuItem(LibLoader.RootMethodItem);
                return this;
            }
            else throw new Exception();
        }

        public Registers RegisterAction(ActionRegister actionRegister)
        {
            actionRegisters.Add(actionRegister);
            ItemRegistered?.Invoke(actionRegister);

            return this;
        }

        public Registers RegisterAction(Action action, IEnumerable<char> name, params IEnumerable<char>[] nameGroups)
        {
            return RegisterAction(new ActionRegister(action, name, nameGroups));
        }

        #region MethodReg

        public Registers RegisterMethod(ItemRegister methodRegister)
        {
            methodRegisters.Add(methodRegister);
            //new Thread(() =>
            ItemRegistered?.Invoke(methodRegister);
            //).Start();
            return this;
        }

        public Registers RegisterMethod<T>(bool showEditParams, IEnumerable<char> name, params IEnumerable<char>[] nameGroups) where T : BaseMethod
            => RegisterMethod(new MethodRegister<T>(showEditParams, name, nameGroups));

        public Registers RegisterMethods(IEnumerable<ItemRegister> methodRegisters)
        {
            foreach (var methodRegister in methodRegisters)
                RegisterMethod(methodRegister);
            return this;
        }

        #endregion MethodReg

        #region LocaleReg

        public Registers RegisterLocale(string key, string @default) =>
            RegisterLocale(new LocaleString(key, @default));

        public Registers RegisterLocale(LocaleString locale)
        {
            localeRegisters.Add(locale);
            LocaleRegistered?.Invoke(locale);
            return this;
        }

        public Registers RegisterLocales(IEnumerable<LocaleString> locales)
        {
            foreach (var item in locales)
            {
                localeRegisters.Add(item);
            }
            return this;
        }

        #endregion LocaleReg

        private Registers()
        {
            MethodRegisters = new ReadOnlyCollection<ItemRegister>(methodRegisters);
            LocaleRegisters = new ReadOnlyCollection<LocaleString>(localeRegisters);
            //FormateRegisters = new ReadOnlyCollection<IImageTypeInfo>(imageTypeInfos);
        }

        public event Action<ItemRegister> ItemRegistered;

        public event Action<LocaleString> LocaleRegistered;

        //[Obsolete] public event Action<IImageTypeInfo> FormateRegistered;
    }

    /// <summary>
    /// Метод работает с OpenCV изображением (<see cref="OpenCvSharp.Mat"/>)
    /// </summary>
    [Obsolete("Не реализовано")]
    public interface ICVMethod
    {
        OpenCvSharp.Mat Invoke(OpenCvSharp.Mat mat);
    }

    /// <summary>
    /// Метод не реализован
    /// </summary>
    public interface INotImplemented
    {
    }

    [Obsolete("Нереализован")]
    public interface ICustomInputFrame
    {
        //void FrameInputInit(PlatformRegister platform, CloseEvent close, EventAction ok, EventAction cancel) => throw new NotImplementedException();
    }

    //public class BindableInfo
    //{
    //    public string PropertyName { get; set; }
    //    public string DisplayName { get; set; }
    //    public bool ReadOnly { get; set; }
    //    public FrameContext Context { get; set; }
    //}

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class InitLoaderAttribute : Attribute
    {
        public Type InitType { get; }

        public InitLoaderAttribute(Type initType)
        {
            InitType = initType;
        }
    }

    public sealed class IgnoreAutoPropSerializableAttribute : Attribute { }
}