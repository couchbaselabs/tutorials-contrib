package com.cb.demo.userProfile.reprositories;

import com.cb.demo.userProfile.model.UserEntity;
import org.springframework.data.couchbase.repository.ReactiveCouchbaseSortingRepository;

public interface ReactiveUserEventRepository extends ReactiveCouchbaseSortingRepository<UserEntity, String> {



}
