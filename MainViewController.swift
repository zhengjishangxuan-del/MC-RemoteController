//
//  MainViewController.swift
//  MCRemoteClient
//
//  主控制页面 - 视频显示 + 触屏操控界面
//

import UIKit

class MainViewController: UIViewController {

    // MARK: - 视频显示
    private let videoImageView: UIImageView = {
        let iv = UIImageView()
        iv.contentMode = .scaleAspectFit
        iv.backgroundColor = .black
        iv.translatesAutoresizingMaskIntoConstraints = false
        return iv
    }()

    // MARK: - 触控板区域（虚拟鼠标）
    private let touchpadView: UIView = {
        let view = UIView()
        view.backgroundColor = UIColor.systemGray5.withAlphaComponent(0.5)
        view.layer.borderWidth = 1
        view.layer.borderColor = UIColor.systemGray3.cgColor
        view.layer.cornerRadius = 8
        view.translatesAutoresizingMaskIntoConstraints = false
        view.isUserInteractionEnabled = true
        return view
    }()

    private let touchpadLabel: UILabel = {
        let label = UILabel()
        label.text = "触控板 (滑动转视角)"
        label.font = .systemFont(ofSize: 12)
        label.textColor = .secondaryLabel
        label.textAlignment = .center
        label.translatesAutoresizingMaskIntoConstraints = false
        return label
    }()

    // MARK: - 控制按钮
    private var controlButtons: [String: UIButton] = [:]

    // 方向键 W A S D
    private let wasdStack: UIStackView = {
        let sv = UIStackView()
        sv.axis = .vertical
        sv.spacing = 6
        sv.alignment = .center
        sv.translatesAutoresizingMaskIntoConstraints = false
        return sv
    }()

    // 功能键区
    private let functionStack: UIStackView = {
        let sv = UIStackView()
        sv.axis = .horizontal
        sv.spacing = 6
        sv.distribution = .fillEqually
        sv.translatesAutoresizingMaskIntoConstraints = false
        return sv
    }()

    // 鼠标按键区
    private let mouseStack: UIStackView = {
        let sv = UIStackView()
        sv.axis = .horizontal
        sv.spacing = 8
        sv.distribution = .fillEqually
        sv.translatesAutoresizingMaskIntoConstraints = false
        return sv
    }()

    // 断开连接按钮
    private let disconnectButton: UIButton = {
        let btn = UIButton(type: .system)
        btn.setTitle("断开", for: .normal)
        btn.titleLabel?.font = .systemFont(ofSize: 14, weight: .medium)
        btn.tintColor = .systemRed
        btn.translatesAutoresizingMaskIntoConstraints = false
        return btn
    }()

    // 触控板状态
    private var lastTouchPoint: CGPoint?
    private let mouseSensitivity: CGFloat = 1.5

    // MARK: - 生命周期

    override func viewDidLoad() {
        super.viewDidLoad()
        setupUI()
        setupControls()
        setupGestures()
        setupVideoCallback()
    }

    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        // 如果页面消失，断开连接
        // UdpClient.shared.disconnect()
    }

    // MARK: - UI 布局

    private func setupUI() {
        view.backgroundColor = .systemBackground
        title = "MC 远程控制"

        // 隐藏导航栏以获得更大空间
        navigationController?.setNavigationBarHidden(true, animated: false)

        view.addSubview(videoImageView)
        view.addSubview(disconnectButton)
        view.addSubview(touchpadView)
        view.addSubview(touchpadLabel)
        view.addSubview(wasdStack)
        view.addSubview(functionStack)
        view.addSubview(mouseStack)

        // 视频区域 - 上半部分
        NSLayoutConstraint.activate([
            videoImageView.topAnchor.constraint(equalTo: view.safeAreaLayoutGuide.topAnchor),
            videoImageView.leadingAnchor.constraint(equalTo: view.leadingAnchor),
            videoImageView.trailingAnchor.constraint(equalTo: view.trailingAnchor),
            videoImageView.heightAnchor.constraint(equalTo: view.heightAnchor, multiplier: 0.45),

            disconnectButton.topAnchor.constraint(equalTo: view.safeAreaLayoutGuide.topAnchor, constant: 8),
            disconnectButton.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -12),
        ])

        // WASD 方向键 - 左下
        NSLayoutConstraint.activate([
            wasdStack.bottomAnchor.constraint(equalTo: view.safeAreaLayoutGuide.bottomAnchor, constant: -20),
            wasdStack.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 16),
            wasdStack.widthAnchor.constraint(equalToConstant: 120),
        ])

        // 鼠标按键 - 右下
        NSLayoutConstraint.activate([
            mouseStack.bottomAnchor.constraint(equalTo: view.safeAreaLayoutGuide.bottomAnchor, constant: -20),
            mouseStack.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -16),
            mouseStack.widthAnchor.constraint(equalToConstant: 140),
            mouseStack.heightAnchor.constraint(equalToConstant: 50),
        ])

        // 功能键 - 底部中间
        NSLayoutConstraint.activate([
            functionStack.bottomAnchor.constraint(equalTo: mouseStack.topAnchor, constant: -12),
            functionStack.leadingAnchor.constraint(equalTo: wasdStack.trailingAnchor, constant: 8),
            functionStack.trailingAnchor.constraint(equalTo: mouseStack.leadingAnchor, constant: -8),
            functionStack.heightAnchor.constraint(equalToConstant: 40),
        ])

        // 触控板 - 中间偏右
        NSLayoutConstraint.activate([
            touchpadView.topAnchor.constraint(equalTo: videoImageView.bottomAnchor, constant: 12),
            touchpadView.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -16),
            touchpadView.widthAnchor.constraint(equalToConstant: 160),
            touchpadView.bottomAnchor.constraint(equalTo: functionStack.topAnchor, constant: -12),

            touchpadLabel.topAnchor.constraint(equalTo: touchpadView.bottomAnchor, constant: 4),
            touchpadLabel.centerXAnchor.constraint(equalTo: touchpadView.centerXAnchor),
        ])
    }

    // MARK: - 创建控制按钮

    private func setupControls() {
        // WASD 布局
        let wRow = createButtonRow(keys: ["W"])
        let adRow = createButtonRow(keys: ["A", "S", "D"])
        wasdStack.addArrangedSubview(wRow)
        wasdStack.addArrangedSubview(adRow)

        // 功能键
        let functionKeys = ["Shift", "E", "F1", "F2", "F3", "F5"]
        for key in functionKeys {
            let btn = createControlButton(title: key, key: key)
            functionStack.addArrangedSubview(btn)
        }

        // 鼠标按键
        let leftBtn = createMouseButton(title: "左键", button: "left")
        let rightBtn = createMouseButton(title: "右键", button: "right")
        mouseStack.addArrangedSubview(leftBtn)
        mouseStack.addArrangedSubview(rightBtn)

        // 断开按钮
        disconnectButton.addTarget(self, action: #selector(disconnectTapped), for: .touchUpInside)
    }

    private func createButtonRow(keys: [String]) -> UIStackView {
        let row = UIStackView()
        row.axis = .horizontal
        row.spacing = 6
        row.distribution = .fillEqually
        for key in keys {
            let btn = createControlButton(title: key, key: key)
            row.addArrangedSubview(btn)
        }
        return row
    }

    private func createControlButton(title: String, key: String) -> UIButton {
        let btn = UIButton(type: .system)
        btn.setTitle(title, for: .normal)
        btn.titleLabel?.font = .systemFont(ofSize: 16, weight: .semibold)
        btn.backgroundColor = .secondarySystemFill
        btn.setTitleColor(.label, for: .normal)
        btn.layer.cornerRadius = 8
        btn.translatesAutoresizingMaskIntoConstraints = false
        btn.widthAnchor.constraint(equalToConstant: 38).isActive = true
        btn.heightAnchor.constraint(equalToConstant: 38).isActive = true

        // 按下
        btn.addTarget(self, action: #selector(buttonDown(_:)), for: .touchDown)
        // 松开（包括手指滑出）
        btn.addTarget(self, action: #selector(buttonUp(_:)), for: [.touchUpInside, .touchUpOutside, .touchCancel])

        controlButtons[key] = btn
        return btn
    }

    private func createMouseButton(title: String, button: String) -> UIButton {
        let btn = UIButton(type: .system)
        btn.setTitle(title, for: .normal)
        btn.titleLabel?.font = .systemFont(ofSize: 15, weight: .semibold)
        btn.backgroundColor = .systemBlue.withAlphaComponent(0.15)
        btn.setTitleColor(.systemBlue, for: .normal)
        btn.layer.cornerRadius = 10

        btn.addTarget(self, action: #selector(mouseButtonDown(_:)), for: .touchDown)
        btn.addTarget(self, action: #selector(mouseButtonUp(_:)), for: [.touchUpInside, .touchUpOutside, .touchCancel])

        btn.tag = button == "left" ? 0 : 1
        return btn
    }

    // MARK: - 触控板手势

    private func setupGestures() {
        let panGesture = UIPanGestureRecognizer(target: self, action: #selector(handlePan(_:)))
        panGesture.maximumNumberOfTouches = 1
        touchpadView.addGestureRecognizer(panGesture)
    }

    @objc private func handlePan(_ gesture: UIPanGestureRecognizer) {
        let translation = gesture.translation(in: touchpadView)

        switch gesture.state {
        case .began:
            lastTouchPoint = translation
        case .changed:
            guard let last = lastTouchPoint else { return }
            let dx = Int((translation.x - last.x) * mouseSensitivity)
            let dy = Int((translation.y - last.y) * mouseSensitivity)
            if dx != 0 || dy != 0 {
                UdpClient.shared.sendMouseMove(dx: dx, dy: dy)
            }
            lastTouchPoint = translation
        case .ended, .cancelled:
            lastTouchPoint = nil
        default:
            break
        }
    }

    // MARK: - 按钮事件

    @objc private func buttonDown(_ sender: UIButton) {
        guard let key = controlButtons.first(where: { $0.value === sender })?.key else { return }
        UdpClient.shared.sendKey(key: key, action: "down")
        // 视觉反馈
        sender.backgroundColor = .systemBlue.withAlphaComponent(0.3)
    }

    @objc private func buttonUp(_ sender: UIButton) {
        guard let key = controlButtons.first(where: { $0.value === sender })?.key else { return }
        UdpClient.shared.sendKey(key: key, action: "up")
        sender.backgroundColor = .secondarySystemFill
    }

    @objc private func mouseButtonDown(_ sender: UIButton) {
        let button = sender.tag == 0 ? "left" : "right"
        UdpClient.shared.sendMouseButton(button: button, action: "down")
        sender.backgroundColor = .systemBlue.withAlphaComponent(0.4)
    }

    @objc private func mouseButtonUp(_ sender: UIButton) {
        let button = sender.tag == 0 ? "left" : "right"
        UdpClient.shared.sendMouseButton(button: button, action: "up")
        sender.backgroundColor = .systemBlue.withAlphaComponent(0.15)
    }

    @objc private func disconnectTapped() {
        let alert = UIAlertController(title: "断开连接", message: "确定要断开连接吗？", preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: "取消", style: .cancel))
        alert.addAction(UIAlertAction(title: "断开", style: .destructive) { _ in
            UdpClient.shared.disconnect()
            self.dismiss(animated: true)
        })
        present(alert, animated: true)
    }

    // MARK: - 视频回调

    private func setupVideoCallback() {
        UdpClient.shared.onVideoFrame = { [weak self] image in
            self?.videoImageView.image = image
        }

        UdpClient.shared.onDisconnected = { [weak self] in
            DispatchQueue.main.async {
                self?.dismiss(animated: true)
            }
        }
    }

    // MARK: - 状态栏样式

    override var prefersStatusBarHidden: Bool {
        return true
    }
}
