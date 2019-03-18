package com.cb.demo.userProfile;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.couchbase.repository.config.EnableReactiveCouchbaseRepositories;

// tag::code[]
@SpringBootApplication
@EnableReactiveCouchbaseRepositories
public class UserProfileApplication {

	public static void main(String[] args) {
		SpringApplication.run(UserProfileApplication.class, args);
	}
}
// end::code[]
