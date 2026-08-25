//
//  ConnectViewController.swift
//  MCRemoteClient
//
//  连接页面 - 输入电脑局域网IP地址并连接
//

import UIKit

class ConnectViewController: UIViewController {

    private let titleLabel: UILabel = {
        let label = UILabel()
        label.text = "MC 远程控制器"
        label.font = .systemFont(ofSize: 28, weight: .bold)
        label.textAlignment = .center
        label.translatesAutoresizingMaskIntoConstraints = false
        return label
    }()

    private let subtitleLabel: UILabel = {
        let label = UILabel()
        label.text = "输入电脑端显示的局域网IP地址"
        label.font = .systemFont(ofSize: 14)
        label.textColor = .secondaryLabel
        label.textAlignment = .center
        label.translatesAutoresizingMaskIntoConstraints = false
        return label
    }()

    private let ipTextField: UITextField = {
        let tf = UITextField()
        tf.placeholder = "例如: 192.168.1.100"
        tf.font = .systemFont(ofSize: 18)
        tf.borderStyle = .roundedRect
        tf.keyboardType = .decimalPad
        tf.textAlignment = .center
        tf.translatesAutoresizingMaskIntoConstraints = false
        return tf
    }()

    private let connectButton: UIButton = {
        let btn = UIButton(type: .system)
        btn.setTitle("连接", for: .normal)
        btn.titleLabel?.font = .systemFont(ofSize: 18, weight: .semibold)
        btn.backgroundColor = .systemBlue
        btn.setTitleColor(.white, for: .normal)
        btn.layer.cornerRadius = 12
        btn.translatesAutoresizingMaskIntoConstraints = false
        return btn
    }()

    private let statusLabel: UILabel = {
        let label = UILabel()
        label.text = ""
        label.font = .systemFont(ofSize: 14)
        label.textColor = .secondaryLabel
        label.textAlignment = .center
        label.numberOfLines = 0
        label.translatesAutoresizingMaskIntoConstraints = false
        return label
    }()

    private let logTextView: UITextView = {
        let tv = UITextView()
        tv.font = .systemFont(ofSize: 12)
        tv.backgroundColor = .secondarySystemBackground
        tv.layer.cornerRadius = 8
        tv.isEditable = false
        tv.translatesAutoresizingMaskIntoConstraints = false
        return tv
    }()

    override func viewDidLoad() {
        super.viewDidLoad()
        setupUI()
        setupActions()

        // 设置 UDP 回调
        UdpClient.shared.onConnected = { [weak self] in
            DispatchQueue.main.async {
                self?.statusLabel.text = "连接成功！"
                self?.statusLabel.textColor = .systemGreen
                // 跳转到主控制页面
                DispatchQueue.main.asyncAfter(deadline: .now() + 0.5) {
                    self?.showMainController()
                }
            }
        }

        UdpClient.shared.onDisconnected = { [weak self] in
            DispatchQueue.main.async {
                self?.statusLabel.text = "连接已断开"
                self?.statusLabel.textColor = .systemRed
                self?.connectButton.isEnabled = true
                self?.connectButton.setTitle("连接", for: .normal)
            }
        }

        UdpClient.shared.onLog = { [weak self] message in
            DispatchQueue.main.async {
                let timestamp = DateFormatter.localizedString(from: Date(), dateStyle: .none, timeStyle: .medium)
                self?.logTextView.text += "[\(timestamp)] \(message)\n"
                // 滚动到底部
                let bottom = NSRange(location: self?.logTextView.text.count ?? 0, length: 1)
                self?.logTextView.scrollRangeToVisible(bottom)
            }
        }
    }

    private func setupUI() {
        view.backgroundColor = .systemBackground
        title = "连接"

        view.addSubview(titleLabel)
        view.addSubview(subtitleLabel)
        view.addSubview(ipTextField)
        view.addSubview(connectButton)
        view.addSubview(statusLabel)
        view.addSubview(logTextView)

        NSLayoutConstraint.activate([
            titleLabel.topAnchor.constraint(equalTo: view.safeAreaLayoutGuide.topAnchor, constant: 60),
            titleLabel.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 20),
            titleLabel.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -20),

            subtitleLabel.topAnchor.constraint(equalTo: titleLabel.bottomAnchor, constant: 12),
            subtitleLabel.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 20),
            subtitleLabel.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -20),

            ipTextField.topAnchor.constraint(equalTo: subtitleLabel.bottomAnchor, constant: 40),
            ipTextField.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 40),
            ipTextField.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -40),
            ipTextField.heightAnchor.constraint(equalToConstant: 50),

            connectButton.topAnchor.constraint(equalTo: ipTextField.bottomAnchor, constant: 30),
            connectButton.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 40),
            connectButton.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -40),
            connectButton.heightAnchor.constraint(equalToConstant: 50),

            statusLabel.topAnchor.constraint(equalTo: connectButton.bottomAnchor, constant: 20),
            statusLabel.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 20),
            statusLabel.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -20),

            logTextView.topAnchor.constraint(equalTo: statusLabel.bottomAnchor, constant: 20),
            logTextView.leadingAnchor.constraint(equalTo: view.leadingAnchor, constant: 20),
            logTextView.trailingAnchor.constraint(equalTo: view.trailingAnchor, constant: -20),
            logTextView.bottomAnchor.constraint(equalTo: view.safeAreaLayoutGuide.bottomAnchor, constant: -20),
        ])
    }

    private func setupActions() {
        connectButton.addTarget(self, action: #selector(connectTapped), for: .touchUpInside)

        // 点击空白处收起键盘
        let tapGesture = UITapGestureRecognizer(target: view, action: #selector(UIView.endEditing))
        view.addGestureRecognizer(tapGesture)
    }

    @objc private func connectTapped() {
        view.endEditing(true)

        guard let ip = ipTextField.text?.trimmingCharacters(in: .whitespacesAndNewlines),
              !ip.isEmpty else {
            statusLabel.text = "请输入IP地址"
            statusLabel.textColor = .systemRed
            return
        }

        // 简单的 IP 格式验证
        let ipRegex = "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$"
        guard ip.range(of: ipRegex, options: .regularExpression) != nil else {
            statusLabel.text = "IP地址格式不正确"
            statusLabel.textColor = .systemRed
            return
        }

        connectButton.isEnabled = false
        connectButton.setTitle("连接中...", for: .normal)
        statusLabel.text = "正在连接 \(ip)..."
        statusLabel.textColor = .secondaryLabel

        UdpClient.shared.connect(to: ip)
    }

    private func showMainController() {
        let mainVC = MainViewController()
        mainVC.modalPresentationStyle = .fullScreen
        present(mainVC, animated: true)
    }
}
