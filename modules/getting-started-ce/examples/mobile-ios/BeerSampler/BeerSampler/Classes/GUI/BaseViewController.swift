//
//  BaseViewController.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/5/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import UIKit

class BaseViewController: UIViewController {

    @IBOutlet weak var topSpace: NSLayoutConstraint!
    
    override func viewDidLoad() {
        super.viewDidLoad()

        // Do any additional setup after loading the view.
        // Update main view position according to device type
        if UIDevice().userInterfaceIdiom == .phone {
            var iPhoneX = false
            switch UIScreen.main.nativeBounds.height {
            case 2436:
                print("iPhone X/XS/11 Pro")
                iPhoneX = true
            case 2688:
                print("iPhone XS Max/11 Pro Max")
                iPhoneX = true
            case 1792:
                print("iPhone XR/ 11 ")
                iPhoneX = true
            default:
                print("Unknown")
            }
            if (!iPhoneX) {
                topSpace.constant = 20
            }
        }
    }
    

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
