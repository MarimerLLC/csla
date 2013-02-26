using System;
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using Rolodex.Silverlight.ViewModels;
using Rolodex.Silverlight.Views;

namespace Rolodex.Silverlight.Regions
{
    public static class RegionManagerExtensions
    {
        public static void ClearRegion(this IRegionManager regionManager, string regionName)
        {
            regionManager.Regions[regionName].Views.ToList()
                .ForEach(view => RemoveView(regionManager, view));
        }

        public static object GetView(this IRegionManager regionManager, string regionName, Type interfaceForView)
        {
            object view = null;
            regionManager.Regions[regionName].Views.ToList()
               .ForEach(aView =>
               {
                   if (interfaceForView.IsAssignableFrom(aView.GetType()))
                   {
                       view = aView;
                   }
               }
               );
            return view;
        }

        public static bool ActivateViewIfExists(this IRegionManager regionManager, string regionName, Type viewType)
        {
            bool returnValue = false;
            IRolodexView existingView = regionManager.GetView(regionName, viewType) as IRolodexView;
            if (existingView != null)
            {
                returnValue = true;
                regionManager.ActivateSingleView(regionName, existingView);
            }
            return returnValue;
        }

        private static void ActivateSingleView(this IRegionManager regionManager, string regionName, IRolodexView existingView)
        {
            regionManager.Regions[regionName].Activate(existingView);
            var viewModel = existingView.DataContext as IRolodexViewModel;
            if (viewModel != null)
            {
                viewModel.Activated();
            }
        }

        public static void DeactivateViews(this IRegionManager regionManager, string regionName)
        {
            var activeViews = regionManager.Regions[regionName].ActiveViews.ToList();
            if (activeViews.Count > 0)
            {
                activeViews.ForEach(one => regionManager.Regions[regionName].Deactivate(one));
            }
        }

        public static void ActivateLastView(this IRegionManager regionManager, string regionName)
        {
            var activeViews = regionManager.Regions[regionName].ActiveViews.ToList();
            if (activeViews.Count > 0)
            {
                IRolodexView existingView = activeViews.Last() as IRolodexView;
                if (existingView != null)
                {
                    regionManager.ActivateSingleView(regionName, existingView);
                }
            }
            else
            {
                activeViews = regionManager.Regions[regionName].Views.ToList();
                if (activeViews.Count > 0)
                {
                    IRolodexView existingView = activeViews.Last() as IRolodexView;
                    if (existingView != null)
                    {
                        regionManager.ActivateSingleView(regionName, existingView);
                    }
                }
            }
        }

        public static void AddViewToRegion(this IRegionManager regionManager, string regionName, IRolodexView view)
        {
            regionManager.Regions[regionName].Add(view);
        }

        public static void RemoveView(this IRegionManager regionManager, object view)
        {
            foreach (var region in regionManager.Regions)
            {
                if (region.Views.Contains(view))
                {
                    region.Remove(view);
                }
            }
        }
    }
}
