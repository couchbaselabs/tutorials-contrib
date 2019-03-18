package com.cb.demo.userProfile.service;

import com.cb.demo.userProfile.model.UserEventEntity;
import reactor.core.publisher.Flux;

import java.util.List;

public interface UserEventService {

    Flux<UserEventEntity> save(List<UserEventEntity> events);

    Flux<UserEventEntity> findLatestUserEvents(String userId, String eventType,  int limit, int offset);
}
