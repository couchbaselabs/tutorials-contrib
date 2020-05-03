//
//  BeerQueries.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/5/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import CouchbaseLiteSwift

/// Helper class to define the main methods to access data in the opened database
/// - seealso: `DatabaseManager`
class BeerQueries {
    /// Constructor
       init() {
       }
       
       /// Singleton instance of `BeerQueries`
       static let shared: BeerQueries = BeerQueries()
    
    /// Gets the list of existent beers for the current user
    /// - returns: A `Beer` array with found beers (or empty if current user doesn't have beers created yet)
    func fetchBeers() -> [Beer] {
        NSLog("\(AppDelegate.appLogTag): Inside FetchPubs");
        
        var rows: ResultSet? = nil
        // Get the opened database instance
        if let database = DatabaseManager.shared.database {
            NSLog("\(AppDelegate.appLogTag): Connected to database: \(database.name)")
            
            // Create the database Query to select all documents of type "beer"
            ///  - Note: If the opened database is standalone, the locally created beers are fetched. If there is an authenticated user, all the synchronized beers will be fetched from cluod (or all beers already synchronized if connection is not available)
            let searchQuery: Query  = QueryBuilder
                .select(SelectResult.expression(Expression.property("name")),
                        SelectResult.expression(Expression.property("style")),
                        SelectResult.expression(Expression.property("brewery_id")),
                        SelectResult.expression(Expression.property("category")),
                        SelectResult.expression(Expression.property("abv")),
                        SelectResult.expression(Meta.id))
                .from(DataSource.database(database))
                .where(
                    Expression.property("type").equalTo(Expression.string("beer"))
            );
            
            NSLog("\(AppDelegate.appLogTag): Query loaded")
            
            do {
                NSLog("\(AppDelegate.appLogTag): Ready to run query")
                // Execute the query
                rows = try searchQuery.execute()
                
            } catch let error {
                NSLog("\(AppDelegate.appLogTag): Failed to run query \(error.localizedDescription)")
            }
        }
        // Process Query results
        var beersArray = [Beer]()
        if (rows != nil) {
            for row in rows! {
                let beer = Beer(
                    id: row.string(forKey: "id") ?? "",
                    name: row.string(forKey: "name") ?? "",
                    style: row.string(forKey: "style") ?? "",
                    brewery: row.string(forKey: "brewery_id") ?? "",
                    category: row.string(forKey: "category") ?? "",
                    abv: row.string(forKey: "abv") ?? "")
                beersArray.append(beer)
            }
        }
        
        return beersArray
    }
    
    /// Search for a beer in the database for the current user given it's identifier
    /// - Parameter id: Beer identifier
    /// - returns: The found `Beer` or nil if no beer is found
    func fetchBeerByID( _ id: String) -> Beer? {
        NSLog("\(AppDelegate.appLogTag): Inside FetchPubs");
        
        var rows: ResultSet? = nil
        if let database = DatabaseManager.shared.database {
            NSLog("\(AppDelegate.appLogTag): Connected to database: \(database.name)")
            
            // Create the database Query
            let searchQuery: Query  = QueryBuilder
                .select(SelectResult.expression(Expression.property("name")),
                        SelectResult.expression(Expression.property("style")),
                        SelectResult.expression(Expression.property("brewery_id")),
                        SelectResult.expression(Expression.property("category")),
                        SelectResult.expression(Expression.property("abv")),
                        SelectResult.expression(Meta.id))
                .from(DataSource.database(database))
                .where(
                    Meta.id.equalTo(Expression.string(id))
            );
            
            NSLog("\(AppDelegate.appLogTag): Query loaded")
            
            do {
                NSLog("\(AppDelegate.appLogTag): Ready to run query")
                // Execute the query
                rows = try searchQuery.execute()
                
            } catch let error {
                NSLog("\(AppDelegate.appLogTag): Failed to run query \(error.localizedDescription)")
            }
        }
        // Process query results
        var beer: Beer? = nil
        if (rows != nil) {
            for row in rows! {
                beer = Beer(
                    id: row.string(forKey: "id") ?? "",
                    name: row.string(forKey: "name") ?? "",
                    style: row.string(forKey: "style") ?? "",
                    brewery: row.string(forKey: "brewery_id") ?? "",
                    category: row.string(forKey: "category") ?? "",
                    abv: row.string(forKey: "abv") ?? "")
                break
            }
        }
        
        return beer
    }
    
    /// Inserts (or update if already exists) a beer in the current user database. If the user is a registered user, the beer will be synchronized to the cloud when connection is available
    /// - Parameters:
    ///   - brewery: Brewery name
    ///   - beerName: Beer name
    ///   - category: Beer category
    ///   - style: Beer style
    ///   - abv: Alcohol by volume
    ///   - username: The current user name
    /// - returns: true if beer was inserted or updated, false otherwise
    @discardableResult
    func insertBeer(brewery: String, beerName: String, category: String, style: String, abv: String, username: String) -> Bool {
        
        // Get the database in use
        if let database = DatabaseManager.shared.database {
            
            // Filling the beer's data
            var properties: Dictionary<String, String> = Dictionary<String, String>()
            properties["type"] = "beer";
            properties["name"] = beerName
            properties["brewery_id"] = brewery
            properties["category"] = category
            properties["style"] = style
            properties["abv"] = abv
            properties["username"] = username
            
            // Create the MutableDocument to insert
            let mutableCopy: MutableDocument = MutableDocument(id: "\(brewery)-\(beerName)", data: properties)
            
            do {
                NSLog("\(AppDelegate.appLogTag): Ready to insert")
                // Save the document
                try database.saveDocument(mutableCopy)
                NSLog("\(AppDelegate.appLogTag): Inserted")
                
                return true
                
            } catch let error {
                NSLog("\(AppDelegate.appLogTag): Error \(error.localizedDescription)")
            }
        }
        return false
    }
}
