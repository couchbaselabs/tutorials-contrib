package com.cb.demo.userProfile.controllers;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.repositories.ReactiveUserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import reactor.core.publisher.Flux;
import reactor.core.publisher.Mono;

import javax.validation.Valid;

@RestController
public class UserController {

    @Autowired
    private ReactiveUserRepository reactiveUserRepository;

    @GetMapping("/findByUsername")
    public Mono<UserEntity> findByUsername( @RequestParam("username") String username) {
        return reactiveUserRepository.findByUsername(username);
    }

    @GetMapping("/save")
    public Mono<UserEntity> save( @Valid @RequestBody UserEntity user) {
        return reactiveUserRepository.save(user);
    }

    @GetMapping(value="/list", produces = MediaType.TEXT_EVENT_STREAM_VALUE)
    public Flux<UserEntity> save( @RequestParam("tenantId") Integer tenantId,
                                  @RequestParam("countryCode") String countryCode,
                                  @RequestParam("limit") int limit,
                                  @RequestParam("offset") int offset) {
        return reactiveUserRepository.findByCountryAndTenantId(tenantId, countryCode, limit, offset);
    }
}
