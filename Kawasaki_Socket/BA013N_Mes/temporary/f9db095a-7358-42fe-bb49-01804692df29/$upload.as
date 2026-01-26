.PROGRAM autostart5.pc() #0; 通信主程序
  port = 60000
  ip[1] = 192
  ip[2] = 168
  ip[3] = 44
  ip[4] = 1
  max_length = 255
  tout_open = 60
  tout_rec = 60
  eret = 0
  ret = 0
; sock_id = 0
  sock_stat = 0
  CALL mes_int
  CALL mes_protocol
  WHILE (1) DO
    CALL open_socket; 连接通信
;PAUSE
  END
.END
.PROGRAM close_err() #6;
  TYPE "关闭错误！"
  RETURNE
.END
.PROGRAM close_socket() #3;断开通信程序
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
.PROGRAM com_test() #0; 根据指示数据进行各处理
  CASE text_id OF
   VALUE 0: ; 无指示
    GOTO break_exit
   VALUE 1: ; 指示处理 1
; 各处理
   VALUE 2: ; 指示处理 2
; 各处理
   VALUE 3: ; 指示处理 3
; 各处理
   ANY :
    GOTO break_exit
  END
  $end_buf[1] = $recv_buf[1]+" OK"
  CALL send(eret,$end_buf[1]); 发送结束处理
break_exit:
.END
.PROGRAM main() #0; 通信主程序
  port = 60000
  ip[1] = 192
  ip[2] = 168
  ip[3] = 44
  ip[4] = 1
  max_length = 255
  tout_open = 60
  tout_rec = 60
  eret = 0
  ret = 0
  CALL open_socket; 连接通信
;IF sock_id<0 THEN
;  GOTO exit_end
;END
  text_id = 0
  tout = 60
  WHILE (1) DO
    ret = 0
    CALL recv; 从 PC接收指示数据
    IF ret<0 THEN
      PRINT "Communication error  code=",ret
      GOTO exit
    ELSE
      CASE ret OF
       VALUE 0:
        text_id = VAL($MID($recv_buf[1],1,3)) ; 读取指示号
       ANY :
      END
    END
    IF text_id>0 THEN
      CALL com_test; 各指示的处理
      IF eret<0 THEN
        GOTO exit
      END
    END
  END
exit:
  CALL close_socket; 断开通信
exit_end:
.END
.PROGRAM mes_int() #0; Mes初始化
;数据初始化
  work_a_state = FALSE;
  work_b_state = FALSE;
  work_c_state = FALSE;
  work_d_state = FALSE;
.END
.PROGRAM mes_protocol() #0
  .$mes_type = "Mes_Info_Date"
  .$robot_type = "川崎"
  .$robot_mode = "T1"
  .$robot_programs = "Main"
  .$robot_process = "Panel_Welding_1"
  $mes_send_data = ""
  $mes_send_data = .$mes_type+","+.$robot_type+","+.$robot_mode+","+.$robot_programs+","+.$robot_process+","+$ENCODE(work_a_state,",",work_b_state,",",work_c_state,",",work_d_state)
  PAUSE
.END
.PROGRAM open_socket() #48;通讯连接程序
;*****************************************
;* FUNCTION: 通讯连接程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_STATUS 返回值，端口号，套接字号，错误代码，错误子代码，IP地址
  DO
    CALL vis_disconnect
    ONE sock_err
    TCP_CONNECT sock_id,port,ip[1],tout_open
;PAUSE
    IF sock_id<0 THEN
      .$ret = "通讯连接错误，通信序号="+$ENCODE(sock_id)
      TYPE .$ret
    END
    IF sock_id==0 THEN
      .$ret = "通讯连接对象无回应！"
      TYPE .$ret
    END
    TWAIT 2
;PAUSE
  UNTIL sock_id>0
  TYPE "机器人与服务器通讯连接成功!,Socket_ID=",sock_id
.END
.PROGRAM recv() #0; 通信 接收数据
  .num = 0
  TCP_RECV ret,sock_id,$recv_buf[1],.num,tout_rec,max_length
  IF ret<0 THEN
    PRINT "TCP_RECV error in RECV",ret
    $recv_buf[1] = "000"
  ELSE
    IF .num>0 THEN
      PRINT "TCP_RECV OK  in RECV",ret
    ELSE
      $recv_buf[1] = "000"
    END
  END
.END
.PROGRAM send(.ret,.$data) #0; 通信 发送数据
  $send_buf[1] = .$data
  buf_n = 1
  .ret = 1
send_rt:
  TCP_SEND sret,sock_id,$send_buf[1],buf_n,tout
  IF sret<0 THEN
    .ret = -1
    PRINT "TCP_SEND error in SEND",sret
  ELSE
    PRINT "TCP_SEND OK  in SEND",sret
  END
.END
.PROGRAM sock_err() #3;
  sock_error = 0
  sock_error = ERROR
  ONE close_err
  CALL vis_disconnect
  TYPE "通信错误，通讯已关闭！"
;PRINT $ERROR(ERROR)
;PRINT "错误信息：",sock_error
;PAUSE
  RETURNE
.END
.PROGRAM vis_disconnect() #1903;清除sockets
  sock_cnt = 0
  TCP_STATUS sock_cnt,port_no[0],sock_ids[0],err_cd[0],sub_cd[0],$ip_adrs[0]
;PAUSE
  FOR .i = 0 TO sock_cnt-1
    IF port_no[.i]==port THEN
      sock_id = sock_ids[.i]
      TCP_CLOSE .err1,sock_id
      PRINT "断开连接状态！IP:",$ip_adrs[.i]
;PAUSE
      IF .err1<0 THEN
        PRINT "Error: TCP_CLOSE Fault, code ",.err1
      ELSE
        PRINT "清除Socket全部连接状态！"
      END
    END
  END
  TCP_STATUS sock_cnt,port_no[0],sock_ids[0],err_cd[0],sub_cd[0],$ip_adrs[0]
;PAUSE
.END
.REALS
buf_n = 1
eret = 0
err_cd[0] = 0
ip[1] = 192
ip[2] = 168
ip[3] = 44
ip[4] = 1
max_length = 255
port = 60000
port_no[0] = 60000
ret = 0
sock_close = 0
sock_cnt = 0
sock_error = 0
sock_id = 0
sock_ids[0] = 408
sock_stat = 0
sret = 0
stat_err[0] = 0
stat_port[0] = 60000
stat_sock[0] = 416
stat_sub[0] = 0
sub_cd[0] = 0
text_id = 0
tout = 60
tout_open = 60
tout_rec = 60
.END
.STRINGS
$end_buf[1] = "1 OK"
$ip_add[0] = "192.168.44.1"
$ip_adrs[0] = "192.168.44.1"
$recv_buf[1] = "000"
$send_buf[1] = "1 OK"
.END
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
