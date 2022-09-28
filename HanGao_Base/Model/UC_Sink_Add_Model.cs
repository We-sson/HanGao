

namespace HanGao.Model
{
    [AddINotifyPropertyChangedInterface]
    public class UC_Sink_Add_Model
    {
        public UC_Sink_Add_Model()
        {

        }







    }


    [AddINotifyPropertyChangedInterface]
    public class UI_Sink_Add_Data_Model
    {
        public UI_Sink_Add_Data_Model()
        {

        }

        private bool _UI_Sink_LeftRight_Checked;

        public bool UI_Sink_LeftRight_Check
        {
            get { return _UI_Sink_LeftRight_Checked; }
            set { _UI_Sink_LeftRight_Checked = value; }
        }

        private bool _UI_Sink_UpDowm_Checked;

        public bool UI_Sink_UpDowm_Checked
        {
            get { return _UI_Sink_UpDowm_Checked; }
            set { _UI_Sink_UpDowm_Checked = value; }
        }

        private bool _UI_Sink_Two_Checked;

        public bool UI_Sink_Two_Checked
        {
            get { return _UI_Sink_Two_Checked; }
            set { _UI_Sink_Two_Checked = value; }
        }






    }
}
