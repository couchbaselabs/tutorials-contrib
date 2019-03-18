package com.cb.demo.userProfile.repositories;

import com.cb.demo.userProfile.model.EventType;
import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.model.UserEventEntity;
import org.springframework.data.couchbase.core.query.Query;
import org.springframework.data.couchbase.repository.ReactiveCouchbaseSortingRepository;
import reactor.core.publisher.Flux;

// tag::code[]
public interface ReactiveUserEventRepository extends ReactiveCouchbaseSortingRepository<UserEventEntity, String> {

    @Query("#{#n1ql.selectEntity} where  #{#n1ql.filter}  and userId = $1 and eventType = $2  order by createdDate desc limit $3  offset $4")
    Flux<UserEventEntity> findLatestUserEvents(String userId, String eventType,  int limit, int offset);
}
// end::code[]