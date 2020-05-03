package com.couchbasece.beersampler;

import android.content.Intent;
import android.content.SharedPreferences;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.couchbase.lite.CouchbaseLiteException;
import com.couchbase.lite.DataSource;
import com.couchbase.lite.Database;
import com.couchbase.lite.Document;
import com.couchbase.lite.Expression;
import com.couchbase.lite.Meta;
import com.couchbase.lite.MutableDocument;
import com.couchbase.lite.Query;
import com.couchbase.lite.QueryBuilder;
import com.couchbase.lite.Result;
import com.couchbase.lite.ResultSet;
import com.couchbase.lite.SelectResult;
import com.couchbasece.beersampler.utils.DatabaseManager;

import java.util.HashMap;

public class InsertBeerActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_insert_beer);

        Button buttonInsertBeerInDB = (Button) findViewById(R.id.btn_insert_beer_in_db);
        EditText breweryInput = (EditText) findViewById(R.id.et_brewery);
        EditText beerNameInput = (EditText) findViewById(R.id.et_beername);
        EditText categoryInput = (EditText) findViewById(R.id.et_category);
        EditText styleInput = (EditText) findViewById(R.id.et_style);
        EditText abvInput = (EditText) findViewById(R.id.et_abv);

        Intent myIntent = getIntent();
        String beerId = myIntent.getStringExtra("beerId");

        // If we are editing an existing beer
        if (!(beerId.equals(""))) {
            Database database = DatabaseManager.getDatabase();

            Log.i("appBeerSampler", "Connected to database: " + database.getName().toString());
            Log.i("appBeerSampler", "Ready to retrieve: " + beerId);

            Query searchQuery = QueryBuilder
                    .select(SelectResult.expression(Expression.property("name")),
                            SelectResult.expression(Expression.property("style")),
                            SelectResult.expression(Expression.property("brewery_id")),
                            SelectResult.expression(Expression.property("category")),
                            SelectResult.expression(Expression.property("abv")),
                            SelectResult.expression(Meta.id))
                    .from(DataSource.database(database))
                    .where(
                            Meta.id.equalTo(Expression.string(beerId))
                    );

            ResultSet rows = null;
            try {
                Log.i("appBeerSampler", "Ready to run query");
                rows = searchQuery.execute();

            } catch (CouchbaseLiteException e) {
                Log.e("appBeerSampler", "Failed to run query", e);
            }

            Result row;
            while ((row = rows.next()) != null) {

                String beerName = row.getString("name");
                String beerStyle = row.getString("style");
                String beerBrewery = row.getString("brewery_id");
                String beerCategory = row.getString("category");
                String beerABV = row.getString("abv");

                beerNameInput.setText(beerName);
                styleInput.setText(beerStyle);
                breweryInput.setText(beerBrewery);
                categoryInput.setText(beerCategory);
                abvInput.setText(beerABV);

                Log.i("appBeerSampler", "Editing: Beer "+beerName+", Beer ID:"+beerId);
            }
        }

        buttonInsertBeerInDB.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Log.i("appBeerSampler", "Insert a new beer");

                // Retrieving beer data from the interface
                String Brewery = breweryInput.getText().toString();
                String BeerName = beerNameInput.getText().toString();
                String Category = categoryInput.getText().toString();
                String Style = styleInput.getText().toString();
                String ABV = abvInput.getText().toString();

                // Get the database in use
                Database database = DatabaseManager.getDatabase();

                // Filling the beer's data
                HashMap<String, Object> properties = new HashMap<>();
                properties.put("type", "beer");
                properties.put("name", BeerName);
                properties.put("brewery_id", Brewery);
                properties.put("category", Category);
                properties.put("style", Style);
                properties.put("abv", ABV);

                // Retrieves username and password from Shared Preferences
                SharedPreferences sp1= getSharedPreferences("Login", MODE_PRIVATE);
                String user = sp1.getString("username", null);
                String passwd = sp1.getString("password", null);

                if (user.equals("D3m0u53r") && passwd.equals("D3m0u53r")) {

                    properties.put("username", "guest");

                } else {
                    properties.put("username", user);
                }

                MutableDocument mutableCopy = new MutableDocument(Brewery+"-"+BeerName, properties);

                try {
                    Log.i("appBeerSampler", "Ready to insert");

                    database.save(mutableCopy);
                    Log.i("appBeerSampler", "Inserted");

                } catch (CouchbaseLiteException e) {
                    e.printStackTrace();
                }
                finish();
            }
        });

    }
}
