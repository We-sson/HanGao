# Macad3D 视口 API 清单（IRobotSceneService 的 Macad 实现用）

> 用途：核对 Macad3D（OCCT AIS 封装）接口，供仿真场景服务使用。
> 版本：`RobotSimulator/ThirdParty/Macad/` 下 6 个发行 DLL 全部为 **4.4.2193.0**。仓库 `.tmp-macad-src/` 源码是**更新版本**，与发行 DLL 有结构差异（见 0.2 节）——**以发行 DLL 反射为准**。
> 来源标注：`[反射]`=对发行 DLL 的反射结果，`[源码]`=Macad3D 上游源码，`[xml]`=OCCT XML 文档，`[pilot]`=独立 WPF 验证样板。
> 注：`[源码]` 指调研期间克隆的 Macad3D 源码快照（原仓库根 `.tmp-macad-src/`，迁移完成已清理，需要时从 Macad3D GitHub 重新克隆对应版本）；发行行为仍以 `ThirdParty/Macad/` 的 DLL 为准。

## 0. 结论先行

1. **渲染栈可直接落地**：AIS_ColoredShape（按面设色/透明度）、逐对象 `SetLocalTransformation`、相机全参数、MSAA、背景渐变、headlight 跟随视角、AIS_Trihedron/AIS_Line/AIS_TextLabel 全部在发行 DLL 中可用。
2. **IRobotSceneService 需要的能力里，真正缺失的只有 4 处**：
   - `AIS_Segment` **不存在** → 焊接光/连线用 `AIS_Line`（`Geom_Point` 端点）。
   - `AIS_Text` **不存在** → 用 `AIS_TextLabel`（`SetText(TCollection_ExtendedString)`）。
   - `Srgb` 类型 **不存在** → 自建 helper：`new Quantity_Color(r,g,b,Quantity_TypeOfColor.sRGB)`（pilot 第 319 行即此写法）。
   - `AIS_InteractiveContext.DetectedPoint` **不存在** → 双击取点用 `view.Convert(xp,yp,ref X,ref Y,ref Z)` 或 `AisHelper.PickFromContext`。
3. **版本陷阱（已踩过）**：`ViewportLayoutManager`、`MouseButtons` 枚举、`SetPredefinedView(..., bool animate)`、`TriangulationHelper.GetTriangulation` 4 参重载只在源码里有，**发行 DLL 没有**；用这些写代码会编译失败。
4. **相机/视口不要重建**：从 `InteractiveContext.Current.WorkspaceController` 取 `V3dViewer / AisContext`，从 `ViewportController.V3dView` 取 `V3d_View`；自己 `new V3d_Viewer` 或 `SetWindow` 会与 WPF 宿主窗口冲突。

### 0.1 核对方式

| 来源 | 位置 | 用途 |
| --- | --- | --- |
| 发行 DLL 反射 | `ThirdParty/Macad/*.dll`（4.4.2193.0） | 公开成员、重载、返回类型、枚举值（最终依据） |
| 生成/互操作源码 | `.tmp-macad-src/Source/Macad.Occt`、`Macad.Managed`（C++/CLI） | 确认签名细节与 internal 层 |
| Interaction 源码 | `.tmp-macad-src/Source/Macad.Interaction` | 控制器行为（鼠标/选择语义），仅作参考 |
| XML 文档 | `ThirdParty/Macad/Macad.Occt.xml`（只有 Occt 有文档） | 参数语义 |
| pilot | `RobotSimulator/obj/macad-wpf-pilot/MainWindow.xaml.cs` | 已跑通的调用组合 |

### 0.2 源码 vs 发行 DLL 差异（按 DLL 写）

| 项 | 源码 `.tmp-macad-src` | 发行 DLL 4.4.2193.0 |
| --- | --- | --- |
| `ViewportLayoutManager` | 有（`Viewport/ViewportLayoutManager.cs:18`；`WorkspaceController.cs:42` 暴露） | **无**该类型、无该属性 |
| `ViewportController.SetPredefinedView` | `(ViewUtils.PredefinedView / bool animate=false)`（`:380`/`:421`） | `(ViewportController.PredefinedViews)` 单参 |
| `MouseButtons` 枚举 | 有（`IViewportMouseControl.cs:16`） | **无**（接口用 `System.Windows.Input.MouseButton`） |
| `ViewportMouseControlDefault` | internal | public |
| `TriangulationHelper` | 4 参重载 | 只有 2 参 |

## 1. A. V3d_View（`public class V3d_View : Standard_Transient`）

生产取法：`InteractiveContext.Current.ViewportController.V3dView`。

| 能力 | 发行 DLL 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 绑定窗口 | `void SetWindow(Aspect_Window)`；`SetWindow(Aspect_Window, IntPtr theContext)`；`SetWindow(V3d_View theParentView, Graphic3d_Vec2d theSize[, Aspect_TypeOfTriedronPosition theCorner[, Graphic3d_Vec2d theOffset[, Graphic3d_Vec2i theMargins]]])`（共 6 重载） | ✅ [反射] | **生产不要手动调**，Macad 已绑定 WPF 窗口 |
| 窗口状态 | `Aspect_Window Window()`；`bool IfWindow()`；`bool IsMapped()`（Window 上） | ✅ | 校验 `view.Window()?.IsMapped() == true`（pilot） |
| 立即刷新 | `bool SetImmediateUpdate(bool theImmediateUpdate)` | ✅ 注意返回 bool | [反射] pilot：`SetImmediateUpdate(false)` |
| 刷新 | `void Update()`；`void Redraw()`；`void RedrawImmediate()`；`void Invalidate()`；`bool IsInvalidated()` | ✅ | 属性/矩阵改完 `Redraw()` |
| 投影朝向 | `void SetProj(V3d_TypeOfOrientation)`；`SetProj(V3d_TypeOfOrientation, bool theIsYup)`；`SetProj(double Vx, double Vy, double Vz)` | ✅ | pilot：`SetProj(1,-1,1)`；枚举 `Xpos=0,Ypos=1,Zpos=2,Xneg=3,Yneg=4,Zneg=5,…XnegYnegZneg=25` |
| Up | `void SetUp(V3d_TypeOfOrientation)`；`SetUp(double Vx, double Vy, double Vz)`；`void ResetViewOrientation()` | ✅ | pilot：`SetUp(0,0,1)` |
| FitAll | `void FitAll()`；`FitAll(double theMargin)`；`FitAll(double theMargin, bool theToUpdate)`；`FitAll(Bnd_Box theBox)`；`FitAll(Bnd_Box, double theMargin)`；`FitAll(Bnd_Box, double, bool)`；`FitAll(double minXv, minYv, maxXv, maxYv)` | ✅ | **支持指定包围盒**；pilot：`FitAll(0.08,false)` |
| Z 适配 | `void ZFitAll()`；`void ZFitAll(double theScaleFactor)`；`DepthFitAll()/DepthFitAll(double Aspect[, double Margin])` | ✅ | pilot 在 FitAll 后调 `ZFitAll()` |
| 缩放 | `double Scale()`；`void SetScale(double)`；`void SetZoom(double Coef)`；`SetZoom(double Coef, bool Start)`；`void Zoom(int xp1,yp1,xp2,yp2)`；`StartZoomAtPoint(int,int)`；`ZoomAtPoint(int,int,int,int)` | ✅ | 滚轮缩放可用 `SetZoom(k,true)` 或 `ViewportController.Zoom(pos,delta/200)` |
| 视点 | `void SetCenter(int xp, int yp)`；`SetAt(double X,Y,Z)`；`At(ref double X,Y,Z)`；`SetEye(double X,Y,Z)`；`Eye(ref double X,Y,Z)`；`SetAxis(double X,Y,Z, double Vx,Vy,Vz)` | ✅ | 配合 `Camera()` 精确控制 |
| 相机 | `Graphic3d_Camera Camera()`；`void SetCamera(Graphic3d_Camera)`；`Graphic3d_Camera DefaultCamera()` | ✅ | 正交/透视、fov、eye 等改相机对象（见 2 节） |
| 渲染参数 | `Graphic3d_RenderingParams ChangeRenderingParams()`；`Graphic3d_RenderingParams RenderingParams()` | ✅ | pilot：`ChangeRenderingParams().NbMsaaSamples = 4`（返回引用，可直接改） |
| 背景渐变 | `void SetBgGradientColors(Quantity_Color c1, Quantity_Color c2)`；`+ Aspect_GradientFillMethod theFillStyle`；`+ Aspect_GradientFillMethod, bool theToUpdate`；`void SetBackgroundColor(Quantity_Color)`；`Aspect_GradientBackground GradientBackground()` | ✅ | pilot：`SetBgGradientColors(c1,c2,Aspect_GradientFillMethod.Vertical)`；枚举 `None=0, Horizontal=1, Vertical=2, Diagonal1/2, Corner1..4=5..8, Elliptical=9` |
| 自动环绕 | `void StartRotation(int X, int Y)`；`StartRotation(int X, int Y, double zRotationThreshold)`；`void Rotation(int X, int Y)` | ✅（**无 StopRotation**） | 按下 `StartRotation(w/2,h/2)`，移动 `Rotation(dx,dy)`；停止=不再调用（pilot） |
| 平移 | `void Pan(int theDXp, int theDYp)`；`Pan(int,int,double theZoomFactor)`；`Pan(int,int,double, bool theToStart)` | ✅ | — |
| 尺寸/视距 | `void SetSize(double)`；`void Size(ref double Width, ref double Height)`；`SetZSize(double)`；`double ZSize()`；`SetDepth(double)`；`double Depth()`；`SetFocale(double)`；`double Focale()` | ✅ | WPF 尺寸/DPI 优先用 `ViewportController.ScreenSize` |
| 截图 | `bool Dump(string theFile)`；`bool Dump(string theFile, Graphic3d_BufferType theBufferType)` | ✅ | pilot 用 `Dump(path)`；`Graphic3d_BufferType {RGB=0, RGBA=1, Depth=2, …}` |
| 位图 | `bool ToPixMap(Image_PixMap, int theWidth, int theHeight, Graphic3d_BufferType, bool theToAdjustAspect, int theTargetZLayerId, int theIsSingleLayer, V3d_StereoDumpOptions)` | ✅ | 8 参，无短重载 |
| 坐标换算/拾取 | `void Convert(int Xp, int Yp, ref double X, ref double Y, ref double Z)`；`Convert(double X,double Y,double Z, ref int Xp, ref int Yp)`；`void Project(double x,y,z, ref double xp, ref double yp[, ref double zp])`；`ConvertToGrid(double X,Y,Z, ref Xg,Yg,Zg)`；`ConvertToGrid(int Xp,Yp, ref Xg,Yg,Zg)` | ✅ | 双击取点/把辅助对象放到光标射线的世界坐标 |
| 屏内坐标轴 | `void TriedronDisplay()`；`TriedronDisplay(Aspect_TypeOfTriedronPosition)`；`(+Quantity_Color)`；`(+double theScale)`；`(+V3d_TypeOfVisualization)`；`void TriedronErase()`；`ZBufferTriedronSetup(...)` | ✅ | 枚举 `CENTER=0, TOP=1, BOTTOM=2, LEFT=4, RIGHT=8, LEFT_LOWER=6, RIGHT_LOWER=10, …`；与 AIS_Trihedron 二选一 |
| 着色/模式 | `void SetShadingModel(Graphic3d_TypeOfShadingModel)`；`SetVisualization(V3d_TypeOfVisualization)`；`SetComputedMode(bool)`；`bool ComputedMode()` | ✅ | 着色模型枚举 `Unlit=0, PhongFacet=1, Gouraud=2, Phong=3, Pbr=4, PbrFacet=5, DEFAULT=-1` |
| 视图级灯光 | `void SetLightOn()`；`SetLightOn(Graphic3d_CLight)`；`SetLightOff()`；`SetLightOff(Graphic3d_CLight)`；`bool IsActiveLight(Graphic3d_CLight)`；`V3d_ListOfLight ActiveLights()`；`Graphic3d_CLight ActiveLight()` | ✅ | 一般不用，灯走 Viewer（见 3 节） |
| 访问者 | `V3d_Viewer Viewer()`；`Graphic3d_CView View()` | ✅ | — |
| 不存在 | `StopRotation`、`DetectedPoint`、`SetRenderScale`、SSAO 相关 | ❌ | 见 0 结论 2 |

## 2. B. Graphic3d_Camera（`public class`，无参构造）

| 能力 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 构造 | `new Graphic3d_Camera()` | ✅ | 一般由 `view.Camera()` 取现成的改 |
| 投影类型 | `Graphic3d_Camera.Projection ProjectionType()`；`void SetProjectionType(Projection)`；`bool IsOrthographic()`；`bool IsStereo()`；嵌套枚举 `Projection { Orthographic=0, Perspective=1, Stereo=2, MonoLeftEye=3, MonoRightEye=4 }` | ✅ | **正交/透视切换**：`cam.SetProjectionType(...)` + `view.SetCamera(cam)` + `Redraw()`；或用 `V3d_Viewer.SetDefaultTypeOfView(V3d_TypeOfView ORTHOGRAPHIC=0/PERSPECTIVE=1)` |
| Eye/Center | `Pnt Eye()`；`void SetEye(Pnt)`；`void SetEyeAndCenter(Pnt,Pnt)`；`void MoveEyeTo(Pnt)`；`Pnt Center()`；`void SetCenter(Pnt)` | ✅ | 双击居中：只改 Center/Eye，保留 Direction 与 Distance |
| 朝向 | `Dir Direction()`；`void SetDirection(Dir)`；`SetDirectionFromEye(Dir)`；`Dir Up()`；`void SetUp(Dir)`；`Dir OrthogonalizedUp()`；`void SetIdentityOrientation()` | ✅ | Direction 指向 center；eye = center - direction*distance |
| 比例 | `double Scale()`；`void SetScale(double)`；`double Aspect()`；`void SetAspect(double)`；`double Distance()`；`void SetDistance(double)` | ✅ | 正交相机 Scale 决定可见范围；透视用 FOVy |
| FOV/裁剪 | `double FOVy()`；`void SetFOVy(double)`；`FOVx()/FOV2d()`；`void SetZRange(double zNear, double zFar)`；`double ZNear()/ZFar()` | ✅ | — |
| 适配 | `bool FitMinMax(Bnd_Box, double resolution, bool toEnlargeIfLine)`；`ZFitAll(...)` | ✅ | 一般用 `V3d_View.FitAll(Bnd_Box,...)` 更省事 |
| 变换/失效 | `void Transform(Trsf)`；`void InvalidateProjection()`；`void InvalidateOrientation()`；`Copy/CopyMappingData/CopyOrientationData` | ✅ | 直接改 `view.Camera()` 的属性后调 `view.Redraw()` 即可 |
| 投影计算 | `Project/UnProject/ConvertWorld2View/ConvertView2World`；`OrientationMatrix/ProjectionMatrix` | ✅ | 屏幕拾取优先用 `V3d_View.Convert` |

## 3. C. V3d_Viewer 与灯光

取法：`InteractiveContext.Current.WorkspaceController.V3dViewer`（**不要 new**）。

| 能力 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 默认灯光 | `void SetDefaultLights()` | ✅ [反射/源码] | pilot 先 `SetDefaultLights()` 再 `SetLightOn()` |
| 开关灯 | `void SetLightOn()`；`SetLightOn(Graphic3d_CLight)`；`SetLightOff()`；`SetLightOff(Graphic3d_CLight)`；`void UpdateLights()`；`bool IsGlobalLight(Graphic3d_CLight)` | ✅ | — |
| 增删灯 | `void AddLight(Graphic3d_CLight)`；`void DelLight(Graphic3d_CLight)`；`V3d_ListOfLight ActiveLights()`；`V3d_ListOfLight DefinedLights()` | ✅ | **无 CreateLight**；自己 `new Graphic3d_CLight(type)` 再 AddLight |
| 建灯 | `new Graphic3d_CLight(Graphic3d_TypeOfLightSource theType)`；枚举 `Ambient=0, Directional=1, Positional=2, Spot=3`（别名 AMBIENT/V3d_AMBIENT…） | ✅ | — |
| 灯参数 | `Type()`；`SetColor(Quantity_Color)`；`SetIntensity(float)`；`SetEnabled(bool)`；`SetCastShadows(bool)`；`SetDirection(Dir)`/`SetDirection(double vx,vy,vz)`；`SetPosition(Pnt)`/`SetPosition(double x,y,z)`；`SetAngle(float)`；`SetConcentration(float)`；`SetRange(float)`；`SetAttenuation(float const, float linear)`；`Name()/SetName(TCollection_AsciiString)` | ✅ | — |
| **跟随视角灯光** | `bool IsHeadlight()`；`void SetHeadlight(bool)`；`bool Headlight()` | ✅ | 建一盏 Directional 灯 `SetHeadlight(true)`，方向随相机（正是“跟随视角”）；不要和默认灯重复叠加过曝 |
| 渲染参数默认值 | `Graphic3d_RenderingParams DefaultRenderingParams()`；`void SetDefaultRenderingParams(Graphic3d_RenderingParams)` | ✅ | — |
| 背景默认值 | `SetDefaultBackgroundColor(Quantity_Color)`；`SetDefaultBgGradientColors(c1,c2[, Aspect_GradientFillMethod])`；`Aspect_GradientBackground GetGradientBackground()` | ✅ | 新 view 的默认值；已存在的 view 用 `V3d_View.SetBgGradientColors` |
| 默认视角 | `SetDefaultViewProj(V3d_TypeOfOrientation)`；`SetDefaultViewSize(double)`；`SetDefaultTypeOfView(V3d_TypeOfView)` | ✅ | — |
| 网格 | `ActivateGrid(Aspect_GridType, Aspect_GridDrawMode)`；`DeactivateGrid()`；`Aspect_Grid Grid(...)`；`Ax3 PrivilegedPlane()/SetPrivilegedPlane(Ax3)` | ✅ | Macad 自己有工作平面网格（`Workspace.GridEnabled`），别两套混用 |

## 4. D. AIS 对象：Shape / ColoredShape / Trihedron / Line / Point / TextLabel

| 类型/能力 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 基类变换（所有 AIS 对象） | `PrsMgr_PresentableObject: void SetLocalTransformation(Trsf theTrsf)`；`void SetLocalTransformation(TopLoc_Datum3D theTrsf)`；`Trsf LocalTransformation()`；`Trsf Transformation()`；`void ResetTransformation()`；`bool HasTransformation()` | ✅ [反射] | **逐关节 SetLocalTransformation(Trsf)** 即此重载 |
| 基类外观 | `Prs3d_Drawer Attributes()`；`SetAttributes(Prs3d_Drawer)`；`int DisplayMode()/SetDisplayMode(int)/bool AcceptDisplayMode(int)`；`SetColor(Quantity_Color)/UnsetColor()`；`SetMaterial(Graphic3d_MaterialAspect)`；`SetTransparency(double)`；`SetWidth(double)`；`SetZLayer(int)` | ✅ | — |
| `AIS_Shape` | `new AIS_Shape(TopoDS_Shape)`；`TopoDS_Shape Shape()`；`void SetShape(TopoDS_Shape)`；`void Set(TopoDS_Shape)`；`static TopAbs_ShapeEnum SelectionType(int)`；`static int SelectionMode(TopAbs_ShapeEnum)`；`bool AcceptShapeDecomposition()`；`SetColor(Quantity_Color)`；`SetMaterial(Graphic3d_MaterialAspect)` | ✅ | **从 TopoDS_Shape 构造**——.cad 缓存读出的 BREP 直接喂 |
| 面边界 | 不在 AIS_Shape 上：`obj.Attributes().SetFaceBoundaryDraw(true)`；`obj.Attributes().SetFaceBoundaryAspect(new Prs3d_LineAspect(...))`；`SetupOwnFaceBoundaryAspect()` | ✅ | pilot 对 AIS_ColoredShape 用 `CustomAspects(compound).SetFaceBoundaryAspect(line)` |
| `AIS_ColoredShape` | `new AIS_ColoredShape(TopoDS_Shape)`；`new AIS_ColoredShape(AIS_Shape)`；`void SetCustomColor(TopoDS_Shape theShape, Quantity_Color)`；`SetCustomTransparency(TopoDS_Shape, double)`；`SetCustomWidth(TopoDS_Shape, double)`；`AIS_ColoredDrawer CustomAspects(TopoDS_Shape)`；`void UnsetCustomAspects(TopoDS_Shape[, bool theToUnregister])`；`void ClearCustomAspects()`；`AIS_DataMapOfShapeDrawer CustomAspectsMap()`；另继承 `SetColor/SetMaterial/SetTransparency/SetWidth/SetShape` | ✅ | **按面设色**：`foreach face in TopExp.MapShapes(...)` → `coloredShape.SetCustomColor(face, color)`；面对象取自显示的同一 `TopoDS_Shape` |
| `AIS_ColoredDrawer` | `SetOwnColor(Quantity_Color)/UnsetOwnColor()/bool HasOwnColor()`；`SetOwnTransparency(...)/SetOwnWidth(...)/SetOwnMaterial(...)`；`void SetHidden(bool)`；`bool IsHidden()`；继承 `Prs3d_Drawer`（含面边界 API） | ✅ | 单面级别覆盖（颜色/透明度/线宽/隐藏） |
| `AIS_Trihedron` | `new AIS_Trihedron(Geom_Axis2Placement theComponent)`（**无无参构造**）；`void SetSize(double)`；`double Size()`；`void SetDatumDisplayMode(Prs3d_DatumMode)`（`WireFrame=0/Shaded=1`）；`void SetComponent(Geom_Axis2Placement)`；`SetColor(Quantity_Color)`；`SetTextColor/SetArrowColor/SetOriginColor/SetAxisColor`；`void SetDrawArrows(bool)` | ✅ | 坐标轴对象：`new Geom_Axis2Placement(Pnt P, Dir N, Dir Vx)` 或 `new Geom_Axis2Placement(Ax2 A2)`；也可用屏幕 Triedron（见 1 节） |
| `AIS_Line`（替代 AIS_Segment） | `new AIS_Line(Geom_Point aStartPoint, Geom_Point aEndPoint)`；`new AIS_Line(Geom_Line)`；`Geom_Line Line()`；`void SetPoints(Geom_Point, Geom_Point)`；`void SetLine(Geom_Line)`；`SetColor(Quantity_Color)/SetWidth(double)/UnsetColor()/UnsetWidth()` | ✅ | 焊接光辅助线用它；端点 `new Geom_CartesianPoint(Pnt)` 或 `new Geom_CartesianPoint(x,y,z)` |
| `AIS_Point` | `new AIS_Point(Geom_Point)`；`void SetComponent(Geom_Point)`；`SetColor(Quantity_Color)`；`bool AcceptDisplayMode(int)` | ✅ | — |
| `AIS_TextLabel`（替代 AIS_Text） | `new AIS_TextLabel()`；`void SetText(TCollection_ExtendedString)`；`void SetPosition(Pnt)`；`SetColor(Quantity_Color)`；`SetHeight(double)`；`SetFont(string)`；`SetZoomable(bool)`；`SetHJustification(Graphic3d_HorizontalTextAlignment)`；`SetVJustification(...)`；`SetOrientation3D(Ax2)`；`bool HasOrientation3D()` | ✅ | `TCollection_ExtendedString` 有 `ctor(string)`，直接 `new TCollection_ExtendedString("txt")` |
| `AIS_Segment` | — | ❌ 不存在（源码也没有） | 用 `AIS_Line` |
| `AIS_Text` | — | ❌ 不存在 | 用 `AIS_TextLabel` |
| `AIS_ViewCube` | 类型存在（`Macad.Occt.AIS_ViewCube`） | ✅（可选） | 若要 ViewCube 可一试，pilot 未用 |

## 5. E. AIS_InteractiveContext（`workspace.AisContext`）

| 能力 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 显示 | `void Display(AIS_InteractiveObject, int theDispMode, int theSelectionMode, bool theToUpdateViewer)`（另有 4 个扩展重载）；`void Display(AIS_InteractiveObject, bool theToUpdateViewer)` | ✅ | pilot：`Display(part, 1, -1, false)`（1=着色，-1=默认选择模式） |
| 显示模式 | `int DisplayMode()`；`void SetDisplayMode(AIS_InteractiveObject, int, bool)`；`void SetDisplayMode(int, bool)`；`void UnsetDisplayMode(AIS_InteractiveObject, bool)` | ✅ | `AIS_DisplayMode { WireFrame=0, Shaded=1 }` |
| 显示/隐藏 | `void Erase(AIS_InteractiveObject, bool theToUpdateViewer)`；`void EraseAll(bool)`；`void DisplayAll(bool)`；`bool IsDisplayed(AIS_InteractiveObject[, int theMode])`；`PrsMgr_DisplayStatus DisplayStatus(AIS_InteractiveObject)` | ✅ | **显示隐藏切换 = Display / Erase**；保留对象用 Remove 才删 |
| 重绘 | `void Redisplay(AIS_InteractiveObject, bool[, bool theAllModes])`；`void Update(AIS_InteractiveObject, bool)`；`RecomputePrsOnly(...)`；`RecomputeSelectionOnly(obj)` | ✅ | 改完形状后 `Redisplay(obj,false)`；整体刷新用 `view.Redraw()` |
| 删除 | `void Remove(AIS_InteractiveObject, bool theToUpdateViewer)`；`void RemoveAll(bool)` | ✅ | 场景重建时 RemoveAll |
| 自动选择 | `void SetAutoActivateSelection(bool)`；`bool GetAutoActivateSelection()` | ✅ | pilot 关掉（自管交互） |
| 样式 | `SetColor(AIS_InteractiveObject, Quantity_Color, bool theToUpdateViewer)`；`SetMaterial(obj, Graphic3d_MaterialAspect, bool)`；`SetTransparency(obj, double, bool)`；`SetWidth(obj, double, bool)`；`SetDisplayPriority(obj, Graphic3d_DisplayPriority)`；`SetZLayer(obj, int)`；`SetViewAffinity(obj, V3d_View, bool)` | ✅ | 批量改色优先用对象自身 API |
| 悬停检测 | `AIS_StatusOfDetection MoveTo(int theXPix, int theYPix, V3d_View, bool theToRedrawOnUpdate)`；`MoveTo(Ax1, V3d_View, bool)`；`bool HasDetected()`；`SelectMgr_EntityOwner DetectedOwner()`；`AIS_InteractiveObject DetectedInteractive()`；`TopoDS_Shape DetectedShape()`；`bool HasDetectedShape()`；枚举 `AIS_StatusOfDetection { Error=0, Nothing=1, AllBad=2, Selected=3, OnlyOneDetected=4, OnlyOneGood=5, SeveralGood=6 }` | ✅ | **双击取点**：`MoveTo` → `DetectedShape()` + `view.Convert(xp,yp,ref X,ref Y,ref Z)` |
| 选择 | `AIS_StatusOfPick Select(bool)`；`Select(int xmin,ymin,xmax,ymax, V3d_View, bool)`；`SelectDetected()`；`SelectPoint(Graphic3d_Vec2i, V3d_View)`；`SelectRectangle(Graphic3d_Vec2i, Graphic3d_Vec2i, V3d_View[, AIS_SelectionScheme])`；`SelectPolygon(TColgp_Array1OfPnt2d, V3d_View)`；`AddSelect(...)`；枚举 `AIS_StatusOfPick { Error=0, NothingSelected=1, Removed=2, OneSelected=3, SeveralSelected=4 }` | ✅ | — |
| 选中查询 | `FirstSelectedObject()`；`SelectedOwner()`；`SelectedInteractive()`；`int NbSelected()`；`bool IsSelected(...)`；`void ClearSelected(bool)`；`Bnd_Box BoundingBoxOfSelection()` | ✅ | `BoundingBoxOfSelection` 可用于“适配选中零件”（`FitAll(box,0.1,false)`） |
| 不存在 | `DetectedPoint` / `GetDetectedPoint` | ❌ | 用 `V3d_View.Convert` 或 `AisHelper.PickFromContext`（见 8 节） |

## 6. F. WorkspaceController / ViewportController / ViewportPanel / AirspaceOverlay

### 6.1 WorkspaceController（`public`，无 public 构造；取 `InteractiveContext.Current.WorkspaceController`）

| 成员 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 视口 | `Viewport ActiveViewport {get;set;}`；`ViewportController ActiveViewControlller {get;}`（**三个 l 的拼写**）；`ViewportController GetViewController(int viewIndex)`；`GetViewController(Viewport)` | ✅ [反射] | 多视口时按索引取 |
| 底层 | `V3d_Viewer V3dViewer {get;set;}`；`AIS_InteractiveContext AisContext {get;set;}` | ✅ | 本清单所有 A/C/E 的入口 |
| 状态 | `Workspace Workspace`；`bool LockWorkingPlane`；`SelectionManager Selection`；`bool IsSelecting {get;set;}`；`VisualObjectManager VisualObjects`；`Nullable<Pnt> CursorPosition`；`Nullable<Pnt2d> CursorPosition2d`；`Editor CurrentEditor`；`Tool CurrentTool`；`double VisualGridMultiplier` | ✅ | — |
| 鼠标入口 | `void MouseMove(ViewportController, Point pos, ModifierKeys)`；`void MouseDown(ViewportController, ModifierKeys)`；`void MouseUp(ViewportController, ModifierKeys)` | ✅ | WPF 事件由 Macad 的 ViewportPanel 转入，生产不必重接 |
| 框选/选择 | `SelectByRectangle(int[] corners, bool includeTouched, ViewportController)`；`SelectByPolyline(List<Point>, bool, ViewportController)`；`CancelSelection()`；`UpdateSelection()` | ✅ | — |
| 刷新/坐标 | `Invalidate(bool immediateOnly, bool forceRedraw)`；`ComputeGridPoint(Pnt2d, bool)` | ✅ | — |
| 事件 | **无 public 事件** | ⚠️ | 源码仅 `event EventHandler Disposed`；视口变化事件在 `Macad.Core.Viewport.ViewportChanged` |
| ViewportLayoutManager | — | ❌ 发行 DLL 无 | 源码有（`Viewport/ViewportLayoutManager.cs:18`）；多视口布局直接用 XAML + `ViewportPanel` |

### 6.2 ViewportController（`public`，无 public 构造；取 `workspace.GetViewController(i)` 或 `InteractiveContext.Current.ViewportController`）

| 成员 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 属性 | `WorkspaceController WorkspaceController {get;}`；`Viewport Viewport {get;}`；`V3d_View V3dView {get;set;}`；`bool LockedToPlane`；`bool IsInRubberbandSelection`；`ValueTuple<int,int> ScreenSize {get;}`；`double DpiScale {get;set;}`；`double PixelSize`；`double GizmoScale` | ✅ | **无 GetSize()**，尺寸用 `ScreenSize`（`(int width,int height)`） |
| 标准视角 | `void SetPredefinedView(ViewportController.PredefinedViews predefinedView)` | ✅（DLL 单参） | 枚举 `Top=0, Bottom=1, Left=2, Right=3, Front=4, Back=5, WorkingPlane=6`；源码另有两个带 `bool animate=false` 的重载，**DLL 没有** |
| 视图操作 | `void Zoom(Point pos, double value)`；`void Zoom(double value)`；`void ZoomFitAll()`；`void ZoomFit(Bnd_Box boxToFit)`；`void ZoomFitSelected()`；`void Rotate(double yawDeg, double pitchDeg, double rollDeg)`；`void Pan(double dX, double dY)`；`void PanToCenter(Pnt)` | ✅ | 滚轮缩放/自动环绕可复用 `Zoom`/`Rotate`，不必自己算欧拉角 |
| 屏幕↔世界 | `bool ScreenToPoint(int screenX, int screenY, out Pnt)`；`bool ScreenToPoint(Pln plane, int, int, out Pnt)`；`bool PointToScreen(Pnt, out int x, out int y)`；`Ax1 ScreenToViewAxis(int, int)` | ✅ | 双击命中点/放置标注 |
| 鼠标/框选 | `void MouseMove(Point, ModifierKeys, MouseMoveMode)`；`MouseMove(ModifierKeys)`；`MouseDown(ModifierKeys)`；`MouseUp(ModifierKeys)`；`void StartRubberbandSelection(RubberbandSelectionMode mode, bool includeTouched, Nullable<Point> position)` | ✅ | — |
| 尺寸/其它 | `void Resize()`；`Bitmap RenderToBitmap(uint width, uint height)`；`void Dispose()` | ✅ | 截图走 `RenderToBitmap` 或 `V3d_View.Dump` |
| 枚举 | `MouseMoveMode { None=0, Panning=1, Rotating=2, Twisting=3, Zooming=4 }`；`PredefinedViews`（见上）；`RubberbandSelectionMode { Rectangle=0, Freehand=1 }` | ✅ | — |

### 6.3 ViewportPanel（`Macad.Interaction.Panels.ViewportPanel : FrameworkElement`）

| 成员 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| 鼠标控制 | `public IViewportMouseControl MouseControl {get;set;}`；`static DependencyProperty MouseControlProperty`；默认 `new ViewportMouseControlDefault()` | ✅ | **替换/包装 `IViewportMouseControl` 即可自定义滚轮/双击语义**，内部转给 `ViewportController.MouseMove/MouseDown/MouseUp` |
| 叠加层 | `public object OverlayContent {get;set;}`；`OverlayContentProperty`；`TouchControlProperty`；`IViewportTouchControl TouchControl`；`ViewportPanelModel Model {get;}` | ✅ | HUD/标注可走 OverlayContent |
| `IViewportMouseControl` | `ViewportController ViewportController {set;}`；`void MouseMove(Point pos, MouseDevice, ModifierKeys)`；`void MouseWheel(Point pos, Macad.Interaction.Panels.MouseWheel wheel, int delta, InputDevice, ModifierKeys)`；`void MouseDown(Point pos, System.Windows.Input.MouseButton changedButton, int clickCount, MouseDevice, ModifierKeys)`；`void MouseUp(Point, System.Windows.Input.MouseButton, MouseDevice, ModifierKeys)`；`void Cancel()` | ✅ | 鼠标键类型是 **WPF 的 MouseButton** |
| `MouseWheel` | `enum Macad.Interaction.Panels.MouseWheel { Vertical=0, Horizontal=1 }` | ✅ | — |
| `MouseButtons` | — | ❌ 发行 DLL 无 | 源码有（`IViewportMouseControl.cs:16`），别引用 |
| 默认行为 [源码] | Vertical 滚轮 → `viewport.Zoom(pos, delta/200.0)`（Ctrl 时 delta/=10）；Horizontal → `viewport.Rotate(delta/-50.0, 0, 0)`；左键拖拽 >10px 且 `IsSelecting` → `StartRubberbandSelection`；双击 → `WorkspaceCommands.StartEditing` | [源码] | 生产要“双击居中”，建议替换 MouseControl 或直接接管 MouseDown 的 `ClickCount==2` |
| AirspaceOverlay | `Macad.Presentation.AirspaceOverlay : Border`，`[ContentProperty("OverlayChild")]`；`Thickness ClippingBorder {get;set;}`；`object OverlayChild {get;set;}`；`void SetWindowEnabled(bool)` | ✅ | overlay API 公开可用；**DPI 已在内部处理**（`AirspaceOverlay.cs:201-206` 用 `VisualTreeHelper.GetDpi` 折算透明输入窗位置/尺寸），外部不要重复乘 DPI |

## 7. G. 几何/颜色/样式类型（Macad.Occt，public struct）

| 类型 | 需要用的签名 | 状态 | 注意 |
| --- | --- | --- | --- |
| `Trsf` | `new Trsf()`；`void SetRotation(Ax1 A1, double Ang)`；`void SetTranslationPart(Vec V)`；`void Multiply(Trsf)`；`Trsf Multiplied(Trsf)`；`Trsf Inverted()`；`static Trsf Identity`；`void Transforms(ref double X, ref double Y, ref double Z)` | ✅ | `Multiply` 原地乘（this*T），`Multiplied` 返回新值；世界矩阵 = `parentWorld.Multiplied(local)` |
| `Ax1` | `new Ax1(Pnt P, Dir V)`；`Pnt Location`；`Dir Direction`；`static Ax1 OX/OY/OZ` | ✅ | 关节旋转轴 |
| `Ax2` / `Ax3` | `new Ax2(Pnt P, Dir N, Dir Vx)`；`new Ax3(Pnt, Dir[, Dir Vx])`；`static Ax2 XOY/YOZ/ZOX` | ✅ | AIS_Trihedron/TextLabel 朝向 |
| `Pnt` / `Dir` / `Vec` | `new Pnt(double x,y,z)`；`new Dir(x,y,z)`；`new Vec(x,y,z)`；`Pnt.Translated(Vec)`；`Vec.ToPnt()`；`Dir.ToVec([double scale])`；`Pnt.Transformed(Trsf)` | ✅ | 相机 eye/center 都是 `Pnt` |
| `Quantity_Color` | `new Quantity_Color(double c1, double c2, double c3, Quantity_TypeOfColor theType)`；`new Quantity_Color(Quantity_NameOfColor)`；`SetValues(double,double,double,Quantity_TypeOfColor)`；`Values(ref double, ref double, ref double, Quantity_TypeOfColor)`；`Red()/Green()/Blue()`；`static Quantity_Color ColorFromHex(string, Quantity_Color)` | ✅ | 枚举 `Quantity_TypeOfColor { RGB=0, sRGB=1, HLS=2, CIELab=3, CIELch=4 }`；**sRGB 颜色必须走 sRGB 枚举**，否则渐变/面色偏亮 |
| `Srgb` | — | ❌ 不存在 | 自建 helper（pilot 第 319 行：`new Quantity_Color(r,g,b,Quantity_TypeOfColor.sRGB)`） |
| `Aspect_TypeOfLine` | 枚举 `SOLID=0, DASH=1, DOT=2, DOTDASH=3, USERDEFINED=4`（`EMPTY=-1`） | ✅ | 面边界/辅助线线型 |
| `Prs3d_LineAspect` | `new Prs3d_LineAspect(Quantity_Color theColor, Aspect_TypeOfLine theType, double theWidth)`；`new Prs3d_LineAspect(Graphic3d_AspectLine3d)`；`SetColor/SetTypeOfLine/SetWidth` | ✅ | pilot 用它画面边界 |
| `Graphic3d_MaterialAspect` | `new Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial theName)`；`new Graphic3d_MaterialAspect()`；`Name()/RequestedName()`；`SetColor(Quantity_Color)`；`SetTransparency(float)`；`SetShininess(float)`；`SetAmbientColor/SetDiffuseColor/SetSpecularColor/SetEmissiveColor`；`PBRMaterial()/SetPBRMaterial` | ✅ | pilot：`new Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial.Plastified)` |
| `Graphic3d_NameOfMaterial` | 枚举，常用 `Plastified`、`Plastic`、`ShinyPlastic`、`Transparent`；另有全大写别名（`PLASTIC=6` 等） | ✅ | — |

## 8. H. Macad.Managed（C++/CLI）与 BREP/STEP 缓存

| 能力 | 签名 | 状态 | 备注 |
| --- | --- | --- | --- |
| **BREP 二进制读** | `Macad.Occt.Helper.BRepExchange.ReadBinary(byte[]) -> TopoDS_Shape`；`WriteBinary(TopoDS_Shape, bool includeTriangles) -> byte[]` | ✅ [反射] | **.cad 缓存首选**：`File.ReadAllBytes(path)` → `ReadBinary`；不依赖 BinTools |
| BREP ASCII | `BRepExchange.ReadAscii(byte[]) / WriteAscii(TopoDS_Shape, bool)` | ✅ | 若缓存是文本 BREP |
| BinTools | `bool BinTools.Read(TopoDS_Shape theShape, string theFile)`；`bool Write(TopoDS_Shape, string[, bool withTriangles, bool withNormals, BinTools_FormatVersion])` | ✅ | 需先 `var s = new TopoDS_Shape()` 再传入 |
| BRepTools | `bool BRepTools.Read(TopoDS_Shape Sh, string File, BRep_Builder B)`；`Write(...)`；`Triangulation(TopoDS_Shape, double linDefl)` | ✅ | — |
| **STEP 读（替代方案）** | `new StepReader()`；`bool ReadFromFile(string path)`；`TopoDS_Shape GetRootShape()`；`Dispose()` | ✅ | 发行 DLL **无 public STEPControl_Reader**（只有 internal struct）；若 .cad 缓存实际是 STEP，用这个 |
| STEP 写 | `new StepWriter()`；`bool AddSolid(TopoDS_Shape)`；`bool WriteToFile(string path)`；`Dispose()` | ✅ | 回写缓存用 |
| 三角化 | `TriangulationData TriangulationHelper.GetTriangulation(TopoDS_Shape brepShape, bool getNormals)` | ✅ **只有 2 参** | 4 参重载不存在（源码有）；`TriangulationData: TriangleCount/Normals/Vertices/Indices` |
| 面枚举 | `TopExp.MapShapes(TopoDS_Shape, TopAbs_ShapeEnum, TopTools_IndexedMapOfShape)`；`map.FindKey(int)`、`map.Size()`、`map.Contains(shape)`、`map.FindIndex(shape)` | ✅ | 按面设色的面列表来源 |
| 复合体 | `var c = new TopoDS_Compound()`；`new BRep_Builder().MakeCompound(c)`；`builder.Add(c, shape)`（`Add(TopoDS_Shape S, TopoDS_Shape C)`，**子在前**） | ✅ | 多零件合并/分批设色 |
| 包围盒 | `var b = new Bnd_Box()`；`BRepBndLib.Add(TopoDS_Shape, Bnd_Box[, bool useTriangulation])`；`b.Get(ref xmin,ref ymin,ref zmin,ref xmax,ref ymax,ref zmax)`；`b.IsVoid()` | ✅ | 直接喂 `view.FitAll(b, 0.08, false)` |
| 拾取助手 | `AisHelper.PickFromContext(AIS_InteractiveContext, int x, int y, V3d_View, out AIS_InteractiveObject, out TopoDS_Shape) -> bool`；`GetShapeFromEntityOwner(SelectMgr_EntityOwner)`；`SetRubberbandPoints(WNT_Window, AIS_RubberBand, List<Point>)` | ✅ [反射] | 双击取点/框选最省事 |
| 驱动 | `Macad.Occt.Helper.Graphic3d.CreateOpenGlDriver(bool enableDebug) -> Graphic3d_GraphicDriver` | ✅ | **不要自建**，用 Macad 已建好的 viewer |

## 9. 目标能力 → API 落地映射（IRobotSceneService 的 Macad 实现）

| 需求 | 直接用法 | 风险 |
| --- | --- | --- |
| 加载 .cad 缓存 BREP | `BRepExchange.ReadBinary(File.ReadAllBytes(path))` → `new AIS_ColoredShape(shape)` → `ctx.Display(shape, 1, -1, false)` | 缓存实际格式（BREP/Macad 自定义）需对齐写入端 |
| 按面设色 | `TopExp.MapShapes(shape, TopAbs_ShapeEnum.FACE, map)` → `acs.SetCustomColor(map.FindKey(i), color)` | 面必须来自同一 `TopoDS_Shape` |
| 逐关节变换 | `var t = new Trsf(); t.SetRotation(new Ax1(pnt, dir), rad); t.SetTranslationPart(new Vec(x,y,z)); node.SetLocalTransformation(parentWorld.Multiplied(t));` | Trsf 只有旋转+平移+等比缩放，无剪切/非均匀缩放 |
| 相机适配（整体/选中） | `view.FitAll(margin,false)` + `view.ZFitAll()`；或 `BRepBndLib.Add(shape, box)` + `view.FitAll(box,0.08,false)`；或 `vpc.ZoomFitAll()/ZoomFit(box)` | — |
| 标准视角 | `view.SetProj(V3d_TypeOfOrientation.Zpos); view.SetUp(0,0,1);` 或 `vpc.SetPredefinedView(ViewportController.PredefinedViews.Front)` | DLL 单参，别写 animate |
| 自动环绕 | `view.StartRotation(w/2,h/2)`（按下）→ `view.Rotation(dx,dy)`（移动） | 无 StopRotation，靠不再调用停止 |
| 滚轮缩放 | `vpc.Zoom(pos, delta/200.0)` 或 `view.SetZoom(delta>0?1.1:1/1.1, true)` | `SetZoom` 的 `Start` 参数：每个滚轮步进传 true |
| 双击居中 | `AisHelper.PickFromContext(ctx,x,y,view,out obj,out shape)` → `view.Convert(x,y,ref X,ref Y,ref Z)` → `cam.SetEyeAndCenter(new Pnt(X,Y,Z).Translated(cam.Direction().ToVec(-cam.Distance())), new Pnt(X,Y,Z))` → `view.SetCamera(cam); view.Redraw()` | 不要用不存在的 DetectedPoint |
| MSAA 画质档 | `view.ChangeRenderingParams().NbMsaaSamples = 2/4/8`（性能/均衡/高清），极致档再设 `RenderResolutionScale = 1.5` | **无 SSAO 字段**；OIT 透明度用 `OitDepthFactor` |
| 背景渐变 | `view.SetBgGradientColors(c1, c2, Aspect_GradientFillMethod.Vertical, true)` | 颜色用 sRGB 构造 |
| 跟随视角灯光 | `var l = new Graphic3d_CLight(Graphic3d_TypeOfLightSource.Directional); l.SetHeadlight(true); l.SetDirection(...); l.SetIntensity(1f); viewer.AddLight(l); viewer.SetLightOn();` | 先 `SetDefaultLights()` 会重置灯光集，顺序要对 |
| 坐标轴 | `view.TriedronDisplay(Aspect_TypeOfTriedronPosition.LEFT_LOWER)`（屏幕角标）或 `new AIS_Trihedron(new Geom_Axis2Placement(Pnt.Origin, new Dir(0,0,1), new Dir(1,0,0)))` + `SetSize` + `ctx.Display` | Trihedron 无无参构造 |
| 焊接光辅助 | `new AIS_Line(new Geom_CartesianPoint(p1), new Geom_CartesianPoint(p2))` + `SetColor/SetWidth`；点/文字用 `AIS_Point`/`AIS_TextLabel` | 无 AIS_Segment |
| 显示/隐藏切换 | `ctx.Erase(obj,false)` / `ctx.Display(obj,false)` / `ctx.IsDisplayed(obj)` | 要彻底删才用 Remove |

## 10. 风险与注意事项

1. **发行 DLL 是唯一编译依据**：`ViewportLayoutManager`、`MouseButtons`、`SetPredefinedView(..., animate)`、`TriangulationHelper` 4 参重载都只在源码里，用错即编译失败。
2. **不要手动 `SetWindow` / 自建 `V3d_Viewer` / `Graphic3d_GraphicDriver`**：宿主已绑定（`ViewportController.V3dView`、`WorkspaceController.V3dViewer`），重复绑定会黑屏/崩溃。
3. **`V3d_View.SetImmediateUpdate` 返回 `bool`**（不是 void），照抄 OCCT C++ 习惯会编译错。
4. **sRGB**：所有从设计稿/HEX 来的颜色用 `Quantity_TypeOfColor.sRGB`；项目内没有 `Srgb` 类型，需自建 helper（可放在 Macad 服务实现里）。
5. **无 SSAO**：该 OCCT 版本 `Graphic3d_RenderingParams` 没有 SSAO 字段；画质档只做 MSAA + 分辨率缩放（`RenderResolutionScale` 默认 1，极致档 1.5），别找 SSAO 开关。
6. **`DetectedPoint` 不存在**：双击取屏幕点必须走 `V3d_View.Convert` 或 `AisHelper.PickFromContext`。
7. **AIS 类型名字易错**：`AIS_TextLabel`（非 AIS_Text）、`AIS_Line`（非 AIS_Segment）、`ActiveViewControlller`（三个 l，DLL 原名）。
8. **默认双击是 `WorkspaceCommands.StartEditing`**：最终实现直接使用原生鼠标控制，不再替换 `ViewportPanel.MouseControl`，因此没有双击居中。
