using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouiToolkit.Design;
using YouiToolkit.Models;

namespace YouiToolkit.Views
{
    public partial class PageAvoidObstacle : UserControl
    {
        public PageAvoidObstacle()
        {
            InitializeComponent();

            // 地图渲染控件上下文构造完成事件
            render.ContextChanged += (s, e) =>
            {
                // 地图渲染状态信息变化事件
                render.Context.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(render.Context.MouseLoctMap):
                            runMouseLoctMap.Text = render.Context.ToString(e.PropertyName);
                            break;
                        case nameof(render.Context.OriginScale):
                            runOriginScale.Text = render.Context.ToString(e.PropertyName);
                            break;
                    }
                };

                render.Context.Layer.Edited += (s) =>
                {
                    if (stackPanelThumbnailImage.Children.Find(e => e.Index == s.Index, out ThumbnailImage image))
                    {
                        image.SetThumbnail(s.CreateThumbnail());
                    }
                };
            };

            buttonSwitchToSlowDown.Click += (s, e) =>
            {
                render.Context.Layer.DragPointSetBlock.Active(AvoidObstacleAreaTypeAs.Decelerate);
            };

            buttonSwitchToStop.Click += (s, e) =>
            {
                render.Context.Layer.DragPointSetBlock.Active(AvoidObstacleAreaTypeAs.Stop);
            };

            buttonSwitchEmergencyStop.Click += (s, e) =>
            {
                render.Context.Layer.DragPointSetBlock.Active(AvoidObstacleAreaTypeAs.EmergencyStop);
            };

            scrollViewerImage.SetFocusable(false);
#if DEBUG
            KeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    string path = $"{Path.GetTempPath()}\\dragpoint-thumbnail.png";
                    Bitmap bitmap = render.Context.Layer.DragPointSetBlock.CreateThumbnail();
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    stackPanelThumbnailImage.GetChildrenByType<ThumbnailImage>()?.SetThumbnail(bitmap);
                    Process.Start(path);
                }
                else if (e.Key == Key.F6)
                {
                    string path = $"{Path.GetTempPath()}\\dragpoint-thumbnail.png";
                    Bitmap bitmap = render.Context.Layer.DragPointSetBlock.CreateThumbnail();
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    stackPanelThumbnailImage.GetChildrenByType<ThumbnailImage>()?.SetThumbnail(bitmap);
                }
                else if (e.Key == Key.F1)
                {
                    render.Context.Layer.Angle = Ctrls.AvoidObstacleAngle._360;
                }
                else if (e.Key == Key.F2)
                {
                    render.Context.Layer.Angle = Ctrls.AvoidObstacleAngle._270;
                }
                else if (e.Key == Key.F3)
                {
                    render.Context.Layer.Angle = Ctrls.AvoidObstacleAngle._180;
                }
            };
#endif

            for (int i = default; i < AvoidObstacleStock.StockMax; i++)
            {
                var image = new ThumbnailImage()
                {
                    Name = $"thumbnailImage{i}",
                    Height = 80,
                    Width = 160,
                    Index = i,
                };
                image.MouseLeftButtonDown += (s, e) =>
                {
                    render.Context.Layer.SwitchTo(image.Index);
                };
                image.ShortCut += (s, e) =>
                {
                    render.Context.Layer.ShortCutTo(e);
                };
                stackPanelThumbnailImage.Children.Add(image);
            }

            buttonCopy.Click += (s, e) =>
            {
                if (stackPanelThumbnailImage.Children.Find(e => e.IsSelected, out ThumbnailImage image))
                {
                    image.Copy();
                }
                else
                {
                    MessageBoxX.Warning(this, "请选择要复制的数据行");
                }
            };

            buttonPaste.Click += (s, e) =>
            {
                if (stackPanelThumbnailImage.Children.Find(e => e.IsSelected, out ThumbnailImage image))
                {
                    image.Paste();
                }
                else
                {
                    MessageBoxX.Warning(this, "请选择要粘贴的数据行");
                }
            };

            buttonCut.Click += (s, e) =>
            {
                if (stackPanelThumbnailImage.Children.Find(e => e.IsSelected, out ThumbnailImage image))
                {
                    image.Cut();
                }
                else
                {
                    MessageBoxX.Warning(this, "请选择要剪切的数据行");
                }
            };

            buttonClear.Click += (s, e) =>
            {
                if (MessageBoxX.Question(this, "确定要删除全部数据行？\n提示：删除后数据不可恢复。") == MessageBoxResult.Yes)
                {
                    render.Context.Layer.Stock.Clear();
                    render.Context.Layer.DragPointSetBlock.Clear();
                    stackPanelThumbnailImage.Children.Foreach<ThumbnailImage>(e => e.SetThumbnail(null));
                }
            };

            buttonRemove.Click += (s, e) =>
            {
                if (stackPanelThumbnailImage.Children.Find(e => e.IsSelected, out ThumbnailImage image))
                {
                    image.Remove();
                }
                else
                {
                    MessageBoxX.Warning(this, "请选择要删除的数据行");
                }
            };

            buttonSave.Click += (s, e) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    RestoreDirectory = true,
                    DefaultExt = "*.json;",
                    Filter = "Json(*.json)|*.json|Yaml(*.yaml)|*.yaml",
                };
                if (saveFileDialog.ShowDialog() ?? false)
                {
                    try
                    {
                        var jsonObject = AvoidObstacleDragPointSerializeObject.Create(render.Context.Layer.Stock);
                        if (int.TryParse(numTextBoxDelayStart.Text, out int dalayStart))
                        {
                            jsonObject.DelayStart = dalayStart;
                        }
                        if (int.TryParse(numTextBoxDelayEnd.Text, out int dalayEnd))
                        {
                            jsonObject.DelayEnd = dalayEnd;
                        }
                        jsonObject.Sensitivity = (float)(sliderSensitivity.Value / 100d);
                        string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                        File.WriteAllText(saveFileDialog.FileName, jsonString);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxX.Error(this, $"详细错误：{ex}", "保存失败");
                    }
                }
            };
        }
    }
}
