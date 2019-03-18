package com.cb.demo.userProfile.controllers;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.model.UserEventEntity;
import com.cb.demo.userProfile.repositories.ReactiveUserEventRepository;
import com.cb.demo.userProfile.service.UserEventService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import reactor.core.publisher.Flux;
import reactor.core.publisher.Mono;

import javax.validation.Valid;
import java.util.List;

// tag::code[]
@RestController("/events")
public class UserEventController {

    @Autowired
    private UserEventService userEventService;

    @PostMapping(value="/add", produces = MediaType.TEXT_EVENT_STREAM_VALUE)
    public Flux<UserEventEntity> save(@Valid @RequestBody List<UserEventEntity> events) {
        return userEventService.save(events);
    }

    @GetMapping(value="/findLatest", produces = MediaType.TEXT_EVENT_STREAM_VALUE)
    public Flux<UserEventEntity> findLatestUserEvents(@RequestParam("userId") String userId,
                                                 @RequestParam("eventType") String eventType,
                                                 @RequestParam("limit") int limit,
                                                 @RequestParam("offset") int offset) {
        return userEventService.findLatestUserEvents(userId, eventType, limit, offset);
    }
}
// end::code[]