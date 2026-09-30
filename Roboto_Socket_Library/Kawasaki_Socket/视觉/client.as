.INTER_PANEL_D
0,6,"  ip地址","  第一位",10,8,3,1,0,2110,8,1,2,-1,255.000000,0.000000,-1
1,6,"  ip地址","  第二位",10,8,3,1,0,2120,8,1,2,-1,255.000000,0.000000,-1
2,6,"  ip地址","  第三位",10,8,3,1,0,2130,8,1,2,-1,255.000000,0.000000,-1
3,6,"  ip地址","  第四位",10,8,3,1,0,2140,8,1,2,-1,255.000000,0.000000,-1
4,6,"  端口号","",10,8,5,1,0,2150,16,1,2,-1,65535.000000,8192.000000,-1
5,6,"   发送"," 超时时间",10,8,2,1,0,2170,8,1,2,-1,60.000000,1.000000,-1
6,6,"   接收"," 超时时间",10,8,2,1,0,2180,8,1,2,-1,60.000000,1.000000,-1
7,10,"","获取机器人","当前坐标值","",10,8,5,1,"SIGNAL 2105",-1
8,4,2,""," 解析数据","不解析数据","",10,1,2,2102,2103,-1
9,10,""," 参数登录","","",7,2,1,2,"SIGNAL 2100",-1
10,10,""," 通讯程序"," 停止运行","",10,4,1,3,"pcabort 5:",-1
11,10,""," 通讯程序"," 启动运行","",10,4,1,4,"pcexecute 5:autostart5.pc",-1
14,14,"vsend_data"," 发送测试","  字符串",10,11,-1
15,10,"","   发送","   测试","",10,5,5,5,"SIGNAL 2002,2101",-1
16,10,"","   接收","   数据","",10,5,5,6,"SIGNAL 2001",-1
17,10,"","   取消"," 接收数据","",10,5,5,7,"SIGNAL -2001",-1
21,9,1,7,0
.END
.SYSDATA
SWITCH AUTOSTART5.PC    ON  
.END
.PROGRAM autostart5.pc() ;后台通讯程序
 ;*****************************************
 ;* FUNCTION: 通讯主程序                  *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  CALL tcp_init
re_connect:
  CALL open_socket
re_recv:
  IF SIG(2001) THEN
    IFPWPRINT 1,2,10,4,10 = ""
10
    CALL recv($recv_data) ;接收
    IF rret< 0 THEN  
      IF rret==-34024 THEN
        IF SIG(-2001) THEN
          IFPWPRINT 1,2,10,4,10 = ""
          GOTO re_recv
        END
        GOTO 10
      ELSE
        GOTO re_connect
      END
    END
    IF SIG(2102) THEN   
      CALL vision_decode
    END
    SIGNAL -2001
    GOTO re_recv
  END
  IF SIG(2002) THEN
    IF re_link == 1 GOTO 20
    send_cnt = 0
    IFPWPRINT 1,2,10,4,10 = ""
    IF SIG(2101) THEN
      $send_data = $vsend_data
      SIGNAL -2101
    END
20
    CALL send($send_data)
    IF sret < 0 THEN
      IF sret==-34024 THEN
        IF send_cnt == 1 GOTO re_connect
        IF SIG(-2002) THEN
          send_cnt = 0
          re_link = 0
          IFPWPRINT 1,2,10,4,10 = ""
          GOTO re_recv
        END
        GOTO 20
      ELSE
        GOTO re_connect
      END
    END
    SIGNAL -2002
  END
  IF SIG(2105) THEN
    CALL GetPositionData
    SIGNAL -2105
  END
  SIGNAL -2100
  GOTO re_recv
.END
.PROGRAM tcp_init() ;通讯参数初始化
 ;*****************************************
 ;* FUNCTION: 通讯参数初始化程            *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  buf_n = 1
  re_link = 0 
  recv_cnt = 0
  send_cnt = 0
  max_length = 255 
  $vsend_data = "KAWASAKI"
start:
  IF SIG(2100) THEN
    TWAIT 1
    ip[1] = BITS (2110,8)
    ip[2] = BITS (2120,8)
    ip[3] = BITS (2130,8)
    ip[4] = BITS (2140,8)    
    tcp_port = BITS (2150,16)  ;服务器端口号（8192-65535）
    tout_send = BITS (2170,8) ;发送超时时间
    tout_recv = BITS (2180,8) ;接收超时时间
    SIGNAL -2100
  END
  IF (ip[1] == 0 AND ip[2] == 0 AND ip[3] == 0 AND ip[4] == 0) OR tcp_port < 8192 OR tcp_port > 65535 OR tout_send == 0 OR tout_recv == 0 THEN
    IFPWPRINT 1,2,10,4,10 = "请在接口面板输入服务器IP地址、端口号、发送超时时间、接收超时时间"  
    GOTO start
  ELSE
    IFPWPRINT 1,2,10,4,10 = ""
  END
.END
.PROGRAM open_socket() ;通讯连接程序
;*****************************************
;* FUNCTION: 通讯连接程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_STATUS 返回值，端口号，套接字号，错误代码，错误子代码，IP地址
  TCP_STATUS .num_sock,.port[0],.sock[0],.err[0],.sub[0],.$ip_add[0]
  IF .num_sock > 0 THEN ;检查连接状态
    CALL close_socket    
  END
;connect
  DO
    ;TCP_CONNECT 返回值，端口号，IP地址,连接超时时间
    TCP_CONNECT sock_id,tcp_port,ip[1],tout_open
    IF sock_id < 0 THEN
      .$ret = "通讯连接错误，错误代码=" + $ENCODE(sock_id)
      IFPWPRINT 1,2,10,4,10 = .$ret
    END
  UNTIL sock_id >= 0
  IFPWPRINT 1,2,10,4,10 = "机器人与服务器通讯连接成功"
.END
.PROGRAM recv(.$recv) ; 数据接收程序
;*****************************************
;* FUNCTION: 数据接收程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
  .$recv = ""
  ;TCP_RECV 返回值，套接字号，接收字符串数组，元素数，接收超时时间，最大字节数
  TCP_RECV rret,sock_id,$recv_buf[1],num,tout_recv,max_length
  IF rret < 0 THEN
    .$rret = "数据接收超时，错误代码=" + $ENCODE(rret)
    IFPWPRINT 1,2,10,4,10 = .$rret
  ELSE     
    .$recv=$recv_buf[1]     
    $recv_buf[1]=""
    IFPWPRINT 1,2,10,4,10 = "数据接收成功",.$recv
  END
.END
.PROGRAM send(.$data) ;数据发送程序
;*****************************************
;* FUNCTION: 数据发送程序                *
;* WorkType: TCP/IP通讯                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
  $send_buf[1] = .$data
  ;TCP_SEND 返回值，套接字号，发送字符串数组，元素数，发送超时时间
  TCP_SEND sret,sock_id,$send_buf[1],buf_n,tout_send
  IF sret < 0 THEN
    send_cnt = send_cnt +1
    IF send_cnt == 1 THEN
      re_link = 1
    END    
    .$sret = "数据接收超时，错误代码=" + $ENCODE(sret)
    IFPWPRINT 1,2,10,4,10 = .$sret
  ELSE
    send_cnt = 0
    re_link = 0
    IFPWPRINT 1,2,10,4,10 = "数据发送成功",.$data
  END
.END
.PROGRAM close_socket() ;断开通信程序
 ;*****************************************
 ;* FUNCTION: 套接字关闭程序              *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  ;TCP_CLOSE  返回值，套接字号
  TCP_CLOSE .ret,sock_id
  IF .ret < 0 THEN
    .$ret = $ENCODE(.ret)
    IFPWPRINT 1,2,10,4,10 = "通讯错误，错误代码=",.$ret
    TCP_CLOSE .ret,sock_id ;强制关闭套接字
  ELSE
    IFPWPRINT 1,2,10,4,10 = "通讯关闭"
  END
.END
.PROGRAM vision_decode() ;字符串解析程序
 ;*****************************************
 ;* FUNCTION:字符串解析程序               *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  $ccdtext = $recv_data
  i = 1
  DO
    $temp = $DECODE($ccdtext,",",0)
    value[i] = VAL($temp)
    IF $ccdtext == "" GOTO 100
    $temp = ""
    $temp = $DECODE($ccdtext,",",1)
    i = i + 1
  UNTIL $ccdtext == ""
100
  IF i < 3 THEN
    IFPWPRINT 1,2,10,4,10 = "视觉发送的数据异常，请检查数据是否完整",$recv_data
    RETURN
  ELSE
    $value[1] = "X轴value[1]:" + $ENCODE(value[1])
    $value[2] = "Y轴value[2]:" + $ENCODE(value[2])
    $value[3] = "旋转角value[3]:" + $ENCODE(value[3])
    IFPWPRINT 1,1,10,4,10 = "接收的视觉数据：",$value[1],$value[2],$value[3]
  END
.END
.PROGRAM GetPositionData() ;获取机器人位姿程序
 ;*****************************************
 ;* FUNCTION: 获取机器人位姿程序          *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  .$posdata_xyz = ""
  .$posdata_oat = ""
  HERE .pos
  DECOMPOSE .posdata[0] = .pos
  .$posdata_xyz = $ENCODE(.posdata[0])
  FOR i=1 TO 2
   .$posdata_xyz = .$posdata_xyz + "," + $ENCODE(.posdata[i])
  END
  .$posdata_oat = $ENCODE(.posdata[3])
  FOR i=4 TO 5
   .$posdata_oat=.$posdata_oat + "," + $ENCODE(.posdata[i])
  END
  .$posdata_xyz = "X，Y，Z：" + .$posdata_xyz
  .$posdata_oat = "O，A，T：" + .$posdata_oat
  IFPWPRINT 1,2,10,4,10 = "机器人当前坐标",.$posdata_xyz,.$posdata_oat 
.END
.PROGRAM main() ;主程序
 ;*****************************************
 ;* FUNCTION: 视觉应用示例                *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  SIGNAL -2001,-2002
  FOR i=1 TO 3
    value[i] = 0
  END 
  ;移动到相机拍照点
  LMOVE #start 
  BREAK
  ;触发视觉拍照
  CALL tcp_main 
  ;分解抓取基准点
  DECOMPOSE aa[1] = pick_jizhun 
  ;抓取点重组
  POINT pick = TRANS(aa[1]+value[1],aa[2]+value[2],aa[3],aa[4]+value[3],aa[5],aa[6]) 
  LMOVE SHIFT(pick BY ,,50)
  SPEED 10
  LMOVE pick
  BREAK
  CLOSEI
  JMOVE SHIFT(pick BY ,,200)
  JMOVE #guo1
  JMOVE SHIFT(put BY ,,50)
  SPEED 10
  LMOVE put
  BREAK
  OPENI
  JMOVE SHIFT(put BY ,,200)
  JMOVE #guo1
  JMOVE #start  
.END
.PROGRAM tcp_main();相机拍照通讯程序
 ;*****************************************
 ;* FUNCTION: 视觉通讯拍照程序            *
 ;* WorkType: TCP/IP通讯                  *
 ;* Copyright[c]2022 by KRCT              *
 ;*****************************************
  $inque_1 = "S 5" ;第1次触发字符
  $inque_2 = "M" ;第2次触发字符
  $send_data = $inque_1 + $CHR(13)
  SIGNAL 2002 ;发送数据
  SWAIT -2002 ;等待数据发送完成
  SIGNAL 2001,-2102,2103 ;数据接收，不解析字符
  SWAIT -2001 ;等待数据接收完成
  TWAIT 0.1
  IF $recv_data<>"OK" OR rret<0 THEN
    $data[0] = ""
    IFPWPRINT 1,2,10,4,10 = "数据接收异常！"
    GOTO end
  END
  $recv_data = ""
  TWAIT 0.1
;
  $send_data = $inque_2 + $CHR(13)
  SIGNAL 2002 ;发送数据
  SWAIT -2002 ;等待数据发送完成
  SIGNAL 2001,-2102,2103 ;数据接收，不解析字符
  SWAIT -2001 ;等待数据接收完成
  IF $recv_data<>"OK" OR rret<0 THEN
    $data[0] = ""
    IFPWPRINT 1,2,10,4,10 = "数据接收异常！"
    GOTO end
  END
  SIGNAL 2001,2102,-2103 ;数据接收，解析字符
  SWAIT -2001 ;等待数据接收完成
end:
.END
.PROGRAM stabl() ;固定式相机定位抓取测试
  SPEED 50 ALWAYS
  ACCURACY 1 ALWAYS
  FOR i = 1 TO 3
    value[i] = 0
  END
  LMOVE #wait_point
  BREAK
  CALL tcp_main
  DECOMPOSE om_s[0] = stable_pick
  POINT target = TRANS (om_s[0]+value[1],om_s[1]+value[2],om_s[2],om_s[3]+value[3],om_s[4],om_s[5])
  LAPPRO target,20
  SPEED 10
  LMOVE target
  BREAK
.END
.PROGRAM handeye() ;手眼式相机定位抓取测试
  SPEED 50 ALWAYS
  ACCURACY 1 ALWAYS
  FOR i = 1 TO 3
    value[i] = 0
  END
  LMOVE #wait_point
  SPEED 10
  LMOVE #handeye_test 
  BREAK
  CALL tcp_main
  DECOMPOSE om_s[0] = handeye_pick
  POINT target = TRANS (om_s[0]+value[1],om_s[1]+value[2],om_s[2],om_s[3]+value[3],om_s[4],om_s[5])
  LAPPRO target,20
  SPEED 10
  LMOVE target
  BREAK
.END
.PROGRAM stable_teach() ;固定式相机示教程序
  LMOVE wait_point ;机器人待机位置
;-----------------------------------
  LMOVE teach[0] ;固定式相机标定点01
  LMOVE teach[1] ;固定式相机标定点02
  LMOVE teach[2] ;固定式相机标定点03
  LMOVE teach[3] ;固定式相机标定点04
  LMOVE teach[4] ;固定式相机标定点05
  LMOVE teach[5] ;固定式相机标定点06
  LMOVE teach[6] ;固定式相机标定点07
  LMOVE teach[7] ;固定式相机标定点08
  LMOVE teach[8] ;固定式相机标定点09
;-----------------------------------
  LMOVE stable_pick ;固定式相机定位抓取点
.END
.PROGRAM handeye_teach() ;手眼式相机示教程序
  LMOVE wait_point ;机器人待机位置
  ALIGN
  LMOVE handeye_base ;手眼式相机标定拍摄点
;-----------------------------------
  LMOVE teach[0] ;手眼式相机标定点01
  LMOVE teach[1] ;手眼式相机标定点02
  LMOVE teach[2] ;手眼式相机标定点03
  LMOVE teach[3] ;手眼式相机标定点04
  LMOVE teach[4] ;手眼式相机标定点05
  LMOVE teach[5] ;手眼式相机标定点06
  LMOVE teach[6] ;手眼式相机标定点07
  LMOVE teach[7] ;手眼式相机标定点08
  LMOVE teach[8] ;手眼式相机标定点09
;-----------------------------------
  ALIGN
  LMOVE handeye_test ;手眼式相机检测拍摄点
  LMOVE handeye_pick ;手眼式相机定位抓取点
.END
.REALS
  ip[1] = 0 
  ip[2] = 0
  ip[3] = 0
  ip[4] = 0 
  tout_open = 10
  tcp_port = 0
  tout_send = 0
  tout_recv = 0  
.END

