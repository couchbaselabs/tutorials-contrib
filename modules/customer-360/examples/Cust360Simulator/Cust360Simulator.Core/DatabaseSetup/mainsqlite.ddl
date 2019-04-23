CREATE TABLE [customer_details] (
[id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[password] VARCHAR(50)  NOT NULL,
[email] VARCHAR(100)  NOT NULL,
[name] VARCHAR(150)  NOT NULL
);

CREATE TABLE [product] (
[product_id] INTEGER  NOT NULL PRIMARY KEY,
[name] VARCHAR(50)  NOT NULL,
[description] TEXT  NOT NULL,
[category] VARCHAR(50)  NOT NULL
);

CREATE TABLE [order] (
[order_id] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[purchase_date] DATE  NOT NULL,
[quantity] INTEGER  NOT NULL,
[customer_id] INTEGER  NOT NULL,
[product_id] INTEGER  NOT NULL,
FOREIGN KEY(customer_id) REFERENCES customer_details(id),
FOREIGN KEY(product_id) REFERENCES product(product_id)
);
