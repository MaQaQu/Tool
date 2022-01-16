using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public static class ElementHelper
    {
        public static Window FindWindow(this DependencyObject d) => Window.GetWindow(d);
        public static TWindow FindTWindow<TWindow>(this DependencyObject d) where TWindow : Window
            => Window.GetWindow(d) as TWindow;

#if false
        [Obsolete]
        public static Window FindWindow(this FrameworkElement element)
        {
            FrameworkElement? parent = element.Parent as FrameworkElement;
            while (parent != null)
            {
                if (parent?.Parent as FrameworkElement != null)
                {
                    parent = parent?.Parent as FrameworkElement;
                }
                else
                {
                    break;
                }
            }
            return (parent as Window)!;
        }
#endif

        public static FrameworkElement GetTemplateChild(this FrameworkElement d, string childName)
        {
            MethodInfo getTemplateChild = typeof(FrameworkElement).GetMethod("GetTemplateChild", BindingFlags.NonPublic | BindingFlags.Instance);

            return getTemplateChild?.Invoke(d, new object[] { childName }) as FrameworkElement;
        }

        public static FrameworkElement GetVisualChild(this FrameworkElement d, int index)
        {
            MethodInfo getVisualChild = typeof(FrameworkElement).GetMethod("GetVisualChild", BindingFlags.NonPublic | BindingFlags.Instance);

            return getVisualChild?.Invoke(d, new object[] { index }) as FrameworkElement;
        }

        public static T Where<T>(this UIElementCollection elements, Func<T, bool> condition) where T : UIElement
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            foreach (T element in elements)
            {
                if (condition(element))
                {
                    return element;
                }
            }
            return null;
        }

        public static void Foreach<T>(this UIElementCollection elements, Action<T> action) where T : UIElement
        {
            _ = action ?? throw new ArgumentNullException(nameof(action));

            foreach (T element in elements)
            {
                action(element);
            }
        }

        public static bool Find<T>(this UIElementCollection elements, Func<T, bool> condition, out T element) where T : UIElement
        {
            element = elements.Where(condition);
            return element != null;
        }

        public static T GetChildrenByType<T>(this FrameworkElement d) where T : FrameworkElement
        {
            int count = VisualTreeHelper.GetChildrenCount(d);

            for (int i = default; i < count; i++)
            {
                FrameworkElement c = VisualTreeHelper.GetChild(d, i) as FrameworkElement;

                if (c == null)
                {
                    continue;
                }
                if (c.GetType() == typeof(T))
                {
                    return (T)c;
                }
                else if (c is Decorator decorator)
                {
                    if (decorator.Child.GetType() == typeof(T))
                    {
                        return (T)decorator.Child;
                    }
                }
                else if (c is Panel panel)
                {
                    FrameworkElement cp = panel.GetChildrenByType<T>();
                    if (cp?.GetType() == typeof(T))
                    {
                        return (T)cp;
                    }
                }
            }
            return null;
        }

        public static Point RelativeLocation(this Visual from, Visual to)
        {
            try
            {
                GeneralTransform generalTransform = from.TransformToAncestor(to);
                Point point = generalTransform.Transform(new Point(0, 0));
                return point;
            }
            catch
            {
                return new Point(0, 0);
            }
        }

        public static void ShowSubPage(this Panel panel, UserControl subPage, bool isHidden = false)
        {
            foreach (UIElement ui in panel?.Children)
            {
                if (ui != subPage)
                {
                    ui.Visibility = isHidden ? Visibility.Hidden : Visibility.Collapsed;
                }
            }
            if (panel.Children.Contains(subPage))
            {
                subPage.Visibility = Visibility.Visible;
            }
            else
            {
                panel.Children.Add(subPage);
            }
        }
    }
}
