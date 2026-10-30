# 独立机器人与 Tool 资源的 AnyCAD WPF 仿真

项目采用 MVVM：`MainWindow.xaml` 通过 `InvokeCommandAction` 调用 `MainViewModel` 命令，三维加载和显示由场景服务完成。

## 资源和发布规则

- 机器人与 Tool 是两个独立资源库，界面可自由组合，不为每个机器人复制 Tool。
- 机器人 STEP、六轴参数和法兰参数全部内嵌。仿真运行时只读取内嵌配置；Debug 提供开发配置向导，Release 不包含此向导。
- Tool STEP 和默认配置同样内嵌。配置与模型同名，例如 `Tool.STEP` 对应 `Tool.tool.json`；首次运行只生成 `Tools/<工具标识>/<模型名>.tool.json`。
- 机器人只允许改变当前关节角度；行程、CAD 参考角、初始角、轴心、方向和法兰均在开发阶段固定。
- 切换 Tool 只替换工具几何，不重新读取机器人，不重置当前六轴姿态；切换机器人保留当前 Tool 及其 TCP。
- 正式发布使用新的发布目录。程序不会读取或迁移之前版本留在其他目录的配置文件，不要把旧输出目录整包复制进新发布目录。

```text
RobotSimulator.AnyCAD/Resources/
├─ Robots/
│  ├─ KR_10_R1440-2/
│  │  ├─ KR_10_R1440-2.robot.json
│  │  ├─ Base.STEP
│  │  └─ J1.STEP … J6.STEP
│  └─ KR_35_R1820_2/
│     ├─ KR_35_R1820_2.robot.json
│     ├─ Base.STEP
│     └─ J1.STEP … J6.STEP
└─ Tools/
   └─ Tool/
      ├─ Tool.tool.json
      └─ Tool.STEP
```

机器人型号由同目录的 `<型号>.robot.json` 识别。Tool 目录中只放一份 STEP，按模型文件名读取同名 `.tool.json`，目录标识可与模型名不同。文件按目录通配内嵌，新增目录和配置后重新构建即可，无需修改 C# 或 XAML；Tool 命名或模型引用不一致会明确报错，不迁移旧命名。

内嵌用于固定参数、避免现场误删误改，不等于加密或防反编译保护。

## 开发阶段添加机器人

以 Debug 启动，在主界面右侧点击“新机器人配置向导（开发）”。该页面按以下步骤引导配置，右侧随步骤介绍参数的来源、单位和核对方式：

1. **模型导入**：选择七份 STEP 所在的源目录，填写显示名称并选择基座。自动对应 `Base.STEP`、`J1.STEP`～`J6.STEP`；其他文件名可在下一步手动对应。已有同名源配置会载入继续编辑。
2. **逐轴测量**：从 J1 开始，页面指出当前轴和上一级是哪两个零件。依次选模型、抄手册行程、量全局轴心、填方向、核对导出角和启动角。右下角检查通过后自动切换到下一轴，J6 完成后进入法兰；有错误会停在对应关节。新模型的校准值保持空白。
3. **法兰参数**：填写原始 CAD 装配姿态下的全局 XYZ 与 ABC。页面介绍法兰基准的测量方法，以及与当前旋转顺序一致的矩阵换算。
4. **校验保存**：检查七个独立文件、有限数值、行程与初始角、非零方向；保存为源目录下的 `<目录名>.robot.json`。覆盖已有配置前会确认，错误输入不会破坏原文件。

将整个型号目录放入 `Resources/Robots/` 后重新构建 Debug，选择新机器人逐轴核对运动和法兰，再构建 Release。保存不会动态修改当前仿真，也不会将机器人参数输出到发布目录。向导的文件与数值校验不能代替机械校准。

当前支持六个旋转关节。将机器人拆分为 Base、J1～J6，全部保留 SolidWorks 装配的同一个全局坐标，单位为 mm。不要把每个关节分别移到原点。

`KR_10_R1420_HP` 已提供完整[配置演示与参数来源](Resources/Robots/KR_10_R1420_HP/配置演示与参数来源.md)。行程来自该型号单页资料，轴线和法兰中心由 STEP 实测；参考角、正转符号和法兰 X/Y 方向为用户同意的演示映射，待控制器核对。

配置向导右侧为所有型号提供具体操作步骤和静态图解：要查看哪两个零件、在 CAD 中量什么、对应填哪个框，以及保存后怎样核对。图片直接显示在页面，不再提供放大窗口。与坐标轴平行的轴可用 ±X/±Y/±Z 按钮填写方向；斜轴手动填写轴线上两点之差。启动角可从已记录的导出角复制，保持原始装配姿态。

`IRB2600_20_165` 已提供[机器人配置](Resources/Robots/IRB2600_20_165/IRB2600_20_165.robot.json)和[逐步测量记录](Resources/Robots/IRB2600_20_165/配置步骤与测量记录.md)，构建后可选择 `ABB IRB 2600-20/1.65`。行程来自 ABB 数据表，轴线和法兰中心来自 STEP；零位、方向及法兰朝向参考 ROS-Industrial 仿真模型，待本台控制器核对。

在 `Resources/Robots/<型号>/` 放入七份 STEP，参考已有型号创建 `<型号>.robot.json`，只包含以下机器人字段：

| 字段 | 含义与取得方式 |
| --- | --- |
| `modelName` | 机器人下拉框显示名称。 |
| `baseFile` | 基座资源文件名，通常为 `Base.STEP`。 |
| `joints` | 按 J1～J6 顺序排列的六个关节。 |
| `joints[].name` / `modelFile` | 关节名称和本目录中的 STEP 文件名。 |
| `minimum` / `maximum` | 该型号手册中的示教器绝对行程，单位度。 |
| `homeAngle` | 启动和复位时的示教器角度。 |
| `modelReferenceAngle` | 原始 STEP 当前姿态对应的示教器角度；模型实际转角 = 当前角度 - CAD 参考角。 |
| `axisOrigin` | CAD 全局坐标下旋转轴线上的一点，单位 mm，通常取相邻圆柱连接面的圆心。 |
| `axisDirection` | CAD 全局方向向量，按右手规则决定正转方向，不能为零；反转时将 XYZ 同时取反。 |
| `flangeOrigin` | 原始 CAD 姿态下 A6 轴线与法兰安装基准面的交点，单位 mm。 |
| `flangeOrientation` | 法兰 XYZ 在 CAD 全局坐标下的 ABC 朝向，单位度，顺序为 Rz(A)·Ry(B)·Rx(C)。 |

先将 `homeAngle` 设为对应的 `modelReferenceAngle`，可保持原装配姿态。逐轴小幅增加角度：连接处偏心时检查轴心，转向相反时检查方向，整体差一个固定角度时检查 CAD 参考角。开发校准完成后重新构建，客户运行时不能改写这些参数。

现有参数参考点：

| 型号 | 初始/CAD 参考角 A1～A6 | 法兰中心 XYZ（mm） | 法兰 ABC（度） |
| --- | --- | --- | --- |
| KR_10_R1440-2 | 0 / -90 / 90 / 0 / 0 / 0 | 930 / 0 / 1055 | 0 / 90 / 0 |
| KR_35_R1820_2 | 0 / -90 / 90 / 0 / 0 / 0 | 1153.3 / 0 / 1483.5 | 0 / 90 / 0 |
| IRB2600_20_165 | 0 / 0 / 0 / 0 / 0 / 0 | 1030 / 0 / 1260 | 0 / 90 / 0 |

KR35 轴心已与 STEP 同轴圆柱面核对，其主要尺寸与用户提供的 `Spez_KR_20_R1820-2_E_zh.pdf` 第 27 页相符，法兰安装面参考第 30 页。当前 KR35 仿真行程参考该 KR20 手册第 26 页，KR35 的实际行程仍需用对应型号手册或控制器参数核对；这不是 KR35 的厂家确认值。

## 开发阶段添加 Tool

在 `Resources/Tools/<工具标识>/` 放入一份工具 STEP 和同模型名的 `.tool.json`，例如 `Gripper.STEP` 与 `Gripper.tool.json`。工具安装法兰中心放在模型原点，SolidWorks 中的工具 XYZ 对应机器人法兰 XYZ，保留模型相对于自身 XYZ 的摆放。

工具配置格式为：

```json
{
  "toolName": "默认 Tool",
  "modelFile": "Tool.STEP",
  "tcp": { "x": 0, "y": 0, "z": 0, "a": 0, "b": 0, "c": 0 }
}
```

`toolName` 是工具显示名称，`modelFile` 指向该工具目录中的内嵌 STEP。两项由开发时设置；现场主要调整 `tcp`。

工具原点自动安装到当前机器人的法兰，XYZ 同时对齐法兰方向。`tcp` 相当于 KUKA `$TOOL`：XYZ 使用 mm，ABC 使用度，旋转顺序为 `Rz(A)·Ry(B)·Rx(C)`，只改变工具中心坐标轴，不改变工具几何的安装。

## 现场修改工具

1. 在顶部两个下拉框分别选择机器人和 Tool。
2. 在“Tool 坐标”页填写 TCP，坐标箭头立即更新。
3. 点击“保存 Tool 配置”，写入 `Tools/<工具标识>/<模型名>.tool.json`；同一工具搭配另一台机器人时仍使用这份配置。
4. 外部修改 JSON 后点击“重新载入 Tool”。只重读工具，不重置机器人角度；未保存的 TCP 会被文件值替换。

已有工具配置不会被启动或构建覆盖。文件被删除后，下次启动恢复内置默认值。工具配置校准后应备份；如果需要作为新安装的默认值，将结果同步回工具资源目录并重新构建。

## 显示与性能

- 基坐标保持在机器人原点，法兰和 TCP 随运动链更新；三个显示开关互相独立。
- XYZ 以红、绿、蓝表示，无三维文字；箭头固定屏幕尺寸为基坐标/TCP 64px、法兰 48px，缩放相机时保持观察大小。
- STEP 使用 XDE 保留装配、实体与逐面颜色。复合零件中的面先继承所属实体颜色，再应用面上的显式覆盖色；按同色面分组生成显示节点，线性颜色转换为 sRGB 后显示。
- BREP 几何及颜色缓存放在 `%LOCALAPPDATA%/RobotSimulator.AnyCAD/ModelCache`，按部件内容 SHA-256 和格式版本识别，机器人/Tool 改目录不会让原缓存失效。缓存仍需在加载时生成显示网格。
- 实体颜色修正使用 `v7-solid-colors-1mm` 缓存，首次读取会重新生成；旧缓存不会继续遮蔽 Base 外壳和 KUKA 字样的实体颜色，无需手工删除模型或配置。
- 缓存缺失时临时写出 STEP，关闭文件流后交给 XDE 读取，解析结束立即删除。不会向发布目录复制模型文件。
- 显示网格采用 1 mm 弦高误差并关闭实时阴影。动画按 WPF 渲染事件推进，更新排在输入之后，减少播放时拖动视角的卡顿。

## 运行与检查

在仓库根目录执行：

```powershell
dotnet restore RobotSimulator.AnyCAD\RobotSimulator.AnyCAD.csproj --configfile RobotSimulator.AnyCAD\NuGet.Config
dotnet run --project RobotSimulator.AnyCAD\RobotSimulator.AnyCAD.csproj --no-restore
```

回归检查：

```powershell
dotnet restore RobotSimulator.AnyCAD.Tests\RobotSimulator.AnyCAD.Tests.csproj --configfile RobotSimulator.AnyCAD\NuGet.Config
dotnet run --project RobotSimulator.AnyCAD.Tests\RobotSimulator.AnyCAD.Tests.csproj --no-restore
```

代码阅读顺序：`Models/RobotConfiguration.cs`（固定机器人参数）、`Models/ToolConfiguration.cs`（可编辑工具）、`ViewModels/MainViewModel.cs`（选择和命令）、`Services/RobotSceneContent.cs`（层级与坐标）、`Services/CadModelPart.cs`（几何/颜色缓存）、`Services/RobotSceneService.cs`（控件显示）。属性、方法和关键逻辑均带中文注释。
