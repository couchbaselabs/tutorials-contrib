package com.cb.demo.userProfile.reprositories;

import com.cb.demo.userProfile.model.UserEntity;
import org.springframework.data.couchbase.repository.ReactiveCouchbaseSortingRepository;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import reactor.core.publisher.Flux;

public interface ReactiveUserRepository extends ReactiveCouchbaseSortingRepository<UserEntity, String> {


    Flux<UserEntity> findByFirstNameLike(String firstName);
}
