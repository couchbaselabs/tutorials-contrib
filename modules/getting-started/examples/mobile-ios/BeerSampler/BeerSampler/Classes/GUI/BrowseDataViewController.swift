//
//  BrowseDataViewController.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/5/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import UIKit
import CouchbaseLiteSwift

/// Enum to indicate if the beer list is loaded for guest or authenticated user
enum AccessMode {
    case Guest              // Guest user
    case AuthenticatedUser  // Authenticated user
}

class BrowseDataViewController: BaseViewController, UITableViewDataSource, UITableViewDelegate {
    
    @IBOutlet weak var beersTableView: UITableView!
    @IBOutlet weak var btnInsert: UIButton!
    
    // Indicates if databse will be opened for guest or authenticated user
    var accessMode: AccessMode = AccessMode.Guest
    // The beer list
    var beersArray: [Beer] = [Beer]()
    // The selected beer for edit
    var selectedBeer: Beer? = nil

    override func viewDidLoad() {
        super.viewDidLoad()

        // Do any additional setup after loading the view.
        
        beersTableView.tableFooterView = UIView(frame: CGRect.zero)
        beersTableView.separatorColor = UIColor.clear
        beersTableView.separatorStyle = UITableViewCell.SeparatorStyle.none
        
        btnInsert.layer.cornerRadius = 5
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        
        // Register for notification about replicator start completed to fetch beers again (pulling beers from cloud is finished)
        NotificationCenter.default.addObserver(self, selector: #selector(fetchBeers), name: DatabaseManager.notificationSyncGateWayCompleted, object: nil)
        
        // Fetch beers array
        fetchBeers()
        
        self.navigationController?.isNavigationBarHidden = false
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        
        NotificationCenter.default.removeObserver(self, name: DatabaseManager.notificationSyncGateWayCompleted, object: nil)
    }
    
    @objc func fetchBeers() {
        NSLog("\(AppDelegate.appLogTag): fetching beers");
        
        // Fetch beers array
        beersArray = BeerQueries.shared.fetchBeers()
        
        beersTableView.reloadData()
    }

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */
    
    // MARK: - TableView datasource
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return beersArray.count
    }
    
    public func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return 44.0
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell: BeerTableViewCell? = tableView.dequeueReusableCell(withIdentifier: "BeerTableViewCellID", for: indexPath) as? BeerTableViewCell
        
        cell?.lblBeerName.text = beersArray[indexPath.row].name

        return cell!
    }
    
    // MARK: - TableView delegate
    ///:nodoc:
    public func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        tableView.deselectRow(at: indexPath, animated: true)
        
        selectedBeer = beersArray[indexPath.row]
        // Update the beer
        insertBeer()
    }
    
    func insertBeer() {
        performSegue(withIdentifier: "showInsertBeer", sender: self)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if let vcc = segue.destination as? InsertBeerViewController {
            vcc.accessMode = accessMode
            vcc.beer = selectedBeer
            selectedBeer = nil
        }
    }
}
