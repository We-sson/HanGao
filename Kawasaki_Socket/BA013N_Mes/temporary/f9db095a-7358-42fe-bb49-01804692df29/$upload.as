.PROGRAM autostart1.pc() #0; #0; #0; #0; #0; #0; #0; 通信主程序
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
  TWAIT 1
  CALL close_socket; 断开通信
.END
.PROGRAM autostart5.pc() #0; #0; #0; #0; #0; #0; #0; 通信主程序
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
  sock_id = 0
  sock_stat = 0
  WHILE (1) DO
    CALL open_socket; 连接通信
    TWAIT 2
    CALL close_socket; 断开通信
  END
.END
.PROGRAM close_err() #0; #180; #21; #109; #748; #0; #0
;sock_error = 0
;sock_error = ERROR
  TYPE "关闭错误！"
  RETURNE
.END
.PROGRAM close_socket() #0; #0; #3; #24; #17;断开通信程序
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
; IFPWPRINT 1,2,10,4,10 = "通讯错误，错误代码=",.$ret
    TCP_CLOSE sock_close,sock_id;强制关闭套接字
  ELSE
; IFPWPRINT 1,2,10,4,10 = "通讯关闭"
    TYPE "通讯关闭"
  END
.END
.PROGRAM com_test() #0; #0; #0; #0; #0; #0; #4; 根据指示数据进行各处理
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
.PROGRAM main() #0; #0; #0; #0; #0; #0; #0; 通信主程序
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
.PROGRAM mes_close_socke() #0; #0; #0; #0; #0; #0; #38402;断开通信
  TCP_CLOSE ret,sock_id;正常的套接字关闭
  IF ret<0 THEN
    PRINT "TCP_CLOSE OK id=",ret1
  ELSE
    sock_id = 0
  END
.END
.PROGRAM mes_int() #0; #0; #0; #0; #0; #0; #0; #0; #37520; Mes初始化
;数据初始化
  work_a_state = FALSE;
  work_b_state = FALSE;
  work_c_state = FALSE;
  work_d_state = FALSE;
.END
.PROGRAM mes_main() #0; #0; #0; #0; #0; #0; #0; 通信主程序
; max_length = 255
;tout_rec = 5
; eret = 0
  sock_id = 0
; ret = 0
; text_id = 0
;数据初始化
  WHILE (TRUE) DO
    CALL mes_open_socket;连接通信
    TWAIT 1
    CALL mes_close_socke;断开通信
  END
;  IF sock_id>0 THEN
;;;;   CALL tcp_match(.data)
;  CALL tcp_close_socke;断开通信
;END
.END
.PROGRAM mes_open_socket() #0; #0; #0; #0; #0; #0; #101; #0; #0;开始通信
  .er_count = 0
  .ret = 0
  .port = 60000
  .ip[1] = 192
  .ip[2] = 168
  .ip[3] = 44
  .ip[4] = 1
;connect:
;sock_id= TCP_CONNECT (sock_id,.port,.ip[1]);超时1秒
  IF sock_id<0 THEN
    PRINT "TCP_CONNECT error id=",sock_id
  END
.END
.PROGRAM open_socket() #0; #0; #4; #24; #11;通讯连接程序
;*****************************************
;* FUNCTION: 通讯连接程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_STATUS 返回值，端口号，套接字号，错误代码，错误子代码，IP地址
  TCP_STATUS sock_stat,stat_port[0],stat_sock[0],stat_err[0],stat_sub[0],$ip_add[0]
;PAUSE
  IF sock_stat>0 THEN ;检查连接状态
    ONE sock_err
    CALL close_socket
  END
;connect
  DO
;TCP_CONNECT 返回值，端口号，IP地址,连接超时时间
    ONE sock_err
    TCP_CONNECT sock_id,port,ip[1],tout_open
;PAUSE
    IF sock_id<0 THEN
      .$ret = "通讯连接错误，通信序号="+$ENCODE(sock_id)
; IFPWPRINT 1,2,10,4,10=.$ret
      TYPE .$ret
    END
    IF sock_id==0 THEN
      .$ret = "通讯连接对象无回应！"
; IFPWPRINT 1,2,10,4,10=.$ret
      TYPE .$ret
    END
    TWAIT 2
;PAUSE
  UNTIL sock_id>0
;IFPWPRINT 1,2,10,4,10="机器人与服务器通讯连接成功"
  TYPE "机器人与服务器通讯连接成功!,Socket_ID=",sock_id
.END
.PROGRAM recv() #0; #0; #0; #0; #0; #0; #257168; 通信 接收数据
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
.PROGRAM send(.ret,.$data) #0; #0; #0; #0; #0; #0; #4; 通信 发送数据
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
.PROGRAM sock_err() #0; #180; #20; #109; #748; #0; #0
  sock_error = 0
  sock_error = ERROR
  ONE close_err
  TCP_CLOSE sock_close,sock_id
  TYPE "通信错误，通讯已关闭！"
;PRINT $ERROR(ERROR)
;PRINT "错误信息：",sock_error
;PAUSE
  RETURNE
.END
.REALS
buf_n = 1
eret = 0
ip[1] = 192
ip[2] = 168
ip[3] = 44
ip[4] = 1
max_length = 255
port = 60000
ret = 0
sock_close = 0
sock_error = 0
sock_id = 0
sock_stat = 0
sret = 0
stat_err[0] = 0
stat_port[0] = 60000
stat_sock[0] = 284
stat_sub[0] = 0
text_id = 0
tout = 60
tout_open = 60
tout_rec = 60
.END
.STRINGS
$end_buf[1] = "1 OK"
$ip_add[0] = "192.168.44.1"
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
