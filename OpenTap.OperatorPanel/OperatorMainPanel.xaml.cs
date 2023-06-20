using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Keysight.OpenTap.Wpf;
using OpenTap;

namespace PluginDevelopment.Gui.OperatorPanel
{
    public partial class OperatorMainPanel : UserControl
    {
        readonly ITapDockContext tapDockContext;
        public OperatorMainPanelViewModel ViewModel { get; set; } = new OperatorMainPanelViewModel();
        public OperatorMainPanel(ITapDockContext tapDockContext)
        {
            this.tapDockContext = tapDockContext;
            InitializeComponent();
            baseGrid.DataContext = ViewModel;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            
        }
        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            
            OperatorPanelSettings.Current.CacheInvalidated -= CurrentOnCacheInvalidated;
            OperatorPanelSettings.Current.PropertyChanged -= CurrentOnPropertyChanged;   
        }
        
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            OperatorPanelSettings.Current.CacheInvalidated += CurrentOnCacheInvalidated;
            OperatorPanelSettings.Current.PropertyChanged += CurrentOnPropertyChanged;   
        }
        
        void CurrentOnCacheInvalidated(object sender, EventArgs e)
        {
            (sender as ComponentSettings).CacheInvalidated -= CurrentOnCacheInvalidated;
            (sender as ComponentSettings).PropertyChanged -= CurrentOnPropertyChanged;

            OperatorPanelSettings.Current.PropertyChanged += CurrentOnPropertyChanged;
            OperatorPanelSettings.Current.CacheInvalidated += CurrentOnCacheInvalidated;
        }
        void CurrentOnPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            ViewModel.OnPropertyChanged("");
        }

        readonly Dictionary<OperatorPanelSetting, OperatorPanelViewModel> viewModels =
            new Dictionary<OperatorPanelSetting, OperatorPanelViewModel>();
        void PanelContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var decorator = (Decorator)sender;
            var viewModel = decorator.DataContext as OperatorPanelSetting;
            if (viewModel == null) return;
            if (!viewModels.TryGetValue(viewModel, out var model2))
            {
                model2 = new OperatorPanelViewModel();
                viewModels[viewModel] = model2;
            }

            var panel = new SessionView(tapDockContext, viewModel, model2);
            decorator.Child = panel;
        }
    }
}