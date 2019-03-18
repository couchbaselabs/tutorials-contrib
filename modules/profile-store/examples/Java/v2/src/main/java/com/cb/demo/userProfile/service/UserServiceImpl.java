package com.cb.demo.userProfile.service;

import com.cb.demo.userProfile.model.UserEntity;
import com.cb.demo.userProfile.repositories.UserEntityRepository;
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
import reactor.core.publisher.Flux;

import java.util.*;

@Service
public class UserServiceImpl implements UserService {

    @Autowired
    private UserEntityRepository userEntityRepository;

    @Override
    public UserVO addUser(UserEntity user) {
        return toUserVO(userEntityRepository.save(user));
    }

    @Override
    public UserVO getUser(String id) {
        Optional<UserEntity> user = userEntityRepository.findById(id);
        return user.isPresent()? toUserVO(user.get()): null;
    }



    @Override
    public List<SimpleUserVO> listUsers(Integer tenantId, Integer offset, Integer limit) {
        return null;
    }

    // tag::code[]
    @Override
    public List<SimpleUserVO> ftsListActiveUsers(String firstName,  boolean enabled, String countryCode,  Integer limit, Integer skip ) {

        String indexName = "user_index";
        // tag::fuzzy[]
        MatchQuery firstNameFuzzy = SearchQuery.match(firstName).fuzziness(1).field("firstName");
        MatchQuery firstNameSimple = SearchQuery.match(firstName).field("firstName");
        DisjunctionQuery nameQuery = SearchQuery.disjuncts(firstNameSimple, firstNameFuzzy);
        // end::fuzzy[]
        // tag::filter[]
        BooleanFieldQuery isEnabled = SearchQuery.booleanField(enabled).field("enabled");
        MatchQuery countryFilter = SearchQuery.match(countryCode).field("countryCode");
        // end::filter[]
        // tag::conj[]
        ConjunctionQuery conj = SearchQuery.conjuncts(nameQuery, isEnabled, countryFilter);
        // end::conj[]
        // tag::result[]
        SearchQueryResult result = userEntityRepository.getCouchbaseOperations().getCouchbaseBucket().query(
                new SearchQuery(indexName, conj)
                        .fields("id", "tenantId", "firstName", "lastName", "username" )
                        .skip(skip)
                        .limit(limit));


        List<SimpleUserVO> simpleUsers = new ArrayList<>();
        if (result != null && result.errors().isEmpty()) {
            Iterator<SearchQueryRow> resultIterator = result.iterator();
            while (resultIterator.hasNext()) {
                SearchQueryRow row = resultIterator.next();
                Map<String, String> fields = row.fields();
                simpleUsers.add(new SimpleUserVO(
                        row.id(),
                        new Long(fields.get("tenantId")),
                        fields.get("firstName"),
                        fields.get("lastName"),
                        fields.get("username")
                ));
            }
        }

        return simpleUsers;
        // end::result[]
    }

    // end::code[]

    public List<SimpleUserVO> listActiveUsers(String firstName,  boolean enabled, String countryCode,  Integer limit, Integer offset ) {

        String query = "Select meta().id as id, username, tenantId, firstName, lastname from "
                + userEntityRepository.getCouchbaseOperations().getCouchbaseBucket().bucketManager().info().name()
                +" where _class = '"+UserEntity.class.getName()+"' and firstName like '"+firstName+"' and enabled = "+enabled+" " +
                " and countryCode = '"+countryCode+"' order by firstName desc limit "+limit+ " offset "+offset;

        N1qlParams params = N1qlParams.build().consistency(ScanConsistency.STATEMENT_PLUS).adhoc(false);
        ParameterizedN1qlQuery queryParam = N1qlQuery.parameterized(query, JsonObject.create(), params);
        return userEntityRepository.getCouchbaseOperations().findByN1QLProjection(queryParam, SimpleUserVO.class);
    }

    private UserVO toUserVO(UserEntity user) {
        UserVO userVO = new UserVO();
        userVO.setId(user.getId());
        userVO.setCountryCode(user.getCountryCode());
        userVO.setEnabled(user.isEnabled());
        userVO.setFirstName(user.getFirstName());
        userVO.setLastName(user.getLastName());
        userVO.setTenantId(user.getTenantId());
        userVO.setUsername(user.getUsername());
        userVO.setAddresses(user.getAddresses());
        userVO.setPreferences(user.getPreferences());
        userVO.setSecurityRoles(user.getSecurityRoles());
        return userVO;
    }




}
