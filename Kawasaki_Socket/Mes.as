.INTER_PANEL_D
0,9,1,2,15
7,8,"spot_size","  光圈大小","",10,4,2,2,0
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
.PROGRAM mes_int() #0; #0; #37520; Mes初始化
;数据初始化
  work_a_state = FALSE;
  work_b_state = FALSE;
  work_c_state = FALSE;
  work_d_state = FALSE;
.END
.PROGRAM mes_main() #0; #0; #0; 通信主程序
; max_length = 255
;tout_rec = 5
; eret = 0
  sock_id = 0
; ret = 0
; text_id = 0
;数据初始化
  CALL mes_open_socket;连接通信
;  IF sock_id>0 THEN
;;;;   CALL tcp_match(.data)
;  CALL tcp_close_socke;断开通信
;END
.END
.PROGRAM mes_open_socket() #0; #0; #0;开始通信
  .er_count = 0
  .port = 60000
  .ip[1] = 192
  .ip[2] = 168
  .ip[3] = 146
  .ip[4] = 1
  port_no[0] = 0
  sock_idd[0] = 0
  err_cd[0] = 0
  sub_cd[0] = 0
  $ip_adrs[0] = ""
;connect:
  TCP_CONNECT sock_id,.port,.ip[1],1;超时1秒
  TCP_STATUS sock_id,port_no[0],sock_idd[0],err_cd[0],sub_cd[0],$ip_adrs[0]
  IF sock_id<0 THEN
    PRINT "TCP_CONNECT error id=",sock_id
  ELSE
    PRINT "TCP_CONNECT TYPE=",sock_id
  END
.END
.REALS
err_cd[0] = 0
port_no[0] = 60000
sock_id = 0
sock_idd[0] = 424
sub_cd[0] = 0
.END
.STRINGS
$ip_adrs[0] = "192.168.146.1"
.END
