package com.dropzone.statistics.entity;

import java.io.Serializable;
import java.util.Objects;

public class UserMatchStatisticsId implements Serializable {
    private int userId;
    private int match;

    public UserMatchStatisticsId() {}

    public UserMatchStatisticsId(int userId, int match) {
        this.userId = userId;
        this.match = match;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        UserMatchStatisticsId that = (UserMatchStatisticsId) o;
        return userId == that.userId && match == that.match;
    }

    @Override
    public int hashCode() {
        return Objects.hash(userId, match);
    }
}
