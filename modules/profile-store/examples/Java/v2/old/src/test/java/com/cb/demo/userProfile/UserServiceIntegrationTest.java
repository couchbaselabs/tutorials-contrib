package com.cb.demo.userProfile;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.reprositories.ReactiveUserRepository;
import com.cb.demo.userProfile.reprositories.UserEntityRepository;
import com.cb.demo.userProfile.service.UserService;
import com.cb.demo.userProfile.service.vo.SimpleUserVO;
import lombok.extern.slf4j.Slf4j;
import org.junit.Assert;
import org.junit.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import reactor.core.publisher.Flux;

import java.time.Duration;
import java.time.Instant;
import java.util.List;

@Slf4j
public class UserServiceIntegrationTest extends UserProfileApplicationTests {


    @Autowired
    private UserEntityRepository userEntityRepository;

    @Autowired
    private ReactiveUserRepository reactiveUserRepository;

    @Autowired
    private UserService userService;


    @Test
    public void findByFirstNameIgnoresCaseTest2() {

        Instant start = Instant.now();
        List<UserEntity> users =  userEntityRepository
                .findActiveUsersByFirstName("Some%", true, "US", 100, 0);
        Instant finish = Instant.now();

        System.out.println("Total time: "+ Duration.between(start, finish).toMillis());
        System.out.println("Number os users returned = "+users.size() );


        Instant start2 = Instant.now();
        List<SimpleUserVO> simpleUsers =  userService
                .listActiveUsers("Some%", true, "US", 100, 0);
        Instant finish2 = Instant.now();

        System.out.println("Total time: "+ Duration.between(start2, finish2).toMillis());
        System.out.println("Number os users returned = "+simpleUsers.size() );

    }



    @Test
    public void findByFirstNameIgnoresCaseTest3() throws Exception {

        Flux<UserEntity> flux = reactiveUserRepository.findByFirstNameLike("Some%");
        flux.subscribe(
                val -> log.info("-----------------Subscriber received: {}", val.getFirstName()), val ->log.info("|||||||||||||||||||||deu erro "+val));

        Thread.sleep(50000);
    }


//    @Test
//    public void findByFirstNameIgnoresCaseTest3() throws Exception {
//
//        Mono<UserEntity> flux = reactiveUserRepository.findFirstByFirstName("Denis");
//        flux.subscribe(
//                val -> log.info("-----------------Subscriber received: {}", val.getFirstName()), val ->log.info("|||||||||||||||||||||deu erro "+val));
//
//        Thread.sleep(50000);
//    }


    @Test
    public void findByFirstNameIgnoresCaseTest() {

        UserEntity userEntity = new UserEntity();
        userEntity.setFirstName("Denis");
        userEntity.setTenantId(1);
        userEntity.setCountryCode("BR");
        userEntity.setUsername("denis1");
        userEntity.setPassword("fafafa");
        userEntity.setId("someID1");

        userEntityRepository.save(userEntity);


        UserEntity user2 = new UserEntity();
        user2.setFirstName("Denis");
        user2.setTenantId(1);
        user2.setCountryCode("BR");
        user2.setUsername("denis2");
        user2.setPassword("fafafa");
        user2.setId("someID2");

        userEntityRepository.save(user2);

        UserEntity user3 = new UserEntity();
        user3.setFirstName("Denis");
        user3.setTenantId(1);
        user3.setCountryCode("BR");
        user3.setUsername("denis2");
        user3.setPassword("fafafa");
        user3.setId("someID2");

        userEntityRepository.save(user3);


       // Assert.assertTrue(userEntityRepository.findByFirstNameIgnoreCase("denis").size() == 2);

    }


    @Test
    public void findByTenantIdOrderByFirstNameAscTest() {
       // System.out.println(userEntityRepository.findByTenantIdOrderByFirstNameAsc(1, new PageRequest(0, 3)));;

        System.out.println("=================================================");
       // System.out.println(userService.listUsers(1, 0, 3));
        System.out.println(userEntityRepository.listTenantUsers(1, 0, 3));


    }

    @Test
    public void findIsMissing() {
        //System.out.println(userEntityRepository.findUsersWithMissingSocialSecurityNumber(1));
    }
}
