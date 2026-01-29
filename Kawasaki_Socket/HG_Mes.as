.INTER_PANEL_D
.END
.INTER_PANEL_TITLE
"",0
"",0
"",0
"",0
"",0
"",0
"",0
"",0
.END
.INTER_PANEL_COLOR_D
182,3,224,244,28,159,252,255,251,255,0,31,2,241,52,219,
.END
.PROGRAM autostart5.pc() #0;通信主程序
  port = 60000
  ip[1] = 192
  ip[2] = 168
  ip[3] = 44
  ip[4] = 1
; sock_id = 0
; sock_stat = 0
  work_a_state = FALSE;
  work_b_state = FALSE;
  work_c_state = FALSE;
  work_d_state = FALSE;
  sock_connt = FALSE
;CALL mes_protocol
  WHILE (1) DO
    CALL open_socket; 连接通信
;PAUSE
    CALL mes_protocol
;PAUSE
    CALL send($mes_send_data)
;PAUSE
    CALL recv
  END
.END
.PROGRAM close_err() #0
  TYPE "关闭错误！"
  RETURNE
.END
.PROGRAM close_socket() #0;断开通信程序
;*****************************************
;* FUNCTION: 套接字关闭程序              *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_CLOSE  返回值，套接字号
  ONE close_err
  sock_close = 0
  TCP_CLOSE sock_close,sock_id
;PAUSE
  IF sock_close<0 THEN
    .$ret = $ENCODE(sock_close)
    TYPE "通讯错误，错误代码=",.$ret
    TCP_CLOSE sock_close,sock_id;强制关闭套接字
  ELSE
; IFPWPRINT 1,2,10,4,10 = "通讯关闭"
    TYPE "通讯关闭"
;sock_id = 0
    TWAIT 1
  END
.END
.PROGRAM mes_main() #0
  work_a_state = FALSE;
  work_b_state = FALSE;
  work_c_state = FALSE;
  work_d_state = FALSE;
  work_a_state = TRUE
  TWAIT 5
  work_a_state = FALSE
  TWAIT 1
  work_c_state = TRUE;
  TWAIT 1
  work_c_state = FALSE;
.END
.PROGRAM mes_protocol() #492
  $mes_type = "Mes_Info_Data"
  $robot_type = "Kawasaki"
  .$a_state = ""
  .$b_state = ""
  .$c_state = ""
  .$d_state = ""
  IF SWITCH(ERROR )==0 THEN
    IF SWITCH(REPEAT )==-1 AND SWITCH(CS )==-1 AND SWITCH(POWER )==-1 AND SWITCH(ERROR )==0 THEN
      $robot_mode = "Run"
    ELSE
      $robot_mode = "T1"
    END
  ELSE
    $robot_mode = "Error"
  END
;PAUSE
  $robot_programs = "Main"
  $robot_process = "Panel_Welding_1"
  IF (work_a_state==TRUE) THEN
    .$a_state = "TRUE"
  ELSE
    .$a_state = "FALSE"
  END
  IF (work_b_state==TRUE) THEN
    .$b_state = "TRUE"
  ELSE
    .$b_state = "FALSE"
  END
  IF (work_c_state==TRUE) THEN
    .$c_state = "TRUE"
  ELSE
    .$c_state = "FALSE"
  END
  IF (work_d_state==TRUE) THEN
    .$d_state = "TRUE"
  ELSE
    .$d_state = "FALSE"
  END
  $mes_send_data = ""
  $mes_send_data = $mes_type+":"+$robot_type+","+$robot_mode+","+$robot_programs+","+$robot_process+","+.$a_state+","+.$b_state+","+.$c_state+","+.$d_state
;PAUSE
.END
.PROGRAM open_socket() #492;通讯连接程序
;*****************************************
;* FUNCTION: 通讯连接程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_STATUS 返回值，端口号，套接字号，错误代码，错误子代码，IP地址
  .tout_open = 10
  IF sock_connt==FALSE THEN
    DO
;  CALL vis_disconnect
      ONE sock_err
      TCP_CONNECT sock_id,port,ip[1],.tout_open
;PAUSE
      IF sock_id<0 THEN
        .$ret = "通讯连接错误，通信序号="+$ENCODE(sock_id)
        TYPE .$ret
      END
      IF sock_id==0 THEN
        .$ret = "通讯连接对象无回应！"
        TYPE .$ret
      END
      TWAIT 1
;PAUSE
      TCP_STATUS sock_cnt,port_no[0],sock_ids[0],err_cd[0],sub_cd[0],$ip_adrs[0]
    UNTIL (sock_id>0) AND (sock_cnt>0)
    TYPE "机器人与服务器通讯连接成功!,Socket_ID=",sock_id
    sock_connt = TRUE
  END
.END
.PROGRAM recv() #491;通信接收数据
  .tout_rec = 2
  .max_length = 255
  .num = 0
  $recv_buf[0] = ""
  .ret = 0
  .$recv[0] = ""
  ONE sock_err
;TCP_RECV 返回值，套接字号，接收字符串数组，元素数，接收超时时间，最大字节数
  TCP_RECV .ret,sock_id,.$recv[0],.num,.tout_rec,.max_length
;PRINT .ret,sock_id,$recv_buf[0],.num,.tout_rec,.max_length
;PAUSE
  IF (.ret<0) OR (.num==0) THEN
    PRINT "数据接收超时，错误代码="+$ENCODE(.ret)
    CALL sock_err
  ELSE
    PRINT "数据接收成功!",VAL(.$recv[0],2)
  END
.END
.PROGRAM send(.$data) #492;数据发送程序
;*****************************************
;* FUNCTION: 数据发送程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
  $send_buf[0] = .$data
  .buf_n = 1
  .tout_send = 2
;TCP_SEND 返回值，套接字号，发送字符串数组，元素数，发送超时时间
  ONE sock_err
  TCP_SEND sret,sock_id,$send_buf[0],.buf_n,.tout_send
;PAUSE
  IF sret<0 THEN
    .$sret = "数据接收超时，错误代码="+$ENCODE(sret)
    TYPE .$sret
    CALL sock_err
  ELSE
    TYPE "数据发送成功",.$data
  END
.END
.PROGRAM sock_err() #0
  sock_error = 0
  sock_error = ERROR
  sock_cnt = 0
  TCP_STATUS sock_cnt,port_no[0],sock_ids[0],err_cd[0],sub_cd[0],$ip_adrs[0]
;PAUSE
  FOR .i = 0 TO sock_cnt-1
    IF port_no[.i]==port THEN
      sock_id = sock_ids[.i]
      ONE close_err
      TCP_CLOSE .err1,sock_id
      PRINT "断开连接状态！IP=",$ip_adrs[.i]
;PAUSE
      IF .err1<0 THEN
        PRINT "Error: TCP_CLOSE Fault, code ",.err1
      ELSE
        PRINT "清除Socket全部连接状态！"
      END
    END
  END
  TCP_STATUS sock_cnt,port_no[0],sock_ids[0],err_cd[0],sub_cd[0],$ip_adrs[0]
  sock_connt = FALSE
;PAUSE
  RETURNE
.END
.REALS
err_cd[0] = 0
ip[1] = 192
ip[2] = 168
ip[3] = 44
ip[4] = 1
port = 60000
port_no[0] = 60000
sock_cnt = 1
sock_connt = -1
sock_error = 0
sock_id = 368
sock_ids[0] = 368
sret = 0
sub_cd[0] = 0
work_a_state = 0
work_b_state = 0
work_c_state = 0
work_d_state = 0
.END
.STRINGS
$ip_adrs[0] = "192.168.44.1"
$mes_send_data = "Mes_Info_Data:Kawasaki,T1,Main,Panel_Welding_1,FALSE,FALSE,FALSE,FALSE"
$mes_type = "Mes_Info_Data"
$recv_buf[0] = ""
$robot_mode = "T1"
$robot_process = "Panel_Welding_1"
$robot_programs = "Main"
$robot_type = "Kawasaki"
$send_buf[0] = "Mes_Info_Data:Kawasaki,T1,Main,Panel_Welding_1,FALSE,FALSE,FALSE,FALSE"
.END
