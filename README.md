PortTran by k8gege<br>
.NET端口转发工具 支持任意权限<br>

0x001 外网VPS上运行PortTranS<br>
PortTranS.exe 用于中转的端口 用于实际链接的端口<br>
PortTranS.exe 8000 338<br>
<img src=https://github.com/k8gege/PortTran/blob/master/img/vps.PNG></img><br>
0x002 目标内网服务器上运行PortTranC<br>
PortTranC.exe 内网主机IP(可以是127.0.0.1或者内网IP) 内网应用端口 外网主机IP 外网主机用于转发的端口<br>
PortTranC.exe 127.0.0.1 3389 111.11.111.111 8000<br>
<img src=https://github.com/k8gege/PortTran/blob/master/img/target.PNG></img><br>

0x003 VPS连接3389，其它客户链接 外网主机IP:用于实际链接的端口 后将会被导向到内网主机对应端口<br>
mstsc /console /v:127.0.0.1:338<br>
<img src=https://github.com/k8gege/PortTran/blob/master/img/3389.PNG></img><br>

# Useage 2, from local to terminal(as some kind of proxy)
# 使用方式2， 虚拟外网地址为本机地址（作为代理使用）
- 127.0.0.1:localport ------> 127.0.0.1:transferport  ------> remoteIP:remoteport

- step1, run on local:步骤1， 内网主机运行PortTranS20 用于中转的端口 用于实际链接的端口
```
 PortTranS20.exe transferport localport, eg
> ./PortTranS20.exe 8000 338
[+] Listening TranPort 8000 ...
[+] Listening ConnPort 338 ...
```
- step2, run on local 步骤2， PortTranC20 远程主机IP 远程主机端口  内网主机IP 内网主机用于用于中转的端口
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
- step3, run application, eg, remote desktop. 这样你可以使用 127.0.0.1:338来访问外网主机的remoteIP:remoteport

now you could just write 127.0.0.1:338, actually you will be transfer to remoteIP:remoteport


# TODO
## VS2019 + netcore3.1
- vb netcore转发延迟过高, server端崩溃频繁。原版c#测试暂时没发现过高问题
- 原版server+vb netcore client，未发现延迟过高问题不过感觉比原版稍差

## VS2017 + netcore2.1
- test netcore2.1 https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-2.1.519-windows-x64-installer
