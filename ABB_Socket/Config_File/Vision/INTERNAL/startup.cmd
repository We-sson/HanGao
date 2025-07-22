#
# Property of ABB Vasteras/Sweden. All rights reserved.
# Copyright 2003.
#
# Startup.cmd script for 6.0
#
###########################################################
# Option startup info
#
# To execute Option related tasks in the startup, there are
# five places in the startup sequence that can be used.
# Depending on which dependencies the tasks have, the user 
# must select the most suitable place for the Option startup.
#
# In each place, a check is done for a unique named file.
# (These files are created/appended during install sequence.)
# The files checked for in the INTERNAL: folder are named:
# opt_l0.cmd, option.cmd, opt_l1.cmd, opt_l2.cmd, opt_l3.cmd
# If the file exist, then it will be included in the startup.
#
# Search for each file to get info/tip about how to use it.
#
###########################################################

startup_log -file $RAMDISK/startup.log

# Base support initialization start.
baseinit

ifvc -label VC_SKIP_FIRMWARE_UPGRADE
### Check MC board firmware version
invoke -entry mcfirmw_upgrade -noarg -nomode
#VC_SKIP_FIRMWARE_UPGRADE

# Populate bulletin board entries
bb_populate -func sysenergy_state_machine_bb_populate
bb_populate -func sysorch_bb_populate
bb_populate -func operator_mode_bb_populate
bb_populate -func controller_state_bb_populate
bb_populate -func barts_bb_populate
bb_populate -func master_control_bb_populate
bb_populate -func sysacthooks_bb_populate
bb_populate -func pgmrun_bb_populate
bb_populate -func sysorch_holdtorun_bb_populate
bb_populate -func cab_bb_populate
bb_populate -func baseroot_bb_flags_populate
bb_populate -func pgmtbr_bb_populate

invoke2 -entry BulletinBoardItem_enable_uprobe -format char*,int -str1 "NoOfPendingStartupFuncs" -int1 0

# Initialize and enable system dump service
sysdmp_init -max_dumps 3 -delay 100 -dir $INTERNAL/SYSDMP -compr no
ifvc -label VC_SKIP_SYSDMPTASK
task -slotname sysdmpts -slotid 82 -pri 253 -vwopt 0x1c -stcks 50000 -entp sysdmpts_main -auto
#VC_SKIP_SYSDMPTASK

sysdmp_add -source print_spooler -save print_spooler_save_buffer -fileext log 
getkey -id "ASCIILogMedia" -var 10 strvar $ANSWER -errlabel NO_ASCII_LOG
uprobe_init -points 500000 -pretrig_points 45000 -trace_buf_sz 10000000
goto -label NEXT_STEP
#NO_ASCII_LOG 
uprobe_init -points 50000 -pretrig_points 45000 -trace_buf_sz 10000
#NEXT_STEP 
sysdmp_add -class uprobe -fileext log

# By default use uprobe_default_start.cmd
setstr -strvar $UPROBE_DEF_START -value "yes"

# Include command file for logging mechanisms. Mainly used for uprobes
fileexist -path $SYSTEM/service_debug_early.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $SYSTEM/service_debug_early.cmd
sysdmp_add_logfile -move 0 -file "$SYSTEM/service_debug_early.cmd"
#NEXT_STEP

ifvc -label NO_WVLOG
# wvLogging is started after service_debug_early.cmd, make it possible to 
# raise the level of logging by using wvLogSysdmpEvtClassSet
sysdmp_add -source wvlog -start wvLogSysdmpStart -stop wvLogSysdmpStop -save wvLogSysdmpSave -fileext wvr
#NO_WVLOG

ifvc -label NO_ETHCAP
sysdmp_add -source ethcap -start ethcap_sysdmp_start -stop ethcap_stop -save ethcap_sysdmp_save
#NO_ETHCAP

# Start event log 
task -slotname elogts -slotid 1 -pri 85 -vwopt 0x1c -stcks 7000 -entp elog_main -auto
synchronize -level task
go -level task

ifvc -label VC_NO_REMOTE_DIAGNOSTIC
# Enable Service Box in devices
invoke -entry RemoteDiagnosticInit -noarg -nomode
#VC_NO_REMOTE_DIAGNOSTIC

task -slotname delay_high -entp delay_ts -pri 60 -vwopt 0x1c -stcks 65536 -nosync -auto -wait_ready 10
task -slotname delay_medium -entp delay_ts -pri 79 -vwopt 0x1c -stcks 65536 -nosync -auto -wait_ready 10
task -slotname delay_low -entp delay_ts -pri 140 -vwopt 0x1c -stcks 65536 -nosync -auto -wait_ready 10

ifvc -label VC_NO_CLOCK_PROBE
invoke2 -entry sysdmp_periodic_os_clock_event_probe -format void
#VC_NO_CLOCK_PROBE

basenew

# Check here if warmstart is in sleep mode and handle it here if so
handle_startup_in_sleep_mode

# Delete $RW_TEMP/rw-trash directory if it exists
direxist -path $RWTEMP/rw_trash -label CLEAR_TMP
goto -label NEXT_STEP
#CLEAR_TMP
print -text "Deleting the trash folder. This may take a while ..."
xdelete -path $RWTEMP/rw_trash -unprotect
#NEXT_STEP

# Base support initialization finished.
# Start generation of heartbeat interrupt
invoke -entry heartbeatStart
delay -time 3000

ifvc -label NEXT_STEP
# Restore system.xml and RW6system.xml files if needed
setvar -var 10 -value 0
#CHECK_SYSTEM_XML
fileexist -path $SYSTEM/system.xml -label CHECK_RW6_SYSTEM_XML
fileexist -path $INTERNAL/system.xml.rescue -label RESTORE_SYSTEM_XML
echo -text "No system.xml exist neither in SYSTEM or INTERNAL folder"
goto -label SYSTEM_ERROR

#RESTORE_SYSTEM_XML
echo -text "Restore the system.xml rescue file"
copy -from $INTERNAL/system.xml.rescue -to $SYSTEM/system.xml
copy -from $INTERNAL/system.xml.rescue -to $INTERNAL/system.xml.err
# Set temp var 10 to 1 that indicates restart needed
setvar -var 10 -value 1
goto -label CHECK_RW6_SYSTEM_XML

#CHECK_RW6_SYSTEM_XML
fileexist -path $SYSTEM/RW6system.xml -label CHECK_RESTART_NEEDED
fileexist -path $INTERNAL/RW6system.xml.rescue -label RESTORE_RW6_SYSTEM_XML
echo -text "No RW6system.xml exist neither in SYSTEM or INTERNAL folder"
goto -label SYSTEM_ERROR

#RESTORE_RW6_SYSTEM_XML
echo -text "Restore the RW6system.xml rescue file"
copy -from $INTERNAL/RW6system.xml.rescue -to $SYSTEM/RW6system.xml
copy -from $INTERNAL/RW6system.xml.rescue -to $INTERNAL/RW6system.xml.err
# Set temp var 10 to 1 that indicates restart needed
setvar -var 10 -value 1
goto -label CHECK_RESTART_NEEDED

#CHECK_RESTART_NEEDED
if -var 10 -value 1 -label RESTART_NEEDED
goto -label NEXT_STEP
#RESTART_NEEDED
echo -text "Restarting system to recover..."
delay -time 1000
restart
#NEXT_STEP

ifvc -label VC_NO_MC_PSU_UPGRADE

### Check PSU board firmware version
invoke -entry psu_upgrade -noarg -nomode
#VC_NO_MC_PSU_UPGRADE

ifvc -label VC_NO_MC_FPGA_UPGRADE

### Check Main Computer FPGA firmware version
invoke -entry fpgar11_firmw_upgrade -noarg -nomode
#VC_NO_MC_FPGA_UPGRADE

iomgrinstall -entry simFBC -name /simfbc
creat -name /simfbc/SIM_SIM: -pmode 0

invoke -entry read_init

iomgrinstall -entry rccfbc -name /rccfbc
creat -name /rccfbc/LOC_LOC: -pmode 0
task -slotname LOCALBUS -entp read_ts -pri 44 -vwopt 0x1c -stcks 10000 -nosync -auto
readparam -devicename /LOC_LOC:/bus_read -rmode 1 -buffersize 100
iomgrinstall -entry UserSio -name /usersio

# Handle COM1
ifvc -label VC_COM1
invoke2 -entry DebugCardPresent -format void -errlabel NEXT_STEP
#VC_COM1
echo -text "Installing COM1 device"
creat -name /usersio/SIO1: -pmode 0
#NEXT_STEP

# Fieldbus command interface
ifvc -label VC_SKIP_FCI
iomgrinstall -entry Fci -name /fci
creat -name /fci/FCI1: -pmode 0

task -slotname fcits -slotid 80 -pri 80 -vwopt 0x1c -stcks 10000 -entp fcits -auto

#VC_SKIP_FCI

init -resource eio

# Start EtherNet/IP base 
fileexist -path $RELEASE/options/enms/en_base.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $RELEASE/options/enms/en_base.cmd

#start Bonjour daemon only for VxWorks
ifvc -label NO_BONJOUR_DAEMON
invoke2 -entry start_bonjour_daemon -format void

#NO_BONJOUR_DAEMON

# Initialize the memory-pool for RobAPI 2. This should be done before spawning any other task.
invoke2 -entry init_mempools -format void

task -slotname cnmgrts -slotid -1 -pri 125 -vwopt 0x1c -stcks 90000 \
-entp cleanupts -auto -noreg

task -slotname servdisp -slotid 126 -pri 125 -vwopt 0x1c -stcks 90000 \
-entp servdispts -auto -noreg

task -slotname servlist -slotid 127 -pri 125 -vwopt 0x1c -stcks 90000 \
-entp ServiceListingTS -auto -noreg

task -slotname ctrlsrvts -slotid 134 -pri 125 -vwopt 0x1c -stcks 90000 \
-entp rapi_ctrlsrvts -auto -noreg

task -slotname appweb -slotid 129 -pri 125 -vwopt 0x1c -stcks 120000 \
-entp AppwebEntryFunc -auto -noreg

#NEXT_STEP

###########################################		
# opt_l0.cmd - Option startup Sequence no 1
# Example of:
#  - users: Eio, MotionRef, Paint
#  - use: Create/Enable of I/O interfaces, Start of option task 
#  - dependencies: baseinit, sysdmp, elogts, delay tasks, simFBC, rccfbc, UserSio, Fci
#  - why here: Must be done before init, task and reconfigure of eio and sio
#
fileexist -path $INTERNAL/opt_l0.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $INTERNAL/opt_l0.cmd
#NEXT_STEP

# Increase number of available interpreters for basic use (40MB memory)
python_config -incr_interpreters 1 -memsize 41943040

# Initialize Python support, including the pyrobapi. Starts all interpreters as configured.
invoke2 -entry pyrobapi_init -format void
python_init

init -resource sio
init -resource motion
init -resource rol
init -resource cab
init -resource bar
init -resource pgmrun
init -resource rapid
init -resource puscfg

task -slotname alarmts -slotid 16 -pri 42 -vwopt 0x1c -stcks 65536 \
-entp alarmts -auto

ifvc -label VC_CABSUPTS_SKIP
task -slotname rapidsupts -slotid 62 -pri 135 -vwopt 0x1c -stcks 7000 \
-entp rapidsup_main -auto
#VC_CABSUPTS_SKIP

# Early safety initialization
invoke2 -entry safety_init -format void

task -slotname pstopts -pri 30 -vwopt 0x1c -stcks 5000 \
-entp pstopts_main -auto -nosync

task -slotname rhaltts -pri 30 -vwopt 0x1c -stcks 5000 \
-entp rhaltts_main -auto -nosync

task -slotname eiocrsts -slotid 56 -pri 74 -vwopt 0x1c -stcks 10000 \
-entp eiocrsts -auto

task -slotname eiots -slotid 54 -pri 70 -vwopt 0x1c -stcks 10000 \
-entp eiots -auto -slots 2

task -slotname eioadmts -slotid 55 -pri 76 -vwopt 0x1c -stcks 25000 \
-entp eioadmts -auto

# Start of EIO Event task
task -slotname eioEventts -slotid 140 -pri 73 -vwopt 0x1c -stcks 10000 -entp eioEventts -auto

# Start of Device Trust Level task
task -slotname deviceTLts -slotid 141 -pri 77 -vwopt 0x1c -stcks 10000 -entp deviceTLts -auto

# Start of Device Transfer task
task -slotname deviceTRts -slotid 144 -pri 102 -vwopt 0x1c -stcks 10000 -entp deviceTRts -auto

# Reconfigure the Elementary I/O System
reconfigure -resource eio

# Synchronize all local industrial networks and their I/O devices 
io_synchronize -timeout 30000 -local_network 1

reconfigure -resource sio

task -slotname safevtts  -slotid 20 -pri 20 -vwopt 0x1c -stcks 10000 \
-entp safevtts -auto -noreg

task -slotname safcycts  -slotid 29 -pri 80 -vwopt 0x1c -stcks 10000 \
-entp safcycts -auto

task -slotname sysevtts  -slotid 30 -pri 95 -vwopt 0x1c -stcks 30000 \
-entp sysevtts_main -auto

task -slotname ipolts0 -slotid 65 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname ipolts1 -slotid 66 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname ipolts2 -slotid 67 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname ipolts3 -slotid 68 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname ipolts4 -slotid 91 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname ipolts5 -slotid 92 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname ipolts6 -slotid 150 -pri 90 -vwopt 0x1c -stcks 80000 \
-entp ipolts_main -auto

task -slotname statmats -slotid 64 -pri 8 -vwopt 0x1c -stcks 80000 \
-entp statmats -auto

task -slotname compliance_masterts -slotid 124 -pri 10 -vwopt 0x1c -stcks 80000 \
-entp compliance_masterts -auto

task -slotname sens_memts -slotid 89 -pri 150 -vwopt 0x1c -stcks 20000 \
-entp sens_memts -auto

task -slotname servots0 -slotid 69 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname servots1 -slotid 71 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname servots2 -slotid 73 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname servots3 -slotid 75 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname servots4 -slotid 93 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname servots5 -slotid 95 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname servots6 -slotid 151 -pri 7 -vwopt 0x1c -stcks 80000 \
-entp servots -auto -slots 2

task -slotname evhats -slotid 117 -pri 5 -vwopt 0x1c -stcks 20000 \
-entp evhats -auto

task -slotname mcits -slotid 133 -pri 6 -vwopt 0x1c -stcks 20000 \
-entp mcits -auto

task -slotname refmats -slotid 52 -pri 4 -vwopt 0x1c -stcks 80000 \
-entp refmats -auto

task -slotname mocutilts -slotid 135 -pri 170 -vwopt 0x1c -stcks 20000 \
-entp mocutilts -auto

task -slotname sismats -slotid 63 -pri 106 -vwopt 0x1c -stcks 12500 \
-entp sismats -auto 

task -slotname bcalcts -slotid 44 -pri 200 -vwopt 0x1c -stcks 45000 \
-entp bcalcts -auto

task -slotname logsrvts -slotid 51 -pri 100 -vwopt 0x1c -stcks 30000 \
-entp logsrvts -auto

ifvc -label RW_MOTION_END
task -slotname com_rec_fdbts -slotid 77 -pri 4 -vwopt 0x1c -stcks 5000 -entp com_rec_fdbts -auto

task -slotname com_rec_safety_fdbts -slotid 97 -pri 4 -vwopt 0x1c -stcks 7000 -entp com_rec_safety_fdbts -auto

task -slotname axc_failts -slotid 28 -pri 80 -vwopt 0x1c -stcks 7000 -entp axc_failts -auto

#RW_MOTION_END

task -slotname peventts -slotid 21 -pri 77 -vwopt 0x1c -stcks 65000 \
-entp peventts -auto

task -slotname peventts_high -slotid 145 -pri 65 -vwopt 0x1c -stcks 65000 \
-entp peventts -auto

task -slotname rloadts -slotid 45 -pri 105 -vwopt 0x1c -stcks 40000 \
-entp rloadts -auto

task -slotname barts -slotid 79 -pri 140 -vwopt 0x1c -stcks 80000 \
-entp barts_main -auto

task -slotname rlcomts -slotid 27 -pri 100 -vwopt 0x1c -stcks 36000 \
-entp rlcomts_main -auto

task -slotname rlsocketts -slotid 108 -pri 99 -vwopt 0x1c -stcks 8000 \
-entp rlsocketts_main -auto
task -slotname rlsocketts_rec -slotid 120 -pri 99 -vwopt 0x1c -stcks 8000 \
-entp rlsocketts_rec_main -auto -noreg

goto -label NEXT_STEP

#NEXT_STEP
task -slotname rapidts -slotid 17 -pri 101 -vwopt 0x1c -stcks 120000 \
-entp rapidts_main -auto

task -slotname sysorchcompts -slotid 142 -pri 99 -vwopt 0x1c -stcks 78000 \
-entp sysorchcompts_main -auto

task -slotname sysorchts -slotid 19 -pri 100 -vwopt 0x1c -stcks 78000 \
-entp sysorchts_main -auto

task -slotname sysorchhookts -slotid 84 -pri 101 -vwopt 0x1c -stcks 78000 \
-entp sysorchhookts_main -auto

task -slotname cabts -slotid 6 -pri 99 -vwopt 0x1c -stcks 78000 \
-entp cabts_main -auto

task -slotname mocts -slotid 9 -pri 100 -vwopt 0x1c -stcks 78000 \
-entp motion_main -auto

task -slotname hpjts -slotid 22 -pri 99 -vwopt 0x1c -stcks 80000 \
-entp hpj_main -auto

task -slotname rapidlowts -slotid 137 -pri 126 -vwopt 0x1c -stcks 120000 \
-entp rapid_low_main -auto

task -slotname sysreadts -slotid 143 -pri 126 -vwopt 0x1c -stcks 78000 \
-entp systemreadts_main -auto

task -slotname rwssubsc_worker -slotid 128 -pri 125 -vwopt 0x1c -stcks 65000 \
-entp rwssubscr_worker -auto -noreg

task -slotname filesrv -slotid 130 -pri 140 -vwopt 0x1c -stcks 120000 \
-entp rapi_fileservts_main -auto -noreg

task -slotname rwssubsc -slotid 132 -pri 125 -vwopt 0x1c -stcks 65000 \
-entp rwssubscr_main -auto -noreg

task -slotname subscription -slotid -1 -pri 125 -vwopt 0x1c -stcks 120000 \
-entp rapi_subscriptionts_main -auto -noreg

task -slotname rws_dipc -slotid -1 -pri 125 -vwopt 0x1c -stcks 65000 \
-entp rwsdipc_main -auto -noreg

task -slotname rapi_user -slotid 131 -pri 125 -vwopt 0x1c -stcks 90000 \
-entp rapi_usersrvts -auto -noreg

task -slotname rwservice -slotid 42 -pri 125 -vwopt 0x1c -stcks 65000 \
-entp rwsvcts_main -auto -noreg

ifvc -label VC_NETCMD_SKIP
task -slotname elog_axctsr -pri 100 -vwopt 0x1c -stcks 9000 \
-entp elog_axctsr -auto -nosync
#VC_NETCMD_SKIP

synchronize -level task
delay -time 500
go -level task

# Detect axis computer(s) and initialize resources needed for communication
ifvc -label VC_AXC_SKIP
netcmd_run
#VC_AXC_SKIP

#PNT new -entry purge_sync
#PNT delay -time 5000

###########################################		
# option.cmd - Option startup Sequence no 2
# Example of:
#  - users: 3P, Cap, Eio, MotionRef, Paint, SensorInterfaces, SpotWeld
#  - use: Start of option task, invoke of init/sync methods, convey_new, sysdefs
#  - dependencies: Init of resources, reconfig of eio/sio, most of the system task created, synched and started
#  - why here: Must be done before new of motion and rapid, connect_dependings -object SYS_STORED_OBJ_CAB
#
fileexist -path $INTERNAL/option.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $INTERNAL/option.cmd
#NEXT_STEP


ifvc -label VC_AXC_SKIP

print -text "Check AXC board firmware version ..."
invoke -entry netcmd_upgrade -arg 0 -strarg "axcfirmw_upgrade" -nomode
invoke -entry axcfirmw_devices_install_hook -nomode
print -text "Check drive unit board firmware version ..."
invoke -entry netcmd_upgrade -arg 0 -strarg "drive_unit_firmw_upgrade" -nomode
invoke -entry drive_unit_firmw_devices_install_hook -nomode

#VC_AXC_SKIP
new -resource motion
delay -time 1500
connect_dependings -object SYS_STORED_OBJ_CAB -instance 0 -ipm 0 -servo 0 -ipol 0

# syncronization (setsync) to be used with network systems only
# setsync
if -var 52 -value 1 -label SYNCROB
goto -label NOSYNC
#SYNCROB
# syncronization (setsync) to be used with network systems only
setsync
#NOSYNC

synchronize -level task
go -level task

# Synchronize all industrial networks, except the local networks, and their I/O devices 
io_synchronize -timeout 120000

# Create IPM process for DryRun
invoke -entry rldryrun_start
# Create IPM process for Cyclic bool
invoke -entry rlcyclic_start
# Create iPM process for visualization of unfulfilled conditions
invoke -entry rlvisualize_start

new -resource rapid

###########################################		
# opt_l1.cmd - Option startup Sequence no 3
# Example of:
#  - users: 3P
#  - purpose: Start of option tasks, invoke methods that access sdb and creates ipm processes
#  - dependencies: new -resource motion, connect_dependings -object SYS_STORED_OBJ_CAB, new -resource rapid
#  - why here: Must be done before new -resource cab (Start of RAPID tasks and POWER_ON event etc.)
#
fileexist -path $INTERNAL/opt_l1.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $INTERNAL/opt_l1.cmd
#NEXT_STEP

new -resource cab

#task -slotname spoolts -slotid 34 -pri 140 -vwopt 0x1c -stcks 3000 \
#-entp spool_main -auto

# RobAPI communication and event tasks

# Set max number of possible robapi connections. Default is 3 max is 6 
invoke2 -entry set_max_allowed_robapi_wan_connections -format int -int1 3

# Uncomment invoke below to disable tcp_nodelay for robapi on the lan interface. 
# tcp_nodelay enabled, causes packets to be flushed on to the network more frequently.
# Default is tcp_nodelay enabled on all robapi interfaces.
# For other robapi interface use strarg={"lan" | "service" | "tpu"}.
# invoke -entry robcmd_set_tcp_nodelay -arg 0 -strarg "lan" -nomode



task -slotname robdispts -slotid 58 -pri 125 -vwopt 0x1c -stcks 66000 \
-entp robdispts -auto -noreg

task -slotname robesuts -slotid 59 -pri 125 -vwopt 0x1c -stcks 18000 \
-entp robesuts -auto -noreg

task -slotname robesuts_p -slotid 85 -pri 125 -vwopt 0x1c -stcks 30000 \
-entp robesuts_p -auto -noreg

task -slotname robesuts_hp -slotid 123 -pri 125 -vwopt 0x1c -stcks 15000 \
-entp robesuts_hp -auto -noreg

task -slotname robcmdts -slotid 60 -pri 125 -vwopt 0x1c -stcks 23000 \
-entp robcmdts -auto -noreg

task -slotname robayats -slotid 61 -pri 125 -vwopt 0x1c -stcks 7000 \
-entp robayats -auto -noreg

task -slotname robrspts -slotid 57 -pri 124 -vwopt 0x1c -stcks 15000 \
-entp robrspts -auto -noreg

task -slotname robmasts -slotid 13 -pri 126 -vwopt 0x1c -stcks 66000 \
-entp robmasts -auto -noreg

task -slotname dipcts -slotid 122 -pri 126 -vwopt 0x1c -stcks 18000 \
-entp dipcts -auto -noreg

# Start stream support 
task -slotname streamts -slotid 81 -pri 120 -vwopt 0x1c -stcks 10000 \
-entp streamts -auto

# Start the stream read threads
task -slotname stream_readts1 -slotid 104 -entp stream_readts -pri 120 -vwopt 0x1c -stcks 8000 -auto
task -slotname stream_readts2 -slotid 105 -entp stream_readts -pri 120 -vwopt 0x1c -stcks 8000 -auto

###########################################		
# opt_l2.cmd - Option startup Sequence no 4
# Example of:
#  - users: Cap, Dap, Vision
#  - purpose: Start of option tasks, invoke init/start methods 
#  - dependencies: new -resource cab, creation of RobAPI/stream task
#  - why here: Must be done before the last synchronize/start of tasks
#
fileexist -path $INTERNAL/opt_l2.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $INTERNAL/opt_l2.cmd
#NEXT_STEP

task -slotname ns_send -slotid 78 -entp robnetscansendts -pri 98 -vwopt 0x1c \
-stcks 35000 -auto -noreg

ifvc -label NETSCANSKIP
task -slotname ns_receive -slotid 87 -entp robnetscanreceivets -pri 99 -vwopt 0x1c \
-stcks 10000 -auto -noreg
#NETSCANSKIP

synchronize -level task
rapid_startup_ready
cab_startup_ready
go -level task

### Notify CHANREF data is valid for PSC operation
ifvc -label CHANREF
invoke -entry pscio_chanref_valid -noarg -nomode
#CHANREF

ifvc -label VTSPEED
goto -label VTSPEED_END
#VTSPEED
# Set Virtual Time Speed to 100% for VC at end of startup-script
invoke -entry VTSetSpeed -arg 100 -nomode

#VTSPEED_END

ifvc -label VCSHELL
goto -label VCSHELL_END
#VCSHELL
task -slotname vcshell -entp vcshell -pri 120 -vwopt 0x1c -stcks 50000 -auto -noreg -nosync
#VCSHELL_END

###########################################		
# opt_l3.cmd - Option startup Sequence no 5
# Example of:
#  - users: Paint
#  - purpose: Set of enable signal, sysdmp_add 
#  - dependencies: rapid_startup_ready/cab_startup_ready
#  - why here: No need to synch/start any option task
#
fileexist -path $INTERNAL/opt_l3.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $INTERNAL/opt_l3.cmd
#NEXT_STEP 

### Reset firmware upgrade error monitoring. No device upgrade methods may be invoked after this point.
upgrade_warm_start_completed

# Notify user if this is an unofficial RobotWare release in a customer system
unofficial_rw_notification

sysdmp_add -show alarm_show_failed -source alarm_failed
# Add IPC to the sysdump
sysdmp_add -show ipc_show
# Add elog to the sysdump (dumps elog messages to a text file)
sysdmp_add -class elog -fileext txt
# Add semaphore info to the sysdump
sysdmp_add -show sysdmp_sem_show
# Add devices info to system dump service
sysdmp_add -show devices_show
# Add ipm to the sysdump
sysdmp_add -show ipm_show_data -source ipm_data
# Add pgm info to system dump service
sysdmp_add -show rapid_show_pgm -source pgm
# Add pgmrun info to system dump service
sysdmp_add -show rapid_show_pgmrun -source pgmrun 
# Add pgmrun info to system dump service
sysdmp_add -source pgmrun_spy_circular -start pgmrun_spy_circular_start -stop pgmrun_spy_circular_stop -save pgmrun_spy_circular_save_log -fileext log
# Add pgmexe info to system dump service
sysdmp_add -show rapid_show_pgmexe_all -source pgmexe
# Add operator mode info to system dump service
sysdmp_add -show operator_mode_show -source operator_mode
# Add master control info to system dump service
sysdmp_add -show master_control_show -source mastership
# Add SystemRead info to system dum service
sysdmp_add -show systemread_show -source systemread
# Add robdisp info to system dump service
sysdmp_add -source robdisp_show -start robdisp_show_sysdmp_start -stop robdisp_show_sysdmp_stop -save robdisp_show_sysdmp_save -fileext log
# Add bspNetDump info to system dump service
sysdmp_add -show bspNetDump
# Add mchw_show info to system dump service
sysdmp_add -show mchw_show
# Add FlexPendant dump info to system dump service
sysdmp_add -source fpcmd -save fpcmd_diagnostics -fileext log
# Add EIO to system dump service
sysdmp_add -source eio_sysdmp -save eio_sysdmp
# Add Bulletin Board info to system dump service
sysdmp_add -show BulletinBoard_show_ex
# Add SysEnergy info to system dump service
sysdmp_add -show sysenergy_show
# Add Semaphore info to system dump service
sysdmp_add -show os_sem_show_all
# Add C++ memory pool info to system dump service
sysdmp_add -show RobotWare_Memory_Pool_ShowInfoForAllPoolsInSysDmp
# Add rlcyclic info to the system dump service
sysdmp_add -show RlcyclicShow -source CyclicBool
# Add rldryrun info to the system dump service
sysdmp_add -show RlDryRunShow -source DryRun
# Add Appweb info to system dump service
sysdmp_add -class sysdump_appweb -fileext log  
# Add RobotWebsevice info to system dump service
sysdmp_add -show rws_info_show
# Add robcmd_show_diag info to the system dump service
sysdmp_add -show robcmd_show_diag
# Add robesu_show_diag info to the system dump service
sysdmp_add -show robesu_show_diag
# Add sisma_log_show info to system dump service
sysdmp_add -show sisma_log_show


goto -label NEXT_STEP
#SHELL_SCRIPT_TASK
task -slotname sysdmp_shell_script -slotid 18 -entp sysdmp_shell_script -pri 250 -vwopt 0x1c -stcks 25000 -auto
synchronize -level task
go -level task
sysdmp_add -class sysdmp_shell_script
#NEXT_STEP

# Generate sysdump on "refma underrun"
sysdmp_trigger_add -elog_domain 5 -elog_number 226
# Generate sysdump on "Communication lost with Drive Module"
sysdmp_trigger_add -elog_domain 3 -elog_number 9520
# Generate sysdump on "Overrun of Receive Feedback task"
sysdmp_trigger_add -elog_internal "Overrun of Receive Feedback task"
# Generate sysdump on "refma command queue timeout"
sysdmp_trigger_add -elog_domain 5 -elog_number 235
# Generate sysdump on "axc underrun of references"
sysdmp_trigger_add -elog_domain 5 -elog_number 430
# Generate sysdump on "servo underrun"
sysdmp_trigger_add -elog_domain 5 -elog_number 82
# Generate sysdump on "servo tool error"
sysdmp_trigger_add -elog_domain 5 -elog_number 248
# Generate sysdump on "servogun opening error"
sysdmp_trigger_add -elog_domain 5 -elog_number 251
# Generate sysdump on "In Position timeout"
sysdmp_trigger_add -elog_domain 5 -elog_number 308

# Additional log files to be copied to the sysdump
sysdmp_add_logfile -move 0 -file "$INTERNAL/upgrade.log"
sysdmp_add_logfile -move 0 -file "$INTERNAL/install.log"
sysdmp_add_logfile -move 0 -file "$INTERNAL/startup.log"
sysdmp_add_logfile -move 0 -file "$INTERNAL/ESTARTUP.LOG"
sysdmp_add_logfile -move 0 -file "$INTERNAL/SHUTDOWN.LOG"
sysdmp_add_logfile -move 0 -file "$CTRLROOT/pf_info.log"

# Include command file for additional logging mechanisms 

# Skip uprobe_default_start if requested by service_debug_early
# That is done by adding the folling row to the service_debug_early file
# setstr -strvar $UPROBE_DEF_START -value "no"
ifstr -strvar $UPROBE_DEF_START -value "no" -label UPROBE_DEF_SKIP
include -path $RELEASE/system/uprobe_default_start.cmd
#UPROBE_DEF_SKIP

fileexist -path $SYSTEM/service_debug.cmd -label LOAD_CMD
goto -label NEXT_STEP
#LOAD_CMD
include -path $SYSTEM/service_debug.cmd
sysdmp_add_logfile -move 0 -file "$SYSTEM/service_debug.cmd"
#NEXT_STEP

ifvc -label NEXT_STEP
# Report if system.xml has been restored
fileexist -path $INTERNAL/system.xml.err -label REPORT_SYSTEM_XML
goto -label NEXT_STEP
#REPORT_SYSTEM_XML
delete -path $INTERNAL/system.xml.err
print -text "*** RESTORE OF SYSTEM.XML HAS BEEN DONE ***"
#NEXT_STEP

ifvc -label NEXT_STEP
# Report if RW6system.xml has been restored
fileexist -path $INTERNAL/RW6system.xml.err -label REPORT_RW6_SYSTEM_XML
goto -label NEXT_STEP
#REPORT_RW6_SYSTEM_XML
delete -path $INTERNAL/RW6system.xml.err
print -text "*** RESTORE OF RW6SYSTEM.XML HAS BEEN DONE ***"
#NEXT_STEP

invoke -entry RobotWare_Memory_Pool_AddAllPoolsToDevices -noarg -nomode

systemrun

# Handle post-update
fileexist -path $INTERNAL/post_startup_action.cmd -label LOAD_CMD
goto -label NO_POST_ACTION
#LOAD_CMD
fileexist -path $INTERNAL/post_startup_action.cmd.old -label DEL_OLD
goto -label NEXT_STEP
#DEL_OLD
delete -path $INTERNAL/post_startup_action.cmd.old
#NEXT_STEP
rename -from $INTERNAL/post_startup_action.cmd -to $INTERNAL/post_startup_action.cmd.old
include -path $INTERNAL/post_startup_action.cmd.old
delete -path $INTERNAL/post_startup_action.cmd.old
#NO_POST_ACTION

goto -label NO_ERROR

ifvc -label NO_ERROR
#SYSTEM_ERROR
echo -text "Error during system startup!"
echo -text "The system is corrupt, try to reinstall the system."
echo -text "Forcing startup of the BootServer..."
report_corrupt_rw
delete -path /hd0a/internal/active
delay -time 1000
restart
#NO_ERROR
