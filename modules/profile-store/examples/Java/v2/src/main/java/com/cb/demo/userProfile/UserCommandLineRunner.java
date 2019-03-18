package com.cb.demo.userProfile;

import com.cb.demo.userProfile.model.EventType;
import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.model.UserEventEntity;
import com.cb.demo.userProfile.repositories.UserEntityRepository;
import com.cb.demo.userProfile.repositories.UserEventRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

import java.util.Date;
import java.util.concurrent.ThreadLocalRandom;

@Component
public class UserCommandLineRunner implements CommandLineRunner {

    @Autowired
    private UserEntityRepository userEntityRepository;

    @Autowired
    private UserEventRepository userEventRepository;

    @Override
    public void run(String... strings) throws Exception {

        String[] names = {"Olivia", "Alex", "Allex", "Alec", "Charlotte", "Benjamin",
                "James", "Elijah", "Michael", "Liam", "Emma", "Isabella", "Mia", "Robert", "Maria", "David", "Mary",
                "George", "Henry", "Thomas", "Joseph", "Samuel", "Elizabeth", "Margaret", "Martha", "Ann", "Catherine"};

        String[] surnames = {"Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller",
                "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez",  "Gonzalez" };


        for(int i=0; i < 300;i++) {

            if(i%100 == 0) {
                System.out.println("----------- i = "+i);
            }
            UserEntity userEntity = new UserEntity();
            userEntity.setId("user::"+i);
            userEntity.setFirstName(names[ThreadLocalRandom.current().nextInt(0, 26 + 1)]);
            userEntity.setLastName(surnames[ThreadLocalRandom.current().nextInt(0, 12 + 1)]);
            userEntity.setTenantId(1);
            userEntity.setCountryCode( "US");
            userEntity.setUsername("user"+i);
            userEntity.setPassword("secret");
            userEntity.setEnabled(true);
            userEntityRepository.save(userEntity);

            UserEventEntity evt = new UserEventEntity();
            evt.setId("userevt::"+i);
            evt.setUserId(userEntity.getId());
            evt.setEventType(EventType.PRODUCT_VIEWED);
            evt.setCreatedDate(new Date().getTime());

            userEventRepository.save(evt);
        }

//        for(int i=40000; i < 1000001;i++) {
//
//            if(i%100 == 0) {
//                System.out.println("----------- i = "+i);
//                Thread.sleep(100);
//            }
//            UserEntity userEntity = new UserEntity();
//            userEntity.setId("user-"+i);
//            userEntity.setFirstName("Some Name "+i);
//            userEntity.setTenantId(1);
//            userEntity.setCountryCode("US");
//            userEntity.setUsername("user"+i);
//            userEntity.setPassword("secret");
//            userEntity.setEnabled(true);
//            userEntityRepository.save(userEntity);
//        }

    }
}
