.***************************************************************************
.*=== AS ×é ===         : ASE_033300Z4X 2020/09/02 10:26
.*ÓÃ»§IF AS              : UASE033300Z4X 2020/09/02 10:26
.*ÓÃ»§IF TP              : UTPE033300Z4X 2020/09/02 10:15
.*ÊÖ±Û¿ØÖÆ AS            : AASE033300Z4X 2020/09/02 10:43
.*ÓÃ»§IF AS ÐÅÏ¢ÎÄ¼þ    : MASE0300Z4XCH 2020/09/02 10:09
.*ÓÃ»§IF TP ÐÅÏ¢ÎÄ¼þ    : MTPE0300Z4XCH 2020/09/02 10:03
.*ÊÖ±ÛÊý¾ÝÎÄ¼þ     : ARME033300Z4X 2020/09/02 10:15
.*=== ËÅ·þ×é ===   : 
.*ÊÖ±Û¿ØÖÆ ËÅ·þ        : ASVE080000068 2020/03/25 09:36
.*ËÅ·þÊý¾ÝÎÄ¼þ     : 
.*ÊÖ±Û¿ØÖÆËÅ·þFPGA£ºASFE08000000A 2015/04/01 09:29
.*   [³ö³§Éè¶¨Êý¾Ý] 
.*	default.as
.***************************************************************************
.NETCONF     192.168.44.2,"pcc",255.255.255.0,0.0.0.0,0.0.0.0,0.0.0.0,""
.NETCONF2     192.168.1.101,255.255.0.0,0.0.0.0
.INTER_PANEL_D
9,10,""," ¼¾¼¶¼¾¼Ò¼¾","","",7,2,1,2,"SIGNAL 2100",-1
15,10,"","   ¼¾¼»¼¾¼¦   ¼¾¼¶€Ò¼¾","   ¼¾¼¶€Ò¼¾","",10,5,5,5,"SIGNAL 2002,2101",-1
16,10,"","   ¼¾¼Á¼¾¼×   ¼¾¼Î¼Ö¼Ä","   ¼¾¼Î¼Ö¼Ä","",10,5,5,6,"SIGNAL 2001",-1
21,9,1,7,0
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
.PROGRAM autostart1.pc() #0; #0; #0; #0; #0; #0; Í¨ÐÅÖ÷³ÌÐò
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
  CALL open_socket; Á¬½ÓÍ¨ÐÅ
  TWAIT 1
  CALL close_socket; ¶Ï¿ªÍ¨ÐÅ
.END
.PROGRAM autostart5.pc() #0; #0; #0; #0; #0; #0; Í¨ÐÅÖ÷³ÌÐò
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
    CALL open_socket; Á¬½ÓÍ¨ÐÅ
  END
;TWAIT 1
;CALL close_socket; ¶Ï¿ªÍ¨ÐÅ
.END
.PROGRAM close_err() #54; #21; #109; #748; #0; #0
  sock_error = 0
  sock_error = ERROR
  TYPE "¹Ø±Õ´íÎó£¡"
  RETURNE
.END
.PROGRAM close_socket() #6; #3; #24; #17;¶Ï¿ªÍ¨ÐÅ³ÌÐò
;*****************************************
;* FUNCTION: Ì×½Ó×Ö¹Ø±Õ³ÌÐò              *
;* WorkType: TCP/IPÍ¨Ñ¶                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_CLOSE  ·µ»ØÖµ£¬Ì×½Ó×ÖºÅ
  ONE close_err
  sock_close = 0
  TCP_CLOSE sock_close,sock_id
;PAUSE
  IF sock_close<0 THEN
    .$ret = $ENCODE(sock_close)
    TYPE "Í¨Ñ¶´íÎó£¬´íÎó´úÂë=",.$ret
; IFPWPRINT 1,2,10,4,10 = "Í¨Ñ¶´íÎó£¬´íÎó´úÂë=",.$ret
    TCP_CLOSE sock_close,sock_id;Ç¿ÖÆ¹Ø±ÕÌ×½Ó×Ö
  ELSE
; IFPWPRINT 1,2,10,4,10 = "Í¨Ñ¶¹Ø±Õ"
    TYPE "Í¨Ñ¶¹Ø±Õ"
  END
.END
.PROGRAM com_test() #0; #0; #0; #0; #0; #4; ¸ù¾ÝÖ¸Ê¾Êý¾Ý½øÐÐ¸÷´¦Àí
  CASE text_id OF
   VALUE 0: ; ÎÞÖ¸Ê¾
    GOTO break_exit
   VALUE 1: ; Ö¸Ê¾´¦Àí 1
; ¸÷´¦Àí
   VALUE 2: ; Ö¸Ê¾´¦Àí 2
; ¸÷´¦Àí
   VALUE 3: ; Ö¸Ê¾´¦Àí 3
; ¸÷´¦Àí
   ANY :
    GOTO break_exit
  END
  $end_buf[1] = $recv_buf[1]+" OK"
  CALL send(eret,$end_buf[1]); ·¢ËÍ½áÊø´¦Àí
break_exit:
.END
.PROGRAM main() #0; #0; #0; #0; #0; #0; Í¨ÐÅÖ÷³ÌÐò
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
  CALL open_socket; Á¬½ÓÍ¨ÐÅ
;IF sock_id<0 THEN
;  GOTO exit_end
;END
  text_id = 0
  tout = 60
  WHILE (1) DO
    ret = 0
    CALL recv; ´Ó PC½ÓÊÕÖ¸Ê¾Êý¾Ý
    IF ret<0 THEN
      PRINT "Communication error  code=",ret
      GOTO exit
    ELSE
      CASE ret OF
       VALUE 0:
        text_id = VAL($MID($recv_buf[1],1,3)) ; ¶ÁÈ¡Ö¸Ê¾ºÅ
       ANY :
      END
    END
    IF text_id>0 THEN
      CALL com_test; ¸÷Ö¸Ê¾µÄ´¦Àí
      IF eret<0 THEN
        GOTO exit
      END
    END
  END
exit:
  CALL close_socket; ¶Ï¿ªÍ¨ÐÅ
exit_end:
.END
.PROGRAM mes_close_socke() #0; #0; #0; #0; #0; #38402;¶Ï¿ªÍ¨ÐÅ
  TCP_CLOSE ret,sock_id;Õý³£µÄÌ×½Ó×Ö¹Ø±Õ
  IF ret<0 THEN
    PRINT "TCP_CLOSE OK id=",ret1
  ELSE
    sock_id = 0
  END
.END
.PROGRAM mes_int() #0; #0; #0; #0; #0; #0; #0; #37520; Mes³õÊ¼»¯
;Êý¾Ý³õÊ¼»¯
  work_a_state = FALSE;
  work_b_state = FALSE;
  work_c_state = FALSE;
  work_d_state = FALSE;
.END
.PROGRAM mes_main() #0; #0; #0; #0; #0; #0; Í¨ÐÅÖ÷³ÌÐò
; max_length = 255
;tout_rec = 5
; eret = 0
  sock_id = 0
; ret = 0
; text_id = 0
;Êý¾Ý³õÊ¼»¯
  WHILE (TRUE) DO
    CALL mes_open_socket;Á¬½ÓÍ¨ÐÅ
    TWAIT 1
    CALL mes_close_socke;¶Ï¿ªÍ¨ÐÅ
  END
;  IF sock_id>0 THEN
;;;;   CALL tcp_match(.data)
;  CALL tcp_close_socke;¶Ï¿ªÍ¨ÐÅ
;END
.END
.PROGRAM mes_open_socket() #0; #0; #0; #0; #0; #101; #0; #0;¿ªÊ¼Í¨ÐÅ
  .er_count = 0
  .ret = 0
  .port = 60000
  .ip[1] = 192
  .ip[2] = 168
  .ip[3] = 44
  .ip[4] = 1
;connect:
;sock_id= TCP_CONNECT (sock_id,.port,.ip[1]);³¬Ê±1Ãë
  IF sock_id<0 THEN
    PRINT "TCP_CONNECT error id=",sock_id
  END
.END
.PROGRAM open_socket() #6; #4; #24; #11;Í¨Ñ¶Á¬½Ó³ÌÐò
;*****************************************
;* FUNCTION: Í¨Ñ¶Á¬½Ó³ÌÐò                *
;* WorkType: TCP/IPÍ¨Ñ¶                  *
;* Copyright[c]2022 by KRCT              *
;*****************************************
;TCP_STATUS ·µ»ØÖµ£¬¶Ë¿ÚºÅ£¬Ì×½Ó×ÖºÅ£¬´íÎó´úÂë£¬´íÎó×Ó´úÂë£¬IPµØÖ·
  ONE sock_err
  TCP_STATUS sock_stat,stat_port[0],stat_sock[0],stat_err[0],stat_sub[0],$ip_add[0]
;PAUSE
  IF sock_stat>0 THEN ;¼ì²éÁ¬½Ó×´Ì¬
    CALL close_socket
  END
;connect
  DO
;TCP_CONNECT ·µ»ØÖµ£¬¶Ë¿ÚºÅ£¬IPµØÖ·,Á¬½Ó³¬Ê±Ê±¼ä
    ONE sock_err
    TCP_CONNECT sock_id,port,ip[1],tout_open
;PAUSE
    IF sock_id<0 THEN
      .$ret = "Í¨Ñ¶Á¬½Ó´íÎó£¬´íÎó´úÂë="+$ENCODE(sock_id)
; IFPWPRINT 1,2,10,4,10=.$ret
      TYPE .$ret
    END
    IF sock_id==0 THEN
      .$ret = "Í¨Ñ¶Á¬½Ó¶ÔÏóÎÞ»ØÓ¦£¡"+$ENCODE(sock_id)
; IFPWPRINT 1,2,10,4,10=.$ret
      TYPE .$ret
    END
    TWAIT 2
;PAUSE
  UNTIL sock_id>0
;IFPWPRINT 1,2,10,4,10="»úÆ÷ÈËÓë·þÎñÆ÷Í¨Ñ¶Á¬½Ó³É¹¦"
  TYPE "»úÆ÷ÈËÓë·þÎñÆ÷Í¨Ñ¶Á¬½Ó³É¹¦!,Socket_ID=",sock_id
.END
.PROGRAM recv() #0; #0; #0; #0; #0; #257168; Í¨ÐÅ ½ÓÊÕÊý¾Ý
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
.PROGRAM send(.ret,.$data) #0; #0; #0; #0; #0; #4; Í¨ÐÅ ·¢ËÍÊý¾Ý
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
.PROGRAM sock_err() #54; #20; #109; #748; #0; #0
  sock_error = 0
  sock_error = ERROR
  ONE close_err
  TCP_CLOSE sock_close,sock_id
  TYPE "Í¨ÐÅ´íÎó£¬Í¨Ñ¶ÒÑ¹Ø±Õ£¡"
;PRINT $ERROR(ERROR)
;PRINT "´íÎóÐÅÏ¢£º",sock_error
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
sock_stat = 1
sret = 0
stat_err[0] = 0
stat_port[0] = 60000
stat_sock[0] = 424
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
