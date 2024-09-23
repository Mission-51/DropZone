package com.dropzone.statistics.service;

import com.dropzone.statistics.dto.MatchAllUserDTO;
import com.dropzone.statistics.dto.UserAllMatchDTO;
import com.dropzone.statistics.dto.UserMatchDTO;
import com.dropzone.statistics.entity.MatchInfoEntity;
import com.dropzone.statistics.entity.UserMatchStatisticsEntity;
import com.dropzone.statistics.repository.MatchInfoRepository;
import com.dropzone.statistics.repository.UserMatchStatisticsRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.stream.Collectors;

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

                // MatchInfoEntity도 설정 (match_id를 통해 가져와서 설정)
                MatchInfoEntity matchInfo = matchInfoRepository.findById(userMatchDTO.getMatch_id())
                        .orElseThrow(() -> new RuntimeException("Match not found with ID: " + userMatchDTO.getMatch_id()));
                entity.setMatch(matchInfo);

                // 기록을 DB에 저장
                repository.save(entity);
            }
        }
    }
}
