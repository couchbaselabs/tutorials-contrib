package com.cb.demo.userProfile.service.vo;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

import java.io.Serializable;

// tag::code[]
@Data
@ToString
@AllArgsConstructor
@NoArgsConstructor
public class SimpleUserVO implements Serializable {

    private String id;
    private Long tenantId;
    private String firstName;
    private String lastName;
    private String username;
}
// end::code[]
