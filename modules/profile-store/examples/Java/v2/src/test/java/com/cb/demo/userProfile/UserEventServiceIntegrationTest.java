package com.cb.demo.userProfile;


import com.cb.demo.userProfile.model.EventType;
import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.model.UserEventEntity;
import com.cb.demo.userProfile.repositories.ReactiveUserEventRepository;
import com.cb.demo.userProfile.repositories.UserEventRepository;
import lombok.extern.slf4j.Slf4j;
import org.junit.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.reactive.AutoConfigureWebTestClient;
import reactor.core.publisher.Flux;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

import static org.hamcrest.Matchers.hasSize;
import static org.junit.Assert.assertThat;

@Slf4j
@AutoConfigureWebTestClient
public class UserEventServiceIntegrationTest  extends UserProfileApplicationTests {


    @Autowired
    private ReactiveUserEventRepository reactiveUserEventRepository;

    @Autowired
    private UserEventRepository userEventRepository;

    @Test
    public void findLatestUserEventsTest() throws Exception {

        String userId = "user1";
        int limit = 30;
        EventType eventType = EventType.PRODUCT_VIEWED;
        createEvents(userId, eventType);

        List<UserEventEntity> events = new ArrayList<>();
        Flux<UserEventEntity> flux = reactiveUserEventRepository.findLatestUserEvents(userId, eventType.name(), limit, 0);
        flux.subscribe(val->events.add(val));

        new CountDownLatch(1).await(2500, TimeUnit.MILLISECONDS);

        assertThat(events, hasSize(limit));
    }

    private void createEvents(String userId,  EventType eventType) throws Exception {

        Long start =  new Date().getTime();
        for(int i= 0;  i < 300; i++) {
            UserEventEntity evt = new UserEventEntity();
            evt.setCreatedDate(start - 5*60*1000);
            evt.setUserId(userId);
            evt.setEventType(eventType);
            evt.setId(UUID.randomUUID().toString());
            userEventRepository.save(evt);
        }
    }

}
