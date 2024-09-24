package com.dropzone.statistics.service;

import com.dropzone.statistics.dto.MatchAllUserDTO;
import com.dropzone.statistics.dto.UserAllMatchDTO;
import com.dropzone.statistics.dto.UserMatchDTO;
import com.dropzone.statistics.entity.MatchInfoEntity;
import com.dropzone.statistics.entity.UserMatchStatisticsEntity;
import com.dropzone.statistics.repository.MatchInfoRepository;
import com.dropzone.statistics.repository.UserMatchStatisticsRepository;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.sql.Time;
import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Slf4j
@Service
public class UserMatchStatisticsServiceImp implements UserMatchStatisticsService {

    private final UserMatchStatisticsRepository repository;
    private final MatchInfoRepository matchInfoRepository;  // MatchInfoRepository 주입

    // 두 레포지토리 모두 생성자에서 주입받도록 수정
    @Autowired
    public UserMatchStatisticsServiceImp(UserMatchStatisticsRepository repository, MatchInfoRepository matchInfoRepository) {
        this.repository = repository;
        this.matchInfoRepository = matchInfoRepository;  // matchInfoRepository 초기화
    }

    @Override
    public UserMatchDTO getUserMatchStatistics(int userId, int matchId) {
        UserMatchStatisticsEntity entity = repository.findByUserIdAndMatch_MatchId(userId, matchId)
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
        List<UserMatchDTO> userRecords = repository.findByMatch_MatchId(matchId).stream()
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

    @Override
    public void saveMatchRecords(List<MatchAllUserDTO> matchRecords) {

    // 새로운 매치 생성 (match_id는 데이터베이스가 자동 생성)
    MatchInfoEntity newMatchInfo = new MatchInfoEntity();
    newMatchInfo.setMatchCreatedAt(LocalDateTime.now()); // 현재 시간으로 설정
    matchInfoRepository.save(newMatchInfo); // matchInfo 저장 후 matchId 자동 생성

        for (MatchAllUserDTO matchRecord : matchRecords) {
            // 매치 기록을 각 유저별로 엔티티로 변환하여 저장
            for (UserMatchDTO userMatchDTO : matchRecord.getUserRecords()) {
                UserMatchStatisticsEntity entity = new UserMatchStatisticsEntity();
                entity.setUserId(userMatchDTO.getUserId());
                entity.setCharacterId(userMatchDTO.getCharacter_id());
                entity.setMatchRank(userMatchDTO.getMatch_rank());
                entity.setMatchDps(userMatchDTO.getMatch_dps());
                entity.setMatchKills(userMatchDTO.getMatch_kills());
                entity.setMatchPlaytime(userMatchDTO.getMatch_playtime());

                // 매치 정보 설정 (MatchInfoEntity 객체 사용)
                entity.setMatch(newMatchInfo);

                // 기록을 DB에 저장
                repository.save(entity);
            }
        }
    }
}
