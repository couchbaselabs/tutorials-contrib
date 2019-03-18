package com.cb.demo.userProfile.repositories;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.model.UserEventEntity;
import org.springframework.data.couchbase.core.query.N1qlPrimaryIndexed;
import org.springframework.data.couchbase.core.query.N1qlSecondaryIndexed;
import org.springframework.data.couchbase.repository.CouchbasePagingAndSortingRepository;

@N1qlPrimaryIndexed
@N1qlSecondaryIndexed(indexName = "userEventEntity")
public interface UserEventRepository extends CouchbasePagingAndSortingRepository<UserEventEntity, String> {


}
