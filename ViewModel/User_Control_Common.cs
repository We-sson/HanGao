using GalaSoft.MvvmLight;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 悍高软件.Model;

namespace 悍高软件.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class User_Control_Common : ViewModelBase
    {


        /// <summary>
        /// 把功能状态写入自己泛型中
        /// </summary>
        public static void User_Check_Write_List(int W)
        {

            foreach (Sink_Models it in List_Show.SinkModels)
            {
                if (W == 1)
                {


                    if (it.Model_Number.ToString() == User_Control_Working_VM_1.WM.Work_Type)
                    {

                        it.User_Check_1.Work_Pause = User_Control_Working_VM_1.WM.Work_Pause;
                        it.User_Check_1.Work_Connt = User_Control_Working_VM_1.WM.Work_Connt;
                        it.User_Check_1.Work_NullRun = User_Control_Working_VM_1.WM.Work_NullRun;
                        it.User_Check_1.Work_JumpOver = User_Control_Working_VM_1.WM.Work_JumpOver;

                        return;
                    }
                }
                else if (W == 2)
                {

                    if (it.Model_Number.ToString() == User_Control_Working_VM_2.WM.Work_Type)
                    {

                        it.User_Check_2.Work_Pause = User_Control_Working_VM_2.WM.Work_Pause;
                        it.User_Check_2.Work_Connt = User_Control_Working_VM_2.WM.Work_Connt;
                        it.User_Check_2.Work_NullRun = User_Control_Working_VM_2.WM.Work_NullRun;
                        it.User_Check_2.Work_JumpOver = User_Control_Working_VM_2.WM.Work_JumpOver;

                        return;
                    }



                }

            }
        }


        public  void User_Features_OnOff_Log(bool? IsCheck,int User,string Fea)
        {
            if (IsCheck == true)
            {
                User_Control_Log_ViewModel.User_Log_Add("NO."+ User .ToString()+ Fea+"功能开启");
            }
            else if (IsCheck == false)
            {
                User_Control_Log_ViewModel.User_Log_Add("NO." + User.ToString() + Fea + "功能关闭");

            }


        }

    }
}
