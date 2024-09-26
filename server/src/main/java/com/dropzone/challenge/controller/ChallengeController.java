package com.dropzone.challenge.controller;

import com.dropzone.challenge.dto.ChallengeCompletionDTO;
import com.dropzone.challenge.dto.ChallengeCreateRequestDTO;
import com.dropzone.challenge.entity.ChallengeEntity;
import com.dropzone.challenge.service.ChallengeService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@Slf4j
@RestController // REST API 요청을 처리하는 컨트롤러
@RequestMapping("api/challenge") // 'api/challenge' 경로로 들어오는 요청을 처리
@Tag(name = "도전 과제 API", description = "도전 과제 생성 및 유저의 도전 과제 달성 여부 확인 API")
public class ChallengeController {

    private final ChallengeService challengeService;

    // 생성자를 통해 service 계층 주입
    @Autowired
    public ChallengeController(ChallengeService challengeService) {
        this.challengeService = challengeService;
    }

    // 도전 과제 생성 API (POST 요청 처리)
    @Operation(summary = "도전 과제 생성", description = "도전 과제를 생성")
    @PostMapping("/create")
    public ResponseEntity<?> createChallenge(
            @io.swagger.v3.oas.annotations.parameters.RequestBody(
                    description = "도전과제 설명, 대상 필드, 연산자, 값",
                    required = true,
                    content = @Content(
                            mediaType = "application/json",
                            schema = @Schema(
                                    example = "{ \"challengeContent\": \"10킬 이상 달성\", \"challengeField\": \"total_kills\", \"challengeOperator\": \">=\", \"challengeValue\": 10}"
                            )
                    )
            )
            @RequestBody ChallengeCreateRequestDTO challengeCreateRequest) {
        try {
            // 도전 과제 생성 로직
            ChallengeEntity createdChallenge = challengeService.createChallenge(
                    challengeCreateRequest.getChallengeContent(),
                    challengeCreateRequest.getChallengeField(),
                    challengeCreateRequest.getChallengeOperator(),
                    challengeCreateRequest.getChallengeValue()
            );

            // 성공 시 메시지 반환
            return ResponseEntity.ok("도전 과제 생성 성공");
        } catch (Exception e) {
            // 실패 시 에러 메시지 반환
            return ResponseEntity.status(500).body("도전 과제 생성 중 오류 발생: " + e.getMessage());
        }
    }

    // 특정 유저의 모든 도전 과제 달성 여부를 확인하는 API
    @Operation(summary = "특정 유저의 모든 도전 과제 달성 여부 확인", description = "특정 유저의 도전 과제 리스트와 달성 여부를 반환")
    @GetMapping("/check-all")
    public ResponseEntity<?> checkAllChallengesForUser(@RequestParam int userId) {
        List<ChallengeCompletionDTO> result = challengeService.checkAllChallengesForUser(userId);
        return ResponseEntity.ok(result);
    }

}
