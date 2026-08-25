//
//  AppDelegate.swift
//  MCRemoteClient
//

import UIKit

@main
class AppDelegate: UIResponder, UIApplicationDelegate {

    var window: UIWindow?

    func application(_ application: UIApplication,
                     didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {

        window = UIWindow(frame: UIScreen.main.bounds)
        let connectVC = ConnectViewController()
        let navController = UINavigationController(rootViewController: connectVC)
        navController.setNavigationBarHidden(true, animated: false)
        window?.rootViewController = navController
        window?.makeKeyAndVisible()

        return true
    }

    func applicationWillResignActive(_ application: UIApplication) {
        // 应用进入后台时断开连接
        // UdpClient.shared.disconnect()
    }

    func applicationDidEnterBackground(_ application: UIApplication) {
        // 保持 UDP 连接（局域网内后台可能受限）
    }

    func applicationWillEnterForeground(_ application: UIApplication) {
        // 恢复连接
    }
}
