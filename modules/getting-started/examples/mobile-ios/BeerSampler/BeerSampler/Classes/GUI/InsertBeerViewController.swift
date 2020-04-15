//
//  InsertBeerViewController.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/5/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import UIKit
import CouchbaseLiteSwift

class InsertBeerViewController: BaseViewController {
    
    var accessMode: AccessMode = AccessMode.Guest
    
    @IBOutlet weak var edtBrewery: UITextField!
    @IBOutlet weak var edtBeerName: UITextField!
    @IBOutlet weak var edtCategory: UITextField!
    @IBOutlet weak var edtStyle: UITextField!
    @IBOutlet weak var edtAVB: UITextField!
    @IBOutlet weak var btnInsert: UIButton!
    @IBOutlet weak var lblHeader: UILabel!
    
    var beer: Beer? = nil

    override func viewDidLoad() {
        super.viewDidLoad()

        // Do any additional setup after loading the view.
        btnInsert.layer.cornerRadius = 5
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        
        if (beer != nil) {
            if let foundBeer = BeerQueries.shared.fetchBeerByID(beer!.id) {
                beer = foundBeer
            }
            btnInsert.setTitle("UPDATE", for: UIControl.State.normal)
            edtBrewery.isEnabled = false
            edtBeerName.isEnabled = false
            lblHeader.text = "Update \(beer?.name ?? "beer")"
            self.title = beer?.name ?? "Update beer"
            
            fillDataToUI(beer: beer!)
        } else {
            self.title = "Insert beer"
            btnInsert.setTitle("INSERT", for: UIControl.State.normal)
            lblHeader.text = "Insert a new beer"
            edtBrewery.isEnabled = true
            edtBeerName.isEnabled = true
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
    
    func fillDataToUI(beer: Beer) {
        edtBrewery.text = beer.brewery
        edtBeerName.text = beer.name
        edtCategory.text = beer.category
        edtStyle.text = beer.style
        edtAVB.text = beer.abv
    }

    func insertBeer() {
        // Retrieving beer data from the interface
        if (!(edtBrewery.text?.isEmpty ?? true) &&
            !(edtBeerName.text?.isEmpty ?? true) /*&&
            !(edtCategory.text?.isEmpty ?? true) &&
            !(edtStyle.text?.isEmpty ?? true) &&
            !(edtAVB.text?.isEmpty ?? true)*/) {
            let brewery = edtBrewery.text!
            let beerName = edtBeerName.text!
            let category = edtCategory.text!
            let style = edtStyle.text!
            let avb = edtAVB.text!
            var username = "guest"
            if (accessMode == AccessMode.AuthenticatedUser) {
                username = AppPreferenceManager.shared.getUsername() ?? ""
            }
            
            let result = BeerQueries.shared.insertBeer(brewery: brewery, beerName: beerName, category: category, style: style, abv: avb, username: username)
            if (result) {
                self.navigationController?.popViewController(animated: true)
            } else {
                NSLog("\(AppDelegate.appLogTag): Error inserting beer")
            }
        } else {
            NSLog("\(AppDelegate.appLogTag): Empty fields")
        }
    }
    
    @IBAction func insertBeerAction(_ sender: Any) {
        insertBeer()
    }
}
