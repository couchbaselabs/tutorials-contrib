package com.cb.demo.userProfile.model;

import lombok.Data;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

@Data
public class AddressEntity {

    @NotNull
    private String name;
    @NotNull
    private String street;
    @NotNull
    private String number;
    @NotNull
    private String zipCode;
    @NotNull
    private String city;
    @NotNull
    private String state;
    @NotNull
    @Size(min = 2, max = 2)
    private String countryCode;
}
