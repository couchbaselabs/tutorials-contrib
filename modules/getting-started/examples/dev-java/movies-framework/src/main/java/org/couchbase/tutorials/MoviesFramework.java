package org.couchbase.tutorials;


import java.text.DecimalFormat;
import java.util.Collections;

import com.couchbase.client.java.*;
import com.couchbase.client.java.json.JsonArray;
import com.couchbase.client.java.json.JsonObject;
import com.couchbase.client.java.kv.*;
import com.couchbase.client.java.query.QueryOptions;
import com.couchbase.client.java.query.QueryResult;


public class MoviesFramework {

    Cluster cluster;
    Bucket bucket;

    public MoviesFramework(String hostname, String username, String password, String bucketName) {

        cluster = Cluster.connect(hostname, username, password);
        bucket = cluster.bucket(bucketName);
    }

    public void addRating(String ratingId, String movieId, String userId, int value) {

        final JsonObject ratingJSON = JsonObject.create()
                .put("movie_id", movieId)
                .put("user_id", userId)
                .put("value", value);

        try {
            bucket.defaultCollection().upsert(ratingId, ratingJSON);
            System.out.println("OK");
        }
        catch (Exception e){
            System.out.println("ERROR: " + e.getMessage());
        }
    }

    public void getRating(String ratingId) {

        try {
            GetResult answer = bucket.defaultCollection().get(ratingId);
            System.out.println(answer.contentAs(JsonObject.class));
        }
        catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
        }
    }

    public void deleteRating(String ratingId) {

        try {
            bucket.defaultCollection().remove(ratingId);
            System.out.println("OK");
        }
        catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
        }
    }

    public void getUserCountry(String userId) {

        try {
            LookupInResult answer = bucket.defaultCollection().lookupIn(
                    userId, Collections.singletonList(
                            LookupInSpec.get("country")));
            System.out.println(answer.contentAs(0, String.class));
        }
        catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
        }
    }

    public void updateUserCountry(String userId, String country) {

        try {
            MutateInResult answer = bucket.defaultCollection().mutateIn(
                    userId, Collections.singletonList(
                            MutateInSpec.upsert("country", country)));
            System.out.println("OK");
        }
        catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
        }
    }

    public void deleteMovie(String movieId) {

        QueryOptions parameters = QueryOptions.queryOptions().parameters(JsonArray.from(movieId));

        try {
            cluster.query("DELETE FROM `rate-these-movies` USE KEYS ?", parameters);
            cluster.query("DELETE FROM `rate-these-movies` WHERE id_movie=?", parameters);
            System.out.println("OK");
        }
        catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
        }
    }

    public void listTop5() {

        final DecimalFormat df = new DecimalFormat("0.00");

        try {
            QueryResult answer = cluster.query("SELECT a.name AS name, AVG(b.`value`) AS avg FROM `rate-these-movies` AS a JOIN `rate-these-movies` AS b ON META(a).id=b.id_movie GROUP BY a.name ORDER BY avg DESC LIMIT 5");

            for (JsonObject row : answer.rowsAsObject()) {
                System.out.println(row.get("name") + " -> " + df.format(row.get("avg")));
            }
        }
        catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
        }
    }
}
