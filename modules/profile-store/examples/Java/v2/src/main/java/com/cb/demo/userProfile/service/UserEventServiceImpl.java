package com.cb.demo.userProfile.service;

import com.cb.demo.userProfile.model.UserEventEntity;
import com.cb.demo.userProfile.repositories.ReactiveUserEventRepository;
import com.cb.demo.userProfile.repositories.ReactiveUserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import reactor.core.publisher.Flux;

import java.util.ArrayList;
import java.util.List;
// tag::code[]
@Service
public class UserEventServiceImpl implements UserEventService {

    @Autowired
    private ReactiveUserEventRepository reactiveUserEventRepository;

    @Override
    public Flux<UserEventEntity> save(List<UserEventEntity> events){
        return reactiveUserEventRepository.saveAll(events);
    }

    @Override
    public Flux<UserEventEntity> findLatestUserEvents(String userId, String eventType,  int limit, int offset) {
        return reactiveUserEventRepository.findLatestUserEvents(userId, eventType, limit, offset);
    }
}

// end::code[]
