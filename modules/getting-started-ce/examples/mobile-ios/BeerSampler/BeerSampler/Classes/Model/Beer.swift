//
//  BeerModel.swift
//  BeerSampler
//
//  Created by Pedro Llanes on 2/5/20.
//  Copyright © 2020 CouchBaseCE. All rights reserved.
//

import UIKit

/// Beer model class
class Beer: BaseModel {
    var id: String
    var name: String
    var style: String
    var brewery: String
    var category: String
    var abv: String
    
    init(id: String, name: String, style: String, brewery: String, category: String, abv: String) {
        self.id = id
        self.name = name
        self.style = style
        self.brewery = brewery
        self.category = category
        self.abv = abv
    }
}
