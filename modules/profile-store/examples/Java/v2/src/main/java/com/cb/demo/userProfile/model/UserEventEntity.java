package com.cb.demo.userProfile.model;

import lombok.Data;
import org.springframework.data.annotation.Id;

import javax.validation.constraints.NotNull;

@Data
public class UserEventEntity {

    @Id
    private String id;

    @NotNull
    private String userId;

    @NotNull
    private Long createdDate;

    @NotNull
    private EventType eventType;
}
