package com.dropzone.matchstatistics.service;

import com.dropzone.matchstatistics.dto.*;
import com.dropzone.matchstatistics.entity.MatchInfoEntity;
import com.dropzone.matchstatistics.entity.UserMatchStatisticsEntity;
import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import com.dropzone.matchstatistics.repository.MatchInfoRepository;
import com.dropzone.matchstatistics.repository.UserMatchStatisticsRepository;
import com.dropzone.matchstatistics.repository.UserStatisticsRepository;
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

    private final MatchInfoRepository matchInfoRepository;
    private final UserMatchStatisticsRepository repository;
    private final UserStatisticsRepository userStatisticsRepository;

    // 두 레포지토리 모두 생성자에서 주입받도록 수정
    @Autowired
    public UserMatchStatisticsServiceImp(UserMatchStatisticsRepository repository, MatchInfoRepository matchInfoRepository, UserStatisticsRepository userStatisticsRepository) {
        this.repository = repository;
        this.matchInfoRepository = matchInfoRepository;  // matchInfoRepository 초기화
        this.userStatisticsRepository = userStatisticsRepository;
    }

    // 1. 특정 유저의 특정 매치 기록 조회
    @Override
    public UserMatchResponseDTO getUserMatchStatistics(int userId, int matchId) {
        UserMatchStatisticsEntity entity = repository.findByUserIdAndMatch_MatchId(userId, matchId)
                .orElseThrow(() -> new RuntimeException("Record not found"));

        UserMatchResponseDTO result = UserMatchResponseDTO.builder()
                .userId(entity.getUserId())
                .match_id(entity.getMatch().getMatchId())
                .character_id(entity.getCharacterId())
                .match_rank(entity.getMatchRank())
                .match_kills(entity.getMatchKills())
                .match_playtime(entity.getMatchPlaytime())
                .build();

        return result;
    }

    // 2. 특정 유저의 모든 매치 기록 조회
    @Override
    public UserAllMatchDTO getUserAllMatchStatistics(int userId) {
        List<UserMatchResponseDTO> matches = repository.findByUserId(userId).stream()
                .map(entity -> UserMatchResponseDTO.builder()
                        .userId(entity.getUserId())
                        .match_id(entity.getMatch().getMatchId())
                        .character_id(entity.getCharacterId())
                        .match_rank(entity.getMatchRank())
                        .match_kills(entity.getMatchKills())
                        .match_playtime(entity.getMatchPlaytime())
                        .build())
                .collect(Collectors.toList());

        // 매치 기록이 없으면 빈 리스트를 반환
        return new UserAllMatchDTO(userId, matches);
    }


    // 3. 툭종 매치의 모든 유저 기록 조회
    @Override
    public MatchAllUserDTO getMatchAllUserStatistics(int matchId) {
        List<UserMatchResponseDTO> userRecords = repository.findByMatch_MatchId(matchId).stream()
                .map(entity -> UserMatchResponseDTO.builder()
                        .userId(entity.getUserId())
                        .match_id(entity.getMatch().getMatchId())
                        .character_id(entity.getCharacterId())
                        .match_rank(entity.getMatchRank())
                        .match_kills(entity.getMatchKills())
                        .match_playtime(entity.getMatchPlaytime())
                        .build())
                .collect(Collectors.toList());

        if (userRecords.isEmpty()) {
            throw new RuntimeException("No users found for matchId: " + matchId);
        }

        return new MatchAllUserDTO(matchId, userRecords);
    }

    // 4. 매치 기록을 DB에 저장 + userStatistics 테이블에 유저 통계를 기록
    @Override
    public void saveMatchRecords(List<MatchAllUserResponseDTO> matchRecords) {

        // 4-1. 새로운 매치 생성 (match_id는 데이터베이스가 자동 생성)
        MatchInfoEntity newMatchInfo = new MatchInfoEntity();
        newMatchInfo.setMatchCreatedAt(LocalDateTime.now()); // 현재 시간으로 설정
        matchInfoRepository.save(newMatchInfo); // matchInfo 저장 후 matchId 자동 생성

        // 플레이어 수 계산
        int playerCount = 0;
        for (MatchAllUserResponseDTO matchRecord : matchRecords) {
            playerCount += matchRecord.getUserRecords().size();  // 각 매치에 포함된 유저 수를 더함
        }

        // 랭크 포인트 계산을 위한 객체 생성
        RankPointCalculator rankPointCalculator = new RankPointCalculator();

        // 4-2. 각 유저의 매치 기록 저장 및 통계 업데이트
        for (MatchAllUserResponseDTO matchRecord : matchRecords) {
            // 매치 기록을 각 유저별로 엔티티로 변환하여 저장
            for (UserMatchDTO userMatchDTO : matchRecord.getUserRecords()) {

                int getPlayTime = userMatchDTO.getMatch_playtime();

                int hours = getPlayTime / 3600;
                int minutes = (getPlayTime % 3600) / 60;
                int seconds = getPlayTime % 60;

                String timeString = String.format("%02d:%02d:%02d", hours, minutes, seconds);
                Time matchPlayTime = Time.valueOf(timeString);

                // 매치 기록 저장
                UserMatchStatisticsEntity entity = new UserMatchStatisticsEntity();
                entity.setUserId(userMatchDTO.getUserId());
                entity.setCharacterId(userMatchDTO.getCharacter_id());
                // entity.setMatchRank(userMatchDTO.getMatch_rank());
                entity.setMatchKills(userMatchDTO.getMatch_kills());
                entity.setMatchPlaytime(matchPlayTime);

                // 랭크가 0이면 1로 변경
                int matchRank = userMatchDTO.getMatch_rank() == 0 ? 1 : userMatchDTO.getMatch_rank();
                entity.setMatchRank(matchRank);

                // 매치 정보 설정 (MatchInfoEntity 객체 사용)
                entity.setMatch(newMatchInfo);

                // 기록을 DB에 저장
                repository.save(entity);

                // 4-3. 유저 통계 테이블 업데이트
                UserStatisticsEntity userStatistics = userStatisticsRepository.findByUserId(userMatchDTO.getUserId())
                        .orElseGet(() -> {
                            // 통계 정보가 없으면 새로운 통계 생성
                            UserStatisticsEntity newUserStatistics = new UserStatisticsEntity();
                            newUserStatistics.setUserId(userMatchDTO.getUserId());
                            newUserStatistics.setRankingPoints(100);
                            newUserStatistics.setTotalKills(0);
                            newUserStatistics.setTotalPlaytime(Time.valueOf("00:00:00"));
                            newUserStatistics.setTotalGames(0);
                            newUserStatistics.setTotalWins(0);
                            return newUserStatistics;
                        });

                // 기존 유저 통계 업데이트
                System.out.println("플레이어 수: " + playerCount + ", 등수: " + userMatchDTO.getMatch_rank());
                System.out.println("계산된 점수: " + rankPointCalculator.calculateRankPoint(playerCount, userMatchDTO.getMatch_rank()));
                userStatistics.setRankingPoints(userStatistics.getRankingPoints() + rankPointCalculator.calculateRankPoint(playerCount, userMatchDTO.getMatch_rank()));
                userStatistics.setTotalKills(userStatistics.getTotalKills() + userMatchDTO.getMatch_kills()); // 총 킬수

                long newPlaytimeMillis = userStatistics.getTotalPlaytime().getTime() + matchPlayTime.getTime();
                userStatistics.setTotalPlaytime(new Time(newPlaytimeMillis)); // 총 플레이시간
                userStatistics.setTotalGames(userStatistics.getTotalGames() + 1); // 총 게임 수

                // 1등일 경우에만 승리를 증가
                if (userMatchDTO.getMatch_rank() == 1) { // 총 승리 횟수
                    userStatistics.setTotalWins(userStatistics.getTotalWins() + 1);
                }

                userStatisticsRepository.save(userStatistics);
            }
        }
    }
}
