using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Assimp;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using PropertyChanged;
using Robot_3D.Model;
using SharpAssimp;
using SharpDX.Direct3D9;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using Color = HelixToolkit.Maths.Color;


namespace Robot_3D.ViewMode
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel:ObservableRecipient
    {

        public MainViewModel()
        {


            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

        
           EffectsManager = new DefaultEffectsManager();   
               
        


            //Camera =new Camera();
            Camera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera()
            {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
                Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = 5000,
                NearPlaneDistance = 0.1f
            };


        }






        public EffectsManager? EffectsManager { get; set; }

        public HelixToolkit.Wpf.SharpDX.Camera? Camera { get; set; } 



        //public ObservableElement3DCollection Robot_Group { get; set; } = new ObservableElement3DCollection();


        public SceneNodeGroupModel3D GroupModel { get; set; } = new SceneNodeGroupModel3D();

        public Robot_Group Robot_Group { set; get; }=new Robot_Group();



        public HelixToolkit.Maths.BoundingBox ModelBound { set; get; } = new();


        public Point3D ModelCentroid { set; get; } = default;


        private void FocusCameraToScene()
        {
            if (Camera is null)
            {
                return;
            }

            var maxWidth = Math.Max(Math.Max(ModelBound.Width, ModelBound.Height), ModelBound.Depth);
            var pos = ModelBound.Center + new Vector3(0, 0, maxWidth);
            Camera.Position = pos.ToPoint3D();
            Camera.LookDirection = (ModelBound.Center - pos).ToVector3D();
            Camera.UpDirection = Vector3.UnitY.ToVector3D();

            if (Camera is HelixToolkit.Wpf.SharpDX.OrthographicCamera orthCam)
            {
                orthCam.Width = maxWidth;
            }
        }




        /// <summary>
        ///服务器启动停止按钮
        /// </summary>
        public ICommand Save_Date_File_Comm
        {
            get => new RelayCommand<RoutedEventArgs>((Sm) =>
            {
                Button? _Contol = Sm!.Source as Button;
                try
                {



                    Int_Read_Robot_3D();


                }
                catch (Exception _e)
                {

                    MessageBox.Show("保存设置参数失败！原因：" + _e.Message);

                }



            });
        }



        public void Add_Robot_Model(SceneNode Node)
        {

          var node_name = Node.Name.Split('|');

            if (node_name.Length ==5)
            {

            foreach (var string_name in node_name)
            {




                switch (  Enum.Parse<Robot_Model_Type>(string_name))
                {
                    case Robot_Model_Type.Base:





                        continue;
                 


                    case Robot_Model_Type.J1:

                        break;
                    case Robot_Model_Type.J2:

                        break;
                    case Robot_Model_Type.J3:

                        break;
                    case Robot_Model_Type.J4:

                        break;
                    case Robot_Model_Type.J5:

                        break;
                    case Robot_Model_Type.J6:

                        break;

                }




            }


            }
        }







        public void Int_Read_Robot_3D()
        {

            Importer import = new();


            //默认材质
            PhongMaterialCore defaultMaterial = new PhongMaterialCore
            {

                // 白
                DiffuseColor = Color.Red,
                AmbientColor = Color.Red,
                ReflectiveColor = Color.Red,
                EmissiveColor = Color.Red,
                SpecularColor = Color.Red,
                //AmbientColor = new Color4(0.2f, 0.2f, 0.2f, 1f),
                //EmissiveColor = new Color4(0.1f, 0.1f, 0.1f, 1f),  // 轻微发光
                //SpecularColor = new Color4(0.8f, 0.8f, 0.8f, 1f),
                //SpecularShininess = 150f

                EnableFlatShading = true,
            };




            var _Path = AppDomain.CurrentDomain.BaseDirectory + "\\Resources" + "\\1111.obj";


            var scene = import.Load(_Path) ?? throw new Exception($"3D模型加载失败: {_Path}");
          
            //var loader = new Importer();
            //var scene = loader.Load(_Path) ?? throw new Exception($"3D模型加载失败: {_Path}");



            scene.Root.Attach(EffectsManager);

            scene.Root.UpdateAllTransformMatrix();



            scene.Root.TryGetBound(out var bound);
            ModelBound = bound;
            scene.Root.TryGetCentroid(out var centroid);
            ModelCentroid = centroid.ToPoint3D();



            foreach (var node in scene.Root.Traverse())
            {


                


                if (node is MaterialGeometryNode m)
                {
                    //m.Geometry.SetAsTransient();
                    if (m.Material is PBRMaterialCore pbr)
                    {
                        pbr.RenderEnvironmentMap = false;
                        pbr.EnableFlatShading = true ;
                    }
                    else if (m.Material is PhongMaterialCore phong)
                    {
                        phong.RenderEnvironmentMap = false;
                        phong.SpecularShininess = 50;
                        phong.SpecularColor = Colors.Gray.ToColor4();
                        phong.EnableFlatShading = true ;
                    }
                    //    RenderEnvironmentMap = RenderEnvironmentMap
                    //};

                    //// 强制替换，不管原来是什么材质，全部变红
                    //m.Material = redMaterial;
                }




                //Robot_Group.Add();
                Add_Robot_Model(node);


            }




           // Robot_Group.Clear();


            GroupModel.AddNode(scene.Root);





            FocusCameraToScene();
            //viewPort3d.Items.Add(Robot_Group);


            ///viewPort3d.ZoomExtents();
        }








    }


}
