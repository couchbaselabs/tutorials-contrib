package com.cb.demo.userProfile.repositories;

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

    List<UserEntity> findByFirstNameIgnoreCase(String firstName);

    List<UserEntity> findByTenantIdOrderByFirstNameAsc(Integer tenantId, Pageable pageable);

    List<UserEntity> deleteByTenantId(Integer tenantId);
    
    @Query("Select meta().id as id, username, tenantId, firstName, lastname from  #{#n1ql.bucket} where #{#n1ql.filter} " +
            " and  tenantId = $1 order by firstName asc limit $3 offset $2 ")
    List<SimpleUserVO> listTenantUsers(Integer tenantId, Integer offset, Integer limit);


    @Query("#{#n1ql.selectEntity} where  #{#n1ql.filter}  and firstName like $1 and enabled = $2 and countryCode = $3 order by firstName desc limit $4 offset $5")
    List<UserEntity> findActiveUsersByFirstName(String firstName, boolean enabled, String countryCode,
                                                                                 int limit, int offset);
}
// end::code[]
