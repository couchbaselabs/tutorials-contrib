package org.couchbase.tutorials;


public class TestFramework {

    public static void main(String[] args) {

        MoviesFramework mf = new MoviesFramework("192.168.1.10", "Administrator", "123456", "rate-these-movies");

        // Core operations

        //mf.addRating("rating_98", "movie_0", "user_20", 10);
        //mf.getRating("rating_98");
        //mf.deleteRating("rating_98");

        // Sub-document operations

        //mf.getUserCountry("user_0");
        //mf.updateUserCountry("user_0", "Canada");

        // N1QL queries

        //mf.deleteMovie("movie_9");
        //mf.listTop5();
    }
}