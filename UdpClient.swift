//
//  UdpClient.swift
//  MCRemoteClient
//
//  UDP 客户端，负责与 Windows 服务端通信
//  控制指令端口: 8888
//  视频流端口: 8889
//

import Foundation
import Network

class UdpClient {
    static let shared = UdpClient()

    private var connection: NWConnection?
    private var videoConnection: NWConnection?
    private var host: NWEndpoint.Host?
    private let controlPort: UInt16 = 8888
    private let videoPort: UInt16 = 8889

    // 视频帧重组
    private var frameBuffer: [Int: [Int: Data]] = [:] // frameNumber -> [packetIndex: data]
    private var frameInfo: [Int: (totalPackets: Int, receivedPackets: Int)] = [:]
    private let maxCachedFrames = 5

    // 回调
    var onConnected: (() -> Void)?
    var onDisconnected: (() -> Void)?
    var onVideoFrame: ((UIImage) -> Void)?
    var onLog: ((String) -> Void)?

    private init() {}

    // MARK: - 连接

    func connect(to ipAddress: String) {
        host = NWEndpoint.Host(ipAddress)

        // 控制连接
        let controlEndpoint = NWEndpoint.hostPort(host: host!, port: NWEndpoint.Port(rawValue: controlPort)!)
        connection = NWConnection(to: controlEndpoint, using: .udp)
        connection?.stateUpdateHandler = { [weak self] state in
            switch state {
            case .ready:
                self?.onLog?("控制通道已连接")
                self?.onConnected?()
                self?.startReceivingControl()
                // 发送心跳
                self?.sendPing()
            case .failed(let error):
                self?.onLog?("连接失败: \(error.localizedDescription)")
                self?.onDisconnected?()
            default:
                break
            }
        }
        connection?.start(queue: .global())

        // 视频连接
        let videoEndpoint = NWEndpoint.hostPort(host: host!, port: NWEndpoint.Port(rawValue: videoPort)!)
        videoConnection = NWConnection(to: videoEndpoint, using: .udp)
        videoConnection?.stateUpdateHandler = { [weak self] state in
            switch state {
            case .ready:
                self?.onLog?("视频通道已连接")
                self?.startReceivingVideo()
            default:
                break
            }
        }
        videoConnection?.start(queue: .global(qos: .userInteractive))
    }

    func disconnect() {
        connection?.cancel()
        videoConnection?.cancel()
        connection = nil
        videoConnection = nil
        frameBuffer.removeAll()
        frameInfo.removeAll()
        onDisconnected?()
    }

    // MARK: - 发送控制指令

    /// 发送按键指令
    func sendKey(key: String, action: String) {
        let message = "type=key;key=\(key);action=\(action)"
        sendControlMessage(message)
    }

    /// 发送鼠标按键指令
    func sendMouseButton(button: String, action: String) {
        let message = "type=mouse;button=\(button);action=\(action)"
        sendControlMessage(message)
    }

    /// 发送鼠标移动指令
    func sendMouseMove(dx: Int, dy: Int) {
        let message = "type=mousemove;dx=\(dx);dy=\(dy)"
        sendControlMessage(message)
    }

    /// 发送心跳
    private func sendPing() {
        sendControlMessage("type=ping")
    }

    private func sendControlMessage(_ message: String) {
        guard let connection = connection,
              let data = message.data(using: .utf8) else { return }

        connection.send(content: data, completion: .contentProcessed { error in
            if let error = error {
                self.onLog?("发送失败: \(error.localizedDescription)")
            }
        })
    }

    // MARK: - 接收控制数据

    private func startReceivingControl() {
        connection?.receiveMessage { [weak self] content, _, isComplete, error in
            if let data = content, !data.isEmpty {
                let message = String(data: data, encoding: .utf8) ?? ""
                self?.onLog?("收到: \(message)")
            }
            if error == nil {
                self?.startReceivingControl()
            }
        }
    }

    // MARK: - 接收视频数据

    private func startReceivingVideo() {
        videoConnection?.receiveMessage { [weak self] content, _, isComplete, error in
            if let data = content, data.count >= 8 {
                self?.processVideoPacket(data)
            }
            if error == nil {
                self?.startReceivingVideo()
            }
        }
    }

    /// 处理视频数据包
    /// 包格式: 4字节帧号(大端) + 4字节包信息(高16位总包数, 低16位当前包序号) + JPEG数据
    private func processVideoPacket(_ data: Data) {
        guard data.count >= 8 else { return }

        // 解析帧号
        let frameNumber = data.withUnsafeBytes { ptr -> Int in
            let value = ptr.load(fromByteOffset: 0, as: Int32.self)
            return Int(value)
        }

        // 解析包信息
        let packetInfo = data.withUnsafeBytes { ptr -> Int in
            let value = ptr.load(fromByteOffset: 4, as: Int32.self)
            return Int(value)
        }
        let totalPackets = (packetInfo >> 16) & 0xFFFF
        let currentPacket = packetInfo & 0xFFFF

        // JPEG 数据
        let jpegChunk = data.subdata(in: 8..<data.count)

        // 存储数据包
        if frameBuffer[frameNumber] == nil {
            frameBuffer[frameNumber] = [:]
            frameInfo[frameNumber] = (totalPackets: totalPackets, receivedPackets: 0)
        }

        frameBuffer[frameNumber]?[currentPacket] = jpegChunk
        frameInfo[frameNumber]?.receivedPackets += 1

        // 检查是否收齐一帧
        guard let info = frameInfo[frameNumber],
              info.receivedPackets >= info.totalPackets,
              let packets = frameBuffer[frameNumber] else { return }

        // 重组 JPEG 数据
        var frameData = Data()
        for i in 0..<info.totalPackets {
            if let packet = packets[i] {
                frameData.append(packet)
            }
        }

        // 转换为 UIImage
        if let image = UIImage(data: frameData) {
            DispatchQueue.main.async { [weak self] in
                self?.onVideoFrame?(image)
            }
        }

        // 清理已完成的帧
        frameBuffer.removeValue(forKey: frameNumber)
        frameInfo.removeValue(forKey: frameNumber)

        // 清理过期帧
        cleanupOldFrames(currentFrame: frameNumber)
    }

    private func cleanupOldFrames(currentFrame: Int) {
        let oldFrames = frameBuffer.keys.filter { $0 < currentFrame - maxCachedFrames }
        for frame in oldFrames {
            frameBuffer.removeValue(forKey: frame)
            frameInfo.removeValue(forKey: frame)
        }
    }
}
