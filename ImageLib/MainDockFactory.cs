using System;
using System.Collections.Generic;
using Avalonia.Data;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using ImageLib.Model;

namespace ImageLib.Dock
{
    public class MainDockFactory : Factory
    {
        private object _context;
        public ProportionalDock MainLayout { get; private set; }

        public MainDockFactory(object context)
        {
            _context = context;
        }

        public override void PinDockable(IDockable dockable)
        {
            // Nothing
            //base.PinDockable(dockable);
        }

        public override void CloseDockable(IDockable dockable)
        {
            if (dockable != null)
                base.CloseDockable(dockable);
        }

        public override void RemoveDockable(IDockable dockable, bool collapse)
        {
            //if (!(dockable is MainDocumentDock))
            if (dockable is DocumentDock documentDock)
            {
                // Документ находится в окне или на форме больше 1 форм с документами
                if (documentDock.Owner is IRootDock || DocDockCount(MainLayout) > 1) base.RemoveDockable(dockable, collapse);
            }
            else base.RemoveDockable(dockable, collapse);
        }

        public int DocDockCount(IDock dock)
        {
            if (dock == null) return 0;
            if (dock.VisibleDockables != null)
            {
                int counter = 0;
                foreach (var item in dock.VisibleDockables)
                {
                    if (item is DocumentDock) counter++;
                    counter += DocDockCount(item as IDock);
                }
                return counter;
            }
            return 0;
        }

        public override void SplitToWindow(IDock dock, IDockable dockable, double x, double y, double width, double height)
        {
            base.SplitToWindow(dock, dockable, x, y, width, height);
        }

        public override void SplitToDock(IDock dock, IDockable dockable, DockOperation operation)
        {
            base.SplitToDock(dock, dockable, operation);
        }

        public override void MoveDockable(IDock sourceDock, IDock targetDock, IDockable sourceDockable, IDockable targetDockable)
        {
            base.MoveDockable(sourceDock, targetDock, sourceDockable, targetDockable);
            FixSplitter(MainLayout);
        }

        public override void MoveDockable(IDock dock, IDockable sourceDockable, IDockable targetDockable)
        {
            base.MoveDockable(dock, sourceDockable, targetDockable);
            FixSplitter(MainLayout);
        }

        public void FixSplitter(IDock dock)
        {
            if (dock != null)
            {
                if (dock is ProportionalDock pd)
                {
                    if (pd.VisibleDockables != null)
                    {
                        int index = 0;
                        bool needSpliter = false;
                        while (index < pd.VisibleDockables.Count)
                        {
                            if (!needSpliter)
                            {
                                if (pd.VisibleDockables[index] is SplitterDock splitterDock)
                                {
                                    RemoveDockable(splitterDock, false);
                                }
                                else
                                {
                                    needSpliter = true;
                                    index++;
                                }
                            }
                            else
                            {
                                if (!(pd.VisibleDockables[index] is SplitterDock))
                                {
                                    InsertDockable(pd, CreateSplitterDock(), index);
                                }
                                needSpliter = false;
                                index++;
                            }
                        }
                    }
                }
                if (dock.VisibleDockables != null)
                    foreach (var item in dock.VisibleDockables)
                    {
                        FixSplitter(item as IDock);
                    }
            }
        }

        public override void AddWindow(IRootDock rootDock, IDockWindow window)
        {
            base.AddWindow(rootDock, window);
        }

        public override void InsertDockable(IDock dock, IDockable dockable, int index)
        {
            base.InsertDockable(dock, dockable, index);
        }

        public override void RemoveWindow(IDockWindow window)
        {
            base.RemoveWindow(window);
        }

        public override IDock CreateSplitLayout(IDock dock, IDockable dockable, DockOperation operation)
        {
            return base.CreateSplitLayout(dock, dockable, operation);
        }

        public override IDock CreateLayout()
        {
            var mainLayout = MainLayout = new ProportionalDock
            {
                Id = "MainLayout",
                Title = "MainLayout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new DocumentDock
                    {
                        Id = "DocumentsPane",
                        Title = "DocumentsPane",
                        Proportion = double.NaN,
                        VisibleDockables = CreateList<IDockable>()
                    },
                    new ToolDock
                    {
                        Id = "RightPaneTop",
                        Title = "RightPaneTop",
                        Proportion = double.NaN,
                        VisibleDockables = CreateList<IDockable>()
                    }
                )
            };

            var mainView = new MainViewModel
            {
                Id = "Main",
                Title = "Main",
                ActiveDockable = mainLayout,
                VisibleDockables = CreateList<IDockable>(mainLayout)
            };

            var root = CreateRootDock();

            root.Id = "Root";
            root.Title = "Root";
            root.ActiveDockable = mainView;
            root.DefaultDockable = mainView;
            root.VisibleDockables = CreateList<IDockable>(mainView);
            FixSplitter(MainLayout);
            return root;
        }

        public override IPinDock CreatePinDock()
        {
            return null;
        }

        public override object GetContext(string id)
        {
            return base.GetContext(id);
        }

        public override void UpdateDockable(IDockable dockable, IDockable owner)
        {
            if (dockable is IDockModel dockModel)
            {
                dockable.Context = dockModel.ModelData;
                dockable.Owner = owner;
                if (dockable is IDock dock)
                {
                    dock.Factory = this;

                    if (!(dock.VisibleDockables is null))
                    {
                        foreach (var child in dock.VisibleDockables)
                        {
                            UpdateDockable(child, dockable);
                        }
                    }
                }
            }
            else base.UpdateDockable(dockable, owner);
        }

        public override void InitLayout(IDockable layout)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(IRootDock)] = () => _context,
                [nameof(IPinDock)] = () => _context,
                [nameof(IProportionalDock)] = () => _context,
                [nameof(IDocumentDock)] = () => _context,
                [nameof(IToolDock)] = () => _context,
                [nameof(ISplitterDock)] = () => _context,
                [nameof(IDockWindow)] = () => _context,
                [nameof(IDocument)] = () => _context,
                [nameof(ITool)] = () => _context,
                ["MainLayout"] = () => _context,
                ["Main"] = () => _context,
            };

            this.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () =>
                {
                    var hostWindow = new HostWindow()
                    {
                        [!HostWindow.TitleProperty] = new Binding("ActiveDockable.Title")
                    };
                    return hostWindow;
                }
            };

            this.DockableLocator = new Dictionary<string, Func<IDockable>>
            {
            };

            base.InitLayout(layout);
        }

        public void OpenDocument(Document document)
        {
        }

        public void OpenTool(Tool tool)
        {
        }

        public IDocumentDock FindOrCreateDocumentDock()
        {
            var docDock = FindDocumentDock(MainLayout);
            if (docDock == null)
            {
                docDock = CreateDocumentDock();
                InsertDockable(MainLayout, docDock, MainLayout.VisibleDockables.Count / 2);
                FixSplitter(MainLayout);
            }
            return docDock;
        }

        public IDocumentDock FindDocumentDock(IDock dock)
        {
            if (dock == null || dock.VisibleDockables == null) return null;
            foreach (var item in dock.VisibleDockables)
            {
                if (item is IDocumentDock docDock) return docDock;
                docDock = FindDocumentDock(item as IDock);
                if (docDock != null) return docDock;
            }
            return null;
        }

        public IToolDock FindOrCreateToolDock()
        {
            var toolDock = FindToolDock(MainLayout);
            if (toolDock == null)
            {
                toolDock = CreateToolDock();
                InsertDockable(MainLayout, toolDock, MainLayout.VisibleDockables.Count);
                FixSplitter(MainLayout);
            }
            return toolDock;
        }

        public IToolDock FindToolDock(IDock dock)
        {
            if (dock == null || dock.VisibleDockables == null) return null;
            foreach (var item in dock.VisibleDockables)
            {
                if (item is IToolDock toolDock) return toolDock;
                toolDock = FindToolDock(item as IDock);
                if (toolDock != null) return toolDock;
            }
            return null;
        }
    }
}