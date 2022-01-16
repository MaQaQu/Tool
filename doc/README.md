# ROS

## 编译PILOT

```bash
mkdir catkin_ws
cd catkin_ws
mkdir src
catkin_make
./src/YOUIPILOT_YOUIBOT/build.sh
```



## 编译G2O

```bash
sudo apt install git
git checkout 20201223_git
mkdir build
cd build/
cmake ..
make
sudo make install
```



## 建图节点

> 编译`map_assist`和`youibot_graph_slam`节点：

```bash
catkin_make --only-pkg-with-deps map_assist
catkin_make --only-pkg-with-deps youibot_graph_slam -j2
rostopic echo /youibot_graph_slam/markers # 测试
```

>模拟启动

```bash
roscore
roslaunch youibot_graph_slam youibot_graph_slam_offline.launch
roslaunch map_assist map_assist_node_test.launch
```

>正常启动

```bash
roslaunch youibot_graph_slam rviz.launch
roslaunch map_assist map_assist_node_test.launch
```



# PCL库

>官方库：https://github.com/pointcloudlibrary/pcl

> PCD点云数据格式解释：https://www.cnblogs.com/flyinggod/p/8591520.html



# 流程图

[YouiToolkit_MapFlow.vsdx](YouiToolkit_MapFlow.vsdx)


