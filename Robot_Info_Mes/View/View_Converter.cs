using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Robot_Info_Mes.View
{
    public  class View_Converter
    {
    }




    /// <summary>
    /// 拓展标记枚举转换方法
    /// </summary>
    public class EnumBindingSourceExtension : MarkupExtension
    {


        public Type? Enum_List { set; get; }

        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new Exception("类型必须是枚举类型");
            Enum_List = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            //返回枚举集合
            return Enum.GetValues(Enum_List!).Cast<Enum>();

        }


    }

}
