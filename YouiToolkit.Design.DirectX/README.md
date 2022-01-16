# YouiToolkit.Design.DirectX

DirectX的适用于WPF的D2D控件封装库。

## 控件库

| 封装控件类   | 封装基类                                | 最终类             | 说明                                               |
| ------------ | --------------------------------------- | ------------------ | -------------------------------------------------- |
| D2DComponent | DirectXComponent<br />Win32HwndControl  | MapRenderComponent | 基于窗体句柄绘制，`单线程`效率正常                 |
| D2DControl   | D2DContentControl<br />Win32HwndControl | MapRenderControl   | 基于窗体句柄绘制，`多线程`绘制力求高效             |
| D2DImage     | Dx11ImageSource                         | MapRenderImage     | 基于WPF的Image控件Source绑定绘制，`单线程`效率较低 |

## 参考文档

### CSDN

在 WinForm 中使用 Direct2D

> https://blog.csdn.net/weixin_30429201/article/details/98996891

### MSDN

Direct2D 编程指南

>https://docs.microsoft.com/zh-cn/windows/win32/direct2d/programming-guide

几何概述

>https://docs.microsoft.com/zh-cn/windows/win32/direct2d/direct2d-geometries-overview

Direct2D 示例

>https://docs.microsoft.com/zh-cn/windows/win32/direct2d/d2d-samples

Direct2D 接口

>https://docs.microsoft.com/zh-cn/windows/win32/direct2d/interfaces

提高 Direct2D 应用的性能

> https://docs.microsoft.com/zh-cn/windows/win32/direct2d/improving-direct2d-performance#reuse-resources

多线程 Direct2D 应用

>https://docs.microsoft.com/zh-cn/windows/win32/direct2d/multi-threaded-direct2d-apps
