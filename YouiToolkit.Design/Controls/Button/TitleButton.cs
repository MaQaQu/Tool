using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace YouiToolkit.Design
{
    [Obsolete]
    public class TitleButton : Button
    {
        internal BaseCommand InternalCommand => Type switch
        {
            ETitleButtonType.None => new BaseCommand()
            {
                Action = (o) => _ = false,
            },
            ETitleButtonType.Min => new BaseCommand()
            {
                Action = (o) => (o as TitleButton)!.FindWindow()!.WindowState = WindowState.Minimized,
            },
            ETitleButtonType.Max => new BaseCommand()
            {
                Action = (o) => (o as TitleButton)!.FindWindow()!.WindowState = WindowState.Maximized,
            },
            ETitleButtonType.Close => new BaseCommand()
            {
                Action = (o) => (o as TitleButton)?.FindWindow()?.Close(),
            },
            ETitleButtonType.Custom => new BaseCommand()
            {
                Action = (o) => OnCustomCommandRun(o),
            },
            _ => default!,
        };

        /// <summary>
        /// 鼠标悬浮背景色
        /// </summary>
        public Brush MouseOverBackground
        {
            get => (Brush)GetValue(MouseOverBackgroundProperty);
            set => SetValue(MouseOverBackgroundProperty, value);
        }
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(TitleButton), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 鼠标悬浮前景色
        /// </summary>
        public Brush MouseOverForeground
        {
            get => (Brush)GetValue(MouseOverForegroundProperty);
            set => SetValue(MouseOverForegroundProperty, value);
        }
        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(TitleButton), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 标题按钮类型
        /// </summary>
        public ETitleButtonType Type
        {
            get => (ETitleButtonType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(ETitleButtonType), typeof(TitleButton), new PropertyMetadata(ETitleButtonType.None, OnTypeChanged));

        /// <summary>
        /// 构造
        /// </summary>
        public TitleButton()
        {
            //Trigger trigger = new Trigger()
            //{
            //    Property = IsMouseOverProperty,
            //    Value = false,
            //};
            //trigger.Setters.Add(new Setter(BackgroundProperty, MouseOverBackground));
            //Triggers.Add(trigger);
        }

        public static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleButton? sender = d as TitleButton;
            var newValue = (ETitleButtonType)e.NewValue;

            sender!.Command = sender!.InternalCommand;
            sender!.CommandParameter = d;

            switch (newValue)
            {
                case ETitleButtonType.None:
                default:
                    break;
                case ETitleButtonType.Min:
                    {
                        // <Path Stroke="#0A0C0D" StrokeThickness="1">
                        //     <Path.Data>
                        //         <LineGeometry StartPoint="0,0" EndPoint="12,0"/>
                        //     </Path.Data>
                        // </Path>
                        Path path = new Path() { Stroke = BrushUtils.GetBrush("#0A0C0D") };
                        path.Data = new LineGeometry(startPoint: new Point(0, 0), endPoint: new Point(12, 0));
                        sender!.Content = path;
                        sender!.MouseOverBackground = BrushUtils.GetBrush("#F5F5F5");
                    }
                    break;
                case ETitleButtonType.Max:
                    {
                        // <Canvas Width="10" Height="10">
                        //     <Rectangle Visibility="Visible" Width="10" Height="10" Stroke="#0A0C0D" StrokeThickness="1"/>
                        //     < Rectangle Visibility="Collapsed" Width="8" Height="8" Stroke="#0A0C0D" StrokeThickness="1" Canvas.Top="2"/>
                        //     < Polyline Visibility="Collapsed" Stroke="#0A0C0D" Points="2,2 2,0 10,0 10,8 8,8" StrokeThickness="1"/>
                        // </Canvas>
                        Canvas canvas = new Canvas() { Width = 10, Height = 10 };
                        canvas.Children.Add(new Rectangle()
                        {
                            Name = "PART_max",
                            Visibility = Visibility.Visible,
                            Width = 10,
                            Height = 10,
                            Stroke = BrushUtils.GetBrush("#0A0C0D"),
                            StrokeThickness = 1,
                        });
                        canvas.Children.Add(new Rectangle()
                        {
                            Name = "PART_resotre1",
                            Visibility = Visibility.Collapsed,
                            Width = 10,
                            Height = 10,
                            Stroke = BrushUtils.GetBrush("#0A0C0D"),
                            StrokeThickness = 1,
                        });
                        canvas.Children.Add(new Polyline()
                        {
                            Name = "PART_resotre2",
                            Visibility = Visibility.Collapsed,
                            Width = 8,
                            Height = 8,
                            Stroke = BrushUtils.GetBrush("#0A0C0D"),
                            StrokeThickness = 1,
                            Points = new PointCollection()
                            {
                                new Point(2, 2),
                                new Point(2, 0),
                                new Point(10, 0),
                                new Point(10, 8),
                                new Point(8, 8),
                            },
                        });
                        sender!.Content = canvas;
                        sender!.MouseOverBackground = BrushUtils.GetBrush("#F5F5F5");
                    }
                    break;
                case ETitleButtonType.Close:
                    {
                        // <Canvas Width="10" Height="10">
                        //     <Polyline Stroke="#0A0C0D" Points="0,0 10,10" StrokeThickness="1"/>
                        //     <Polyline Stroke="#0A0C0D" Points="0,10 10,0" StrokeThickness="1"/>
                        // </Canvas>
                        Canvas canvas = new Canvas() { Width = 10, Height = 10 };
                        canvas.Children.Add(new Polyline()
                        {
                            Name = "PART_close1",
                            Stroke = BrushUtils.GetBrush("#0A0C0D"),
                            StrokeThickness = 1,
                            Points = new PointCollection()
                            {
                                new Point(0, 0),
                                new Point(10, 10),
                            },
                        });
                        canvas.Children.Add(new Polyline()
                        {
                            Name = "PART_close2",
                            Stroke = BrushUtils.GetBrush("#0A0C0D"),
                            StrokeThickness = 1,
                            Points = new PointCollection()
                            {
                                new Point(0, 10),
                                new Point(10, 0),
                            },
                        });
                        sender!.Content = canvas;
                        sender!.MouseOverBackground = BrushUtils.GetBrush("#E81123");
                    }
                    break;
                case ETitleButtonType.Custom:
                    sender!.OnCustomTypeChanged(sender);
                    break;
            }

            FrameworkElement? frameworkElement = sender!.Content as FrameworkElement;
            switch (newValue)
            {
                case ETitleButtonType.None:
                case ETitleButtonType.Min:
                case ETitleButtonType.Max:
                case ETitleButtonType.Custom:
                    if (frameworkElement is Shape shape)
                    {
                        shape.Stroke = sender!.MouseOverForeground;
                    }
                    else if (frameworkElement is Canvas canvas)
                    {
                        foreach (Shape shapeCur in canvas.Children)
                        {
                            if (shapeCur != null)
                            {
                                shapeCur.Stroke = sender!.MouseOverForeground;
                            }
                        }
                    }
                    break;
                case ETitleButtonType.Close:
                    break;
            }
        }

        public virtual void OnCustomCommandRun(object sender)
        {
        }

        public virtual void OnCustomTypeChanged(object sender)
        {
        }
    }

    /// <summary>
    /// 标题按钮类型
    /// </summary>
    public enum ETitleButtonType
    {
        None,
        Min,
        Max,
        Close,
        Custom,
    }
}
