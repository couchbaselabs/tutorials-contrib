//
//  AppPreferenceManager.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/4/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import UIKit


/// Class to save to user defaults the required persistent data (username, password, etc)
class AppPreferenceManager {
    static let UsernameKey: String = "UsernameKey"
    static let PasswordKey: String = "PasswordKey"
    
    /// Constructor
    init() {
        
    }
    
    /// Singleton instance of `AppPreferenceManager`
    static let shared: AppPreferenceManager = AppPreferenceManager()

    fileprivate let defaults = UserDefaults.standard
    
    func setUsername(_ value: String) {
        defaults.setValue(value, forKey: AppPreferenceManager.UsernameKey)
        defaults.synchronize()
    }
    
    func getUsername() -> String? {
        return defaults.string(forKey: AppPreferenceManager.UsernameKey)
    }
    
    func setPassword(_ value: String) {
        defaults.setValue(value, forKey: AppPreferenceManager.PasswordKey)
        defaults.synchronize()
    }
    
    func getPassword() -> String? {
        return defaults.string(forKey: AppPreferenceManager.PasswordKey)
    }
}
