package com.cb.demo.userProfile.service;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.reprositories.UserEntityRepository;
import com.cb.demo.userProfile.service.vo.SimpleUserVO;
import com.cb.demo.userProfile.service.vo.UserVO;
import com.couchbase.client.java.document.json.JsonObject;
import com.couchbase.client.java.query.N1qlParams;
import com.couchbase.client.java.query.N1qlQuery;
import com.couchbase.client.java.query.ParameterizedN1qlQuery;
import com.couchbase.client.java.query.consistency.ScanConsistency;
import com.couchbase.client.java.search.SearchQuery;
import com.couchbase.client.java.search.queries.BooleanFieldQuery;
import com.couchbase.client.java.search.queries.ConjunctionQuery;
import com.couchbase.client.java.search.queries.DisjunctionQuery;
import com.couchbase.client.java.search.queries.MatchQuery;
import com.couchbase.client.java.search.result.SearchQueryResult;
import com.couchbase.client.java.search.result.SearchQueryRow;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.*;

@Service
public class UserServiceImpl implements UserService {

    // tag::code[]
    @Autowired
    private UserEntityRepository userEntityRepository;

    public void doSomething() {
        UserEntity user = new UserEntity();
        user.setId("someId");
        user.setCountryCode("DE");
        user.setPassword("password");
        user.setUsername("bilbo");

        userEntityRepository.save(user);
        //read after write
        Optional<UserEntity> savedUser = userEntityRepository.findById(user.getId());
        System.out.println(savedUser.get());
    }
    // end::code[]

}
