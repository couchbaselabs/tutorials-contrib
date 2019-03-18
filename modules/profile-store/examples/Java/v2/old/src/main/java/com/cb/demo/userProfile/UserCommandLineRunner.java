package com.cb.demo.userProfile;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.reprositories.UserEntityRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

@Component
public class UserCommandLineRunner implements CommandLineRunner {

    @Autowired
    private UserEntityRepository userEntityRepository;

    @Override
    public void run(String... strings) throws Exception {

//        for(int i=0; i < 100000;i++) {
//
//            if(i%100 == 0) {
//                System.out.println("----------- i = "+i);
//            }
//            UserEntity userEntity = new UserEntity();
//            userEntity.setId("user-"+i);
//            userEntity.setFirstName("Some Name "+i);
//            userEntity.setTenantId(1);
//            userEntity.setCountryCode(i> 40000?"DE": "US");
//            userEntity.setUsername("user"+i);
//            userEntity.setPassword("secret");
//            userEntity.setEnabled(i>70000? false:true);
//            userEntityRepository.save(userEntity);
//        }
//
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
