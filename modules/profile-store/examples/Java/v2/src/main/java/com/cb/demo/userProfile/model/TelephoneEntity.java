package com.cb.demo.userProfile.model;

import lombok.Data;

import javax.validation.constraints.NotNull;

@Data
public class TelephoneEntity {

    @NotNull
    private String name;
    @NotNull
    private String number;
}
