package com.dropzone.statistics.controller;


import com.dropzone.statistics.dto.MatchAllUserDTO;
import com.dropzone.statistics.dto.UserAllMatchDTO;
import com.dropzone.statistics.dto.UserMatchDTO;
import com.dropzone.statistics.service.UserMatchStatisticsService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/matches/statistics")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "매치 기록 API", description = "유저의 매치 기록 조회 및 매치에 참여한 모든 유저 기록 조회 API")
public class StatisticsController {

    private final UserMatchStatisticsService userMatchStatisticsService;

    // 1. 특정 유저의 특정 매치 기록 조회
    @Operation(summary = "특정 유저의 특정 매치 기록 조회", description = "특정 유저가 특정 매치에서의 기록 조회")
    @GetMapping("/user/{userId}/match/{matchId}")
    public ResponseEntity<?> getUserMatchStatistics(
            @Parameter(description = "유저 ID", required = true, example = "1") @PathVariable("userId") int userId,
            @Parameter(description = "매치 ID", required = true, example = "100") @PathVariable("matchId") int matchId) {
        log.info("유저 ID {}의 매치 ID {} 기록 조회 요청", userId, matchId);
        try {
            UserMatchDTO userMatchDTO = userMatchStatisticsService.getUserMatchStatistics(userId, matchId);
            return ResponseEntity.ok(userMatchDTO);
        } catch (Exception e) {
            log.error("유저 ID {}의 매치 ID {} 기록 조회 중 오류 발생", userId, matchId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("매치 기록 조회 중 서버 오류 발생");
        }
    }

    // 2. 특정 유저의 모든 매치 기록 조회
    @Operation(summary = "특정 유저의 모든 매치 기록 조회", description = "특정 유저가 참가한 모든 매치 기록 조회")
    @GetMapping("/user/{userId}")
    public ResponseEntity<?> getAllMatchesForUser(
            @Parameter(description = "유저 ID", required = true, example = "1") @PathVariable("userId") int userId) {
        log.info("유저 ID {}의 모든 매치 기록 조회 요청", userId);
        try {
            UserAllMatchDTO userAllMatchDTO = userMatchStatisticsService.getUserAllMatchStatistics(userId);
            return ResponseEntity.ok(userAllMatchDTO);
        } catch (Exception e) {
            log.error("유저 ID {}의 모든 매치 기록 조회 중 오류 발생", userId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("모든 매치 기록 조회 중 서버 오류 발생");
        }
    }

    // 3. 특정 매치의 모든 유저 기록 조회
    @Operation(summary = "특정 매치의 모든 유저 기록 조회", description = "특정 매치에 참가한 모든 유저의 기록 조회")
    @GetMapping("/match/{matchId}")
    public ResponseEntity<?> getAllUsersForMatch(
            @Parameter(description = "매치 ID", required = true, example = "100") @PathVariable("matchId") int matchId) {
        log.info("매치 ID {}의 모든 유저 기록 조회 요청", matchId);
        try {
            MatchAllUserDTO matchAllUserDTO = userMatchStatisticsService.getMatchAllUserStatistics(matchId);
            return ResponseEntity.ok(matchAllUserDTO);
        } catch (Exception e) {
            log.error("매치 ID {}의 모든 유저 기록 조회 중 오류 발생", matchId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("모든 유저 기록 조회 중 서버 오류 발생");
        }
    }

    // 4. 매치 기록을 DB에 저장
    @Operation(summary = "매치 기록 저장", description = "매치 기록을 DB에 저장")
    @PostMapping("/record")
    public ResponseEntity<?> saveMatchRecords(@RequestBody List<MatchAllUserDTO> matchRecords) {
        try {
            userMatchStatisticsService.saveMatchRecords(matchRecords);
            return ResponseEntity.ok("Match records saved successfully");
        } catch (Exception e) {
            return ResponseEntity.status(500).body("매치 기록 저장 중 오류 발생: " + e.getMessage());
        }
    }
}