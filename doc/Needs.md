# YouiToolkit

## 背景

在Compass的B/S架构下一些不方便实现的功能，希望能实现在一个C#工具集里，工具集不仅可以满足当前需要改善的功能，如建图、维护、配置，并且可以在该工具集里继续扩展今后可能需要的功能。

考虑，C#主要是用.NET Framework框架，可直接用于win10/11，或者使用.NET 6，需要安装.NET Runtime或直接内置。

## 当前需要功能

### 1. 部署

> 目前已有基于Winform+Unity的demo。

#### 新建地图功能

概述：需要处理Pilot传送的点云信息实时显示地图的建立过程和结果。

① 开始新建地图：选择目标地图参数开始建图，点云是30Hz频率，如果难以实时更新可以建立过程每3秒刷新一次中间结果地图。**--已有算法支持？**

② 结束新建地图：输出最终地图。**--输出png图片？**

③ 遥控：方向键遥控。

④ 编辑地图：添加诸如移动目标点，包括直线和2次贝塞尔曲线的移动方式。**--橡皮擦？**

⑤ 录制功能：录制地图、录制特征点、录制点位

```json
{"AGVMap":[{"cameraLens":"0","createTime":"2021-11-16 16:56:28","description":"","height":1788.0,"id":"159b671e-46bb-11ec-acd5-0242ac110002","idVersion":"159b671e-46bb-11ec-acd5-0242ac110002-5","layerVersion":8,"mapData":"data:image/png;base64,","mapPicture":"","name":"xxx","originX":0.0,"originY":0.0,"originYaw":0.0,"physicsHeight":53.64,"physicsWidth":50.91,"resolution":0.03,"type":"LASER_MAP","updateTime":"2021-11-16 17:43:20","usageStatus":"DISABLE","version":5,"versionIncrease":true,"width":1697.0}],"AdjustAction":[],"AutoDoor":[],"DataCodeMatrix":[],"DockingPoint":[],"MapArea":[],"Marker":[{"agvMapId":"159b671e-46bb-11ec-acd5-0242ac110002","code":"1","createTime":"2021-11-16 17:07:23","id":"9c6454e1-46bc-11ec-acd5-0242ac110002","isPark":0,"manipulatorAngle":0.0,"type":"NAVIGATION_MARKER","updateTime":"2021-11-16 17:07:23","usageStatus":"ENABLE","vehicleAngle":0.0,"x":17.124,"y":35.015},{"agvMapId":"159b671e-46bb-11ec-acd5-0242ac110002","code":"2","createTime":"2021-11-16 17:07:54","id":"aef91157-46bc-11ec-acd5-0242ac110002","isPark":0,"manipulatorAngle":0.0,"type":"NAVIGATION_MARKER","updateTime":"2021-11-16 17:07:54","usageStatus":"ENABLE","vehicleAngle":0.0,"x":18.163,"y":32.901}],"Path":[{"agvMapId":"159b671e-46bb-11ec-acd5-0242ac110002","createTime":"2021-11-16 17:08:08","direction":0,"endControl":"{\"x\":17.539,\"y\":34.169}","endMarkerId":"9c6454e1-46bc-11ec-acd5-0242ac110002","forwardAgvDirection":1,"id":"037efc4a-9a85-4ac7-a613-ccf5adf56e33","length":2.355529173317813,"lineType":2,"reverseAgvDirection":1,"startControl":"{\"x\":17.747,\"y\":33.747}","startMarkerId":"aef91157-46bc-11ec-acd5-0242ac110002","updateTime":"2021-11-16 17:08:08","usageStatus":"ENABLE"}],"PathParam":[],"SidePath":[{"agvDirection":1,"agvMapId":"159b671e-46bb-11ec-acd5-0242ac110002","createTime":"2021-11-16 17:08:08","endControl":"{\"x\":17.747,\"y\":33.747}","endMarkerId":"aef91157-46bc-11ec-acd5-0242ac110002","id":"48fc9125-96a3-4bed-917f-dd21679df04c","inAngle":-1.1147423070038518,"length":2.355529173317813,"lineType":2,"outAngle":-1.1137899863753515,"pathId":"037efc4a-9a85-4ac7-a613-ccf5adf56e33","startControl":"{\"x\":17.539,\"y\":34.169}","startMarkerId":"9c6454e1-46bc-11ec-acd5-0242ac110002","updateTime":"2021-11-16 17:08:08"},{"agvDirection":1,"agvMapId":"159b671e-46bb-11ec-acd5-0242ac110002","createTime":"2021-11-16 17:08:08","endControl":"{\"x\":17.539,\"y\":34.169}","endMarkerId":"9c6454e1-46bc-11ec-acd5-0242ac110002","id":"e28b4638-b81b-41f3-8c76-3c88b9e8d437","inAngle":2.027802667214442,"length":2.355529173317813,"lineType":2,"outAngle":2.0268503465859418,"pathId":"037efc4a-9a85-4ac7-a613-ccf5adf56e33","startControl":"{\"x\":17.747,\"y\":33.747}","startMarkerId":"aef91157-46bc-11ec-acd5-0242ac110002","updateTime":"2021-11-16 17:08:08"}]}
```

**疑问--输入是点云，输出是什么格式的地图才能用于定位？**

#### 修改地图功能

概述：支持包括对已有地图的补建和扩建。

思路：工具栏提供一个小剪刀的工具，可以对连通轮廓进行截断，线段两端截断成另一个轮廓，轮廓可以选中删除。

① 地图选中：用户鼠标可以选中地图轮廓线段，并且可以删除选中。

② 修改地图：当前地图的基础上继续新建地图的过程。

### 2. 维护

#### 回溯播放

概述：可以回溯播放pilot日志，播放不同时刻的路径轨迹动画、显示状态信息、时间轴滑动或翻页，支持\*.log和\*.gz。

① 日志解析：按照pilot的日志规则正则解析有效数据如路点信息、状态信息到数据库或自定义格式，便于后续的读操作。

② 路径轨迹动画播放：动画形式播放AGV路劲轨迹，支持播放、暂停、停止按钮。

③ 时间轴滑动或翻页功能：可通过时间轴跳转到指定时刻开始播放轨迹。

④ 异常提醒：在时间轴上提醒异常时间位置，异常列表突出提醒。

⑤ 快速定位：时间轴与日志行号同步，快速定位问题。

```js
// 看compass的日志（debug、info、error）目前没有类似的信息
// 伪日志
[timestamp] location 0,0,0
[timestamp] location 10,10,10
[timestamp] status change to ready
[timestamp] status change to error
```

### 3. 配置

概述：可视化配置与*.yaml映射的配置。

① 可视化配置：根据不同配置的类型，显示不同的配置控件，比如配置文本TextBox、浮点型Slider滑块或DoubleInput控件。

② yaml操作：导入yaml一键配置、导出当前配置为yaml。

### 4. 标定

C:\Users\ema\AppData\Roaming\SuperWest

