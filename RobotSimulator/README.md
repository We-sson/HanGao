# 独立机器人、Tool 与工装资源的 Macad3D / OpenCASCADE WPF 仿真

项目采用 MVVM：`MainWindow.xaml` 通过 `InvokeCommandAction` 调用 `MainViewModel` 命令，三维加载和显示由场景服务完成。

## 显示与操作

- 视口操作沿用常见 CAD 习惯：左键拖动旋转，右键拖动平移，滚轮缩放（Ctrl 精细缩放）。视口不启用框选与选择；适合窗口用工具栏按钮。
- MES 的「仿真设置」提供性能（2× MSAA）、均衡（4×）、高清（8×），独立窗口还提供极致（8× MSAA + 1.5 倍超采样）。画质档映射到 Macad 的 `NbMsaaSamples` 与 `RenderResolutionScale`，默认高清档。当前渲染路径的接触阴影效果待另行选型。
- 相机主光/补光跟随视线，转到任何方位正面都有光。
- 近/远裁剪面按模型八个角点的真实深度计算，缩放或平移贴近模型时不会切掉表面。

## 资源和发布规则

- 机器人、Tool 和 Work 工装是三个独立资源库，可自由组合，不为每个机器人复制 Tool 或工装。
- 机器人显示缓存 `.cad`、六轴参数和法兰参数内嵌。Debug 提供基于源 STEP 的开发配置向导，Release 不包含此向导。
- Tool 显示缓存 `.cad` 和默认配置内嵌。`Tool.STEP` 是配置里的源模型名，运行时加载同名 `Tool.cad`；首次运行只生成 `Tools/<工具标识>/<模型名>.tool.json`。
- 工装显示缓存 `.cad` 与默认安装配置内嵌。首次运行生成 `Work/<工装标识>/<模型名>.work.json`，工装保持在机器人基坐标下。
- 机器人只允许改变当前关节角度；行程、CAD 参考角、初始角、轴心、方向和法兰均在开发阶段固定。
- 切换 Tool 只替换工具几何，不重新读取机器人，不重置当前六轴姿态；切换机器人保留当前 Tool 及其 TCP。
- 切换或重载工装保留机器人姿态和 Tool；切换机器人保留所选工装及当前安装位姿。
- 顶部统一提供机器人、Tool、工装三个选择列表。切换机器人时，旧场景保留到新机器人准备完成，只替换机器人节点；Tool 与工装复用现有几何，相机视角保持不变，读取失败保留原机器人。
- 正式发布使用新的发布目录。程序不会读取或迁移之前版本留在其他目录的配置文件，不要把旧输出目录整包复制进新发布目录。

```text
RobotSimulator/Resources/
├─ Robots/
│  ├─ KR_10_R1440-2/
│  │  ├─ KR_10_R1440-2.robot.json
│  │  ├─ Base.STEP
│  │  └─ J1.STEP … J6.STEP
│  └─ KR_35_R1820_2/
│     ├─ KR_35_R1820_2.robot.json
│     ├─ Base.STEP
│     └─ J1.STEP … J6.STEP
├─ Tools/
│  └─ Tool/
│     ├─ Tool.tool.json
│     └─ Tool.STEP
└─ Work/
   └─ 双点焊工装/
      ├─ 双点焊工装.work.json
      └─ 双点焊工装.STEP
```

机器人型号由同目录的 `<型号>.robot.json` 识别。Tool 与工装目录各放一份 STEP 和同名配置。`Resources` 只保存 STEP 源模型和配置，`modelFile` 仍填写原文件名，客户已保存的 TCP 和工装位姿可直接继续使用。

构建时，`Build/ConvertStep.py` 用 OCCT XDE 一次读取 STEP 的装配位置、BRep 和表面颜色，直接写入内嵌的 `.cad` 缓存。机器人、Tool 和工装走同一流程。缓存使用毫米、Z 向上，保留逐面 sRGB 和 CAD 拓扑。运行时由 Macad 读取缓存、绘制彩色面和边线。

构建机准备 Windows x64 Python 3.12（含 pip），让 `python` 在 PATH 可用。也可设置 MSBuild 属性 `StepPython`，或在 `Build/Python.local.props` 指定本机解释器：

```xml
<Project>
  <PropertyGroup>
    <StepPython>C:\Python312\python.exe</StepPython>
  </PropertyGroup>
</Project>
```

首次构建会在 `obj` 还原固定版本的转换依赖。之后只转换新增或修改的 STEP，Debug 与 Release 共用缓存。客户端运行 WPF 程序和随程序发布的 Macad3D/OCCT 运行库。

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

`KR_20_R1810_2` 已提供[机器人配置及数据来源](Resources/Robots/KR_20_R1810_2/配置与参数来源.md)。行程按对应型号 V15.1 数据表填写，A5 为 ±130°；几何测量包含 J5/J6 的 STEP 装配平移。参考角与方向采用项目 KUKA 仿真映射，待控制器核对。

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

机器人配置中的关节行程、启动角、CAD 参考角、轴心/方向 XYZ 和法兰 ABC 必须逐项填写，实测为 0 也要写出，缺失时会明确报错。Tool/工装位姿仍允许省略值为 0 的分量。

现有参数参考点：

| 型号 | 初始/CAD 参考角 A1～A6 | 法兰中心 XYZ（mm） | 法兰 ABC（度） |
| --- | --- | --- | --- |
| KR_10_R1440-2 | 0 / -90 / 90 / 0 / 0 / 0 | 930 / 0 / 1055 | 0 / 90 / 0 |
| KR_35_R1820_2 | 0 / -90 / 90 / 0 / 0 / 0 | 1153.3 / 0 / 1483.5 | 0 / 90 / 0 |
| IRB2600_20_165 | 0 / 0 / 0 / 0 / 0 / 0 | 1030 / 0 / 1260 | 0 / 90 / 0 |
| KR_20_R1810_2 | 0 / -90 / 90 / 0 / 0 / 0 | 1173 / 0 / 1450 | 0 / 90 / 0 |

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

`toolName` 是工具显示名称，`modelFile` 是用于定位同名 `.cad` 显示缓存的源模型名。两项由开发时设置；现场主要调整 `tcp`。

工具原点自动安装到当前机器人的法兰，XYZ 同时对齐法兰方向。`tcp` 相当于 KUKA `$TOOL`：XYZ 使用 mm，ABC 使用度，旋转顺序为 `Rz(A)·Ry(B)·Rx(C)`，只改变工具中心坐标轴，不改变工具几何的安装。

## 现场修改工具

1. 在顶部选择机器人和 Tool，旁边的工装列表可独立选择。
2. 在“Tool 坐标”页填写 TCP，坐标箭头立即更新。
3. 点击“保存 Tool 配置”，写入 `Tools/<工具标识>/<模型名>.tool.json`；同一工具搭配另一台机器人时仍使用这份配置。
4. 外部修改 JSON 后点击“重新载入 Tool”。只重读工具，不重置机器人角度；未保存的 TCP 会被文件值替换。

已有工具配置不会被启动或构建覆盖。文件被删除后，下次启动恢复内置默认值。工具配置校准后应备份；如果需要作为新安装的默认值，将结果同步回工具资源目录并重新构建。

启动时遇到损坏或不可读的 Tool/工装配置，会保留原文件并单独提示路径与原因，跳过该附件，其余正常组合仍可使用。修复文件后重新启动即可恢复；程序不会用默认位姿替换错误配置。

## 工装安装与后续新增

1. 在顶部与机器人、Tool 并列的工装列表中选择一套工装；右侧“工装安装”页用于调整位置。首次启动自动安装工装列表中的第一套。
2. 工装的 CAD 原点对齐机器人基坐标原点，方向也对齐。当前“双点焊工装”六项安装参数均为 0，保留模型内所有零件的原始装配位置，不按包围盒重新居中，也不挂在机器人法兰上。
3. 调整 XYZ/ABC 时整体移动工装，位置相对于机器人基坐标。XYZ 为 mm，ABC 为度，旋转顺序为 `Rz(A)·Ry(B)·Rx(C)`。机器人六轴运动、复位及示例动作不会带动工装。
4. 点击“保存工装配置”，写入程序目录的 `Work/<工装标识>/<模型名>.work.json`。外部修改 JSON 后点击“重新载入工装”；未保存的页面修改会被文件值替换。

后续每套工装单独放入 `Resources/Work/<工装标识>/`，每个目录包含一份装配 STEP 和同模型名的 `.work.json`。例如 `夹具A.STEP` 对应 `夹具A.work.json`：

```json
{
  "workName": "夹具A",
  "modelFile": "夹具A.STEP",
  "pose": { "x": 0, "y": 0, "z": 0, "a": 0, "b": 0, "c": 0 }
}
```

添加目录后重新构建，自动生成显示资源，工装会出现在下拉框中。运行时只输出工装配置。已有工装配置不会被启动或构建覆盖；删除后下次启动恢复内置默认值。校准完成后可把保存结果同步回源资源目录，作为新安装的默认值。

## 显示与性能

- 机器人 Base 与 J1～J6 最多四路并行读取，全部完成后一次组装并显示，保留六轴父子运动关系。加载器直接从内嵌 `.cad` 读取 BREP、三角化与逐面 sRGB 颜色。
- 仅首次加载显示中央加载提示。后续切换期间保留当前模型，在状态栏显示进度；成功后替换对应模型，切换机器人不再重读 Tool 和工装，也不清空整个场景。
- 基坐标保持在机器人原点，法兰和 TCP 随运动链更新；三个显示开关互相独立。
- XYZ 以红、绿、蓝表示，无三维文字；箭头固定屏幕尺寸为基坐标/TCP 64px、法兰 48px，缩放相机时保持观察大小。
- 点击等轴测、正视或俯视时停止自转，页面环绕速度同步为 0；之后仍可手动拖动视角或重新设置自转速度。
- 客户端从程序集读取预先生成的显示缓存 `.cad`（BREP + 三角化 + 逐面颜色）。模型范围只计算机器人、Tool 和工装的几何，坐标箭头和焊光不影响相机适配。
- 转换时把 CAD 材质色统一写成 `.cad` 的 sRGB 字节，Macad 视口按 sRGB 设色，再配合中性材质与默认灯光保持亮部和暗部容易分辨。
- 独立窗口和 MES 视口共用同一套视口设置：`RobotSceneService` 的浅蓝灰渐变背景（sRGB 151/165/186 → 213/225/243，垂直）与 `V3d_Viewer.SetDefaultLights()` 的默认灯光。
- 显示缓存的三角化使用 0.35 mm 线性偏差和 0.45 rad 角度偏差。示例动作按 WPF 渲染帧推进。调整视角时自转和示例动作都暂停，停止操作后从原姿态继续。
- 相机自转按 WPF 渲染时间推进，相机约以 30 Hz 更新。手动调整期间暂停自转，结束后从当前视角继续。MES 切离三维页时停止渲染，返回时复用场景并接续最新姿态。
- 鼠标旋转、平移与自动自转都作用于当前相机；整圈适配使用各部件边界的最大投影范围。

## 运行与检查

部署到干净的发布目录，安装包包含第三方许可声明 `THIRD-PARTY-NOTICES.txt`。

在仓库根目录执行：

```powershell
dotnet restore RobotSimulator\RobotSimulator.csproj --configfile RobotSimulator\NuGet.Config
dotnet run --project RobotSimulator\RobotSimulator.csproj --no-restore
```

回归检查：

```powershell
dotnet restore RobotSimulator.Tests\RobotSimulator.Tests.csproj --configfile RobotSimulator\NuGet.Config
dotnet run --project RobotSimulator.Tests\RobotSimulator.Tests.csproj --no-restore
```

代码阅读顺序：`Models/RobotConfiguration.cs`（固定机器人参数）、`Models/ToolConfiguration.cs`（可编辑工具）、`Models/WorkConfiguration.cs`（工装安装）、`ViewModels/MainViewModel.cs`（选择和命令）、`Services/RobotSceneContent.cs`（AIS 装配与坐标）、`Services/CadModelPart.cs`（内嵌 `.cad` 读取）、`Services/CadDisplayPackage.cs`（Brotli 解包）、`Services/RobotSceneService.cs`（视口、相机与原生鼠标操作检测）、`Services/MacadViewportBootstrap.cs`（Macad 交互上下文）。
