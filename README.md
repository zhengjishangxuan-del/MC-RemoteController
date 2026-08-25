# MC 远程控制器 - 我的世界局域网远程控制系统

## 项目简介

整套工程由两大部分组成：
- **Windows 电脑端服务程序**（C# / .NET Framework 4.8）
- **iPhone/iPad 客户端**（Swift / iOS 14+）

两台设备必须连接同一个家里的无线网络（同一个局域网）才可以互相通信。电脑全程必须保持开机状态。

---

## 目录结构

```
MC_RemoteController/
├── WindowsServer/              # Windows 服务端 C# 项目
│   ├── MCRemoteController.csproj
│   ├── Program.cs              # 程序入口
│   ├── MainForm.cs             # 主窗体逻辑
│   ├── MainForm.Designer.cs    # 主窗体设计
│   ├── MainForm.resx
│   ├── UdpServer.cs            # UDP 网络通信
│   ├── InputSimulator.cs       # 键盘鼠标模拟
│   ├── ScreenCapture.cs        # 屏幕捕获
│   ├── VideoStreamer.cs        # 视频编码推流
│   ├── App.config
│   └── Properties/
│       └── AssemblyInfo.cs
│
├── iOSClient/                  # iOS 客户端 Swift 项目
│   ├── MCRemoteClient.xcodeproj/
│   │   └── project.pbxproj
│   └── MCRemoteClient/
│       ├── AppDelegate.swift
│       ├── ConnectViewController.swift   # 连接页面
│       ├── MainViewController.swift      # 主控制页面
│       ├── UdpClient.swift               # UDP 通信
│       ├── Info.plist
│       ├── Assets.xcassets/
│       └── Base.lproj/
│           └── LaunchScreen.storyboard
│
├── codemagic.yaml              # Codemagic 云端编译配置
└── README.md                   # 本文件
```

---

## 第一部分：Windows 服务端编译与运行

### 环境要求
- Windows 10/11
- Visual Studio 2019/2022（安装 .NET 桌面开发工作负载）
- .NET Framework 4.8 SDK

### 编译步骤

1. **打开项目**
   - 启动 Visual Studio
   - 文件 → 打开 → 项目/解决方案
   - 选择 `WindowsServer/MCRemoteController.csproj`

2. **编译**
   - 菜单栏选择「生成」→「生成解决方案」（或按 Ctrl+Shift+B）
   - 编译成功后，exe 文件在 `bin/Release/MCRemoteController.exe`

3. **运行**
   - 双击 `MCRemoteController.exe`
   - 程序界面会显示本机局域网 IP 地址
   - 点击「启动服务」按钮开始监听

### 功能说明

| 功能 | 说明 |
|------|------|
| 显示本机 IP | 界面顶部显示局域网 IP，需填入手机客户端 |
| 启动/停止服务 | 开启或关闭 UDP 监听服务 |
| 一键打开 PCL2 | 自动查找并启动 PCL2 启动器 |
| 视频推流设置 | 可调整分辨率（640x360 ~ 1920x1080）和画质 |
| 实时日志 | 显示连接状态和按键执行记录 |

### 网络端口
- 控制指令端口：**8888**（UDP）
- 视频流端口：**8889**（UDP）

> 注意：Windows 防火墙可能会弹出提示，请选择「允许访问」。

---

## 第二部分：iOS 客户端编译（Codemagic 云端编译）

由于 Windows 电脑无法直接编译 iOS 应用，需要使用 Codemagic 云端编译平台。

### 步骤一：上传代码到 GitHub

1. 注册/登录 GitHub：https://github.com
2. 创建一个新的公开或私有仓库
3. 将整个 `MC_RemoteController` 项目文件夹上传到仓库

### 步骤二：配置 Codemagic

1. 注册/登录 Codemagic：https://codemagic.io
2. 使用 GitHub 账号授权登录
3. 在 Applications 页面选择你的仓库
4. Codemagic 会自动检测到 `codemagic.yaml` 配置文件

### 步骤三：配置代码签名（可选，用于真机安装）

要安装到 iPad/iPhone，需要对 IPA 进行签名：

**方式 A：使用 Apple 开发者账号（推荐）**
1. 在 Codemagic 项目设置中，进入「Code signing identities」
2. 点击「+」→ 选择「iOS App Development」
3. 登录你的 Apple ID，Codemagic 会自动创建证书和描述文件
4. Bundle ID 设置为：`com.mcremote.client`

**方式 B：不签名，后续用 Sideloadly 签名**
- 直接编译未签名的 IPA，然后用 Sideloadly 签名安装

### 步骤四：开始编译

1. 在 Codemagic 页面点击「Start new build」
2. 选择分支 `main`，Workflow 选择 `ios-client-build`
3. 等待编译完成（约 5-10 分钟）
4. 编译成功后，在 Artifacts 中下载 `.ipa` 文件

---

## 第三部分：IPA 侧载安装到 iPad

使用 Sideloadly 工具将 IPA 安装到你的 iPad。

### 环境要求
- Windows 电脑
- iPad（系统版本 27.0）
- 数据线（USB 连接电脑和 iPad）
- 免费普通 Apple ID（每 7 天需重签）

### 安装步骤

1. **下载 Sideloadly**
   - 官网：https://sideloadly.io
   - 下载 Windows 版本并安装

2. **连接 iPad**
   - 用数据线连接 iPad 和电脑
   - iPad 上弹出「是否信任此电脑」，选择「信任」

3. **签名并安装 IPA**
   - 打开 Sideloadly
   - 「Apple account」填入你的 Apple ID 邮箱
   - 「IPA file」选择下载好的 `MCRemoteClient.ipa`
   - 确认 Bundle ID 为 `com.mcremote.client`
   - 点击「Start」开始签名和安装
   - 过程中可能需要输入 Apple ID 密码或验证码

4. **信任开发者证书**
   - 安装完成后，iPad 桌面上会出现「MC远程控制」图标
   - 打开 iPad「设置」→「通用」→「VPN与设备管理」
   - 找到你的 Apple ID 对应的开发者证书，点击「信任」

5. **7 天重签**
   - 免费 Apple ID 签名的应用有效期为 7 天
   - 到期后需要重新连接电脑，用 Sideloadly 再次签名安装
   - 应用数据不会丢失

---

## 第四部分：使用说明

### 连接流程

1. **电脑端**
   - 运行 `MCRemoteController.exe`
   - 记下界面显示的「本机局域网IP」
   - 点击「启动服务」
   - （可选）点击「一键打开 PCL2 启动器」启动游戏

2. **iPad 端**
   - 确保 iPad 和电脑连接同一个 WiFi
   - 打开「MC远程控制」APP
   - 在输入框填入电脑显示的 IP 地址
   - 点击「连接」
   - 连接成功后自动进入控制界面

### 操控说明

| 按钮 | 功能 |
|------|------|
| W/A/S/D | 移动方向键 |
| Shift | 潜行（同时按下左右 Shift） |
| E | 打开背包 |
| F1/F2/F3/F5 | 功能键 |
| 左键 | 鼠标左键（支持长按） |
| 右键 | 鼠标右键（支持长按） |
| 触控板区域 | 手指滑动控制鼠标移动（转动视角） |

### 延迟说明
- 局域网投屏延迟约 30~150 毫秒
- 建议使用 5GHz WiFi 以获得更低延迟
- 可在电脑端降低分辨率和画质来减少延迟

---

## 通信协议说明

### 控制指令格式（UDP，端口 8888）
使用简单的键值对格式，分号分隔：

```
type=key;key=W;action=down
type=key;key=W;action=up
type=mouse;button=left;action=down
type=mousemove;dx=10;dy=-5
type=ping
```

### 视频流格式（UDP，端口 8889）
每帧 JPEG 图片分片发送，每个数据包格式：
- 4 字节：帧号（大端序 Int32）
- 4 字节：包信息（高 16 位总包数，低 16 位当前包序号）
- 剩余：JPEG 数据分片

---

## 常见问题

**Q: 手机连不上电脑？**
A: 检查是否在同一 WiFi；电脑防火墙是否放行；IP 地址是否正确。

**Q: 画面很卡？**
A: 降低分辨率到 854x480 或 640x360；降低画质；使用 5GHz WiFi。

**Q: 按键没反应？**
A: 确保游戏窗口是当前活动窗口；检查电脑端日志是否收到指令。

**Q: IPA 安装后打不开？**
A: 需要在设置中信任开发者证书；7 天后需要重签。

**Q: PCL2 启动器找不到？**
A: 点击按钮后会弹出文件选择框，手动选择 PCL2 的 exe 文件。

---

## 技术栈

- **Windows 端**：C# / .NET Framework 4.8 / WinForms / GDI / WinAPI SendInput
- **iOS 端**：Swift 5 / UIKit / Network.framework / UDP
- **云端编译**：Codemagic / Xcode
- **侧载工具**：Sideloadly

---

## 版本信息

- 版本：1.0.0
- 开发日期：2026年8月
