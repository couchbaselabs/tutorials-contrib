package com.cb.demo.userProfile.model;

import lombok.Data;
import org.springframework.data.annotation.Id;
import org.springframework.data.couchbase.core.mapping.Document;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;
import java.util.List;

// tag::code[]
@Data
@Document
public class UserEntity {

    @Id
    private String id;
    @NotNull
    private String firstName;
    private String middleName;
    private String lastName;
    private boolean enabled;
    @NotNull
    private Integer tenantId;
    @NotNull
    @Size(max = 2, min=2)
    private String countryCode;
    @NotNull
    private String username;
    @NotNull
    private String password;
    private String socialSecurityNumber;

    private List<TelephoneEntity> telephones;
    private List<PreferenceEntity> preferences;
    private List<AddressEntity> addresses;
    private List<String> securityRoles;
}
// end::code[]