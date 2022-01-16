using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace YouiToolkit.Design
{
    public class LeftContextMenuHelper : DependencyObject
    {
        /// <summary>
        /// 注册改为左键菜单
        /// </summary>
        /// <param name="frameworkElement">源控件</param>
        /// <param name="placementOffset">菜单显示偏移</param>
        /// <param name="placement">菜单显示模式</param>
        public static void Register(FrameworkElement frameworkElement, Point? placementOffset = null, PlacementMode placement = PlacementMode.Bottom)
        {
            if (frameworkElement?.ContextMenu == null)
            {
                return;
            }
            frameworkElement.PreviewMouseRightButtonUp += (s, e) => e.Handled = true;
            frameworkElement.MouseRightButtonUp += (s, e) => e.Handled = true;
            frameworkElement.PreviewMouseLeftButtonDown += (s, e) =>
            {
                ContextMenu contextMenu = frameworkElement.ContextMenu;

                if (contextMenu != null)
                {
                    if (contextMenu.PlacementTarget != frameworkElement)
                    {
                        contextMenu.PlacementTarget = frameworkElement;
                        contextMenu.PlacementRectangle = new Rect((Point)(placementOffset ?? new Point()), new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
                        contextMenu.Placement = placement;
                        contextMenu.StaysOpen = false;
                    }
                    contextMenu.IsOpen = !contextMenu.IsOpen;
                }
            };
        }
    }
}
