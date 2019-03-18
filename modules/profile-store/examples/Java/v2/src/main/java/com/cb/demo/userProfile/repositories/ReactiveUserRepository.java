package com.cb.demo.userProfile.repositories;

import com.cb.demo.userProfile.model.UserEntity;
import org.springframework.data.couchbase.core.query.Query;
import org.springframework.data.couchbase.repository.ReactiveCouchbaseSortingRepository;
import reactor.core.publisher.Flux;
import reactor.core.publisher.Mono;

import java.util.List;

public interface ReactiveUserRepository extends ReactiveCouchbaseSortingRepository<UserEntity, String> {


    Mono<UserEntity> findByUsername(String username);

    Flux<UserEntity> findByFirstNameLike(String firstName);

    @Query("#{#n1ql.selectEntity} where  #{#n1ql.filter}  and tenantId = $1 and countryCode = $2 and enabled = true  order by firstName desc limit $3  offset $4")
    Flux<UserEntity> findByCountryAndTenantId(Integer tenantId, String countryCode,  int limit, int offset);

}
