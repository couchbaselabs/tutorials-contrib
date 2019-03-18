package com.cb.demo.userProfile.reprositories;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.service.vo.SimpleUserVO;
import org.springframework.data.couchbase.core.query.N1qlPrimaryIndexed;
import org.springframework.data.couchbase.core.query.N1qlSecondaryIndexed;
import org.springframework.data.couchbase.core.query.Query;
import org.springframework.data.couchbase.repository.CouchbasePagingAndSortingRepository;
import java.util.List;

// tag::code[]
@N1qlPrimaryIndexed
@N1qlSecondaryIndexed(indexName = "userEntity")
public interface UserEntityRepository extends CouchbasePagingAndSortingRepository<UserEntity, String> {

}
// end::code[]
