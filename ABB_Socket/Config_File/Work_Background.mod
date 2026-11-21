MODULE Work_Background
    PERS bool Work_A_1:=FALSE;
    PERS bool Work_A_2:=FALSE;
    PERS bool Work_B_1:=FALSE;
    PERS bool Work_B_2:=FALSE;
    PERS bool Work_C_1:=FALSE;
    PERS bool Work_C_2:=FALSE;

    PROC main()
        Work_A_1:=FALSE;
        Work_A_2:=FALSE;
        Work_B_1:=FALSE;
        Work_B_2:=FALSE;
        Work_C_1:=FALSE;
        Work_C_2:=FALSE;

        WHILE TRUE DO
            IF DNDI02_A_1=1 and Work_A_1=false THEN
                Work_A_1:=TRUE;
                TPWrite "Work_A_1:="+ValToStr(Work_A_1);
            ELSEIF DNDI03_A_2=1 and Work_A_2=false THEN
                Work_A_2:=TRUE;
                TPWrite "Work_A_2:="+ValToStr(Work_A_2);
            ELSEIF DNDI04_B_1=1 and Work_B_1=false THEN
                Work_B_1:=TRUE;
                TPWrite "Work_B_1:="+ValToStr(Work_B_1);
            ELSEIF DNDI05_B_2=1 and Work_B_2=false THEN
                Work_B_2:=TRUE;
                TPWrite "Work_B_2:="+ValToStr(Work_B_2);
            ELSEIF DNDI06_C_1=1 and Work_C_1=false THEN
                Work_C_1:=TRUE;
                TPWrite "Work_C_1:="+ValToStr(Work_C_1);
            ELSEIF DNDI07_C_2=1 and Work_C_2=false THEN
                Work_C_2:=TRUE;
                TPWrite "Work_C_2:="+ValToStr(Work_C_2);
            ENDIF

            WaitTime 0.05;
        ENDWHILE

    ENDPROC
ENDMODULE