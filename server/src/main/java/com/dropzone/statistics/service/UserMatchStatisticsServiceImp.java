package com.dropzone.statistics.service;

import com.dropzone.statistics.dto.MatchAllUserDTO;
import com.dropzone.statistics.dto.UserAllMatchDTO;
import com.dropzone.statistics.dto.UserMatchDTO;
import com.dropzone.statistics.entity.UserMatchStatisticsEntity;
import com.dropzone.statistics.repository.UserMatchStatisticsRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.stream.Collectors;

@Service
public class UserMatchStatisticsServiceImp implements UserMatchStatisticsService {

    private final UserMatchStatisticsRepository repository;

    @Autowired
    public UserMatchStatisticsServiceImp(UserMatchStatisticsRepository repository) {
        this.repository = repository;
    }

    @Override
    public UserMatchDTO getUserMatchStatistics(int userId, int matchId) {
        UserMatchStatisticsEntity entity = repository.findByUserIdAndMatchId(userId, matchId)
                .orElseThrow(() -> new RuntimeException("Record not found"));
        return new UserMatchDTO(
                entity.getUserId(),
                entity.getCharacterId(),
                entity.getMatch().getMatchId(),
                entity.getMatchRank(),
                entity.getMatchDps(),
                entity.getMatchKills(),
                entity.getMatchPlaytime()
        );
    }

    @Override
    public UserAllMatchDTO getUserAllMatchStatistics(int userId) {
        List<UserMatchDTO> matches = repository.findByUserId(userId).stream()
                .map(entity -> new UserMatchDTO(
                        entity.getUserId(),
                        entity.getCharacterId(),
                        entity.getMatch().getMatchId(),
                        entity.getMatchRank(),
                        entity.getMatchDps(),
                        entity.getMatchKills(),
                        entity.getMatchPlaytime()
                ))
                .collect(Collectors.toList());

        if (matches.isEmpty()) {
            throw new RuntimeException("No matches found for userId: " + userId);
        }

        return new UserAllMatchDTO(userId, matches);
    }

    @Override
    public MatchAllUserDTO getMatchAllUserStatistics(int matchId) {
        List<UserMatchDTO> userRecords = repository.findByMatchId(matchId).stream()
                .map(entity -> new UserMatchDTO(
                        entity.getUserId(),
                        entity.getCharacterId(),
                        entity.getMatch().getMatchId(),
                        entity.getMatchRank(),
                        entity.getMatchDps(),
                        entity.getMatchKills(),
                        entity.getMatchPlaytime()
                ))
                .collect(Collectors.toList());

        if (userRecords.isEmpty()) {
            throw new RuntimeException("No users found for matchId: " + matchId);
        }

        return new MatchAllUserDTO(matchId, userRecords);
    }
}
