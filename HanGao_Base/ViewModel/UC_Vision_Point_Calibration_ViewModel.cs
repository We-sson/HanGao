
namespace HanGao.ViewModel
{
    [AddINotifyPropertyChangedInterface]
     public  class UC_Vision_Point_Calibration_ViewModel: ObservableRecipient
    {

        public UC_Vision_Point_Calibration_ViewModel()
        {


            Calibration_Results_List = new ObservableCollection<Calibration_Results_Model>()
            {
                new Calibration_Results_Model()
                {
                     Number=1,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=2,
                      X=12.121,
                      Y=232.235
                }, 
                new Calibration_Results_Model()
                {
                     Number=3,
                      X=12.121,
                      Y=232.235
                }, 
                new Calibration_Results_Model()
                {
                     Number=4,
                      X=12.121,
                      Y=232.235
                }, 
                new Calibration_Results_Model()
                {
                     Number=5,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=6,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=7,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=8,
                      X=12.121,
                      Y=232.235
                },
                new Calibration_Results_Model()
                {
                     Number=9,
                      X=12.121,
                      Y=232.235
                },


            };

        }





        public ObservableCollection<Calibration_Results_Model> Calibration_Results_List { set; get; } = new ObservableCollection<Calibration_Results_Model>() { };









    }


   


    public class Calibration_Results_Model
    {
        public int Number { get; set; }
        public  double X { set; get; }
        public double Y { set; get; }

    }
}
