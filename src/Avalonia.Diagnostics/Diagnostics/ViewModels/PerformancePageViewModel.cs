using System;
using System.Collections.Generic;
using Avalonia.Layout;

#nullable enable

namespace Avalonia.Diagnostics.ViewModels
{
    internal class PerformancePageViewModel : ViewModelBase
    {
        public PerformancePageViewModel(MainViewModel mainView)
        {
            MainView = mainView;
        }

        public void Refresh()
        {
            var lm = (LayoutManager)MainView.Root.LayoutManager;

            LayoutHelper.InvalidateSelfAndChildrenMeasure(MainView.Root);

            lm.BeginProfileLayoutPass();
            lm.ExecuteLayoutPass();
            Timings = lm.EndProfileLayoutPass();

            RaisePropertyChanged(nameof(Timings));
        }

        public IReadOnlyDictionary<ILayoutable, TimeSpan>? Timings { get; private set; }

        public MainViewModel MainView { get; }
    }
}
