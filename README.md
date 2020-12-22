PortTran by k8gege<br>
.NET端口转发工具 支持任意权限<br>

0x001 外网VPS监听<br>
PortTranS.exe 8000 338<br>
<img src=https://github.com/k8gege/PortTran/blob/master/img/vps.PNG></img><br>
0x002 目标内网转发<br>
PortTranC.exe 192.168.85.169 3389 192.168.85.142 8000<br>
<img src=https://github.com/k8gege/PortTran/blob/master/img/target.PNG></img><br>

0x003 VPS连接3389<br>
mstsc /console /v:127.0.0.1:338<br>
<img src=https://github.com/k8gege/PortTran/blob/master/img/3389.PNG></img><br>

# Useage 2, from local to terminal(as some kind of proxy)

- 127.0.0.1:localport ------> 127.0.0.1:transferport  ------> remoteIP:remoteport

- step1, run on local:
```
 PortTranS20.exe transferport localport, eg
> ./PortTranS20.exe 8000 338
[+] Listening TranPort 8000 ...
[+] Listening ConnPort 338 ...
```
- step2, run on local
```
./PortTranC20.exe remoteIP:remoteport 127.0.0.1:transferport
./PortTranC20.exe 111.11.111.111 3389 127.0.0.1 8000
[+] Make a Connection to 127.0.0.1:8000...

now on ./PortTranS20.exe you shall see two new line come up
[+] Listening TranPort 8000 ...
[+] Listening ConnPort 338 ...
[+] Accept Connect OK!
[+] Waiting Another Client on ConnPort ...
```
- step3, run application, eg, remote desktop.

now you could just write 127.0.0.1:338, actually you will be transfer to remoteIP:remoteport
