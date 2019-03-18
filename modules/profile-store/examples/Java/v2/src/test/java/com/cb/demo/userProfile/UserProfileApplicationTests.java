package com.cb.demo.userProfile;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.repositories.UserEntityRepository;
import lombok.extern.slf4j.Slf4j;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.reactive.AutoConfigureWebTestClient;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.test.context.junit4.SpringRunner;
import org.springframework.test.web.reactive.server.WebTestClient;

@RunWith(SpringRunner.class)
@SpringBootTest
@Slf4j
@AutoConfigureWebTestClient
public class UserProfileApplicationTests {

	//Non Reactive repository
	@Autowired
	private UserEntityRepository userEntityRepository;

	@Autowired
	private WebTestClient webTestClient;


	@Test
	public void testReactiveUserListing() {

		UserEntity userEntity = new UserEntity();
		userEntity.setFirstName("User1");
		userEntity.setTenantId(1);
		userEntity.setCountryCode("DE");
		userEntity.setUsername("someuser1");
		userEntity.setPassword("secret");
		userEntity.setId("someuser1");

		userEntityRepository.save(userEntity);


		webTestClient.get().uri("/findByUsername?username=someuser1")
				.accept(MediaType.APPLICATION_JSON_UTF8)
				.exchange()
				.expectStatus().isOk()
				.expectHeader().contentType(MediaType.APPLICATION_JSON_UTF8)
				.expectBodyList(UserEntity.class);
	}
}
