package com.couchbasece.beersampler;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;


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

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class BrowseDataActivity extends AppCompatActivity implements BeerRecyclerViewAdapter.ItemClickListener {

    BeerRecyclerViewAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.browse_data);

        Button buttonInsertBeer = (Button) findViewById(R.id.btn_insert_beer);

        ResultSet beers = this.fetchBeers();
        ArrayList<HashMap> beersList = new ArrayList<>();

        Result row;
        while ((row = beers.next()) != null) {

            HashMap map = new HashMap<String, String>();
            map.put("beerId", row.getString("id"));
            map.put("beerName", row.getString("name"));
            //beerNames.add(row.getString("name"));
            beersList.add(map);

            Log.i("appBeerSampler","Beer ID: "+row.getString("id"));
            Log.i("appBeerSampler","Beer name: "+row.getString("name"));
        }

        // set up the RecyclerView
        RecyclerView recyclerView = findViewById(R.id.rvBeers);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new BeerRecyclerViewAdapter(this, beersList);
        adapter.setClickListener(this);
        recyclerView.setAdapter(adapter);

        buttonInsertBeer.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Intent intent = new Intent(getApplicationContext(), InsertBeerActivity.class);
                intent.putExtra("beerName", "");
                intent.putExtra("beerId", "");
                startActivity(intent);
            }
        });

    }

    @Override
    public void onItemClick(View view, int position) {

        // When the user selects one beer from the list
        Intent intent = new Intent(getApplicationContext(), InsertBeerActivity.class);
        intent.putExtra("beerName", adapter.getItem(position));
        intent.putExtra("beerId", adapter.getBeerId(position));
        startActivity(intent);

    }

    @Override
    protected void onResume() {
        super.onResume();
        Log.i("appBeerSampler", "Updating Beers list...");

        ResultSet beers = this.fetchBeers();
        ArrayList<HashMap> beersList = new ArrayList<>();

        Result row;
        while ((row = beers.next()) != null) {

            HashMap map = new HashMap<String, String>();
            map.put("beerId", row.getString("id"));
            map.put("beerName", row.getString("name"));
            //beerNames.add(row.getString("name"));
            beersList.add(map);

            Log.i("appBeerSampler","Beer ID: "+row.getString("id"));
            Log.i("appBeerSampler","Beer name: "+row.getString("name"));
        }
        RecyclerView recyclerView = findViewById(R.id.rvBeers);
        adapter = new BeerRecyclerViewAdapter(this, beersList);
        adapter.setClickListener(this);
        recyclerView.setAdapter(adapter);
        adapter.notifyDataSetChanged();

    }


    public ResultSet fetchBeers() {
        Log.i("appBeerSampler", "Inside FetchPubs");

        SharedPreferences sp1=this.getSharedPreferences("Login", MODE_PRIVATE);

        // Retrieves username and password from Shared Preferences
        String user = sp1.getString("username", null);
        String passwd = sp1.getString("password", null);

        if (user.equals("D3m0u53r") && passwd.equals("D3m0u53r")) {

            Log.i("appBeerSampler", "Opening local DB as user Guest");
            DatabaseManager dbMgr = DatabaseManager.getSharedInstance();
            dbMgr.initCouchbaseLite(getApplicationContext());
            dbMgr.OpenGuestDatabase();

        }

        Database database = DatabaseManager.getDatabase();

        Log.i("appBeerSampler", "Connected to database: "+database.getName().toString());

        Query searchQuery = QueryBuilder
                .select(SelectResult.expression(Expression.property("name")),
                        SelectResult.expression(Expression.property("style")),
                        SelectResult.expression(Expression.property("brewery")),
                        SelectResult.expression(Expression.property("category")),
                        SelectResult.expression(Expression.property("abv")),
                        SelectResult.expression(Meta.id))
                .from(DataSource.database(database))
                .where(
                        Expression.property("type").equalTo(Expression.string("beer"))
                );

        Log.i("appBeerSampler", "Query loaded");

        ResultSet rows = null;
        try {
            Log.i("appBeerSampler", "Ready to run query");
            rows = searchQuery.execute();

        } catch (CouchbaseLiteException e) {
            Log.e("appBeerSampler", "Failed to run query", e);
        }

        return rows;

    }
}
