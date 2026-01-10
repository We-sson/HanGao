.PROGRAM autostart3.pc() #0; 激光头设置
; *******************************************************************
;
; Program:      lsset.pc
; Comment:      激光头设置
; Author:       User
;
; Date:         2023/2/2
;
; *******************************************************************
;
  .size = 1
  WHILE (1) DO
    IF spot_size<>.size THEN
      FOR .i = 1 TO 10
        CALL tcp_main(spot_size)
      END
    END
    .size = spot_size
    TWAIT 0.1
  END
.END
.PROGRAM spot_data(.size) #48; 设置光圈大小
; *******************************************************************
;
; Program:      spot_data
; Comment:      设置光圈大小
; Author:       User
;
; Date:         2023/2/4
;
; *******************************************************************
;
  spot_size = .size
  BITS 2010,10 = 0
  RETURN
.END
.PROGRAM tcp_close_socke() #38300;断开通信
  TCP_CLOSE ret,sock_id;正常的套接字关闭
  IF ret<0 THEN
    PRINT "TCP_CLOSE OK id=",ret1
  ELSE
    sock_id = 0
  END
.END
.PROGRAM tcp_main(.data) #37520; 通信主程序
  port = 20480
  ip[1] = 192
  ip[2] = 168
  ip[3] = 1
  ip[4] = 200
  max_length = 255
  tout_open = 5
  tout_rec = 5
  eret = 0
  sock_id = 0
  ret = 0
  text_id = 0
;数据初始化
  CALL tcp_open_socket;连接通信
  IF sock_id>0 THEN
    CALL tcp_match(.data)
    CALL tcp_close_socke;断开通信
  END
.END
.PROGRAM tcp_match(.data) #37375; 计算内容
  mudbus_number = mudbus_number+1;发送数据序号
  IF mudbus_number>65535 THEN
    mudbus_number = 1
  END
  modbus_protocol = 0;协议码（固定）
  modbus_len = 6;后续数据长度（固定）
  modbus_id = 1;设备ID号
  modbus_code = 6;协议功能码
  modbus_address = 94;设备地址号
;modbus_data = 13;设备设置数据
  .$send_buf[1] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,mudbus_number),1,2))))
  .$send_buf[2] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,mudbus_number),3,4))))
  .$send_buf[3] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,modbus_protocol),1,2))))
  .$send_buf[4] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,modbus_protocol),3,4))))
  .$send_buf[5] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,modbus_len),1,2))))
  .$send_buf[6] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,modbus_len),3,4))))
  .$send_buf[7] = $CHR(VAL($ENCODE("^H"+$ENCODE(/J2,modbus_id))))
  .$send_buf[8] = $CHR(VAL($ENCODE("^H"+$ENCODE(/J2,modbus_code))))
  .$send_buf[9] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,modbus_address),1,2))))
  .$send_buf[10] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,modbus_address),3,4))))
  .$send_buf[11] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,.data*10),1,2))))
  .$send_buf[12] = $CHR(VAL($ENCODE("^H"+$MID($ENCODE(/J4,.data*10),3,4))))
;Val($ENCODE("^H"+$MID($ENCODE(/J4,410),3,4)))
  CALL tcp_send(.$send_buf[])
.END
.PROGRAM tcp_open_socket() #38470;开始通信
  .er_count = 0
connect:
  TCP_CONNECT sock_id,port,ip[1],tout_open
  IF sock_id<0 THEN
    IF .er_count>=5 THEN
      PRINT "不能与 PC 连接。停止程序。"
    ELSE
      .er_count = .er_count+1
      PRINT "TCP_CONNECT error id=",sock_id," error count=",.er_count
      GOTO connect
    END
  ELSE
    PRINT "TCP_CONNECT OK id=",sock_id
  END
.END
.PROGRAM tcp_recv() #0;通信 接收数据
  .num = 0
  ret = 0;接收状态
  TCP_RECV ret,sock_id,$recv_buf[1],.num,tout_rec,max_length
  IF ret<0 THEN
    PRINT "TCP_RECV error in RECV",ret
    $recv_buf[1] = ""
  ELSE
    IF .num>0 THEN
      PRINT "TCP_RECV OK in RECV",ret,$recv_buf[1]
    ELSE
      $recv_buf[1] = ""
    END
  END
.END
.PROGRAM tcp_send(.$data[]) #37375; 通信 发送数据
  .buf_n = 12
  .tout = 60
  TCP_SEND sret,sock_id,.$data[1],.buf_n,.tout
  IF sret<0 THEN
    PRINT "TCP_SEND error in SEND",sret
  ELSE
    PRINT "TCP_SEND OK !"
  END
;.n = MAXINDEX (".data")
; FOR .sdata = 1 TO .n;清除发送你数据
;  $send_buf[.sdata] = ""
; END
.END
.REALS
anglersltp = -0.0033328
anglersltr = 179.968
anglerslty = 47.3881
buf_n = 12
eret = 0
home_area = 1
in_restart = 2068
in_start = 2066
in_stop = 2067
ip[1] = 192
ip[2] = 168
ip[3] = 1
ip[4] = 200
keycurposrx = 179.968
keycurposry = -0.0033328
keycurposrz = 47.3881
keycurposx = -501.185
keycurposy = 517.877
keycurposz = 685.734
keyfloor = 0
keynormangle = 47.3881
max_length = 255
modbus_address = 94
modbus_code = 6
modbus_data = 30
modbus_id = 1
modbus_len = 6
modbus_protocol = 0
mudbus_number = 38847
pg.no = 0
port = 20480
pos[4] = 179.968
pos[5] = 359.997
pos[6] = 47.3881
ret = 0
rot_range = 360
set_lscir = 1.5
sock_id = 0
spost_size = 1.2
spot_size = 1.4
sret = 0
text_id = 0
tiny_val = 1e-05
tout = 60
tout_open = 5
tout_rec = 5
tpx = -501.185
tpy = 517.877
tpz = 685.734
wx_restart = 1001
wx_start = 1031
wx_stop = 1002
.END
.STRINGS
$send_buf[1] = ""
$send_buf[2] = ""
$send_buf[3] = ""
$send_buf[4] = ""
$send_buf[5] = ""
$send_buf[6] = ""
$send_buf[7] = ""
$send_buf[8] = ""
$send_buf[9] = ""
$send_buf[10] = ""
$send_buf[11] = ""
$send_buf[12] = ""
.END
