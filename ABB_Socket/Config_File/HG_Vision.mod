MODULE HG_Vision
	VAR wobjdata Model_2:=[FALSE,TRUE,"",[[695.763,-308.932,242.802],[0.688146,0.238317,-0.632091,0.264803]],[[0,0,0],[1,0,0,0]]];
	VAR wobjdata Model_1:=[FALSE,TRUE,"",[[0,0,0],[1,0,0,0]],[[0,0,0],[1,0,0,0]]];
   ! PERS tooldata too11OSPR:=[TRUE,[[-150,0,388],[1,0,0,0]],[6,[-150,0,0],[1,0,0,0],0,0,0]];



    PROC HE_Calibration_Demo()
        VAR robtarget Calib_Result:=[[652.44,-645.95,424.36],[0.000307345,-0.370347,-0.928893,-0.000160461],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];
        VAR robtarget p20:=[[652.42,-645.93,424.32],[0.000325347,-0.37035,-0.928892,-0.000170594],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];
        MoveJ [[652.46,-645.95,424.38],[0.000298076,-0.37034,-0.928896,-0.000155272],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v400,z50,too11OSPR;
        HG_HandEye_Calib HE_Calib_Start_Model;
        MoveL [[663.03,-637.33,290.32],[0.000312806,-0.370348,-0.928893,-0.000167541],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[663.03,-595.69,290.32],[0.000311708,-0.370346,-0.928894,-0.00016641],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[634.94,-627.84,290.31],[0.00030815,-0.370359,-0.928889,-0.000168251],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[634.94,-627.84,290.31],[0.130319,0.366702,0.919702,-0.0519162],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[707.24,-651.98,285.13],[0.220565,-0.361993,-0.901832,-0.0837295],[-1,-1,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[730.51,-739.43,248.04],[0.235061,-0.595201,-0.768165,-0.0201067],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[660.09,-752.24,248.04],[0.0320826,-0.641937,-0.713524,-0.278876],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[591.41,-790.65,281.87],[0.0555422,0.861645,0.442148,0.242876],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[534.97,-804.04,276.93],[0.0797587,0.905952,0.342304,0.236047],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[659.13,-754.41,276.91],[0.0682756,-0.700733,-0.65192,-0.281622],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[648.13,-772.09,300.63],[0.0682701,-0.700732,-0.651925,-0.281616],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[648.13,-678.97,273.65],[0.160704,-0.434676,-0.852696,-0.241124],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_Progress_Model;
        MoveL [[652.46,-645.95,424.38],[0.000296716,-0.370339,-0.928897,-0.000154652],[-1,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]],v200,fine,too11OSPR;
        HG_HandEye_Calib HE_Calib_End_Model\Calib_Pos:=Calib_Result;
        MoveL Calib_Result,v20,fine,too11OSPR;
    ENDPROC


    PROC HE_Creation_Model_Demo()
        HG_Creation_Model Model_2;
    ENDPROC

ENDMODULE