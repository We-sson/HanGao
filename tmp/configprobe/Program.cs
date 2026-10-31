using Robot_Info_Mes.Model;

var model = File_Xml_Model.Read_Xml_File<File_Int_Model>();
var simulation = model.Mes_Digital_Simulation;
Console.WriteLine($"Robot={simulation.Robot_Model} Tool={simulation.Tool_Model} Work={simulation.Work_Model}");
Console.WriteLine($"Axes base={simulation.Show_Base_Axes} flange={simulation.Show_Flange_Axes} tcp={simulation.Show_Tcp_Axes}");
