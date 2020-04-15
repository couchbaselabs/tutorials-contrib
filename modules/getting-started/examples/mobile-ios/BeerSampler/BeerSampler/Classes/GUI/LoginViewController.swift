//
//  ViewController.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/4/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import UIKit

class LoginViewController: BaseViewController {
    @IBOutlet weak var lblHeader: UILabel!
    @IBOutlet weak var lblInfo1: UILabel!
    @IBOutlet weak var lblInfo2: UILabel!
    @IBOutlet weak var edtUsername: UITextField!
    @IBOutlet weak var edtPassword: UITextField!
    @IBOutlet weak var btnLogin: UIButton!
    @IBOutlet weak var btnGuest: UIButton!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        
        btnLogin.layer.cornerRadius = 5
        btnGuest.layer.cornerRadius = 5
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        
        // Set saved username / password from User defaults to UI controls
        edtUsername.text = AppPreferenceManager.shared.getUsername()
        edtPassword.text = AppPreferenceManager.shared.getPassword()
        
        self.navigationController?.isNavigationBarHidden = true
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        // Hide keyboard
        view.endEditing(true)
    }
    
    /// Login with a user. The database will be opened for the given user and the replicator to pull and push data to and from the cloud is started.
    /// - Parameter sender: Action sender
    @IBAction func loginAction(_ sender: Any) {
        if (!(edtUsername.text?.isEmpty ?? true) && !(edtPassword.text?.isEmpty ?? true)) {
            let user = edtUsername.text!
            let password = edtPassword.text!
            
            // Save username and password to user defaults
            AppPreferenceManager.shared.setUsername(user)
            AppPreferenceManager.shared.setPassword(password)
            
            NSLog("\(AppDelegate.appLogTag): Opening Database for user \(user)")
            // Open database for the given user
            DatabaseManager.shared.openDatabaseForUser(username: user)
            // Start the replicator to pull and push data from and to the cloud
            DatabaseManager.shared.startPushAndPullReplicationForCurrentUser(username: user, password: password)
            
            performSegue(withIdentifier: "showBrowseDataWithUser", sender: self)
        } else {
            NSLog("\(AppDelegate.appLogTag): Empty username or password")
        }
    }
    
    /// Open guest database. The database will be opened locally for guest user.
    /// - Parameter sender: Action sender
    @IBAction func accessAsGuestAction(_ sender: Any) {
        // Open database for guest user
        NSLog("\(AppDelegate.appLogTag): Opening local Database")
        DatabaseManager.shared.openGuestDatabase()
        performSegue(withIdentifier: "showBrowseDataAsGuest", sender: self)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if (segue.identifier == "showBrowseDataWithUser") {
            if let vcc = segue.destination as? BrowseDataViewController {
                vcc.accessMode = AccessMode.AuthenticatedUser
            }
        } else if (segue.identifier == "showBrowseDataAsGuest") {
            if let vcc = segue.destination as? BrowseDataViewController {
                vcc.accessMode = AccessMode.Guest
            }
        }
    }
}

