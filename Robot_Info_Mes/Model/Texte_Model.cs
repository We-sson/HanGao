using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_Info_Mes.Model
{
    /// <summary>
    /// 为界面提供一段可绑定、可通知变更的通用文本；当前默认用作应用标题。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Texte_Model
    {
        /// <summary>
        /// 创建文本模型。属性变更通知由 PropertyChanged.Fody 在编译期织入。
        /// </summary>
        public Texte_Model()
        {
        }

        /// <summary>
        /// 界面显示的文本内容。
        /// </summary>
        public string Texte { get; set; } = "Robot_Info_Mes";
    }
}
