MODULE MainModule
    TASK PERS tooldata too11OSPR:=[TRUE,[[235.169,1.34148,301.574],[0.612151,0.00153081,-0.790737,0.0019774]],[6.5,[200,5,186],[1,0,0,0],0,0,0.094]];
    CONST robtarget p10:=[[414.77,484.12,433.97],[0.0684014,-0.177859,0.933975,0.302288],[0,0,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];
    LOCAL PERS tooldata p0:=[TRUE,[[0,0,0],[1,0,0,0]],[-1,[0,0,0],[1,0,0,0],0,0,0]];
    VAR tooldata p1:=[TRUE,[[0,0,0],[1,0,0,0]],[-1,[0,0,0],[1,0,0,0],0,0,0]];
    PERS loaddata load1:=[6.5,[-172,-7.4,10.8],[1,0,0,0],0,0,0];
    VAR signalgo GO8_10;
    PERS bool Work_A_1:=FALSE;
    PERS bool Work_A_2:=FALSE;
    PERS bool Work_B_1:=FALSE;
    PERS bool Work_B_2:=FALSE;
    PERS bool Work_C_1:=FALSE;
    PERS bool Work_C_2:=FALSE;
    PERS bool Work_A_state:=FALSE;
    PERS bool Work_B_state:=FALSE;
    PERS bool Work_C_state:=FALSE;
    PERS bool Work_D_state:=FALSE;
    TASK PERS welddata Weld_duijiaao:=[40,0,[0,0,2,0,0,0,0,0,0],[0,0,2,0,0,0,0,0,0]];
    CONST robtarget R_Home:=[[353.47,-355.72,586.26],[0.878153,0.120413,0.280633,-0.368231],[-1,-1,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];

    PROC main()
        VAR tooldata p2:=[TRUE,[[0,0,0],[1,0,0,0]],[-1,[0,0,0],[1,0,0,0],0,0,0]];
        Work_A_1:=FALSE;
        Work_A_2:=FALSE;
        Work_B_1:=FALSE;
        Work_B_2:=FALSE;
        Work_C_1:=FALSE;
        Work_C_2:=FALSE;
        Reset DNDO02_A_jia;
        Reset DNDO03_B_jia;

        WHILE True DO
            Work_A_state:=FALSE;
            Work_B_state:=FALSE;
            Work_C_state:=FALSE;
            Work_D_state:=FALSE;
            MoveJ R_Home,v500,fine,too11OSPR;
            IF Work_A_1=TRUE THEN
                Reset DNDO02_A_jia;
                Reset DNDO03_B_jia;
                Work_A_state:=TRUE;
                !-------------------------------
                A_93_L;


                Reset DNDO02_A_jia;
                Reset DNDO03_B_jia;
                Work_A_1:=FALSE;
                Work_A_state:=FALSE;


            ELSEIF Work_A_2=TRUE THEN
                Reset DNDO02_A_jia;
                Reset DNDO03_B_jia;
                Work_B_state:=TRUE;

                !----------------------
                A_93_R_902181;
                !A_93_R_902446;
                !A_93_R_902000;
                !A_93_R_902266;
                !A_93_R_956014;
                !A_93_R_902426;
                !A_93_R_940019;
                !A_93_R_940030;
                !A_93_R_902429;
                !A_93_R_952154;
                !A_93_R_952212;
                !A_93_R_952128;
                !A_93_R_952127;
                !-----------------------
                Work_A_2:=FALSE;
                Work_B_state:=FALSE;


                Reset DNDO02_A_jia;
                Reset DNDO03_B_jia;
            ELSEIF Work_B_1=TRUE THEN
                Work_C_state:=TRUE;

                !-----------------------
                A_90_L;

                Work_B_1:=FALSE;
                Work_C_state:=FALSE;


            ELSEIF Work_B_2=TRUE THEN
                Work_D_state:=TRUE;

                !-------------------------------
                !A_90_R_952666;
                !A_90_R_952496;
                !A_90_R_952644;
                !A_90_R_952648;
                !A_90_R_952376;
                !A_90_R_952478;
                !A_90_R_952493;
                !A_90_R_952575;
                !A_90_R_952475;
                !A_90_R_952497;
                !A_90_R_952481;
                !A_90_R_952373;
                !A_90_R_952033;
                !A_90_R_952494;
                !A_90_R_952472;
                !A_90_R_952618;
                A_90_R_952503;
                
                !-------------------------------

                Work_B_2:=FALSE;
                Work_D_state:=FALSE;


            ELSEIF Work_C_1=TRUE THEN
                !-------------------------------

                !-------------------------------

                Work_C_1:=FALSE;

            ELSEIF Work_C_2=TRUE THEN
                !-------------------------------

                !-------------------------------

                Work_C_2:=FALSE;

            ENDIF


            WaitTime 0.05;
        ENDWHILE
    ENDPROC


ENDMODULE