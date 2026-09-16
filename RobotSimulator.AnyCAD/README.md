# 多型号机器人 AnyCAD WPF 演示

这是一个独立于原 HelixToolkit 原型的 `.NET 10` WPF 演示项目，使用 AnyCAD Rapid.NET 直接读取 STEP/B-Rep 模型。

程序会先显示主窗口和加载动画，再在后台读取模型，加载期间界面保持响应。首次读取通过 XDE 保留 STEP 内的零件与逐面颜色，同时在本机生成 BREP 几何和颜色缓存；后续启动会跳过耗时的 STEP 解析。缓存按源文件大小与修改时间自动失效，位置为 `%LOCALAPPDATA%/RobotSimulator.AnyCAD/ModelCache`。

三维视图在模型加载前即设置 AnyCAD 的浅蓝 CAD 背景主题，并使用默认实体光照和清晰轮廓线。XDE 返回的线性颜色会在显示前转换回 sRGB；不同颜色的面分别生成显示节点，避免复合 STEP 优化网格造成面索引串色。显示网格采用 1 mm 弦高误差并启用安全优化，以兼顾接近 SolidWorks 的显示观感和交互流畅度。为避免大型 CAD 网格拖慢操作，没有启用实时阴影。

## 模型约定

- 每个机器人使用一个独立目录，例如 `模型文件/KR_10_R1440-2`。
- 目录内放置 `Base.STEP`、`J1.STEP`～`J6.STEP`、可选的 `Tool.STEP`，以及与目录同名的 `KR_10_R1440-2.robot.json`。
- 构建时所有型号目录会原样复制到输出目录的 `Models`；程序启动后自动发现全部 `Models/<型号>/<型号>.robot.json`，可从窗口顶部切换。
- 后续只需将 `Tool.STEP` 放入同一源模型目录，重新构建并点击“重新载入模型”。
- 所有零件必须保持同一个全局坐标系与毫米单位。

添加型号 `MyRobot` 时，目录结构必须是：

```text
模型文件/
└─ MyRobot/
   ├─ MyRobot.robot.json
   ├─ Base.STEP
   ├─ J1.STEP
   ├─ J2.STEP
   ├─ J3.STEP
   ├─ J4.STEP
   ├─ J5.STEP
   ├─ J6.STEP
   └─ Tool.STEP           # 可选
```

## 运行

在仓库根目录执行：

```powershell
dotnet restore RobotSimulator.AnyCAD\RobotSimulator.AnyCAD.csproj --configfile NuGet.Config
dotnet run --project RobotSimulator.AnyCAD\RobotSimulator.AnyCAD.csproj --no-restore
```

## 配置

每个 `<型号>.robot.json` 保存关节机械行程、初始角度、CAD 模型参考角、轴心、轴方向和 Tool 参数。默认轴参数通过相邻 STEP 零件中重复出现的同轴圆柱面推导，并在程序的“轴参数”页中开放校准。

KR 10 R1440-2 的官方机械行程为 A1 ±170°、A2 -185°～65°、A3 -137°～163°、A4 ±185°、A5 ±120°、A6 ±350°。拆分后的 STEP 保留了原装配姿态；从关节中心位置可确认该 CAD 姿态对应 A1=0°、A2=-90°、A3=90°、A4=0°、A5=0°、A6=0°。因此程序不再把 STEP 姿态误认为六轴 0°，而是始终按 `显示转角 = 示教器角度 - modelReferenceAngle` 计算。

手册列出的机械校准位置为 A1=-19°、A2=-90°、A3=90°、A4=90°、A5=0°、A6=0°。它用于 mastering，不等同于本项目的 CAD 初始装配姿态，也不应直接作为全部模型的零偏。

界面的 A1～A6 角度正方向与 KUKA 示教器保持一致。相对于当前 STEP 的全局坐标系，方向映射为：

| 示教器轴 | STEP 模型部件 | 正方向 |
| --- | --- | --- |
| A1 | J1 | -Z |
| A2 | J2 | +Y |
| A3 | J3 | +Y |
| A4 | J4 | -X |
| A5 | J5 | +Y |
| A6 | J6 | -X |

机器人采用嵌套场景树：

`Base → J1 → J2 → J3 → J4 → J5 → J6 → Tool`

因此任一上游关节转动时，所有下游关节和工具都会随动。

## 怎样配置关节旋转轴

每个拆分 STEP 必须保留原机器人装配的全局坐标，不能把 J1～J6 分别移动到各自原点。每个关节配置含义如下：

- `axisOrigin`：旋转轴线上的任意一点，单位 mm。通常取该关节两侧同轴圆柱面的圆心。
- `axisDirection`：旋转轴方向向量，不要求单位长度。方向决定示教器角度增加时的正转方向；反向时将三个分量同时取反。
- `modelReferenceAngle`：未经旋转的 STEP 装配姿态对应的 KUKA 示教器角度。
- `homeAngle`：启动及“回到初始位”使用的示教器角度。
- `minimum/maximum`：设备手册规定的控制器绝对行程，不是相对 STEP 姿态的转角。

程序实际应用的角度是 `angle - modelReferenceAngle`。校准某一轴时，先只调整该轴；正确结果是该零件及所有下游零件围绕同一轴线运动，连接处不偏心、不脱节。

## 怎样配置 Tool

Tool 配置分为两个用途不同的坐标：

- `toolModelPose`：只控制 `Tool.STEP` 的显示安装位置，是从 Tool 模型局部坐标到机器人 CAD 基准坐标的变换。如果 Tool 与机器人在同一个 CAD 装配中定位后导出，保持六项为 0；如果 Tool 单独以自身原点导出，则填写其在机器人基准姿态下相对 Base 的 X/Y/Z/A/B/C。
- `toolTcp`：控制器使用的 TCP，等价于 KUKA `$TOOL={X,Y,Z,A,B,C}`，必须填写相对于 A6 法兰坐标系的位姿。它与 Tool 模型如何导出无关。

X/Y/Z 使用 mm，A/B/C 使用度，旋转约定为 KUKA 的 `Rz(A) · Ry(B) · Rx(C)`。当前版本使用 `toolModelPose` 安装显示模型，并保存 `toolTcp` 供后续正逆运动学和轨迹功能使用。
