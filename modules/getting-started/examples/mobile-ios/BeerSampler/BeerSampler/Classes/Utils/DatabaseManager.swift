//
//  DatabaseManager.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/4/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import CouchbaseLiteSwift

/// The singleton `DatabaseManager` class contains the mehods to open the couchbase database for gust or registered users. Also allows to start and stop the replicator to sinchronize data to and from the cloud (for registerd users only).
final class DatabaseManager {
    
    static let notificationSyncGateWayCompleted: Notification.Name = Notification.Name(rawValue: "NotificationSyncGateWayCompleted")
    
    /// `Database` instance
    var database: Database? = nil
    
    /// `Replicator` instance
    var replicator: Replicator? = nil
    
    /// Constructor
    fileprivate init() {
        Database.log.console.level = LogLevel.debug
    }
    
    /// Singleton instance of `DatabaseManager`
    static let shared: DatabaseManager = DatabaseManager()
    
    /// Holds the current logged in username
    var currentUser: String = ""
    
    // Endpoint URL to access couchbase sync gateway
    static let syncGatewayEndpoint: String = "ws://192.168.0.100:4984/beer-sample"
    
    /// Gets the documents folder path for the app in the device
    func documentsPathString() -> String {
        return NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true).first ?? ""
    }
    
    /// Opens standalone database for guest user. CRUD operations will be possible.
    func openGuestDatabase() {
        NSLog("\(AppDelegate.appLogTag): Opening Guest Database")
        // Create the `DatabaseConfiguration` to open database
        let config: DatabaseConfiguration = DatabaseConfiguration()
        // Set directory in documents folder to save the local database instance
        config.directory = "\(documentsPathString())/guest"
        do {
            // Open the database with the defined configuration. If the database does not exists, it will be created.
            database = try Database(name: "guest", config: config)
        } catch let error {
            NSLog("\(AppDelegate.appLogTag): Error opening database: \(error.localizedDescription)")
        }
    }
    
    /// Opens standalone database for authenticated user. CRUD operations wil lbe possible. Synchronization of data with the cloud through Sync Gateway is available.
    /// - Parameter username: Authenticated username
    func openDatabaseForUser(username: String) {
        NSLog("\(AppDelegate.appLogTag): Opening database for user \(username)")
        // Create the `DatabaseConfiguration` to open database
        let config: DatabaseConfiguration = DatabaseConfiguration()
        // Set directory in documents folder to save the local database instance
        config.directory = "\(documentsPathString())/\(username)"
        currentUser = username

        do {
            // Open the database with the defined configuration. If the database does not exists, it will be created.
            database = try Database(name: "beer-sample", config: config)
            createFTSQueryIndex()
        } catch let error {
            NSLog("\(AppDelegate.appLogTag): Error opening database for user \(username): \(error.localizedDescription)")
        }
    }
    
    /// Returns current user Doc ID
    func getCurrentUserDocId() -> String {
        return "user:: \(currentUser)"
    }
    
    /// Creates FTS Query Index for opened database
    func createFTSQueryIndex() {
        do {
            try database?.createIndex(IndexBuilder.fullTextIndex(items: FullTextIndexItem.property("description")), withName: "descFTSIndex")
        } catch let error {
            NSLog("\(AppDelegate.appLogTag): Error creating FTSQueryIndex: \(error.localizedDescription)")
        }
    }
    
    /// Starts replication process to sync data with the cloud through Sync Gateway. Only used for authenticated users.
    /// - Parameters:
    ///   - username: Authenticated user name
    ///   - password: Authenticated user password
    func startPushAndPullReplicationForCurrentUser(username: String, password: String) {
        NSLog("\(AppDelegate.appLogTag): Starting push and pull replication for user \(username)")
        let url = URL(string: DatabaseManager.syncGatewayEndpoint)
        
        if (database == nil) {
            NSLog("\(AppDelegate.appLogTag): Database is nil!")
            return
        }
        
        if (url == nil) {
            NSLog("\(AppDelegate.appLogTag): Replication Endpoint URL is invalid!")
            return
        }
        
        // Create the URLEndpoint with the endpint url to configure replicator
        let targetEndpoint = URLEndpoint(url: url!)
        // Create the ReplicatorConfiguration instance to configure replicator
        let config: ReplicatorConfiguration = ReplicatorConfiguration(database: database!, target: targetEndpoint)
        // Set replicator type to `ReplicatorType.pushAndPull` to synchronize in both ways (from and to cloud)
        config.replicatorType = ReplicatorType.pushAndPull
        config.continuous = true
        NSLog("\(AppDelegate.appLogTag): About to config replicator: \(username)::\(password)")
        config.authenticator = BasicAuthenticator(username: username, password: password)

        // Create the replicator
        replicator = Replicator(config: config)
        // Set listener to be notified about replicator status changes
        replicator?.addChangeListener({ change in
            if (change.replicator.status.activity == Replicator.ActivityLevel.idle) {
                NSLog("\(AppDelegate.appLogTag): Replication Comp Log: Scheduler Completed")
                // Post a notification to notify possible observers about replicator is running now
                NotificationCenter.default.post(name: DatabaseManager.notificationSyncGateWayCompleted, object: nil)
            }
            if (change.replicator.status.activity == Replicator.ActivityLevel.stopped || change.replicator.status.activity == Replicator.ActivityLevel.offline) {
                NSLog("\(AppDelegate.appLogTag): Rep scheduler  Log ReplicationTag Stopped")
            }
        })

        NSLog("\(AppDelegate.appLogTag): Starting Replicator")
        // Start the replicator
        replicator?.start()
    }
    
    /// Stops replication process
    func stopPushAndPullReplicationForCurrentUser() {
        replicator?.stop()
    }

}
